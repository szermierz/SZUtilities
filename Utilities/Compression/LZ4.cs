#region LZ4 original

/*
   LZ4 - Fast LZ compression algorithm
   Copyright (C) 2011-2012, Yann Collet.
   BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions are
   met:

	   * Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
	   * Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following disclaimer
   in the documentation and/or other materials provided with the
   distribution.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
   OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
   SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
   LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
   DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
   THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

   You can contact the author at :
   - LZ4 homepage : http://fastcompression.blogspot.com/p/lz4.html
   - LZ4 source repository : http://code.google.com/p/lz4/
*/

#endregion

#region LZ4 port

/*
Copyright (c) 2013, Milosz Krajewski
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided
that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions
  and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions
  and the following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion

using System.Collections.Generic;
using System.Linq;
using static SZUtilities.Compression.LZ4_Helpers;

namespace SZUtilities.Compression
{
	internal static class LZ4_Helpers
	{
		#region Consts

		public static readonly int[] DECODER_TABLE_32 = { 0, 3, 2, 3, 0, 0, 0, 0 };
		public static readonly int[] DEBRUIJN_TABLE_32 = {
				0, 0, 3, 0, 3, 1, 3, 0, 3, 2, 2, 1, 3, 2, 0, 1,
				3, 3, 1, 2, 2, 2, 2, 0, 3, 1, 2, 0, 1, 0, 1, 1
			};
		public const int COPYLENGTH = 8;
		public const int LASTLITERALS = 5;
		public const int ML_BITS = 4;
		public const int ML_MASK = (1 << ML_BITS) - 1;
		public const int RUN_BITS = 8 - ML_BITS;
		public const int RUN_MASK = (1 << RUN_BITS) - 1;
		public const int STEPSIZE_32 = 4;
		public const int MEMORY_USAGE = 14;
		public const int HASH_LOG = MEMORY_USAGE - 2;
		public const int HASH_TABLESIZE = 1 << HASH_LOG;
		public const int MINMATCH = 4;
		public const int MFLIMIT = COPYLENGTH + MINMATCH;
		public const int MINLENGTH = MFLIMIT + 1;
		public const int HASH_ADJUST = (MINMATCH * 8) - HASH_LOG;
		public const int SKIPSTRENGTH = 6; //TODO:SZ - consider increase or decrease
		public const int MAX_DISTANCE = (1 << MAXD_LOG) - 1;
		public const int MAXD_LOG = 16;
		public const int HASHHC_LOG = MAXD_LOG - 1;
		public const int HASHHC_TABLESIZE = 1 << HASHHC_LOG;
		public const int HASHHC_ADJUST = (MINMATCH * 8) - HASHHC_LOG;
		public const int MAXD = 1 << MAXD_LOG;
		public const int OPTIMAL_ML = (ML_MASK - 1) + MINMATCH;
		public const int MAXD_MASK = MAXD - 1;
		public const int MAX_NB_ATTEMPTS = 256;

		#endregion

		#region Byte block copy

		public static void Copy4(byte[] buf, int src, int dst)
		{
			buf[dst + 3] = buf[src + 3];
			buf[dst + 2] = buf[src + 2];
			buf[dst + 1] = buf[src + 1];
			buf[dst] = buf[src];
		}

		public static void Copy8(byte[] buf, int src, int dst)
		{
			buf[dst + 7] = buf[src + 7];
			buf[dst + 6] = buf[src + 6];
			buf[dst + 5] = buf[src + 5];
			buf[dst + 4] = buf[src + 4];
			buf[dst + 3] = buf[src + 3];
			buf[dst + 2] = buf[src + 2];
			buf[dst + 1] = buf[src + 1];
			buf[dst] = buf[src];
		}

		public static void BlockCopy(byte[] src, int src_0, byte[] dst, int dst_0, int len)
		{
			while(len >= 8)
			{
				dst[dst_0] = src[src_0];
				dst[dst_0 + 1] = src[src_0 + 1];
				dst[dst_0 + 2] = src[src_0 + 2];
				dst[dst_0 + 3] = src[src_0 + 3];
				dst[dst_0 + 4] = src[src_0 + 4];
				dst[dst_0 + 5] = src[src_0 + 5];
				dst[dst_0 + 6] = src[src_0 + 6];
				dst[dst_0 + 7] = src[src_0 + 7];
				len -= 8;
				src_0 += 8;
				dst_0 += 8;
			}

			while(len >= 4)
			{
				dst[dst_0] = src[src_0];
				dst[dst_0 + 1] = src[src_0 + 1];
				dst[dst_0 + 2] = src[src_0 + 2];
				dst[dst_0 + 3] = src[src_0 + 3];
				len -= 4;
				src_0 += 4;
				dst_0 += 4;
			}

			while(len-- > 0)
			{
				dst[dst_0++] = src[src_0++];
			}
		}

		public static int WildCopy(byte[] src, int src_0, byte[] dst, int dst_0, int dst_end)
		{
			var len = dst_end - dst_0;

			// apparently (tested) this is an overkill
			// it seems to be faster without this 8-byte loop
			//while (len >= 8)
			//{
			//	dst[dst_0] = src[src_0];
			//	dst[dst_0 + 1] = src[src_0 + 1];
			//	dst[dst_0 + 2] = src[src_0 + 2];
			//	dst[dst_0 + 3] = src[src_0 + 3];
			//	dst[dst_0 + 4] = src[src_0 + 4];
			//	dst[dst_0 + 5] = src[src_0 + 5];
			//	dst[dst_0 + 6] = src[src_0 + 6];
			//	dst[dst_0 + 7] = src[src_0 + 7];
			//	len -= 8; src_0 += 8; dst_0 += 8;
			//}

			while(len >= 4)
			{
				dst[dst_0] = src[src_0];
				dst[dst_0 + 1] = src[src_0 + 1];
				dst[dst_0 + 2] = src[src_0 + 2];
				dst[dst_0 + 3] = src[src_0 + 3];
				len -= 4;
				src_0 += 4;
				dst_0 += 4;
			}

			while(len-- > 0)
			{
				dst[dst_0++] = src[src_0++];
			}

			return len;
		}

		public static int SecureCopy(byte[] buffer, int src, int dst, int dst_end)
		{
			var diff = dst - src;
			var length = dst_end - dst;
			var len = length;

			if(diff >= length)
			{
				System.Buffer.BlockCopy(buffer, src, buffer, dst, length);
				return length; // done
			}

			do
			{
				System.Buffer.BlockCopy(buffer, src, buffer, dst, diff);
				src += diff;
				dst += diff;
				len -= diff;
			} while(len >= diff);
			

			// apparently (tested) this is an overkill
			// it seems to be faster without this 8-byte loop
			//while (len >= 8)
			//{
			//	buffer[dst] = buffer[src];
			//	buffer[dst + 1] = buffer[src + 1];
			//	buffer[dst + 2] = buffer[src + 2];
			//	buffer[dst + 3] = buffer[src + 3];
			//	buffer[dst + 4] = buffer[src + 4];
			//	buffer[dst + 5] = buffer[src + 5];
			//	buffer[dst + 6] = buffer[src + 6];
			//	buffer[dst + 7] = buffer[src + 7];
			//	dst += 8; src += 8; len -= 8;
			//}

			while(len >= 4)
			{
				buffer[dst] = buffer[src];
				buffer[dst + 1] = buffer[src + 1];
				buffer[dst + 2] = buffer[src + 2];
				buffer[dst + 3] = buffer[src + 3];
				dst += 4;
				src += 4;
				len -= 4;
			}

			while(len-- > 0)
			{
				buffer[dst++] = buffer[src++];
			}

			return length; // done
		}

		#endregion

		public static void Poke2(byte[] buffer, int offset, ushort value)
		{
			buffer[offset] = (byte)value;
			buffer[offset + 1] = (byte)(value >> 8);
		}

		public static ushort Peek2(byte[] buffer, int offset)
		{
			// NOTE: It's faster than BitConverter.ToUInt16 (suprised? me too)
			return (ushort)(((uint)buffer[offset]) | ((uint)buffer[offset + 1] << 8));
		}

		public static uint Peek4(byte[] buffer, int offset)
		{
			// NOTE: It's faster than BitConverter.ToUInt32 (suprised? me too)
			return
				((uint)buffer[offset]) |
				((uint)buffer[offset + 1] << 8) |
				((uint)buffer[offset + 2] << 16) |
				((uint)buffer[offset + 3] << 24);
		}

		public static bool Equal4(byte[] buffer, int offset1, int offset2)
		{
			// return Peek4(buffer, offset1) == Peek4(buffer, offset2);
			if(buffer[offset1] != buffer[offset2])
				return false;
			if(buffer[offset1 + 1] != buffer[offset2 + 1])
				return false;
			if(buffer[offset1 + 2] != buffer[offset2 + 2])
				return false;
			return buffer[offset1 + 3] == buffer[offset2 + 3];
		}

		public static uint Xor4(byte[] buffer, int offset1, int offset2)
		{
			// return Peek4(buffer, offset1) ^ Peek4(buffer, offset2);
			var value1 =
				((uint)buffer[offset1]) |
				((uint)buffer[offset1 + 1] << 8) |
				((uint)buffer[offset1 + 2] << 16) |
				((uint)buffer[offset1 + 3] << 24);
			var value2 =
				((uint)buffer[offset2]) |
				((uint)buffer[offset2 + 1] << 8) |
				((uint)buffer[offset2 + 2] << 16) |
				((uint)buffer[offset2 + 3] << 24);
			return value1 ^ value2;
		}

		public static ulong Xor8(byte[] buffer, int offset1, int offset2)
		{
			// return Peek8(buffer, offset1) ^ Peek8(buffer, offset2);
			var value1 =
				((ulong)buffer[offset1]) |
				((ulong)buffer[offset1 + 1] << 8) |
				((ulong)buffer[offset1 + 2] << 16) |
				((ulong)buffer[offset1 + 3] << 24) |
				((ulong)buffer[offset1 + 4] << 32) |
				((ulong)buffer[offset1 + 5] << 40) |
				((ulong)buffer[offset1 + 6] << 48) |
				((ulong)buffer[offset1 + 7] << 56);
			var value2 =
				((ulong)buffer[offset2]) |
				((ulong)buffer[offset2 + 1] << 8) |
				((ulong)buffer[offset2 + 2] << 16) |
				((ulong)buffer[offset2 + 3] << 24) |
				((ulong)buffer[offset2 + 4] << 32) |
				((ulong)buffer[offset2 + 5] << 40) |
				((ulong)buffer[offset2 + 6] << 48) |
				((ulong)buffer[offset2 + 7] << 56);
			return value1 ^ value2;
		}

		public static bool Equal2(byte[] buffer, int offset1, int offset2)
		{
			// return Peek2(buffer, offset1) == Peek2(buffer, offset2);
			if(buffer[offset1] != buffer[offset2])
				return false;
			return buffer[offset1 + 1] == buffer[offset2 + 1];
		}

		public class LZ4HC_Data_Structure
		{
			public byte[] src;
			public int src_base;
			public int src_end;
			public int src_LASTLITERALS;
			public byte[] dst;
			public int dst_base;
			public int dst_len;
			public int dst_end;
			public int[] hashTable;
			public ushort[] chainTable;
			public int nextToUpdate;
		};

		public static LZ4HC_Data_Structure LZ4HC_Create(byte[] src, int src_0, int src_len, byte[] dst, int dst_0, int dst_len)
		{
			var hc4 = new LZ4HC_Data_Structure
			{
				src = src,
				src_base = src_0,
				src_end = src_0 + src_len,
				src_LASTLITERALS = (src_0 + src_len - LASTLITERALS),
				dst = dst,
				dst_base = dst_0,
				dst_len = dst_len,
				dst_end = dst_0 + dst_len,
				hashTable = new int[HASHHC_TABLESIZE],
				chainTable = new ushort[MAXD],
				nextToUpdate = src_0 + 1,
			};

			var ct = hc4.chainTable;
			for(var i = ct.Length - 1; i >= 0; i--)
				ct[i] = unchecked((ushort)-1);

			return hc4;
		}

		private static void LZ4HC_Insert_32(LZ4HC_Data_Structure ctx, int src_p)
		{
			var chainTable = ctx.chainTable;
			var hashTable = ctx.hashTable;
			var nextToUpdate = ctx.nextToUpdate;
			var src = ctx.src;
			var src_base = ctx.src_base;

			while(nextToUpdate < src_p)
			{
				var p = nextToUpdate;
				var delta = (p) - (hashTable[(((Peek4(src, p)) * 2654435761u) >> HASHHC_ADJUST)] + src_base);
				if(delta > MAX_DISTANCE)
					delta = MAX_DISTANCE;
				chainTable[(p) & MAXD_MASK] = (ushort)delta;
				hashTable[(((Peek4(src, p)) * 2654435761u) >> HASHHC_ADJUST)] = ((p) - src_base);
				nextToUpdate++;
			}

			ctx.nextToUpdate = nextToUpdate;
		}

		private static int LZ4HC_CommonLength_32(LZ4HC_Data_Structure ctx, int p1, int p2)
		{
			var debruijn32 = DEBRUIJN_TABLE_32;
			var src = ctx.src;
			var src_LASTLITERALS = ctx.src_LASTLITERALS;

			var p1t = p1;

			while(p1t < src_LASTLITERALS - (STEPSIZE_32 - 1))
			{
				var diff = (int)Xor4(src, p2, p1t);
				if(diff == 0)
				{
					p1t += STEPSIZE_32;
					p2 += STEPSIZE_32;
					continue;
				}
				p1t += debruijn32[((uint)((diff) & -(diff)) * 0x077CB531u) >> 27];
				return (p1t - p1);
			}
			if((p1t < (src_LASTLITERALS - 1)) && (Equal2(src, p2, p1t)))
			{
				p1t += 2;
				p2 += 2;
			}
			if((p1t < src_LASTLITERALS) && (src[p2] == src[p1t]))
				p1t++;
			return (p1t - p1);
		}

		private static int LZ4HC_InsertAndFindBestMatch_32(LZ4HC_Data_Structure ctx, int src_p, ref int src_match)
		{
			var chainTable = ctx.chainTable;
			var hashTable = ctx.hashTable;
			var src = ctx.src;
			var src_base = ctx.src_base;

			var nbAttempts = MAX_NB_ATTEMPTS;
			int repl = 0, ml = 0;
			ushort delta = 0;

			// HC4 match finder
			LZ4HC_Insert_32(ctx, src_p);
			var src_ref = (hashTable[(((Peek4(src, src_p)) * 2654435761u) >> HASHHC_ADJUST)] + src_base);


			// Detect repetitive sequences of length <= 4
			if(src_ref >= src_p - 4) // potential repetition
			{
				if(Equal4(src, src_ref, src_p)) // confirmed
				{
					delta = (ushort)(src_p - src_ref);
					repl = ml = LZ4HC_CommonLength_32(ctx, src_p + MINMATCH, src_ref + MINMATCH) + MINMATCH;
					src_match = src_ref;
				}
				src_ref = ((src_ref) - chainTable[(src_ref) & MAXD_MASK]);
			}

			while((src_ref >= src_p - MAX_DISTANCE) && (nbAttempts != 0))
			{
				nbAttempts--;
				if(src[(src_ref + ml)] == src[(src_p + ml)])
				{
					if(Equal4(src, src_ref, src_p))
					{
						var mlt = LZ4HC_CommonLength_32(ctx, src_p + MINMATCH, src_ref + MINMATCH) + MINMATCH;
						if(mlt > ml)
						{
							ml = mlt;
							src_match = src_ref;
						}
					}
				}
				src_ref = ((src_ref) - chainTable[(src_ref) & MAXD_MASK]);
			}


			// Complete table
			if(repl != 0)
			{
				var src_ptr = src_p;

				var end = src_p + repl - (MINMATCH - 1);
				while(src_ptr < end - delta)
				{
					chainTable[(src_ptr) & MAXD_MASK] = delta; // Pre-Load
					src_ptr++;
				}
				do
				{
					chainTable[(src_ptr) & MAXD_MASK] = delta;
					hashTable[(((Peek4(src, src_ptr)) * 2654435761u) >> HASHHC_ADJUST)] = ((src_ptr) - src_base); // Head of chain
					src_ptr++;
				} while(src_ptr < end);
				ctx.nextToUpdate = end;
			}

			return ml;
		}

		private static int LZ4HC_InsertAndGetWiderMatch_32(LZ4HC_Data_Structure ctx, int src_p, int startLimit, int longest, ref int matchpos, ref int startpos)
		{
			var chainTable = ctx.chainTable;
			var hashTable = ctx.hashTable;
			var src = ctx.src;
			var src_base = ctx.src_base;
			var src_LASTLITERALS = ctx.src_LASTLITERALS;
			var debruijn32 = DEBRUIJN_TABLE_32;
			var nbAttempts = MAX_NB_ATTEMPTS;
			var delta = (src_p - startLimit);

			// First Match
			LZ4HC_Insert_32(ctx, src_p);
			var src_ref = (hashTable[(((Peek4(src, src_p)) * 2654435761u) >> HASHHC_ADJUST)] + src_base);

			while((src_ref >= src_p - MAX_DISTANCE) && (nbAttempts != 0))
			{
				nbAttempts--;
				if(src[(startLimit + longest)] == src[(src_ref - delta + longest)])
				{
					if(Equal4(src, src_ref, src_p))
					{
						var reft = src_ref + MINMATCH;
						var ipt = src_p + MINMATCH;
						var startt = src_p;

						while(ipt < src_LASTLITERALS - (STEPSIZE_32 - 1))
						{
							var diff = (int)Xor4(src, reft, ipt);
							if(diff == 0)
							{
								ipt += STEPSIZE_32;
								reft += STEPSIZE_32;
								continue;
							}
							ipt += debruijn32[((uint)((diff) & -(diff)) * 0x077CB531u) >> 27];
							goto _endCount;
						}
						if((ipt < (src_LASTLITERALS - 1)) && (Equal2(src, reft, ipt)))
						{
							ipt += 2;
							reft += 2;
						}
						if((ipt < src_LASTLITERALS) && (src[reft] == src[ipt]))
							ipt++;

						_endCount:
						reft = src_ref;

						while((startt > startLimit) && (reft > src_base) && (src[startt - 1] == src[reft - 1]))
						{
							startt--;
							reft--;
						}

						if((ipt - startt) > longest)
						{
							longest = (ipt - startt);
							matchpos = reft;
							startpos = startt;
						}
					}
				}
				src_ref = ((src_ref) - chainTable[(src_ref) & MAXD_MASK]);
			}

			return longest;
		}

		private static int LZ4_encodeSequence_32(LZ4HC_Data_Structure ctx, ref int src_p, ref int dst_p, ref int src_anchor, int matchLength, int src_ref, int dst_end)
		{
			int len;
			var src = ctx.src;
			var dst = ctx.dst;

			// Encode Literal length
			var length = src_p - src_anchor;
			var dst_token = dst_p++;
			if((dst_p + length + (2 + 1 + LASTLITERALS) + (length >> 8)) > dst_end)
				return 1; // Check output limit
			if(length >= RUN_MASK)
			{
				dst[dst_token] = (RUN_MASK << ML_BITS);
				len = length - RUN_MASK;
				for(; len > 254; len -= 255)
					dst[dst_p++] = 255;
				dst[dst_p++] = (byte)len;
			}
			else
			{
				dst[dst_token] = (byte)(length << ML_BITS);
			}

			// Copy Literals
			if(length > 0)
			{
				var _i = dst_p + length;
				src_anchor += WildCopy(src, src_anchor, dst, dst_p, _i);
				dst_p = _i;
			}

			// Encode Offset
			Poke2(dst, dst_p, (ushort)(src_p - src_ref));
			dst_p += 2;

			// Encode MatchLength
			len = (matchLength - MINMATCH);
			if(dst_p + (1 + LASTLITERALS) + (length >> 8) > dst_end)
				return 1; // Check output limit
			if(len >= ML_MASK)
			{
				dst[dst_token] += ML_MASK;
				len -= ML_MASK;
				for(; len > 509; len -= 510)
				{
					dst[(dst_p)++] = 255;
					dst[(dst_p)++] = 255;
				}
				if(len > 254)
				{
					len -= 255;
					dst[(dst_p)++] = 255;
				}
				dst[(dst_p)++] = (byte)len;
			}
			else
			{
				dst[dst_token] += (byte)len;
			}

			// Prepare next loop
			src_p += matchLength;
			src_anchor = src_p;

			return 0;
		}

		public static int LZ4_compressHCCtx_32(LZ4HC_Data_Structure ctx)
		{
			var src = ctx.src;
			var dst = ctx.dst;
			var src_0 = ctx.src_base;
			var src_end = ctx.src_end;
			var dst_0 = ctx.dst_base;
			var dst_len = ctx.dst_len;
			var dst_end = ctx.dst_end;

			var src_p = src_0;
			var src_anchor = src_p;
			var src_mflimit = src_end - MFLIMIT;

			var dst_p = dst_0;

			var src_ref = 0;
			var start2 = 0;
			var ref2 = 0;
			var start3 = 0;
			var ref3 = 0;

			src_p++;

			// Main Loop
			while(src_p < src_mflimit)
			{
				var ml = LZ4HC_InsertAndFindBestMatch_32(ctx, src_p, ref src_ref);
				if(ml == 0)
				{
					src_p++;
					continue;
				}

				// saved, in case we would skip too much
				var start0 = src_p;
				var ref0 = src_ref;
				var ml0 = ml;

				_Search2:
				var ml2 = src_p + ml < src_mflimit
					? LZ4HC_InsertAndGetWiderMatch_32(ctx, src_p + ml - 2, src_p + 1, ml, ref ref2, ref start2)
					: ml;

				if(ml2 == ml) // No better match
				{
					if(LZ4_encodeSequence_32(ctx, ref src_p, ref dst_p, ref src_anchor, ml, src_ref, dst_end) != 0)
						return 0;
					continue;
				}

				if(start0 < src_p)
				{
					if(start2 < src_p + ml0) // empirical
					{
						src_p = start0;
						src_ref = ref0;
						ml = ml0;
					}
				}

				// Here, start0==ip
				if((start2 - src_p) < 3) // First Match too small : removed
				{
					ml = ml2;
					src_p = start2;
					src_ref = ref2;
					goto _Search2;
				}

				_Search3:
				// Currently we have :
				// ml2 > ml1, and
				// ip1+3 <= ip2 (usually < ip1+ml1)
				if((start2 - src_p) < OPTIMAL_ML)
				{
					var new_ml = ml;
					if(new_ml > OPTIMAL_ML)
						new_ml = OPTIMAL_ML;
					if(src_p + new_ml > start2 + ml2 - MINMATCH)
						new_ml = (start2 - src_p) + ml2 - MINMATCH;
					var correction = new_ml - (start2 - src_p);
					if(correction > 0)
					{
						start2 += correction;
						ref2 += correction;
						ml2 -= correction;
					}
				}
				// Now, we have start2 = ip+new_ml, with new_ml=min(ml, OPTIMAL_ML=18)

				var ml3 = start2 + ml2 < src_mflimit
					? LZ4HC_InsertAndGetWiderMatch_32(ctx, start2 + ml2 - 3, start2, ml2, ref ref3, ref start3)
					: ml2;

				if(ml3 == ml2) // No better match : 2 sequences to encode
				{
					// ip & ref are known; Now for ml
					if(start2 < src_p + ml)
						ml = (start2 - src_p);
					// Now, encode 2 sequences
					if(LZ4_encodeSequence_32(ctx, ref src_p, ref dst_p, ref src_anchor, ml, src_ref, dst_end) != 0)
						return 0;
					src_p = start2;
					if(LZ4_encodeSequence_32(ctx, ref src_p, ref dst_p, ref src_anchor, ml2, ref2, dst_end) != 0)
						return 0;
					continue;
				}

				if(start3 < src_p + ml + 3) // Not enough space for match 2 : remove it
				{
					if(start3 >= (src_p + ml)) // can write Seq1 immediately ==> Seq2 is removed, so Seq3 becomes Seq1
					{
						if(start2 < src_p + ml)
						{
							var correction = (src_p + ml - start2);
							start2 += correction;
							ref2 += correction;
							ml2 -= correction;
							if(ml2 < MINMATCH)
							{
								start2 = start3;
								ref2 = ref3;
								ml2 = ml3;
							}
						}

						if(LZ4_encodeSequence_32(ctx, ref src_p, ref dst_p, ref src_anchor, ml, src_ref, dst_end) != 0)
							return 0;
						src_p = start3;
						src_ref = ref3;
						ml = ml3;

						start0 = start2;
						ref0 = ref2;
						ml0 = ml2;
						goto _Search2;
					}

					start2 = start3;
					ref2 = ref3;
					ml2 = ml3;
					goto _Search3;
				}

				// OK, now we have 3 ascending matches; let's write at least the first one
				// ip & ref are known; Now for ml
				if(start2 < src_p + ml)
				{
					if((start2 - src_p) < ML_MASK)
					{
						if(ml > OPTIMAL_ML)
							ml = OPTIMAL_ML;
						if(src_p + ml > start2 + ml2 - MINMATCH)
							ml = (start2 - src_p) + ml2 - MINMATCH;
						var correction = ml - (start2 - src_p);
						if(correction > 0)
						{
							start2 += correction;
							ref2 += correction;
							ml2 -= correction;
						}
					}
					else
					{
						ml = (start2 - src_p);
					}
				}
				if(LZ4_encodeSequence_32(ctx, ref src_p, ref dst_p, ref src_anchor, ml, src_ref, dst_end) != 0)
					return 0;

				src_p = start2;
				src_ref = ref2;
				ml = ml2;

				start2 = start3;
				ref2 = ref3;
				ml2 = ml3;

				goto _Search3;
			}

			// Encode Last Literals
			{
				var lastRun = (src_end - src_anchor);
				if((dst_p - dst_0) + lastRun + 1 + ((lastRun + 255 - RUN_MASK) / 255) > (uint)dst_len)
					return 0; // Check output limit
				if(lastRun >= RUN_MASK)
				{
					dst[dst_p++] = (RUN_MASK << ML_BITS);
					lastRun -= RUN_MASK;
					for(; lastRun > 254; lastRun -= 255)
						dst[dst_p++] = 255;
					dst[dst_p++] = (byte)lastRun;
				}
				else
				{
					dst[dst_p++] = (byte)(lastRun << ML_BITS);
				}
				BlockCopy(src, src_anchor, dst, dst_p, src_end - src_anchor);
				dst_p += src_end - src_anchor;
			}

			// End
			return (dst_p - dst_0);
		}
	}

	public static class LZ4
	{
		public static int MaxOutputLength(int inputLength)
		{
			return inputLength + (inputLength / 255) + 16;
		}

		public static int Compress(byte[] src, byte[] dst)
		{
			var hash_table = new int[HASH_TABLESIZE];
			int src_len = src.Length;
			int dst_maxlen = dst.Length;
			int src_0 = 0;
			int dst_0 = 0;
			var debruijn32 = DEBRUIJN_TABLE_32;
			int _i;

			// ---- preprocessed source start here ----
			// r93
			var src_p = src_0;
			var src_base = src_0;
			var src_anchor = src_p;
			var src_end = src_p + src_len;
			var src_mflimit = src_end - MFLIMIT;

			var dst_p = dst_0;
			var dst_end = dst_p + dst_maxlen;

			var src_LASTLITERALS = src_end - LASTLITERALS;
			var src_LASTLITERALS_1 = src_LASTLITERALS - 1;

			var src_LASTLITERALS_STEPSIZE_1 = src_LASTLITERALS - (STEPSIZE_32 - 1);
			var dst_LASTLITERALS_1 = dst_end - (1 + LASTLITERALS);
			var dst_LASTLITERALS_3 = dst_end - (2 + 1 + LASTLITERALS);

			int length;

			uint h, h_fwd;

			// Init
			if(src_len < MINLENGTH)
				goto _last_literals;

			// First Byte
			hash_table[(((Peek4(src, src_p)) * 2654435761u) >> HASH_ADJUST)] = (src_p - src_base);
			src_p++;
			h_fwd = (((Peek4(src, src_p)) * 2654435761u) >> HASH_ADJUST);

			// Main Loop
			while(true)
			{
				var findMatchAttempts = (1 << SKIPSTRENGTH) + 3;
				var src_p_fwd = src_p;
				int src_ref;
				int dst_token;

				// Find a match
				do
				{
					h = h_fwd;
					var step = findMatchAttempts++ >> SKIPSTRENGTH;
					src_p = src_p_fwd;
					src_p_fwd = src_p + step;

					if(src_p_fwd > src_mflimit)
						goto _last_literals;

					h_fwd = (((Peek4(src, src_p_fwd)) * 2654435761u) >> HASH_ADJUST);
					src_ref = src_base + hash_table[h];
					hash_table[h] = (src_p - src_base);
				} while((src_ref < src_p - MAX_DISTANCE) || (!Equal4(src, src_ref, src_p)));

				// Catch up
				while((src_p > src_anchor) && (src_ref > src_0) && (src[src_p - 1] == src[src_ref - 1]))
				{
					src_p--;
					src_ref--;
				}

				// Encode Literal length
				length = (src_p - src_anchor);
				dst_token = dst_p++;

				if(dst_p + length + (length >> 8) > dst_LASTLITERALS_3)
					return 0; // Check output limit

				if(length >= RUN_MASK)
				{
					var len = length - RUN_MASK;
					dst[dst_token] = (RUN_MASK << ML_BITS);
					if(len > 254)
					{
						do
						{
							dst[dst_p++] = 255;
							len -= 255;
						} while(len > 254);
						dst[dst_p++] = (byte)len;
						BlockCopy(src, src_anchor, dst, dst_p, length);
						dst_p += length;
						goto _next_match;
					}
					else
						dst[dst_p++] = (byte)len;
				}
				else
				{
					dst[dst_token] = (byte)(length << ML_BITS);
				}

				// Copy Literals
				if(length > 0)
				{
					_i = dst_p + length;
					WildCopy(src, src_anchor, dst, dst_p, _i);
					dst_p = _i;
				}

				_next_match:
				// Encode Offset
				Poke2(dst, dst_p, (ushort)(src_p - src_ref));
				dst_p += 2;

				// Start Counting
				src_p += MINMATCH;
				src_ref += MINMATCH; // MinMatch already verified
				src_anchor = src_p;

				while(src_p < src_LASTLITERALS_STEPSIZE_1)
				{
					var diff = (int)Xor4(src, src_ref, src_p);
					if(diff == 0)
					{
						src_p += STEPSIZE_32;
						src_ref += STEPSIZE_32;
						continue;
					}
					src_p += debruijn32[((uint)((diff) & -(diff)) * 0x077CB531u) >> 27];
					goto _endCount;
				}

				if((src_p < src_LASTLITERALS_1) && (Equal2(src, src_ref, src_p)))
				{
					src_p += 2;
					src_ref += 2;
				}
				if((src_p < src_LASTLITERALS) && (src[src_ref] == src[src_p]))
					src_p++;

				_endCount:
				// Encode MatchLength
				length = (src_p - src_anchor);

				if(dst_p + (length >> 8) > dst_LASTLITERALS_1)
					return 0; // Check output limit

				if(length >= ML_MASK)
				{
					dst[dst_token] += ML_MASK;
					length -= ML_MASK;
					for(; length > 509; length -= 510)
					{
						dst[dst_p++] = 255;
						dst[dst_p++] = 255;
					}
					if(length > 254)
					{
						length -= 255;
						dst[dst_p++] = 255;
					}
					dst[dst_p++] = (byte)length;
				}
				else
				{
					dst[dst_token] += (byte)length;
				}

				// Test end of chunk
				if(src_p > src_mflimit)
				{
					src_anchor = src_p;
					break;
				}

				// Fill table
				hash_table[(((Peek4(src, src_p - 2)) * 2654435761u) >> HASH_ADJUST)] = (src_p - 2 - src_base);

				// Test next position

				h = (((Peek4(src, src_p)) * 2654435761u) >> HASH_ADJUST);
				src_ref = src_base + hash_table[h];
				hash_table[h] = (src_p - src_base);

				if((src_ref > src_p - (MAX_DISTANCE + 1)) && (Equal4(src, src_ref, src_p)))
				{
					dst_token = dst_p++;
					dst[dst_token] = 0;
					goto _next_match;
				}

				// Prepare next loop
				src_anchor = src_p++;
				h_fwd = (((Peek4(src, src_p)) * 2654435761u) >> HASH_ADJUST);
			}

			_last_literals:
			// Encode Last Literals
			{
				var lastRun = (src_end - src_anchor);

				if(dst_p + lastRun + 1 + ((lastRun + 255 - RUN_MASK) / 255) > dst_end)
					return 0;

				if(lastRun >= RUN_MASK)
				{
					dst[dst_p++] = (RUN_MASK << ML_BITS);
					lastRun -= RUN_MASK;
					for(; lastRun > 254; lastRun -= 255)
						dst[dst_p++] = 255;
					dst[dst_p++] = (byte)lastRun;
				}
				else
					dst[dst_p++] = (byte)(lastRun << ML_BITS);
				BlockCopy(src, src_anchor, dst, dst_p, src_end - src_anchor);
				dst_p += src_end - src_anchor;
			}

			// End
			return ((dst_p) - dst_0);
		}

		public static int CompressHC(byte[] src, byte[] dst)
		{
			return LZ4_compressHCCtx_32(LZ4HC_Create(src, 0, src.Length, dst, 0, dst.Length));
		}

		public static int Decompress(byte[] src, byte[] dst)
		{
			var dec32table = DECODER_TABLE_32;
			int _i;

			// ---- preprocessed source start here ----
			// r93
			var src_p = 0;
			int dst_ref;

			var dst_p = 0;
			var dst_end = dst_p + dst.Length;
			int dst_cpy;

			var dst_LASTLITERALS = dst_end - LASTLITERALS;
			var dst_COPYLENGTH = dst_end - COPYLENGTH;
			var dst_COPYLENGTH_STEPSIZE_4 = dst_end - COPYLENGTH - (STEPSIZE_32 - 4);

			byte token;

			// Main Loop
			while(true)
			{
				int length;

				// get runlength
				token = src[src_p++];
				if((length = (token >> ML_BITS)) == RUN_MASK)
				{
					int len;
					for(; (len = src[src_p++]) == 255; length += 255)
					{
						/* do nothing */
					}
					length += len;
				}

				// copy literals
				dst_cpy = dst_p + length;

				if(dst_cpy > dst_COPYLENGTH)
				{
					if(dst_cpy != dst_end)
						goto _output_error; // Error : not enough place for another match (min 4) + 5 literals
					BlockCopy(src, src_p, dst, dst_p, length);
					src_p += length;
					break; // EOF
				}
				if(dst_p < dst_cpy)
				{
					_i = WildCopy(src, src_p, dst, dst_p, dst_cpy);
					src_p += _i;
					dst_p += _i;
				}
				src_p -= (dst_p - dst_cpy);
				dst_p = dst_cpy;

				if(src_p >= src.Length)
					return dst_p;

				// get offset
				dst_ref = (dst_cpy) - Peek2(src, src_p);
				src_p += 2;
				if(dst_ref < 0)
					goto _output_error; // Error : offset outside destination buffer

				// get matchlength
				if((length = (token & ML_MASK)) == ML_MASK)
				{
					for(; src[src_p] == 255; length += 255)
						src_p++;
					length += src[src_p++];
				}

				// copy repeated sequence
				if((dst_p - dst_ref) < STEPSIZE_32)
				{
					const int dec64 = 0;
					dst[dst_p + 0] = dst[dst_ref + 0];
					dst[dst_p + 1] = dst[dst_ref + 1];
					dst[dst_p + 2] = dst[dst_ref + 2];
					dst[dst_p + 3] = dst[dst_ref + 3];
					dst_p += 4;
					dst_ref += 4;
					dst_ref -= dec32table[dst_p - dst_ref];
					Copy4(dst, dst_ref, dst_p);
					dst_p += STEPSIZE_32 - 4;
					dst_ref -= dec64;
				}
				else
				{
					Copy4(dst, dst_ref, dst_p);
					dst_p += 4;
					dst_ref += 4;
				}
				dst_cpy = dst_p + length - (STEPSIZE_32 - 4);

				if(dst_cpy > dst_COPYLENGTH_STEPSIZE_4)
				{
					if(dst_cpy > dst_LASTLITERALS)
						goto _output_error; // Error : last 5 bytes must be literals
					if(dst_p < dst_COPYLENGTH)
					{
						_i = SecureCopy(dst, dst_ref, dst_p, dst_COPYLENGTH);
						dst_ref += _i;
						dst_p += _i;
					}

					while(dst_p < dst_cpy)
						dst[dst_p++] = dst[dst_ref++];
					dst_p = dst_cpy;
					continue;
				}

				if(dst_p < dst_cpy)
				{
					SecureCopy(dst, dst_ref, dst_p, dst_cpy);
				}
				dst_p = dst_cpy; // correction
			}

			// end of decoding
			return ((src_p) - 0);

			// write overflow error detected
			_output_error:
			return (-((src_p) - 0));
		}

		public static int Decompress2(byte[] input, byte[] dest)
		{
			var blockStart = 0;
			var destIt = 0;

			while(blockStart < input.Length)
			{
				var blockIt = blockStart;

				// Read token
				var token = input[blockIt];
				++blockIt;

				// Read text len
				var textLen = token >> 4;
				if(textLen == 15)
				{
					while(input[blockIt] == 255)
					{
						textLen += 255;
						++blockIt;
					}

					textLen += input[blockIt];
					++blockIt;
				}

				// Copy text
				for(int i = 0; i < textLen; ++i)
				{
					dest[destIt] = input[blockIt];
					++destIt;
					++blockIt;
				}

				// Last block can be shorter
				if(blockIt >= input.Length)
					break;

				// Read offset
				int offset = input[blockIt];
				++blockIt;
				offset += 256 * input[blockIt];
				++blockIt;

				// Read offset textLen
				var offsetTextLen = token & 15;
				if(offsetTextLen == 15)
				{
					while(input[blockIt] == 255)
					{
						offsetTextLen += 255;
						++blockIt;
					}

					offsetTextLen += input[blockIt];
					++blockIt;
				}
				offsetTextLen += 4;

				// Copy text
				var startPos = destIt - offset;
				var endPos = startPos + offsetTextLen;
				for(int i = startPos; i < endPos; ++i)
				{
					dest[destIt] = dest[i];
					++destIt;
				}

				// Iterate blocks
				blockStart = blockIt;
			}

			return destIt;
		}

		public static void RunAlgorithm(System.Func<byte[], byte[], int> algorithm, byte[] input, out byte[] output)
		{
			output = new byte[MaxOutputLength(input.Length)];
			var destLen = algorithm(input, output);
			output = output.Where((c, i) => i < destLen).ToArray();
		}
	}
}