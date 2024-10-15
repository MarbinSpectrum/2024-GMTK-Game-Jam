using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class House : Build
{
    [SerializeField] protected Animator animator;
    public int getGold;
    [SerializeField] private Animation getGoldEff;
    [SerializeField] private TextMeshProUGUI goldText;

    public override void SetBuild(ENation pNation, Vector2Int pTilePos, Vector3 pPos)
    {
        base.SetBuild(pNation, pTilePos, pPos);
        goldText.text = string.Empty;
        getGoldEff.Stop();
    }

    public override void BuildUpdate()
    {
        if (nowHp > buildData.hp * 0.6f)
            animator.Play("Normal");
        else if (nowHp > 0)
            animator.Play("Fire");
        else
            animator.Play("Destroy");
    }

    public override void RemoveBuild()
    {
        GameMgr gameMgr = GameMgr.Instance;
        gameMgr.gameGold += getGold;
        getGoldEff.Play();
        goldText.text = string.Format("+{0:N0}G", getGold);
        SoundMgr.Instance.PlaySound(transform, ESound.Money);
    }
}
