using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class SimpleColorShader : SerializedMonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    private MaterialPropertyBlock mpb;

    public float LerpValue { get; private set; } = -1;
    [SerializeField, Range(0, 1)]
    private float lerpValue = 0;

    private void Update()
    {
        if (LerpValue == lerpValue)
            return;
        LerpValue = lerpValue;

        Set_Shader(LerpValue);
    }

    public void Set_Shader(float v)
    {
        mpb ??= new MaterialPropertyBlock();
        sprite.GetPropertyBlock(mpb);

        mpb.SetFloat("_LerpValue", v);
        sprite.SetPropertyBlock(mpb);
    }
}
