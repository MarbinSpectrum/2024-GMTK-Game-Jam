using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class BuildMenu : SerializedMonoBehaviour
{
    [SerializeField] private BuildTab       buildTab;
    [SerializeField] private RectTransform  contents;

    private List<BuildTab> buildTabs = new List<BuildTab>();

    public void SetWarBuild()
    {
        List<EBuild> eBuilds = new List<EBuild>();
        eBuilds.Add(EBuild.SkeletonBarracks);
        eBuilds.Add(EBuild.OrcBarracks);
        eBuilds.Add(EBuild.DevilMonsterBarracks);
        eBuilds.Add(EBuild.GolemBarracks);

        SetMenu(eBuilds);
    }

    public void SetProductBuild()
    {
        List<EBuild> eBuilds = new List<EBuild>();
        eBuilds.Add(EBuild.GoldMine);

        SetMenu(eBuilds);
    }

    private void SetMenu(List<EBuild> eBuilds)
    {
        BuildMgr buildMgr = BuildMgr.Instance;
        GameMgr gameMgr = GameMgr.Instance;
        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;

        foreach (BuildTab tabs in buildTabs)
            tabs.gameObject.SetActive(false);

        for (int i = 0; i < eBuilds.Count; i++)
        {
            if (buildTabs.Count <= i)
                buildTabs.Add(Instantiate(buildTab, contents));
            BuildData buildData = buildMgr.GetBuildData(eBuilds[i]);
            buildTabs[i].SetUI(buildData, () =>
            {
                FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
                if (filedObjMgr.hasPlayerBuild.ContainsKey(buildData.eBuild) == false)
                    filedObjMgr.hasPlayerBuild[buildData.eBuild] = 0;
                int cnt = filedObjMgr.hasPlayerBuild[buildData.eBuild];

                if (buildData.maxCnt != 0 && buildData.maxCnt <= cnt)
                    return;
                if (gameMgr.gameGold < buildData.cost)
                    return;
                if (filedObjMgr.IsCanBuildTile(ENation.Devil, buildMgr.selectPos) == false)
                    return;

                gameMgr.gameGold -= buildData.cost;
                filedObjMgr.hasPlayerBuild[buildData.eBuild]++;
                Vector3 worldPos = filedObjMgr.GetTileWorldPos(buildMgr.selectPos);
                Build build = filedObjMgr.CreateBuild(buildMgr.selectPos, buildData.eBuild);
                build.SetBuild(ENation.Devil, buildMgr.selectPos, worldPos);
                buildMgr.CloseBuildUI();
                SoundMgr.Instance.PlaySound(Camera.main.transform, ESound.Create);
            });
            buildTabs[i].gameObject.SetActive(true);
        }
    }
}
