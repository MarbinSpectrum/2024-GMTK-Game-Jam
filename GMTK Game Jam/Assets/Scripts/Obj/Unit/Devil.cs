using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MyLib;

public class Devil : UnitScript
{
    [SerializeField] private float      atkDelay;
    [SerializeField] private Hpbar      hpBar;
    [SerializeField] private GameObject beam;

    private IEnumerator atkCor;

    public override void SetUnit(ENation pNation, Vector2Int pTilePos, Vector3 pPos)
    {
        base.SetUnit(pNation, pTilePos, pPos);
        unitAction = EUnitAction.None;

        if(atkCor != null)
        {
            StopCoroutine(atkCor);
            atkCor = null;
        }
    }

    public override void RemoveUnit()
    {
        GameMgr gameMgr = GameMgr.Instance;
        gameMgr.GameOver();
    }

    protected override void UnitActionUpdate()
    {
        if (IsDie())
            return;

        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        switch (unitAction)
        {
            case EUnitAction.None:
                {
                    if (notFindDelay > 0)
                    {
                        notFindDelay -= Time.deltaTime;
                    }
                    else
                    {
                        notFindDelay = FIND_DELAY;

                        attackBuild = filedObjMgr.FindEnemyBuild(nation, tilePos, 5);
                        attackUnit = filedObjMgr.FindEnemyUnit(nation, tilePos, 5);

                        if (attackUnit != null)
                            unitAction = EUnitAction.AttackUnit;
                        else if (attackBuild != null)
                            unitAction = EUnitAction.AttackBuild;
                    }
                }
                break;
            case EUnitAction.AttackBuild:
                {
                    if (attakDelay > atkDelay)
                    {
                        attakDelay = 0;
                        if (atkCor != null)
                        {
                            StopCoroutine(atkCor);
                            atkCor = null;
                        }

                        IEnumerator RunEffect()
                        {
                            GameObject newBeam = Instantiate(beam, transform.position, Quaternion.identity);
                            StartCoroutine(Action2D.MoveTo(newBeam.transform, attackBuild.transform.position, 0.5f));
                            yield return new WaitForSeconds(1f);

                            Destroy(newBeam);
                            unitAction = EUnitAction.None;
                            attackBuild.HitObj(nowAtk);
                            SoundMgr.Instance.PlaySound(transform, ESound.Attack);
                        }
                        atkCor = RunEffect();
                        StartCoroutine(atkCor);
                    }
                    else
                    {
                        attakDelay += Time.deltaTime;
                    }
                }
                break;
            case EUnitAction.AttackUnit:
                {
                    if (attakDelay > atkDelay)
                    {
                        attakDelay = 0;
                        if (atkCor != null)
                        {
                            StopCoroutine(atkCor);
                            atkCor = null;
                        }

                        IEnumerator RunEffect()
                        {
                            GameObject newBeam = Instantiate(beam, transform.position, Quaternion.identity);
                            StartCoroutine(Action2D.MoveTo(newBeam.transform, attackUnit.transform.position, 0.5f));
                            yield return new WaitForSeconds(1f);

                            Destroy(newBeam);
                            unitAction = EUnitAction.None;
                            attackUnit.HitObj(nowAtk);
                            SoundMgr.Instance.PlaySound(transform, ESound.Attack);
                        }
                        atkCor = RunEffect();
                        StartCoroutine(atkCor);
                    }
                    else
                    {
                        attakDelay += Time.deltaTime;
                    }
                }
                break;
        }
    }

    public override void HitObj(int pDamage)
    {
        base.HitObj(pDamage);
        hpBar.SetHp(nowHp, unitData.hp);
    }
}
