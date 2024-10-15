using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class ShowConquest : SerializedMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI conquestText;
    private int conquestFlag = -1;
    private void Update()
    {
        if (GameMgr.Instance == null)
            return;
        if (conquestFlag == FiledObjMgr.Instance.enemyTileCnt)
            return;

        conquestFlag = FiledObjMgr.Instance.enemyTileCnt;
        float per = (float)FiledObjMgr.Instance.enemyTileCnt / (float)FiledObjMgr.Instance.enemyTileMax;
        conquestText.text = string.Format("{0}%", 100 - (int)(per*100));
    }
}
