using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUnit
{
    Devil           = 50,
    Skeleton        = 100,
    Orc             = 200,
    DevilMonster    = 210,
    Golem           = 220,
    Hero0           = 300,
    Hero1           = 400,
    Hero2           = 500,
}

public enum EBuild
{
    GoldMine            = 50,
    SkeletonBarracks    = 100,
    OrcBarracks         = 200,
    DevilMonsterBarracks= 210,
    GolemBarracks       = 220,
    Hero0Barracks       = 300,
    Hero1Barracks       = 400,
    Hero2Barracks       = 401,
    House0              = 500,
    House1              = 600,
}

public enum ENation
{
    None  = 0,
    Devil = 100,
    Human = 200,
}

public enum EUnitAction
{
    None        = 0,
    Move        = 50,
    AttackUnit  = 100,
    AttackBuild = 200,
    AttackTile  = 300,
}

public enum ESound
{
    None        = 0,
    Attack      = 1,
    Create      = 2,
    Money       = 3,
}