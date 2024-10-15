using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using MyLib;

public class UnitScript : SerializedMonoBehaviour
{
    [SerializeField] private EUnit              unit;
    [SerializeField] protected UnitData         unitData;
    [SerializeField] private SimpleColorShader  colorShader;

    [ReadOnly] public ENation                   nation;
    [ReadOnly] public Vector2Int                tilePos;
    [ReadOnly] protected EUnitAction            unitAction  = EUnitAction.None;

    private Vector3     offset;
    private const int   FIND_DIS = 5;
    protected int         nowHp;
    protected int         nowAtk;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //이동

    protected bool          nowMove = false;
    private Vector2Int      moveToTilePos;
    protected const float   FIND_DELAY = 0.5f;
    protected float         notFindDelay = FIND_DELAY;

    private IEnumerator     moveCor;
    protected int           moveDic;

    private HashSet<Vector2Int>                 closeArea   = new HashSet<Vector2Int>();                //닫힌리스트
    private PriorityQueue<Algorithm.AstarNode>  pq          = new PriorityQueue<Algorithm.AstarNode>(); //열린리스트
    private Dictionary<Vector2Int, Vector2Int>  parent      = new Dictionary<Vector2Int, Vector2Int>(new Vector2IntComparer());
    protected Build                             attackBuild;
    protected UnitScript                        attackUnit;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //공격
    private const float ATK_DELAY   = 1.5f;
    protected float       attakDelay  = 0;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //피격
    private IEnumerator hitCor;

    public EUnit GetEUnit() => unit;

