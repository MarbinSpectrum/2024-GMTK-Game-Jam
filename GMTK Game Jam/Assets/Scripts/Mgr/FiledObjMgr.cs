using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using MyLib;

public class FiledObjMgr : SerializedMonoBehaviour
{
    [SerializeField] private Tilemap groundData;
    [SerializeField] private Tilemap editData;
    [SerializeField] private Tilemap OccupationData;
    [SerializeField] private Tilemap decoData;

    [SerializeField] private Dictionary<ENation, TileBase> nationTile   = new Dictionary<ENation, TileBase>(new ENationComparer());

    [SerializeField] private Dictionary<EBuild, Build>      buildObj    = new Dictionary<EBuild, Build>(new EBuildComparer());
    private Dictionary<EBuild, Queue<Build>>                buildPool   = new Dictionary<EBuild, Queue<Build>>(new EBuildComparer());
    private List<Build>                                     buildList   = new List<Build>();

    [SerializeField] private Dictionary<EUnit, UnitScript>  unitObj     = new Dictionary<EUnit, UnitScript>(new EUnitComparer());
    private Dictionary<EUnit, Queue<UnitScript>>            unitPool    = new Dictionary<EUnit, Queue<UnitScript>>(new EUnitComparer());
    private List<UnitScript>                                unitList    = new List<UnitScript>();

    //좌표별 건물
    private Dictionary<Vector2Int, Build>                nBuild         = new Dictionary<Vector2Int, Build>(new Vector2IntComparer());

    private Dictionary<Vector2Int, ENation>              nNationTile    = new Dictionary<Vector2Int, ENation>(new Vector2IntComparer());
    public static FiledObjMgr   Instance        { get; private set; }
    public TileInfo             playerStartPos  { get; private set; }

    public Dictionary<Vector2Int, TileInfo> humanStartPos0   { get; private set; } = new Dictionary<Vector2Int, TileInfo>(new Vector2IntComparer());
    public Dictionary<Vector2Int, TileInfo> humanStartPos1 { get; private set; } = new Dictionary<Vector2Int, TileInfo>(new Vector2IntComparer());
    public Dictionary<Vector2Int, TileInfo> humanStartPos2 { get; private set; } = new Dictionary<Vector2Int, TileInfo>(new Vector2IntComparer());
    public Dictionary<Vector2Int, TileInfo> house0Pos { get; private set; } = new Dictionary<Vector2Int, TileInfo>(new Vector2IntComparer());
    public Dictionary<Vector2Int, TileInfo> house1Pos { get; private set; } = new Dictionary<Vector2Int, TileInfo>(new Vector2IntComparer());
    [System.NonSerialized] public Dictionary<BakeKey, List<Vector2Int>> bakeRoute = new Dictionary<BakeKey, List<Vector2Int>>(new BakeKeyComparer());
    private Dictionary<Vector2Int, List<Vector2Int>>                    canMoveAroundList = new Dictionary<Vector2Int, List<Vector2Int>>(new Vector2IntComparer());

    [System.NonSerialized] public Dictionary<Vector2Int, float>         conquestPercent = new Dictionary<Vector2Int, float>(new Vector2IntComparer());

    public Dictionary<EBuild, int> hasPlayerBuild { get; private set; } = new Dictionary<EBuild, int>();

    public int enemyTileMax { get; private set; }
    public int enemyTileCnt { get; private set; }

    public struct BakeKey
    {
        public Vector2Int from;
        public Vector2Int to;

        public BakeKey(Vector2Int pFrom,Vector2Int pTo)
        {
            from = pFrom;
            to = pTo;
        }
    }

    public class BakeKeyComparer : IEqualityComparer<BakeKey>
    {
        public bool Equals(BakeKey x, BakeKey y)
        {
            return x.from.x == y.from.x && x.from.y == y.from.y && x.to.x == y.to.x && x.to.y == y.to.y;
        }
        public int GetHashCode(BakeKey vector)
        {
            return vector.from.x.GetHashCode() ^ vector.from.y.GetHashCode() << 2 ^ vector.to.x.GetHashCode() ^ vector.to.y.GetHashCode() << 2;
        }
    }

