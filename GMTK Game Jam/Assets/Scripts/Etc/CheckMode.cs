using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMode
{
    public static bool IsDropMode()
    {
        return false;
    }

    public static bool IsWindow()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
             Application.platform == RuntimePlatform.WindowsEditor)
            return true;
        return false;
    }
}
