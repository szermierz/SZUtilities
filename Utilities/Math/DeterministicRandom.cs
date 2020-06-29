using System.Collections.Generic;

public class DeterministicRandom : DeterministicRandom.IRandomProvider
{
    private ISeedProvider SeedProvider { get; set; } = default;
    
    public DeterministicRandom(ISeedProvider seedProvider = null)
    {
        SeedProvider = seedProvider;
    }

    public int RandomInclusive(int min, int max, int seedMod = 0)
    {
        return RandomExclusive(min, max + 1, seedMod);
    }

    public int RandomExclusive(int min, int max, int seedMod = 0)
    {
        return InternalRandom.Random(RandomSeed + InternalRandom.Random(seedMod)) % (max - min) + min;
    }

    public void SetSeedProvider(ISeedProvider seedProvider)
    {
        SeedProvider = seedProvider;
    }

    private const int c_seedMask = 33984;
    private int RandomSeed => c_seedMask ^ SeedProvider.Seed;

    public bool Ready => Valid;
    public bool Valid => null != SeedProvider;

    private static class InternalRandom
    {
        const int c_internalMax = 0xFFFF;

        public static int Random(int seed)
        {
            seed = (214013 * seed + 2531011);
            return (seed >> 16) & c_internalMax;
        }
    }

    public interface IRandomProvider : IValidable
    {
        bool Ready { get; }
        void SetSeedProvider(ISeedProvider seedProvider);
        int RandomInclusive(int min, int max, int seedMod = 0);
        int RandomExclusive(int min, int max, int seedMod = 0);
    }

    public interface ISeedProvider : IValidable
    {
        int Seed { get; }
    }
}