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
		public static uint ReverseBits(uint number, uint maxBits)
		{
			if (maxBits > 32)
				throw new ArgumentOutOfRangeException(nameof(maxBits));
			uint reverse = 0;
			for (int i = 0; i < maxBits; i++)
			{
				reverse <<= 1;
				reverse |= number & 1;
				number >>= 1;
			}
			return reverse;
		}
	}
}
