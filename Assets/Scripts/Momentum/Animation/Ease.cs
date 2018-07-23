using System;
using UnityEngine;

public static class Ease
{
    private const float _HALF_PI = Mathf.PI / 2;

    public static float Linear(float ratio)
    {
        return ratio;
    }

    public static float In(float ratio)
    {
        return ratio * ratio * ratio;
    }

    public static float Out(float ratio)
    {
        var invRatio = ratio - 1.0f;
        return invRatio * invRatio * invRatio + 1;
    }

    public static float InOut(float ratio)
    {
        return Combined(In, Out, ratio);
    }

    public static float OutIn(float ratio)
    {
        return Combined(Out, In, ratio);
    }

    public static float InBack(float ratio)
    {
        var s = 1.70158f;
        return Mathf.Pow(ratio, 2f) * ((s + 1.0f) * ratio - s);
    }

    public static float OutBack(float ratio)
    {
        var invRatio = ratio - 1.0f;
        var s = 1.70158f;
        return Mathf.Pow(invRatio, 2f) * ((s + 1.0f) * invRatio + s) + 1.0f;
    }

    public static float InOutBack(float ratio)
    {
        return Combined(InBack, OutBack, ratio);
    }

    public static float OutInBack(float ratio)
    {
        return Combined(OutBack, InBack, ratio);
    }

    public static float InElastic(float ratio)
    {
        if (ratio == 0f || ratio == 1f) return ratio;
        else
        {
            var p = 0.3f;
            var s = p / 4.0f;
            var invRatio = ratio - 1f;
            return -1.0f * Mathf.Pow(2.0f, 10.0f * invRatio) * Mathf.Sin((invRatio - s) * (2.0f * Mathf.PI) / p);
        }
    }

    public static float OutElastic(float ratio)
    {
        if (ratio == 0 || ratio == 1) return ratio;
        else
        {
            var p = 0.3f;
            var s = p / 4.0f;
            return Mathf.Pow(2.0f, -10.0f * ratio) * Mathf.Sin((ratio - s) * (2.0f * Mathf.PI) / p) + 1f;
        }
    }

    public static float InOutElastic(float ratio)
    {
        return Combined(InElastic, OutElastic, ratio);
    }

    public static float OutInElastic(float ratio)
    {
        return Combined(OutElastic, InElastic, ratio);
    }

    public static float InBounce(float ratio)
    {
        return 1.0f - OutBounce(1.0f - ratio);
    }

    public static float OutBounce(float ratio)
    {
        float s = 7.5625f;
        float p = 2.75f;
        float l = 0.0f;
        if (ratio < (1.0f / p))
        {
            l = s * Mathf.Pow(ratio, 2);
        }
        else
        {
            if (ratio < (2.0f / p))
            {
                ratio -= 1.5f / p;
                l = s * Mathf.Pow(ratio, 2) + 0.75f;
            }
            else
            {
                if (ratio < 2.5f / p)
                {
                    ratio -= 2.25f / p;
                    l = s * Mathf.Pow(ratio, 2) + 0.9375f;
                }
                else
                {
                    ratio -= 2.625f / p;
                    l = s * Mathf.Pow(ratio, 2) + 0.984375f;
                }
            }
        }
        return l;
    }

    public static float InOutBounce(float ratio)
    {
        return Combined(InBounce, OutBounce, ratio);
    }

    public static float OutInBounce(float ratio)
    {
        return Combined(OutBounce, InBounce, ratio);
    }

    public static float InSine(float ratio)
    {
        return -Mathf.Cos(ratio * _HALF_PI) + 1f;
    }

    public static float OutSine(float ratio)
    {
        return Mathf.Sin(ratio * _HALF_PI);
    }

    public static float InOutSine(float ratio)
    {
        return -0.5f * (Mathf.Cos(Mathf.PI * ratio) - 1f);
    }

    public static float Combined(Func<float, float> startFunc, Func<float, float> endFunc, float ratio)
    {
        float r1 = (float)startFunc.DynamicInvoke(ratio * 2.0f);
        float r2 = (float)endFunc.DynamicInvoke((ratio - 0.5f) * 2.0f);

        if (ratio < 0.5f)
        {
            return 0.5f * r1;
        }
        else
        {
            return 0.5f * r2 + 0.5f;
        }
    }
}