using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class BuildMgr : SerializedMonoBehaviour
{
    private bool InitFlag = false;
    public static BuildMgr Instance { get; private set; }

    public bool buildMode { get; private set; } = false;

    [SerializeField] private BuildMenu  buildUI;
    [SerializeField] private GameObject selectEff;
    public Vector2Int selectPos { get; private set; }

    [SerializeField] private Dictionary<EBuild, BuildData> buildData = new Dictionary<EBuild, BuildData>();


    private void Init()
    {
        if (InitFlag)
            return;
        InitFlag = true;
        Instance = this;
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        GameMgr     gameMgr     = GameMgr.Instance;
        if (gameMgr.loadEndFlag == false)
            return;

        if(buildMode == false)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector2Int tilePos = (Vector2Int)filedObjMgr.GetWorldToTilePos(mousePos);
                Vector3 pos = filedObjMgr.GetTileWorldPos(tilePos);
                if (filedObjMgr.IsCanBuildTile(ENation.Devil,tilePos))
                {
                    buildMode = true;
                    selectPos = tilePos;
                    selectEff.gameObject.SetActive(true);
                    selectEff.transform.position = pos;
                    buildUI.gameObject.SetActive(true);
                    if (filedObjMgr.IsProductBuild
                        (tilePos))
                        buildUI.SetProductBuild();
                    else
                        buildUI.SetWarBuild();
                }
            }
        }
    }

    public void CloseBuildUI()
    {
        selectEff.gameObject.SetActive(false);
        buildUI.gameObject.SetActive(false);
        buildMode = false;
    }

    public BuildData GetBuildData(EBuild eBuild)
    {
        return buildData[eBuild];
    }
}
