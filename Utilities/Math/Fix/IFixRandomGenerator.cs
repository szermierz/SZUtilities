namespace SZUtilities.Math
{
    public static class FixRandomGeneratorExtensions
    {
        public static Fix UniformExclusive(this FixRandomGenerator generator)
        {
            var value = generator.Uniform();

            // 3 sigmas
            while (value == Fix.One)
                value = generator.Uniform();

            return value;
        }

        public static Fix UniformSymmetric(this FixRandomGenerator generator)
        {
            return generator.Uniform() * new Fix(2) - Fix.One;
        }

        public static Fix NormalSymmetric(this FixRandomGenerator generator)
        {
            var value = generator.Normal();

            // 3 sigmas
            while (value < new Fix(-3) || value > new Fix(3))
                value = generator.Normal();

            return Fix.Clamp(value / new Fix(3), -Fix.One,  Fix.One);
        }

        public static int UniformInt(this FixRandomGenerator generator, Fix expected)
        {
            var result = Fix.Floor(expected);

            if (expected - result > generator.Uniform())
                return (int)result + 1;

            return (int)result;
        }

        public static Fix UniformRange(this FixRandomGenerator generator, Fix min, Fix max)
        {
            return min + generator.Uniform() * (max - min);
        }

        public static int UniformRangeInt(this FixRandomGenerator generator, int min, int max)
        {
            return min + (int)Fix.Floor(generator.UniformExclusive() * (Fix)(max - min));
        }

        public static int UniformRangeInt(this FixRandomGenerator generator, Fix min, Fix max)
        {
            var value = generator.UniformRange(min, max);
            var result = Fix.Floor(value);

            if (value - result > generator.Uniform())
                return (int)result + 1;

            return (int)result;
        }

        public static Fix Normal(this FixRandomGenerator generator, Fix stdev)
        {
            return generator.Normal() * stdev;
        }

        public static Fix NormalRange(this FixRandomGenerator generator, Fix min, Fix max)
        {
            var m = (max + min) * Fix.Half;
            var d = m - min;

            var value = m + generator.NormalSymmetric() * d;

            return Fix.Clamp(value, min, max);
        }

        public static int NormalRangeInt(this FixRandomGenerator generator, Fix min, Fix max)
        {
            var value = generator.NormalRange(min, max);
            var result = Fix.Floor(value);

            if (value - result > generator.Uniform())
                return (int)result + 1;

            return (int)result;
        }
    }
}
