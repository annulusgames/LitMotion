using Unity.Burst;
using static Unity.Mathematics.math;

namespace LitMotion
{
    /// <summary>
    /// Utility class that provides calculation of easing functions.
    /// </summary>
    [BurstCompile]
    public static class EaseUtility
    {
        [BurstCompile]
        public static float Evaluate(float t, Ease ease)
        {
            return ease switch
            {
                Ease.InSine => InSine(t),
                Ease.OutSine => OutSine(t),
                Ease.InOutSine => InOutSine(t),
                Ease.InQuad => InQuad(t),
                Ease.OutQuad => OutQuad(t),
                Ease.InOutQuad => InOutQuad(t),
                Ease.InCubic => InCubic(t),
                Ease.OutCubic => OutCubic(t),
                Ease.InOutCubic => InOutCubic(t),
                Ease.InQuart => InQuart(t),
                Ease.OutQuart => OutQuart(t),
                Ease.InOutQuart => InOutQuart(t),
                Ease.InQuint => InQuint(t),
                Ease.OutQuint => OutQuint(t),
                Ease.InOutQuint => InOutQuint(t),
                Ease.InExpo => InExpo(t),
                Ease.OutExpo => OutExpo(t),
                Ease.InOutExpo => InOutExpo(t),
                Ease.InCirc => InCirc(t),
                Ease.OutCirc => OutCirc(t),
                Ease.InOutCirc => InOutCirc(t),
                Ease.InElastic => InElastic(t),
                Ease.OutElastic => OutElastic(t),
                Ease.InOutElastic => InOutElastic(t),
                Ease.InBack => InBack(t),
                Ease.OutBack => OutBack(t),
                Ease.InOutBack => InOutBack(t),
                Ease.InBounce => InBounce(t),
                Ease.OutBounce => OutBounce(t),
                Ease.InOutBounce => InOutBounce(t),
                _ => t,
            };
        }

        [BurstCompile]
        public static float Linear(float x) => x;

        [BurstCompile]
        public static float InSine(float x) => 1 - cos(x * PI / 2);

        [BurstCompile]
        public static float OutSine(float x) => sin(x * PI / 2);

        [BurstCompile]
        public static float InOutSine(float x) => -(cos(PI * x) - 1) / 2;

        [BurstCompile]
        public static float InQuad(float x) => x * x;

        [BurstCompile]
        public static float OutQuad(float x) => 1 - (1 - x) * (1 - x);

        [BurstCompile]
        public static float InOutQuad(float x) => x < 0.5f ? 2 * x * x : 1 - pow(-2 * x + 2, 2) / 2;

        [BurstCompile]
        public static float InCubic(float x) => x * x * x;

        [BurstCompile]
        public static float OutCubic(float x) => 1 - pow(1 - x, 3);

        [BurstCompile]
        public static float InOutCubic(float x) => x < 0.5f ? 4 * x * x * x : 1 - pow(-2 * x + 2, 3) / 2;

        [BurstCompile]
        public static float InQuart(float x) => x * x * x * x;

        [BurstCompile]
        public static float OutQuart(float x) => 1 - pow(1 - x, 4);

        [BurstCompile]
        public static float InOutQuart(float x) => x < 0.5 ? 8 * x * x * x * x : 1 - pow(-2 * x + 2, 4) / 2;

        [BurstCompile]
        public static float InQuint(float x) => x * x * x * x * x;

        [BurstCompile]
        public static float OutQuint(float x) => 1 - pow(1 - x, 5);

        [BurstCompile]
        public static float InOutQuint(float x) => x < 0.5f ? 16 * x * x * x * x * x : 1 - pow(-2 * x + 2, 5) / 2;

        [BurstCompile]
        public static float InExpo(float x) => x == 0 ? 0 : exp2(10 * x - 10);

        [BurstCompile]
        public static float OutExpo(float x) => x == 1 ? 1 : 1 - exp2(-10 * x);

        [BurstCompile]
        public static float InOutExpo(float x)
        {
            return x == 0 ? 0 :
                x == 1 ? 1 :
                x < 0.5f ? exp2(20 * x - 10) / 2 :
                (2 - exp2(-20 * x + 10)) / 2;
        }

        [BurstCompile]
        public static float InCirc(float x) => 1 - sqrt(1 - pow(x, 2));

        [BurstCompile]
        public static float OutCirc(float x) => sqrt(1 - pow(x - 1, 2));

        [BurstCompile]
        public static float InOutCirc(float x)
        {
            return x < 0.5 ?
                (1 - sqrt(1 - pow(2 * x, 2))) / 2 :
                (sqrt(1 - pow(-2 * x + 2, 2)) + 1) / 2;
        }

        [BurstCompile]
        public static float InBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * x * x * x - c1 * x * x;
        }

        [BurstCompile]
        public static float OutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * pow(x - 1, 3) + c1 * pow(x - 1, 2);
        }

        [BurstCompile]
        public static float InOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;

            return x < 0.5f
                ? pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2) / 2
                : (pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        }

        [BurstCompile]
        public static float InElastic(float x)
        {
            const float c4 = 2 * PI / 3;

            return x == 0 ? 0 :
                x == 1 ? 1 :
                -exp2(10 * x - 10) * sin((x * 10 - 10.75f) * c4);
        }

        [BurstCompile]
        public static float OutElastic(float x)
        {
            const float c4 = 2 * PI / 3;

            return x == 0 ? 0 :
                x == 1 ? 1 :
                exp2(-10 * x) * sin((x * 10 - 0.75f) * c4) + 1;
        }

        [BurstCompile]
        public static float InOutElastic(float x)
        {
            const float c5 = 2 * PI / 4.5f;

            return x == 0 ? 0 :
                x == 1 ? 1 :
                x < 0.5f ?
                -(exp2(20 * x - 10) * sin((20 * x - 11.125f) * c5)) / 2 :
                exp2(-20 * x + 10) * sin((20 * x - 11.125f) * c5) / 2 + 1;
        }

        [BurstCompile]
        public static float InBounce(float x) => 1 - OutBounce(1 - x);

        [BurstCompile]
        public static float OutBounce(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            float t = x;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5 / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }
        [BurstCompile]
        public static float InOutBounce(float x)
        {
            return x < 0.5f ?
                (1 - OutBounce(1 - 2 * x)) / 2 :
                (1 + OutBounce(2 * x - 1)) / 2;
        }
    }
}