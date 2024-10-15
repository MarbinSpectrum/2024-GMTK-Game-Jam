using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBarracks : Build
{
    public EUnit unit;
    public int maxUnit;
    [SerializeField] private Hpbar hpBar;

    private int nowUnit;

    public float delay;
    private float nowTime;

    public override void SetBuild(ENation pNation, Vector2Int pTilePos, Vector3 pPos)
    {
        base.SetBuild(pNation, pTilePos, pPos);
        hpBar.SetHp(1, 1);
        nowTime = 0;
    }

    public override void BuildUpdate()
    {
        if (nowUnit >= maxUnit)
            return;

        nowTime += Time.deltaTime;
        if (nowTime > delay)
        {
            nowTime = 0;
            CreateUnit();
        }
    }

    public void CreateUnit()
    {
        if (nowUnit >= maxUnit)
            return;
        if (IsDie())
            return;

        nowUnit++;
        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        Golem unitScript = (Golem)filedObjMgr.CreateUnit(unit);
        unitScript.SetUnit(nation, tilePos, transform.position);
        unitScript.SetBarracks(this);
    }

    public void RemoveUnit(UnitScript pUnit)
    {
        if (nowUnit <= 0)
            return;
        nowUnit--;
        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        filedObjMgr.RemoveUnit(unit, pUnit);
    }

    public override void HitObj(int pDamage)
    {
        base.HitObj(pDamage);

        hpBar.SetHp(nowHp, buildData.hp);
    }
}
