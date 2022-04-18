namespace SZUtilities.Math
{
    public struct FixRandomGenerator
    {
        private Fix? m_normalDistributionSpare;
        private uint m_seed;

        public FixRandomGenerator(uint seed)
        {
            m_seed = seed;
            m_normalDistributionSpare = null;
        }

        private uint MinSeed => uint.MinValue;
        private uint MaxSeed => uint.MaxValue;

        public uint Seed
        {
            get => m_seed;
            set
            {
                m_seed = value;
                NewSeedSet();
            }
        }

        public uint NextSeed()
        {
            return m_seed = NextSeed(m_seed);
        }

        private static uint NextSeed(uint seed)
        {
            seed += 0xe120fc15;
            var tmp = (ulong)seed * 0x4a39b70d;
            var m1 = (uint)((tmp >> 32) ^ tmp);
            tmp = (ulong)m1 * 0x12fad5c9;
            return (uint)((tmp >> 32) ^ tmp);
        }
        
        public FixRandomGenerator(int seed)
            : this(StableHashBuilder.Create().Combine(seed).HashUnsigned)
        {
        }

        private void NewSeedSet()
        {
            m_normalDistributionSpare = null;
        }

        public Fix Uniform()
        {
            m_normalDistributionSpare = null;
            return Fix.FromRatio((int)(NextSeed() >> 1), (int)((MaxSeed >> 1) - 1));
        }

        public Fix Normal()
        {
            // use spare
            if (m_normalDistributionSpare.HasValue)
            {
                var value = m_normalDistributionSpare.Value;
                m_normalDistributionSpare = null;

                return value;
            }

            // calculate
            Fix x1, x2, w;

            do
            {
                x1 = this.UniformSymmetric();
                x2 = this.UniformSymmetric();
                w = x1 * x1 + x2 * x2;
            } while (w >= Fix.One || w <= Fix.Zero);

            w = Fix.Sqrt((Fix)(-2) * Fix.Ln(w) / w);

            m_normalDistributionSpare = x2 * w;

            return x1 * w;
        }
    }
}
