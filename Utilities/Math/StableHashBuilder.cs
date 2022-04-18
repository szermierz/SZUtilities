using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SZUtilities.Math
{
    public struct StableHashBuilder
    {
        public static StableHashBuilder Create()
        {
            return new StableHashBuilder() { Hash = 544654 };
        }

        public int Hash { get; private set; }

        public uint HashUnsigned => new BitConverter { Int0 = Hash }.Uint;

        public StableHashBuilder Combine(int value)
        {
            Hash = (Hash * 756839) ^ value;
            return this;
        }

        public StableHashBuilder Combine(uint value)
        {
            var bc = new BitConverter { Uint = value };
            return Combine(bc.Int0);
        }

        public StableHashBuilder Combine(short value)
        {
            return Combine((int)value);
        }

        public StableHashBuilder Combine(byte value)
        {
            return Combine((int)value);
        }

        public StableHashBuilder Combine(long value)
        {
            var bc = new BitConverter { Long = value };
            return Combine(bc.Int0)
                .Combine(bc.Int1);
        }

        public StableHashBuilder Combine(ulong value)
        {
            var bc = new BitConverter { Ulong = value };
            return Combine(bc.Int0)
                .Combine(bc.Int1);
        }

        public StableHashBuilder Combine(Fix value)
        {
            return Combine(value.RawValue);
        }

        public StableHashBuilder Combine(string value)
        {
            return Combine(Generate(value));
        }

        public StableHashBuilder Combine(IReadOnlyCollection<int> value)
        {
            return Combine(value, (curr, v) => curr.Combine(v));
        }

        public StableHashBuilder Combine(IReadOnlyCollection<uint> value)
        {
            return Combine(value, (curr, v) => curr.Combine(v));
        }

        public StableHashBuilder Combine(IReadOnlyCollection<short> value)
        {
            return Combine(value, (curr, v) => curr.Combine(v));
        }


        public StableHashBuilder Combine(IReadOnlyCollection<byte> value)
        {
            return Combine(value, (curr, v) => curr.Combine(v));
        }

        public StableHashBuilder Combine(IReadOnlyCollection<long> value)
        {
            return Combine(value, (curr, v) => curr.Combine(v));
        }

        public StableHashBuilder Combine(IReadOnlyCollection<ulong> value)
        {
            return Combine(value, (curr, v) => curr.Combine(v));
        }

        public StableHashBuilder Combine(IReadOnlyCollection<Fix> value)
        {
            return Combine(value, (curr, v) => curr.Combine(v));
        }

        public StableHashBuilder Combine(IReadOnlyCollection<string> value)
        {
            return Combine(value, (curr, v) => curr.Combine(v));
        }

        public StableHashBuilder Combine<T>(IReadOnlyCollection<T> value, Func<StableHashBuilder, T, StableHashBuilder> combineFunction)
        {
            if (value == null)
                return Combine(-1);

            if (value.Count == 0)
                return Combine(0);

            return value.Aggregate(this, combineFunction);
        }

        public static int Generate(string str)
        {
            // source: https://stackoverflow.com/a/5155015/12992897
            unchecked
            {
                var hash = 23;
                foreach (var c in str)
                    hash = hash * 31 + c;
                return hash;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct BitConverter
        {
            public BitConverter(int value = 0)
            {
                Uint = 0;
                Long = 0;
                Ulong = 0;
                Int0 = value;
                Int1 = 0;
            }

            [FieldOffset(0)]
            public int Int0;
            [FieldOffset(4)]
            public int Int1;

            [FieldOffset(0)]
            public uint Uint;
            [FieldOffset(0)]
            public long Long;
            [FieldOffset(0)]
            public ulong Ulong;
        }
    }
}
