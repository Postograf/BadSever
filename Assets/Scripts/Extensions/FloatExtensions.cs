using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static int Sign(this float f)
    {
        return f == 0 ? 0 : f < 0 ? -1 : 1;
    }
}
