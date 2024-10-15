using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameMgr : SerializedMonoBehaviour
{
    private bool startInit = false;
    private bool InitFlag = false;
    public static GameMgr Instance { get; private set; }

    public bool loadEndFlag { get; private set; } = false;
    public bool gameOverFlag { get; private set; } = false;
    public int  gameGold;

    public bool gameEndFlag { get; private set; } = false;

    [SerializeField] private int startGold;

    [SerializeField] private Animator loadAni;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject gameEnd;

    private void Init()
    {
        if (InitFlag)
            return;
        InitFlag = true;
        Instance = this;
    }
    private void StartInit()
    {
        if (startInit)
            return;

        startInit = true;
        CameraMgr       cameraControl   = CameraMgr.Instance;
        FiledObjMgr     filedObjMgr     = FiledObjMgr.Instance;

        gameOverFlag = false;
        loadEndFlag = false;
        gameEndFlag = false;

        loadAni.Play("Idle");

        StartCoroutine(CreateObj());
        IEnumerator CreateObj()
        {
            if (filedObjMgr.house0Pos != null)
            {
                foreach (var pair in filedObjMgr.house0Pos)
                {
                    Vector3 house0WorldPos = pair.Value.tileWorldPos;
                    Vector2Int pos = pair.Value.tilePos;

                    Build build = filedObjMgr.CreateBuild(pos, EBuild.House0);
                    build.SetBuild(ENation.Human, pos, house0WorldPos);
                    yield return null;
                }
            }

            if (filedObjMgr.house1Pos != null)
            {
                foreach (var pair in filedObjMgr.house1Pos)
                {
                    Vector3 house0WorldPos = pair.Value.tileWorldPos;
                    Vector2Int pos = pair.Value.tilePos;

                    Build build = filedObjMgr.CreateBuild(pos, EBuild.House1);
                    build.SetBuild(ENation.Human, pos, house0WorldPos);
                    yield return null;
                }
            }

            if (filedObjMgr.playerStartPos != null)
            {
                Vector3 playerWorldPos = filedObjMgr.playerStartPos.tileWorldPos;
                Vector2Int pos = filedObjMgr.playerStartPos.tilePos;

                cameraControl.SetPos(new Vector2(playerWorldPos.x, playerWorldPos.y));

                UnitScript devil = filedObjMgr.CreateUnit(EUnit.Devil);
                devil.SetUnit(ENation.Devil, pos, playerWorldPos);
            }

            if (filedObjMgr.humanStartPos0 != null)
            {
                foreach (var pair in filedObjMgr.humanStartPos0)
                {
                    Vector3 humanWorldPos = pair.Value.tileWorldPos;
                    Vector2Int pos = pair.Value.tilePos;

                    Build build = filedObjMgr.CreateBuild(pos, EBuild.Hero0Barracks);
                    build.SetBuild(ENation.Human, pos, humanWorldPos);
                    yield return null;
                }
            }

            if (filedObjMgr.humanStartPos1 != null)
            {
                foreach (var pair in filedObjMgr.humanStartPos1)
                {
                    Vector3 humanWorldPos = pair.Value.tileWorldPos;
                    Vector2Int pos = pair.Value.tilePos;

                    Build build = filedObjMgr.CreateBuild(pos, EBuild.Hero1Barracks);
                    build.SetBuild(ENation.Human, pos, humanWorldPos);
                    yield return null;
                }
            }

            if (filedObjMgr.humanStartPos2 != null)
            {
                foreach (var pair in filedObjMgr.humanStartPos2)
                {
                    Vector3 humanWorldPos = pair.Value.tileWorldPos;
                    Vector2Int pos = pair.Value.tilePos;

                    Build build = filedObjMgr.CreateBuild(pos, EBuild.Hero2Barracks);
                    build.SetBuild(ENation.Human, pos, humanWorldPos);
                    yield return null;
                }
            }

            loadAni.Play("End");

            loadEndFlag = true;
        }

        gameGold = startGold;
    }

    private void Update()
    {
        if (loadEndFlag == false)
            return;

        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        Vector2Int pos = new Vector2Int(-10000000, -10000000);
        Vector2Int lastFind = filedObjMgr.FindAllEnemyNationTile(ENation.Devil, pos);
        if (lastFind != pos)
            return;
        if (gameEndFlag)
            return;
        gameEndFlag = true;
        GameEnd();
    }

    public void GameOver()
    {
        gameOverFlag = true;
        gameOver.SetActive(true);
    }

    private void GameEnd()
    {
        gameEnd.SetActive(true);
    }

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        StartInit();
    }


}
