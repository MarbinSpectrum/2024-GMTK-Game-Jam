using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class Build : SerializedMonoBehaviour
{
    [SerializeField] private EBuild             build;
    [SerializeField] protected BuildData          buildData;
    [SerializeField] private SimpleColorShader  colorShader;
    [ReadOnly] public ENation          nation;
    [ReadOnly] public Vector2Int       tilePos;

    protected int nowHp;

    private IEnumerator hitCor;

    public virtual void SetBuild(ENation pNation, Vector2Int pTilePos, Vector3 pPos)
    {
        nation              = pNation;
        tilePos             = pTilePos;
        transform.position  = pPos;

        nowHp = buildData.hp;

        if (hitCor != null)
        {
            StopCoroutine(hitCor);
            hitCor = null;
        }

        colorShader.Set_Shader(0);
    }

    public virtual void BuildUpdate()
    {

    }

    public virtual void RemoveBuild()
    {
        if (hitCor != null)
        {
            StopCoroutine(hitCor);
            hitCor = null;
        }

        FiledObjMgr filedObjMgr = FiledObjMgr.Instance;
        filedObjMgr.RemoveBuild(build, this);

        if (nation == ENation.Devil)
        {
            filedObjMgr.hasPlayerBuild[build]--;
        }
    }

    public virtual void HitObj(int pDamage)
    {
        if (IsDie())
            return;
        nowHp -= pDamage;

        if (hitCor != null)
        {
            StopCoroutine(hitCor);
            hitCor = null;
        }
        hitCor = HitAni();
        IEnumerator HitAni()
        {
            for (int i = 0; i < 3; i++)
            {
                colorShader.Set_Shader(1);
                yield return new WaitForSeconds(0.1f);
                colorShader.Set_Shader(0);
                yield return new WaitForSeconds(0.1f);
            }
            hitCor = null;
        }
        StartCoroutine(HitAni());

        if (IsDie())
            RemoveBuild();
    }

    public virtual bool IsDie()
    {
        return (nowHp <= 0);
    }
}
