using System.Collections.Generic;
using System.Linq;

namespace SZUtilities.Math
{
    public class WeightedRandom
    {
        private const int c_defaultCapacity = 64;

        private IList<int> m_weights;
        private int m_sum;
        private List<int> m_cummulatedWeights = default;

        public WeightedRandom(int capacity = c_defaultCapacity)
        {
            m_cummulatedWeights = new List<int>(capacity);
        }

        public void Setup(IList<int> weights)
        {
            m_weights = weights;
            m_cummulatedWeights.Clear();

            for (int i = 0; i < weights.Count; ++i)
            {
                var prev = i > 0 ? m_cummulatedWeights[i - 1] : 0;
                m_cummulatedWeights.Add(prev + weights[i]);
            }

            m_sum = m_cummulatedWeights[m_cummulatedWeights.Count - 1];
        }

        public int PickRandomIndex(DeterministicRandom random, int seedOffset)
        {
            var r = random.RandomExclusive(0, m_sum, seedOffset);
            return m_cummulatedWeights.TakeWhile(_weightSum => _weightSum <= r).Count();
        }
    }
}