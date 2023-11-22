using System.Numerics;

namespace FourierTransforms
{
	public static class BitReverse
	{
		public static T[] ReverseBits<T>(IList<T> list)
		{
			if (!BitOperations.IsPow2(list.Count))
				throw new ArgumentException("list length must be power of two.");
			T[] reverse = new T[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				uint reversedIndex = ReverseBits((uint)i, (uint)BitOperations.Log2((uint)list.Count));
				reverse[reversedIndex] = list[i];
			}
			return reverse;
		}
		public static T[,] ReverseBits<T>(T[,] list)
		{
			if (!BitOperations.IsPow2(list.GetLength(0)) || !BitOperations.IsPow2(list.GetLength(1)))
				throw new ArgumentException("list length must be power of two.");
			T[,] reverse = new T[list.GetLength(0), list.GetLength(0)];
			for (int i = 0; i < list.GetLength(0); i++)
			{
				uint reversedIndex = ReverseBits((uint)i, (uint)BitOperations.Log2((uint)list.GetLength(0)));
				for (int j = 0; j < list.GetLength(1); j++)
				{
					reverse[reversedIndex, j] = list[i, j];
				}
			}
			for (int i = 0; i < list.GetLength(0); i++)
			{
				uint reversedIndex = ReverseBits((uint)i, (uint)BitOperations.Log2((uint)list.GetLength(0)));
				for (int j= 0; j < list.GetLength(1); j++)
				{
					reverse[j, reversedIndex] = reverse[j, i];
				}
			}
			return reverse;
		}

		public static uint ReverseBits(uint number, uint maxBits)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>(maxBits, 32);
			uint reverse = 0;
			for (int i = 0; i < maxBits; i++)
			{
				reverse <<= 1;
				reverse |= number & 1;
				number >>= 1;
			}
			return reverse;
		}

		public static ulong ReverseBits(ulong number, uint maxBits)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>(maxBits, 64);
			ulong reverse = 0;
			for (int i = 0; i < maxBits; i++)
			{
				reverse <<= 1;
				reverse |= number & 1;
				number >>= 1;
			}
			return reverse;
		}

		public static int UnsafeReverseBits(int number, int maxBits)
		{
			int reverse = 0;
			for (int i = 0; i < maxBits; i++)
			{
				reverse <<= 1;
				reverse |= number & 1;
				number >>>= 1;
			}
			return reverse;
		}

		public static T[] UnsafeReverseBits<T>(IList<T> list)
		{
			int logCount = BitOperations.Log2((uint)list.Count);
			T[] reverse = new T[list.Count];
			Parallel.For(0, list.Count, (i) =>
			{
				int reversedIndex = UnsafeReverseBits(i, logCount);
				reverse[reversedIndex] = list[i];
			});
			return reverse;
		}
	}
}
