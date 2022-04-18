using UnityEngine;

namespace SZUtilities.Math
{
    public static class Easing
    {
        public enum Ease
        {
            Linear = 0,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InSine,
            OutSine,
            InOutSine,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InBounce,
            OutBounce,
            InOutBounce,
            InBack,
            OutBack,
            InOutBack,
            InElastic,
            OutElastic,
            InOutElastic,
            Spring,
        }

        public static float Linear(float value)
        {
            return Mathf.Lerp(0f, 1f, value);
        }

        public static float Spring(float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return value;
        }

        public static float InQuad(float value)
        {
            return value * value;
        }

        public static float OutQuad(float value)
        {
            return -1f * value * (value - 2);
        }

        public static float InOutQuad(float value)
        {
            value /= .5f;
            if (value < 1) return 0.5f * value * value;
            value--;
            return -1f * 0.5f * (value * (value - 2) - 1);
        }

        public static float InCubic(float value)
        {
            return value * value * value;
        }

        public static float OutCubic(float value)
        {
            value--;

            return (value * value * value + 1);
        }

        public static float InOutCubic(float value)
        {
            value /= .5f;

            if (value < 1) return 0.5f * value * value * value;
            value -= 2;
            return 0.5f * (value * value * value + 2);
        }

        public static float InQuart(float value)
        {
            return value * value * value * value;
        }

        public static float OutQuart(float value)
        {
            value--;

            return -1f * (value * value * value * value - 1);
        }

        public static float InOutQuart(float value)
        {
            value /= .5f;

            if (value < 1) return 0.5f * value * value * value * value;
            value -= 2;
            return -1f * 0.5f * (value * value * value * value - 2);
        }

        public static float InQuint(float value)
        {
            return value * value * value * value * value;
        }

        public static float OutQuint(float value)
        {
            value--;

            return (value * value * value * value * value + 1);
        }

        public static float InOutQuint(float value)
        {
            value /= .5f;

            if (value < 1) return 0.5f * value * value * value * value * value;
            value -= 2;
            return 0.5f * (value * value * value * value * value + 2);
        }

        public static float InSine(float value)
        {
            return -1f * Mathf.Cos(value * (Mathf.PI * 0.5f)) + 1f;
        }

        public static float OutSine(float value)
        {
            return Mathf.Sin(value * (Mathf.PI * 0.5f));
        }

        public static float InOutSine(float value)
        {
            return -1f * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1);
        }

        public static float InExpo(float value)
        {
            return Mathf.Pow(2, 10 * (value - 1));
        }

        public static float OutExpo(float value)
        {
            return (-Mathf.Pow(2, -10 * value) + 1);
        }

        public static float InOutExpo(float value)
        {
            value /= .5f;

            if (value < 1) return 0.5f * Mathf.Pow(2, 10 * (value - 1));
            value--;
            return 0.5f * (-Mathf.Pow(2, -10 * value) + 2);
        }

        public static float InCirc(float value)
        {
            return -1f * (Mathf.Sqrt(1 - value * value) - 1);
        }

        public static float OutCirc(float value)
        {
            value--;

            return Mathf.Sqrt(1 - value * value);
        }

        public static float InOutCirc(float value)
        {
            value /= .5f;

            if (value < 1) return -1f * 0.5f * (Mathf.Sqrt(1 - value * value) - 1);
            value -= 2;
            return 0.5f * (Mathf.Sqrt(1 - value * value) + 1);
        }

        public static float InBounce(float value)
        {
            float d = 1f;
            return 1f - OutBounce(d - value);
        }

        public static float OutBounce(float value)
        {
            value /= 1f;

            if (value < (1 / 2.75f))
            {
                return (7.5625f * value * value);
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return (7.5625f * (value) * value + .75f);
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return (7.5625f * (value) * value + .9375f);
            }
            else
            {
                value -= (2.625f / 2.75f);
                return (7.5625f * (value) * value + .984375f);
            }
        }

        public static float InOutBounce(float value)
        {
            float d = 1f;
            if (value < d * 0.5f) return InBounce(value * 2) * 0.5f;
            else return OutBounce(value * 2 - d) * 0.5f + 0.5f;
        }

