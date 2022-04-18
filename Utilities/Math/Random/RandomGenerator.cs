
namespace SZUtilities.Math
{
    public struct RandomGenerator
    {
        #region Initialization
        
        public RandomGenerator(uint seed)
        {
            m_seed = seed;
            m_normalDistributionSpare = null;
        }

        #endregion
        
        #region Seed

        private uint m_seed;

        private static uint MinSeed => uint.MinValue;
        private static uint MaxSeed => uint.MaxValue;

        public uint Seed
        {
            get => m_seed;
            set
            {
                m_seed = value;
                NewSeedSet();
            }
        }

        private uint NextSeed()
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

        #endregion

        #region Generation

        private float? m_normalDistributionSpare;

        private void NewSeedSet()
        {
            m_normalDistributionSpare = null;
        }

        public float Uniform()
        {
            m_normalDistributionSpare = null;
            return NextSeed() * 1.0f / (MaxSeed - 1);
        }

        public int Next()
        {
            var result = NextSeed();
            var longResult = result + int.MinValue;
            return (int) longResult;
        }

        public float Normal()
        {
            // use spare
            if (m_normalDistributionSpare.HasValue)
            {
                var value = m_normalDistributionSpare.Value;
                m_normalDistributionSpare = null;

                return value;
            }

            // calculate
            float x1, x2, w;

            do
            {
                x1 = this.UniformSymmetric();
                x2 = this.UniformSymmetric();
                w = x1 * x1 + x2 * x2;
            } while (w >= 1.0f || w <= 0.0f);

            w = (float)System.Math.Sqrt(-2.0f * (float)System.Math.Log(w) / w);

            m_normalDistributionSpare = x2 * w;

            return x1 * w;
        }

        #endregion
    }
}