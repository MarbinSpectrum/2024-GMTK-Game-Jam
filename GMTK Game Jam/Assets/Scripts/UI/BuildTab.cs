using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class BuildTab : SerializedMonoBehaviour
{
    [SerializeField] private Image buildImg;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI buildName;
    [SerializeField] private TextMeshProUGUI canBuild;
    private BuildData buildData;

    private NoParaDel fun;

    public void SetUI(BuildData pBuildData, NoParaDel pFun)
    {
        buildData = pBuildData;
        buildImg.sprite = buildData.img;

        switch(buildData.eBuild)
        {
            case EBuild.SkeletonBarracks:
                buildName.text = "Skeleton Barracks";
                break;
            case EBuild.OrcBarracks:
                buildName.text = "Orc Barracks";
                break;
            case EBuild.DevilMonsterBarracks:
                buildName.text = "Devil Barracks";
                break;
            case EBuild.GolemBarracks:
                buildName.text = "Golem Barracks";
                break;


            case EBuild.GoldMine:
                buildName.text = "Gold Mine";
                break;
        }

        if (buildData.cost == 0)
            cost.text = "Free";
        else
            cost.text = string.Format("{0:N0} G", buildData.cost);

        fun = pFun;
    }

    private void Update()
    {
        if (buildData == null)
            return;

        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        if (filedObjMgr.hasPlayerBuild.ContainsKey(buildData.eBuild) == false)
            filedObjMgr.hasPlayerBuild[buildData.eBuild] = 0;
        int cnt = filedObjMgr.hasPlayerBuild[buildData.eBuild];

        if (buildData.maxCnt == 0)
            canBuild.text = string.Empty;
        else
            canBuild.text = string.Format("{0}/{1}", cnt, buildData.maxCnt);
    }

    public void CreateBuild()
    {
        fun?.Invoke();
    }
}
