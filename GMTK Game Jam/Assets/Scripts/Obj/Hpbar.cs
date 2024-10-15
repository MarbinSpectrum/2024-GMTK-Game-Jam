using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using MyLib;

public class Hpbar : SerializedMonoBehaviour
{
    [SerializeField] private Transform hpBar;
    [SerializeField] private GameObject barObj;



    public void SetHp(int nowHp,int maxHp)
    {
        if (nowHp == maxHp)
        {
            barObj.gameObject.SetActive(false);
        }
        else
        {
            barObj.gameObject.SetActive(true);
            float per = (float)nowHp / (float)maxHp;
            per = Mathf.Max(0, per);
            hpBar.localScale = new Vector3(per, hpBar.localScale.y, hpBar.localScale.z);
        }
    }
}
