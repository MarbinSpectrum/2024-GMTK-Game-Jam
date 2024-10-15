using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Build Data", menuName = "Scriptable Object/Build Data", order = int.MaxValue)]
public class BuildData : ScriptableObject
{
    public int hp;
    public int cost;
    public EBuild eBuild;
    public Sprite img;
    public int maxCnt;
}