using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldMine : Build
{
    public int getGold;
    [SerializeField] private Hpbar              hpBar;
    [SerializeField] private Animation          getGoldEff;
    [SerializeField] private TextMeshProUGUI    goldText;
    public float delay;
    private float nowTime;

    public override void SetBuild(ENation pNation, Vector2Int pTilePos, Vector3 pPos)
    {
        base.SetBuild(pNation, pTilePos, pPos);
        nowTime = 0;
        goldText.text = string.Empty;
        getGoldEff.Stop();
        hpBar.SetHp(1, 1);
    }

    public override void BuildUpdate()
    {
        nowTime += Time.deltaTime;
        if (nowTime > delay)
        {
            nowTime = 0;
            GameMgr gameMgr = GameMgr.Instance;
            gameMgr.gameGold += getGold;
            getGoldEff.Play();
            goldText.text = string.Format("+{0:N0}G", getGold);
            SoundMgr.Instance.PlaySound(transform, ESound.Money);
        }
    }

    public override void HitObj(int pDamage)
    {
        base.HitObj(pDamage);

        hpBar.SetHp(nowHp, buildData.hp);
    }
}
