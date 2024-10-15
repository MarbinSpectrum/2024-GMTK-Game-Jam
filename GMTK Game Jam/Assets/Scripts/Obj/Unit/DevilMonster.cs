using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DevilMonster : UnitScript
{
    private DevilMonsterBarracks barracks;
    [SerializeField] private Animator animator;

    public override void RemoveUnit()
    {
        base.RemoveUnit();
        barracks.RemoveUnit(this);
    }

    public void SetBarracks(Build pBuild)
    {
        barracks = (DevilMonsterBarracks)pBuild;
    }

    public override void UnitUpdate()
    {
        base.UnitUpdate();

        MoveAni();
    }

    public void MoveAni()
    {
        if (moveDic == 1)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (moveDic == -1)
            transform.localScale = new Vector3(+Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        switch (unitAction)
        {
            case EUnitAction.Move:
                {
                    if (nowMove)
                        animator.Play("Move");
                    else
                        animator.Play("Idle");
                }
                break;
            case EUnitAction.AttackBuild:
            case EUnitAction.AttackUnit:
                {
                    animator.Play("Attack");
                }
                break;
            default:
                animator.Play("Idle");
                break;
        }
    }
}
