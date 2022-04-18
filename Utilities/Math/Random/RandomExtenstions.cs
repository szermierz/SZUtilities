
namespace SZUtilities.Math
{
    public static class RandomGeneratorExtensions
    {
        public static float UniformExclusive(this RandomGenerator generator)
        {
            var value = generator.Uniform();

            // 3 sigmas
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            while (value == 1.0f)
                value = generator.Uniform();

            return value;
        }

        public static float UniformSymmetric(this RandomGenerator generator)
        {
            return generator.Uniform() * 2.0f - 1.0f;
        }

        public static float NormalSymmetric(this RandomGenerator generator)
        {
                var value = generator.Normal();

                // 3 sigmas
                while (value < -3.0f || value > 3.0f)
                    value = generator.Normal();

                return Clamp(value / 3.0f, -1.0f, 1.0f);
        }

        public static int UniformInt(this RandomGenerator generator, float expected)
        {
            var result = (int)System.Math.Floor(expected);

            if (expected - result > generator.Uniform())
                ++result;

            return result;
        }

        public static float UniformRange(this RandomGenerator generator, float min, float max)
        {
            return min + generator.Uniform() * (max - min);
        }

        public static int UniformRangeInt(this RandomGenerator generator, int inclusiveMin, int exclusiveMax)
        {
            return inclusiveMin + (int)System.Math.Floor(generator.UniformExclusive() * (exclusiveMax - inclusiveMin));
        }

        public static int UniformRangeInt(this RandomGenerator generator, float min, float max)
        {
            var value = generator.UniformRange(min, max);
            var result = (int)System.Math.Floor(value);

            if (value - result > generator.Uniform())
                ++result;

            return result;
        }

        public static float Normal(this RandomGenerator generator, float stdev)
        {
            return generator.Normal() * stdev;
        }

        public static float NormalRange(this RandomGenerator generator, float min, float max)
        {
            var m = (max + min) * 0.5f;
            var d = m - min;

            var value = m + generator.NormalSymmetric() * d;

            return Clamp(value, min, max);
        }

        public static int NormalRangeInt(this RandomGenerator generator, float min, float max)
        {
            var value = generator.NormalRange(min, max);
            var result = (int)System.Math.Floor(value);

            if (value - result > generator.Uniform())
                ++result;

            return result;
        }

        private static float Clamp(float x, float min, float max)
        {
            if (x > max)
                return max;
            return x < min ? min : x;
        }
    }
}