    public virtual void SetUnit(ENation pNation, Vector2Int pTilePos, Vector3 pPos)
    {
        offset = new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f),0);

        nation              = pNation;
        tilePos             = pTilePos;
        transform.position  = pPos + offset;

        nowHp               = unitData.hp;
        nowAtk              = unitData.atk;

        moveToTilePos = pTilePos;

        if(moveCor != null)
        {
            StopCoroutine(moveCor);
            moveCor = null;
        }
        if(hitCor != null)
        {
            StopCoroutine(hitCor);
            hitCor = null;
        }

        colorShader.Set_Shader(0);
        unitAction = EUnitAction.Move;

    }

    public virtual void UnitUpdate()
    {
        UnitActionUpdate();
    }

    protected virtual void UnitActionUpdate()
    {
        if (IsDie())
            return;

        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        switch (unitAction)
        {
            case EUnitAction.Move:
                if (unitData.isMove)
                {
                    if (tilePos == moveToTilePos)
                    {
                        //길탐색 시작
                        nowMove = false;
                        if (notFindDelay > 0)
                        {
                            notFindDelay -= Time.deltaTime;
                        }
                        else
                        {
                            notFindDelay = FIND_DELAY;
                            moveToTilePos = MoveUnit();
                            ChageAct(moveToTilePos);
                        }
                    }
                    else if (nowMove == false)
                    {
                        nowMove = true;      
                        Vector3 tileWorldPos = filedObjMgr.GetTileWorldPos(moveToTilePos);

                        if (transform.position.x < tileWorldPos.x)
                            moveDic = 1;
                        else if (transform.position.x > tileWorldPos.x)
                            moveDic = -1;

                        moveCor = Action2D.MoveTo(transform, tileWorldPos + offset, 1f / unitData.speed, () =>
                        {
                            tilePos = moveToTilePos;
                            ChageAct(tilePos);
                        });
                        StartCoroutine(moveCor);
                    }
                }
                break;
            case EUnitAction.AttackTile:
                {
                    filedObjMgr.SetNationTile(tilePos, nation);
                    ChageAct(tilePos);
                }
                break;
            case EUnitAction.AttackBuild:
                {
                    if (attakDelay > ATK_DELAY)
                    {
                        attakDelay = 0;
                        attackBuild.HitObj(nowAtk);

                        SoundMgr.Instance.PlaySound(transform, ESound.Attack);
                        ChageAct(tilePos);
                    }
                    else
                    {
                        attakDelay += Time.deltaTime;
                    }
                }
                break;
            case EUnitAction.AttackUnit:
                {
                    if (attakDelay > ATK_DELAY)
                    {
                        attakDelay = 0;
                        attackUnit.HitObj(nowAtk);

                        SoundMgr.Instance.PlaySound(transform, ESound.Attack);
                        ChageAct(tilePos);
                    }
                    else
                    {
                        attakDelay += Time.deltaTime;
                    }
                }
                break;
        }
    }


    public virtual void RemoveUnit()
    {
        if (moveCor != null)
        {
            StopCoroutine(moveCor);
            moveCor = null;
        }
        if (hitCor != null)
        {
            StopCoroutine(hitCor);
            hitCor = null;
        }
    }

    public virtual void HitObj(int pDamage)
    {
        if (IsDie())
            return;
        nowHp -= pDamage;

        if(hitCor != null)
        {
            StopCoroutine(hitCor);
            hitCor = null;
        }
        hitCor = HitAni();
        IEnumerator HitAni()
        {
            for(int i = 0; i < 3; i++)
            {
                colorShader.Set_Shader(1);
                yield return new WaitForSeconds(0.1f);
                colorShader.Set_Shader(0);
                yield return new WaitForSeconds(0.1f);
            }
            hitCor = null;
        }
        StartCoroutine(HitAni());

        if (IsDie())
            RemoveUnit();
    }

    public virtual bool IsDie()
    {
        return (nowHp <= 0);
    }

    protected virtual Vector2Int MoveUnit()
    {
        FiledObjMgr         filedObjMgr = FiledObjMgr.Instance;

        Vector2Int          movePos     = tilePos;
        List<Vector2Int>    moveList    = null;

        Build               enemyBuild  = filedObjMgr.FindEnemyBuild(nation, movePos, FIND_DIS);
        UnitScript          enemyUnit   = filedObjMgr.FindEnemyUnit(nation, movePos, FIND_DIS);
        Vector2Int          enemyTile   = filedObjMgr.FindEnemyNationTile(nation, movePos, FIND_DIS);

        //탐색 우선순위 유닛,건물,타일
        if (enemyUnit != null)
        {
            moveList = AstartHexagonRoute(movePos, enemyUnit.tilePos);
            if (moveList.Count > 0)
                return moveList[moveList.Count - 1];
        }

        if (enemyBuild != null)
        {
            moveList = AstartHexagonRoute(movePos, enemyBuild.tilePos);
            if (moveList.Count > 0)
                return moveList[moveList.Count - 1];
        }

        if (enemyTile != tilePos)
        {
            moveList = AstartHexagonRoute(movePos, enemyTile);
            if (moveList.Count > 0)
                return moveList[moveList.Count - 1];
        }

        Vector2Int lastFind = filedObjMgr.FindAllEnemyNationTile(nation, movePos);
        if (lastFind != tilePos)
        {
            moveList = AstartHexagonRoute(movePos, lastFind);
            if (moveList.Count > 0)
                return moveList[moveList.Count - 1];
        }


        //랜덤하게 이동
        List<Vector2Int> aroundList = filedObjMgr.GetCanMoveAroundPos(movePos);
        if (aroundList.Count > 0)
        {
            int r = Random.Range(0, aroundList.Count);
            return aroundList[r];
        }

        return movePos;
    }

    protected List<Vector2Int> AstartHexagonRoute(Vector2Int pFrom, Vector2Int pTo)
    {
        FiledObjMgr         filedObjMgr             = FiledObjMgr.Instance;
        FiledObjMgr.BakeKey bakeKey                 = new FiledObjMgr.BakeKey(pFrom, pTo);
        if (filedObjMgr.bakeRoute.ContainsKey(bakeKey))
            return filedObjMgr.bakeRoute[bakeKey];

        List<Vector2Int>                    route                   = new List<Vector2Int>();
        closeArea.Clear();
        parent.Clear();
        pq.Clear();

        pq.Push(new Algorithm.AstarNode(Mathf.Abs(pFrom.x - pTo.x) + Mathf.Abs(pFrom.y - pTo.y), 0, pFrom, pFrom,0));

        bool explore = false;

        while (pq.Count() > 0)
        {
            Algorithm.AstarNode node = pq.Pop();
            if (closeArea.Contains(node.pos))
                continue;

            closeArea.Add(node.pos);

            parent[node.pos] = node.parents;

            if (node.pos.x == pTo.x && node.pos.y == pTo.y)
            {
                explore = true;
                break;
            }

            List<Vector2Int> aroundList = filedObjMgr.GetCanMoveAroundPos(node.pos);
            for (int i = 0; i < aroundList.Count; i++)
            {
                int ax = aroundList[i].x;
                int ay = aroundList[i].y;

                if (closeArea.Contains(aroundList[i]))
                    continue;

                float g = node.g + 10;
                float h = 10 * (Mathf.Abs(ax - pTo.x) + Mathf.Abs(ay - pTo.y));

                pq.Push(new Algorithm.AstarNode(g + h, g, new Vector2Int(ax, ay), node.pos,node.deep + 1)); //parent 추가
            }
        }

        if (explore)
        {
            route = new List<Vector2Int>();
            int ax = pTo.x;
            int ay = pTo.y;

            while (ax != pFrom.x || ay != pFrom.y)
            {
                route.Add(new Vector2Int(ax, ay));
                int bx = ax;
                int by = ay;
                Vector2Int key = new Vector2Int(bx, by);
                ax = parent[key].x;
                ay = parent[key].y;
            }
        }

        filedObjMgr.bakeRoute[bakeKey] = route;

        return route;
    }

    private void ChageAct(Vector2Int pos)
    {
        attakDelay = 0;
        notFindDelay = 0;
        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        UnitScript  unit        = filedObjMgr.GetEnemyUnit(nation, pos);
        if(unit != null)
        {
            attackUnit = unit;
            attackUnit.HitObj(nowAtk);
            SoundMgr.Instance.PlaySound(transform, ESound.Attack);
            unitAction = EUnitAction.AttackUnit;
            return;
        }
        attackUnit = null;

        Build       build       = filedObjMgr.GetEnemyBuild(nation, pos);
        if (build != null)
        {
            attackBuild = build;
            attackBuild.HitObj(nowAtk);
            SoundMgr.Instance.PlaySound(transform, ESound.Attack);
            unitAction = EUnitAction.AttackBuild;
            return;
        }
        attackBuild = null;

        bool    enemyTile       = filedObjMgr.GetEnemyTile(nation, pos);
        if(enemyTile)
        {
            unitAction = EUnitAction.AttackTile;
            return;
        }

        unitAction = EUnitAction.Move;
    }
}
