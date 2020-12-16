using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitAttribute : PropertyAttribute
{
    public enum LimitMode
    {
        LimitLower,
        LimitUpper,
        LimitBoth
    }

    private readonly LimitMode limitMode;
    private readonly int lowerLimit;
    private readonly int upperLimit;

    public LimitAttribute(int lowerLimit) : this(LimitMode.LimitLower, lowerLimit, int.MaxValue) { }
    public LimitAttribute(int lowerLimit, int upperLimit) : this(LimitMode.LimitBoth, lowerLimit, upperLimit) { }

    public LimitAttribute(LimitMode limitMode, int lowerLimit, int upperLimit)
    {
        this.lowerLimit = lowerLimit;
        this.upperLimit = upperLimit;
        this.limitMode = limitMode;
    }

    public int Limit(int value)
    {
        switch (limitMode)
        {
            case LimitMode.LimitUpper:
                return Mathf.Clamp(value, lowerLimit, int.MaxValue);
            case LimitMode.LimitLower:
                return Mathf.Clamp(value, -int.MaxValue, upperLimit);
            case LimitMode.LimitBoth:
                return value;
        }

        return value;
    }
}