    public class TileInfo
    {
        public string       tileName;
        public Vector2Int   tilePos;
        public Vector3      tileWorldPos;
    }

    private bool InitFlag = false;

    private const int MAP_W = 100;
    private const int MAP_H = 100;

    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        foreach (Build build in buildList)
            build.BuildUpdate();
        foreach (UnitScript unit in unitList)
            unit.UnitUpdate();
    }

    private void Init()
    {
        if (InitFlag)
            return;
        InitFlag = true;
        Instance = this;

        InitPlayerTile();
        InitGroundTile();
    }
    private void InitPlayerTile()
    {
        List<Vector2Int> preparationStartPos = new List<Vector2Int>();
        for (int x = -MAP_W; x <= MAP_W; x++)
        {
            for (int y = -MAP_H; y <= MAP_H; y++)
            {
                TileBase tileBase = editData.GetTile(new Vector3Int(x, y));
                Vector3 tilePos = editData.CellToWorld(new Vector3Int(x, y));
                if (tileBase == null)
                    continue;
                switch(tileBase.name)
                {
                    case "PlayerStart":
                        preparationStartPos.Add(new Vector2Int(x, y));
                        editData.SetTile(new Vector3Int(x, y), null);
                        break;
                    case "EnemyStart0":
                        {
                            TileInfo humanPos = new TileInfo();
                            humanPos.tileName = tileBase.name;
                            humanPos.tilePos = new Vector2Int(x, y);
                            humanPos.tileWorldPos = tilePos;
                            humanStartPos0.Add(humanPos.tilePos, humanPos);
                            editData.SetTile(new Vector3Int(x, y), null);
                            break;
                        }
                    case "EnemyStart1":
                        {
                            TileInfo humanPos = new TileInfo();
                            humanPos.tileName = tileBase.name;
                            humanPos.tilePos = new Vector2Int(x, y);
                            humanPos.tileWorldPos = tilePos;
                            humanStartPos1.Add(humanPos.tilePos, humanPos);
                            editData.SetTile(new Vector3Int(x, y), null);
                            break;
                        }
                    case "EnemyStart2":
                        {
                            TileInfo humanPos = new TileInfo();
                            humanPos.tileName = tileBase.name;
                            humanPos.tilePos = new Vector2Int(x, y);
                            humanPos.tileWorldPos = tilePos;
                            humanStartPos2.Add(humanPos.tilePos, humanPos);
                            editData.SetTile(new Vector3Int(x, y), null);
                            break;
                        }
                    case "House0":
                        {
                            TileInfo housePos = new TileInfo();
                            housePos.tileName = tileBase.name;
                            housePos.tilePos = new Vector2Int(x, y);
                            housePos.tileWorldPos = tilePos;
                            house0Pos.Add(housePos.tilePos, housePos);
                            editData.SetTile(new Vector3Int(x, y), null);
                            break;
                        }
                    case "House1":
                        {
                            TileInfo housePos = new TileInfo();
                            housePos.tileName = tileBase.name;
                            housePos.tilePos = new Vector2Int(x, y);
                            housePos.tileWorldPos = tilePos;
                            house1Pos.Add(housePos.tilePos, housePos);
                            editData.SetTile(new Vector3Int(x, y), null);
                            break;
                        }
                }
            }
        }

        {
            int r = Random.Range(0, preparationStartPos.Count);
            Vector2Int startPos = preparationStartPos[r];
            Vector3 tilePos = editData.CellToWorld(new Vector3Int(startPos.x, startPos.y));
            playerStartPos ??= new TileInfo();
            playerStartPos.tileName = "PlayerStart";
            playerStartPos.tilePos = startPos;
            playerStartPos.tileWorldPos = tilePos;
        }
    }

    private void InitGroundTile()
    {
        for (int x = -MAP_W; x <= MAP_W; x++)
        {
            for (int y = -MAP_H; y <= MAP_H; y++)
            {
                TileBase tileBase = groundData.GetTile(new Vector3Int(x, y));
                Vector3 tilePos = groundData.CellToWorld(new Vector3Int(x, y));
                if (tileBase == null)
                    continue;

                if (IsDecoTile(new Vector2Int(x, y)))
                    continue;

                enemyTileMax++;
                if (playerStartPos != null && playerStartPos.tilePos.x == x && playerStartPos.tilePos.y == y)
                {
                    SetNationTile(new Vector2Int(x, y),ENation.Devil);
                    List<Vector2Int> aroundList = GetCanMoveAroundPos(new Vector2Int(x, y));
                    foreach(Vector2Int pos in aroundList)
                        SetNationTile(pos, ENation.Devil);

                    continue;
                }


                if (humanStartPos0 != null && humanStartPos0.ContainsKey(new Vector2Int(x, y)))
                {
                    SetNationTile(new Vector2Int(x, y), ENation.Human);
                    continue;
                }

                if(nNationTile.ContainsKey(new Vector2Int(x, y)) == false)
                {
                    SetNationTile(new Vector2Int(x, y), ENation.Human);
                }
            }
        }
    }

    public void SetNationTile(Vector2Int pos, ENation pNation)
    {
        if(nationTile.ContainsKey(pNation))
        {
            TileBase tile = nationTile[pNation];
            OccupationData.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);
            OccupationData.SetColor(new Vector3Int(pos.x, pos.y, 0), new Color(1, 1, 1, 0.5f));
        }
        if (nNationTile.ContainsKey(pos) == false || (nNationTile[pos] != pNation))
        {
            if (pNation == ENation.Devil)
                enemyTileCnt--;
            else if (pNation == ENation.Human)
                enemyTileCnt++;
        }
        nNationTile[pos] = pNation;
    }

    public UnitScript CreateUnit(EUnit pUnit)
    {
        if (unitPool.ContainsKey(pUnit) == false)
            unitPool[pUnit] = new Queue<UnitScript>();

        Queue<UnitScript> unitQueue = unitPool[pUnit];
        if(unitQueue.Count == 0)
        {
            UnitScript unitScript = Instantiate(unitObj[pUnit]);
            unitList.Add(unitScript);

            unitQueue.Enqueue(unitScript);
        }
        UnitScript newUnit = unitQueue.Dequeue();
        newUnit.gameObject.SetActive(true);

        return newUnit;
    }

    public void RemoveUnit(EUnit pUnit,UnitScript unitScript)
    {
        if (unitPool.ContainsKey(pUnit) == false)
            unitPool[pUnit] = new Queue<UnitScript>();

        Queue<UnitScript> unitQueue = unitPool[pUnit];
        unitQueue.Enqueue(unitScript);
        unitScript.gameObject.SetActive(false);
    }

    public Build CreateBuild(Vector2Int pos, EBuild pBuild)
    {
        if (buildPool.ContainsKey(pBuild) == false)
            buildPool[pBuild] = new Queue<Build>();

        Queue<Build> buildQueue = buildPool[pBuild];
        if (buildQueue.Count == 0)
        {
            Build build = Instantiate(buildObj[pBuild]);
            buildList.Add(build);

            buildQueue.Enqueue(build);
        }
        Build newBuild = buildQueue.Dequeue();
        newBuild.gameObject.SetActive(true);
        newBuild.tilePos = pos;

        nBuild[pos] = newBuild;

        return newBuild;
    }

    public void RemoveBuild(EBuild pBuild, Build buildScript)
    {
        if (buildPool.ContainsKey(pBuild) == false)
            buildPool[pBuild] = new Queue<Build>();

        Queue<Build> unitQueue = buildPool[pBuild];
        unitQueue.Enqueue(buildScript);
        buildScript.gameObject.SetActive(false);
        nBuild.Remove(buildScript.tilePos);
    }

    public Build FindEnemyBuild(ENation myNation, Vector2Int pos,int distance)
    {
        //pos기준으로 distance만큼 떨어진 myNation세력과 적대 건물을 찾는 함수

        int minDis = int.MaxValue;
        int findCnt = (4 * distance * distance - 4 * distance + 1) / 2 + 1;
        Build result = null;
        if (findCnt >= nBuild.Count)
        {
            foreach (var pair in nBuild)
            {
                Build build = pair.Value;
                if (build.IsDie())
                    continue;
                if (build.nation == myNation)
                    continue;
                int getDis = Mathf.Abs(pos.x - build.tilePos.x) + Mathf.Abs(pos.y - build.tilePos.y);
                if (getDis > distance)
                    continue;
                if (getDis >= minDis)
                    continue;
                minDis = getDis;
                result = build;
            }
        }
        else
        {
            for (int y = distance; y >= -distance; y--)
            {
                int w = Mathf.Abs(Mathf.Abs(y) - distance);
                for (int x = w; x >= -w; x--)
                {
                    Vector2Int posKey = new Vector2Int(pos.x + x, pos.y + y);
                    if (IsDecoTile(posKey))
                        continue;

                    if (nBuild.ContainsKey(posKey) == false)
                        continue;

                    Build build = nBuild[posKey];
                    if (build.IsDie())
                        continue;

                    if (build.nation == myNation)
                        continue;

                    int getDis = Mathf.Abs(pos.x - posKey.x) + Mathf.Abs(pos.y - posKey.y);
                    if (getDis >= minDis)
                        continue;
                    minDis = getDis;
                    result = build;
                }
            }
        }

        return result;
    }

    public UnitScript FindEnemyUnit(ENation myNation, Vector2Int pos, int distance)
    {
        //pos기준으로 distance만큼 떨어진 myNation세력과 적대 유닛을 찾는 함수

        int minDis = int.MaxValue;
        UnitScript result = null;

        foreach(UnitScript unit in unitList)
        {
            if (unit.IsDie())
                continue;
            if (unit.nation == myNation)
                continue;
            int getDis = Mathf.Abs(pos.x - unit.tilePos.x) + Mathf.Abs(pos.y - unit.tilePos.y);
            if (getDis > distance)
                continue;
            if (getDis >= minDis)
                continue;
            minDis = getDis;
            result = unit;
        }

        return result;
    }

    public Vector2Int FindEnemyNationTile(ENation myNation, Vector2Int pos, int distance)
    {
        //pos기준으로 distance만큼 떨어진 myNation세력과 적대 타일을 찾는 함수

        int minDis = int.MaxValue;
        int findCnt = (4 * distance * distance - 4 * distance + 1) / 2 + 1;
        Vector2Int result = pos;
        if (findCnt >= nNationTile.Count)
        {
            foreach (var pair in nNationTile)
            {
                ENation nation = pair.Value;
                Vector2Int tilePos = pair.Key;
                if (IsDecoTile(tilePos))
                    continue;
                if (nation == myNation)
                    continue;
                int getDis = Mathf.Abs(pos.x - tilePos.x) + Mathf.Abs(pos.y - tilePos.y);
                if (getDis > distance)
                    continue;
                if (getDis >= minDis)
                    continue;
                minDis = getDis;
                result = tilePos;
            }
        }
        else
        {
            for (int y = distance; y >= -distance; y--)
            {
                int w = Mathf.Abs(Mathf.Abs(y) - distance);
                for (int x = w; x >= -w; x--)
                {
                    Vector2Int posKey = new Vector2Int(pos.x + x, pos.y + y);
                    if (IsDecoTile(posKey))
                        continue;
                    if (nNationTile.ContainsKey(posKey) == false)
                        continue;

                    ENation nation = nNationTile[posKey];
                    if (nation == myNation)
                        continue;
                    int getDis = Mathf.Abs(pos.x - posKey.x) + Mathf.Abs(pos.y - posKey.y);
                    if (getDis >= minDis)
                        continue;
                    minDis = getDis;
                    result = posKey;
                }
            }
        }

        return result;
    }

    public Vector2Int FindAllEnemyNationTile(ENation myNation, Vector2Int pos)
    {
        //pos기준으로 myNation세력과 적대 타일을 찾는 함수

        int minDis = int.MaxValue;
        Vector2Int result = pos;
        foreach (var pair in nNationTile)
        {
            ENation nation = pair.Value;
            Vector2Int tilePos = pair.Key;
            if (IsDecoTile(tilePos))
                continue;
            if (nation == myNation)
                continue;
            int getDis = Mathf.Abs(pos.x - tilePos.x) + Mathf.Abs(pos.y - tilePos.y);
            if (getDis >= minDis)
                continue;
            minDis = getDis;
            result = tilePos;
        }

        return result;
    }

    public List<Vector2Int> GetCanMoveAroundPos(Vector2Int pos)
    {
        if(canMoveAroundList.ContainsKey(pos))
            return canMoveAroundList[pos];

        List<Vector2Int> result = new List<Vector2Int>();
        List<Vector2Int> aroundList = Calculator.GetAroundHexagonPos(pos.x, pos.y);
        for(int i = 0; i < aroundList.Count; i++)
        {
            TileBase tileBase = groundData.GetTile(new Vector3Int(aroundList[i].x, aroundList[i].y));
            if (tileBase == null)
                continue;
            if (IsDecoTile(aroundList[i]))
                continue;
            result.Add(aroundList[i]);
        }
        canMoveAroundList[pos] = result;

        return canMoveAroundList[pos];
    }

    public Vector3 GetTileWorldPos(Vector2Int pos)
    {
        return groundData.CellToWorld((Vector3Int)pos);
    }

    public Vector3Int GetWorldToTilePos(Vector3 worldPos)
    {
        return groundData.WorldToCell(worldPos);
    }

    public Build GetEnemyBuild(ENation myNation, Vector2Int pos)
    {
        if (nBuild.ContainsKey(pos))
        {
            Build build = nBuild[pos];
            if (build.IsDie())
                return null;
            if (build.nation == myNation)
                return null;
            return build;
        }
        return null;
    }

    public UnitScript GetEnemyUnit(ENation myNation, Vector2Int pos)
    {
        foreach (UnitScript unit in unitList)
        {
            if (unit.IsDie())
                continue;

            if (unit.nation == myNation)
                continue;

            if (unit.tilePos != pos)
                continue;

            return unit;
        }

        return null;
    }

    public bool GetEnemyTile(ENation myNation, Vector2Int pos)
    {
        if (nNationTile.ContainsKey(pos))
        {
            ENation nation = nNationTile[pos];
            if (nation == myNation)
                return false;
            return true;
        }
        return false;
    }

    public bool IsCanBuildTile(ENation myNation, Vector2Int pos)
    {
        if (nBuild.ContainsKey(pos))
            return false;

        if (nNationTile.ContainsKey(pos))
        {
            ENation nation = nNationTile[pos];
            if (nation == myNation)
                return true;
            return false;
        }
        return false;
    }

    public bool IsProductBuild(Vector2Int pos)
    {
        TileBase tileBase = editData.GetTile((Vector3Int)pos);
        if (tileBase == null)
            return false;
        if (tileBase.name == "Gold")
            return true;
        return false;
    }

    private bool IsDecoTile(Vector2Int pos)
    {
        TileBase tileBase = decoData.GetTile(new Vector3Int(pos.x, pos.y));
        if (tileBase == null)
            return false;

        switch(tileBase.name)
        {
            case "Stone0":
            case "Stone1":
            case "Wood0":
            case "Wood1":
                return true;
        }
        return false;
    }
}
