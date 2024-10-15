using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBarracks : Build
{
    public List<EUnit> unit;
    public int maxUnit;
    [SerializeField] private Hpbar hpBar;

    private bool actbrracks;
    [SerializeField] private int actRange;
    private int nowUnit;

    public float delay;
    private float nowTime;

    public override void SetBuild(ENation pNation, Vector2Int pTilePos, Vector3 pPos)
    {
        base.SetBuild(pNation, pTilePos, pPos);
        nowTime = 0;
        actbrracks = false;
    }

    public override void BuildUpdate()
    {
        if (nowUnit >= maxUnit)
            return;

        nowTime += Time.deltaTime;
        if (nowTime > delay)
        {
            if (actbrracks == false)
            {
                FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
                UnitScript unitScript = filedObjMgr.FindEnemyUnit(nation, tilePos, actRange);
                if (unitScript != null)
                {
                    actbrracks = true;
                    CreateUnit();
                }
            }
            else
            {
                CreateUnit();
            }
            nowTime = 0;
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
        int r = Random.Range(0, unit.Count);
        EUnit eUnit = unit[r];
        UnitScript unitObj = filedObjMgr.CreateUnit(eUnit);
        switch (eUnit)
        {
            case EUnit.Hero0:
                {
                    Hero0 unitScript = (Hero0)unitObj;
                    unitScript.SetBarracks(this);
                }
                break;
            case EUnit.Hero1:
                {
                    Hero1 unitScript = (Hero1)unitObj;
                    unitScript.SetBarracks(this);
                }
                break;
            case EUnit.Hero2:
                {
                    Hero2 unitScript = (Hero2)unitObj;
                    unitScript.SetBarracks(this);
                }
                break;
        }

        unitObj.SetUnit(nation, tilePos, transform.position);
    }

    public void RemoveUnit(UnitScript pUnit)
    {
        if (nowUnit <= 0)
            return;
        nowUnit--;
        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        filedObjMgr.RemoveUnit(pUnit.GetEUnit(), pUnit);
    }

    public override void HitObj(int pDamage)
    {
        base.HitObj(pDamage);

        hpBar.SetHp(nowHp, buildData.hp);
    }
}
