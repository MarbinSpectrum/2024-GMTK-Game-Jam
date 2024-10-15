using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class ShowGold : SerializedMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    private int goldFlag = -1;
    private void Update()
    {
        if (GameMgr.Instance == null)
            return;
        if (goldFlag == GameMgr.Instance.gameGold)
            return;

        goldFlag = GameMgr.Instance.gameGold;
        goldText.text = string.Format("{0:N0} G", goldFlag);
    }
}
