using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero0 : UnitScript
{
    private HeroBarracks barracks;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [SerializeField] private List<Sprite> heroImg = new List<Sprite>();

    public override void SetUnit(ENation pNation, Vector2Int pTilePos, Vector3 pPos)
    {
        base.SetUnit(pNation, pTilePos, pPos);

        int r = Random.Range(0, heroImg.Count);
        Sprite img = heroImg[r];
        spriteRenderer.sprite = img;
    }

    public override void RemoveUnit()
    {
        base.RemoveUnit();
        barracks.RemoveUnit(this);
    }

    public void SetBarracks(Build pBuild)
    {
        barracks = (HeroBarracks)pBuild;
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