        public static float InBack(float value)
        {
            value /= 1;
            float s = 1.70158f;
            return (value) * value * ((s + 1) * value - s);
        }

        public static float OutBack(float value)
        {
            float s = 1.70158f;

            value = (value) - 1;
            return ((value) * value * ((s + 1) * value + s) + 1);
        }

        public static float InOutBack(float value)
        {
            float s = 1.70158f;

            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return 0.5f * (value * value * (((s) + 1) * value - s));
            }

            value -= 2;
            s *= (1.525f);
            return 0.5f * ((value) * value * (((s) + 1) * value + s) + 2);
        }

        public static float InElastic(float value)
        {
            float d = 1f;
            float p = d * .3f;
            float s = p / 4;
            float a = 1f;

            if (value <= 0) return 0f;

            if (value >= 1) return 1f;

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p));
        }

        public static float OutElastic(float value)
        {
            float d = 1f;
            float p = d * .3f;
            float s = p * 0.25f;
            float a = 1f;

            if (value <= 0f) return 0f;

            if (value >= 1f) return 1f;

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + 1f + 0f);
        }

        public static float InOutElastic(float value)
        {
            float d = 1f;
            float p = d * .3f;
            float s = p / 4;
            float a = 1f;

            if (value <= 0) return 0f;

            if ((value /= 0.5f) >= 2f) return 1f;

            if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p));
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + 1f;
        }

        // Derivatives
        
        public static float LinearD(float value)
        {
            return 1f - 0f;
        }

        public static float InQuadD(float value)
        {
            return 2f * value;
        }

        public static float OutQuadD(float value)
        {
            return -1f * value - (value - 2);
        }

        public static float InOutQuadD(float value)
        {
            value /= .5f;


            if (value < 1)
            {
                return value;
            }

            value--;

            return (1 - value);
        }

        public static float InCubicD(float value)
        {
            return 3f * value * value;
        }

        public static float OutCubicD(float value)
        {
            value--;

            return 3f * value * value;
        }

        public static float InOutCubicD(float value)
        {
            value /= .5f;


            if (value < 1)
            {
                return (3f / 2f) * value * value;
            }

            value -= 2;

            return (3f / 2f) * value * value;
        }

        public static float InQuartD(float value)
        {
            return 4f * value * value * value;
        }

        public static float OutQuartD(float value)
        {
            value--;

            return -4f * value * value * value;
        }

        public static float InOutQuartD(float value)
        {
            value /= .5f;


            if (value < 1)
            {
                return 2f * value * value * value;
            }

            value -= 2;

            return -2f * value * value * value;
        }

        public static float InQuintD(float value)
        {
            return 5f * value * value * value * value;
        }

        public static float OutQuintD(float value)
        {
            value--;

            return 5f * value * value * value * value;
        }

        public static float InOutQuintD(float value)
        {
            value /= .5f;


            if (value < 1)
            {
                return (5f / 2f) * value * value * value * value;
            }

            value -= 2;

            return (5f / 2f) * value * value * value * value;
        }

        public static float InSineD(float value)
        {
            return 0.5f * Mathf.PI * Mathf.Sin(0.5f * Mathf.PI * value);
        }

        public static float OutSineD(float value)
        {
            return (Mathf.PI * 0.5f) * Mathf.Cos(value * (Mathf.PI * 0.5f));
        }

        public static float InOutSineD(float value)
        {
            return 0.5f * Mathf.PI * Mathf.Cos(Mathf.PI * value);
        }

        public static float InExpoD(float value)
        {
            return (10f * Const.NaturalLogOf2 * Mathf.Pow(2f, 10f * (value - 1)));
        }

        public static float OutExpoD(float value)
        {
            return 5f * Const.NaturalLogOf2 * Mathf.Pow(2f, 1f - 10f * value);
        }

        public static float InOutExpoD(float value)
        {
            value /= .5f;


            if (value < 1)
            {
                return 5f * Const.NaturalLogOf2 * Mathf.Pow(2f, 10f * (value - 1));
            }

            value--;

            return (5f * Const.NaturalLogOf2 * 1f) / (Mathf.Pow(2f, 10f * value));
        }

        public static float InCircD(float value)
        {
            return (1f * value) / Mathf.Sqrt(1f - value * value);
        }

        public static float OutCircD(float value)
        {
            value--;

            return (-1f * value) / Mathf.Sqrt(1f - value * value);
        }

        public static float InOutCircD(float value)
        {
            value /= .5f;


            if (value < 1)
            {
                return (1f * value) / (2f * Mathf.Sqrt(1f - value * value));
            }

            value -= 2;

            return (-1f * value) / (2f * Mathf.Sqrt(1f - value * value));
        }

        public static float InBounceD(float value)
        {
            return OutBounceD(1f - value);
        }

        public static float OutBounceD(float value)
        {
            value /= 1f;


            if (value < (1 / 2.75f))
            {
                return 2f * 7.5625f * value;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return 2f * 7.5625f * value;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return 2f * 7.5625f * value;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return 2f * 7.5625f * value;
            }
        }

        public static float InOutBounceD(float value)
        {
            if (value < 0.5f)
            {
                return InBounceD(value * 2) * 0.5f;
            }
            else
            {
                return OutBounceD(value * 2 - 1f) * 0.5f;
            }
        }

        public static float InBackD(float value)
        {
            float s = 1.70158f;

            return 3f * (s + 1f) * value * value - 2f * s * value;
        }

        public static float OutBackD(float value)
        {
            float s = 1.70158f;

            value = (value) - 1;

            return ((s + 1f) * value * value + 2f * value * ((s + 1f) * value + s));
        }

        public static float InOutBackD(float value)
        {
            float s = 1.70158f;

            value /= .5f;

            if ((value) < 1)
            {
                s *= (1.525f);
                return 0.5f * (s + 1) * value * value + value * ((s + 1f) * value - s);
            }

            value -= 2;
            s *= (1.525f);
            return 0.5f * ((s + 1) * value * value + 2f * value * ((s + 1f) * value + s));
        }

        public static float InElasticD(float value)
        {
            float d = 1f;
            float p = d * .3f;
            float s = p / 4;
            float a = 1f;

            float c = 2 * Mathf.PI;

            // From an online derivative calculator, kinda hoping it is right.
            return ((-a) * d * c * Mathf.Cos((c * (d * (value - 1f) - s)) / p)) / p -
                   5f * Const.NaturalLogOf2 * a * Mathf.Sin((c * (d * (value - 1f) - s)) / p) *
                   Mathf.Pow(2f, 10f * (value - 1f) + 1f);
        }

        public static float OutElasticD(float value)
        {
            float d = 1f;
            float p = d * .3f;
            float s = p * 0.25f;
            float a = 1f;

            return (a * Mathf.PI * d * Mathf.Pow(2f, 1f - 10f * value) *
                    Mathf.Cos((2f * Mathf.PI * (d * value - s)) / p)) / p - 5f * Const.NaturalLogOf2 * a *
                   Mathf.Pow(2f, 1f - 10f * value) * Mathf.Sin((2f * Mathf.PI * (d * value - s)) / p);
        }

        public static float InOutElasticD(float value)
        {
            float d = 1f;
            float p = d * .3f;
            float s = p / 4;
            float a = 1f;

            if (value < 1)
            {
                value -= 1;

                return -5f * Const.NaturalLogOf2 * a * Mathf.Pow(2f, 10f * value) * Mathf.Sin(2 * Mathf.PI * (d * value - 2f) / p) -
                       a * Mathf.PI * d * Mathf.Pow(2f, 10f * value) * Mathf.Cos(2 * Mathf.PI * (d * value - s) / p) / p;
            }

            value -= 1;

            return a * Mathf.PI * d * Mathf.Cos(2f * Mathf.PI * (d * value - s) / p) / (p * Mathf.Pow(2f, 10f * value)) -
                   5f * Const.NaturalLogOf2 * a * Mathf.Sin(2f * Mathf.PI * (d * value - s) / p) / (Mathf.Pow(2f, 10f * value));
        }

        public static float SpringD(float value)
        {
            value = Mathf.Clamp01(value);


            // Damn... Thanks http://www.derivative-calculator.net/
            return (6f * (1f - value) / 5f + 1f) * (-2.2f * Mathf.Pow(1f - value, 1.2f) *
                                                    Mathf.Sin(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + Mathf.Pow(1f - value, 2.2f) *
                                                    (Mathf.PI * (2.5f * value * value * value + 0.2f) + 7.5f * Mathf.PI * value * value * value) *
                                                    Mathf.Cos(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + 1f) -
                   6f * (Mathf.Pow(1 - value, 2.2f) * Mathf.Sin(Mathf.PI * value * (2.5f * value * value * value + 0.2f)) + value
                         / 5f);
        }

        public delegate float EasingFunction(float v);

        public static EasingFunction Get(Ease ease)
        {
            switch (ease)
            {
                case Ease.Linear:       return Linear;
                case Ease.InQuad:       return InQuad;
                case Ease.OutQuad:      return OutQuad;
                case Ease.InOutQuad:    return InOutQuad;
                case Ease.InCubic:      return InCubic;
                case Ease.OutCubic:     return OutCubic;
                case Ease.InOutCubic:   return InOutCubic;
                case Ease.InQuart:      return InQuart;
                case Ease.OutQuart:     return OutQuart;
                case Ease.InOutQuart:   return InOutQuart;
                case Ease.InQuint:      return InQuint;
                case Ease.OutQuint:     return OutQuint;
                case Ease.InOutQuint:   return InOutQuint;
                case Ease.InSine:       return InSine;
                case Ease.OutSine:      return OutSine;
                case Ease.InOutSine:    return InOutSine;
                case Ease.InExpo:       return InExpo;
                case Ease.OutExpo:      return OutExpo;
                case Ease.InOutExpo:    return InOutExpo;
                case Ease.InCirc:       return InCirc;
                case Ease.OutCirc:      return OutCirc;
                case Ease.InOutCirc:    return InOutCirc;
                case Ease.InBounce:     return InBounce;
                case Ease.OutBounce:    return OutBounce;
                case Ease.InOutBounce:  return InOutBounce;
                case Ease.InBack:       return InBack;
                case Ease.OutBack:      return OutBack;
                case Ease.InOutBack:    return InOutBack;
                case Ease.InElastic:    return InElastic;
                case Ease.OutElastic:   return OutElastic;
                case Ease.InOutElastic: return InOutElastic;
                case Ease.Spring:       return Spring;
            }

            return null;
        }

        public static EasingFunction GetDerivative(Ease ease)
        {
            switch (ease)
            {
                case Ease.Linear:       return LinearD;
                case Ease.InQuad:       return InQuadD;
                case Ease.OutQuad:      return OutQuadD;
                case Ease.InOutQuad:    return InOutQuadD;
                case Ease.InCubic:      return InCubicD;
                case Ease.OutCubic:     return OutCubicD;
                case Ease.InOutCubic:   return InOutCubicD;
                case Ease.InQuart:      return InQuartD;
                case Ease.OutQuart:     return OutQuartD;
                case Ease.InOutQuart:   return InOutQuartD;
                case Ease.InQuint:      return InQuintD;
                case Ease.OutQuint:     return OutQuintD;
                case Ease.InOutQuint:   return InOutQuintD;
                case Ease.InSine:       return InSineD;
                case Ease.OutSine:      return OutSineD;
                case Ease.InOutSine:    return InOutSineD;
                case Ease.InExpo:       return InExpoD;
                case Ease.OutExpo:      return OutExpoD;
                case Ease.InOutExpo:    return InOutExpoD;
                case Ease.InCirc:       return InCircD;
                case Ease.OutCirc:      return OutCircD;
                case Ease.InOutCirc:    return InOutCircD;
                case Ease.InBounce:     return InBounceD;
                case Ease.OutBounce:    return OutBounceD;
                case Ease.InOutBounce:  return InOutBounceD;
                case Ease.InBack:       return InBackD;
                case Ease.OutBack:      return OutBackD;
                case Ease.InOutBack:    return InOutBackD;
                case Ease.InElastic:    return InElasticD;
                case Ease.OutElastic:   return OutElasticD;
                case Ease.InOutElastic: return InOutElasticD;
                case Ease.Spring:       return SpringD;
            }

            return null;
        }
    }
}