using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tweens.Plugins
{
    public enum T_Status
    {
        Normal,
        OnEnable,
        OnDisable,
    }

    public enum T_TypePosPath
    {
        DoPositionPath,
        DoLocalPositionPath,
    }

    public enum T_TypeSequences
    {
        Append,
        Join,
    }

    public enum T_TypeScale
    {
        Normal,
        Punch,
        Shake,
    }
}
