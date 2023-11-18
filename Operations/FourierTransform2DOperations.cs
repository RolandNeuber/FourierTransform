using System.Numerics;

namespace FourierTransforms.Operations
{
	public static class FourierTransform2DOperations
	{
		public static Complex[,] ParallelIterativeDFT(Complex[,] timeDomainSignal)
		{
			timeDomainSignal = BitReverse.ReverseBits(timeDomainSignal);
			for (int slice = 0; slice < timeDomainSignal.GetLength(1); slice++)
			{
				for (int offset = 1; offset <= timeDomainSignal.GetLength(0) / 2; offset *= 2)
				{
					double precomputedFactor = -Math.PI / offset;
					Parallel.For(0, offset, (pair) =>
					{
						double precomputedFactor2 = precomputedFactor * pair;
						for (int section = 0; section < timeDomainSignal.GetLength(0); section += 2 * offset)
						{
							int left = section + pair;
							int right = left + offset;
							Complex factor = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
							factor *= timeDomainSignal[right, slice];
							timeDomainSignal[right, slice] = timeDomainSignal[left, slice] - factor;
							timeDomainSignal[left, slice] = timeDomainSignal[left, slice] + factor;
						}
					});
				}
			}
			for (int slice = 0; slice < timeDomainSignal.GetLength(0); slice++)
			{
				for (int offset = 1; offset <= timeDomainSignal.GetLength(1) / 2; offset *= 2)
				{
					double precomputedFactor = -Math.PI / offset;
					Parallel.For(0, offset, (pair) =>
					{
						double precomputedFactor2 = precomputedFactor * pair;
						for (int section = 0; section < timeDomainSignal.GetLength(1); section += 2 * offset)
						{
							int left = section + pair;
							int right = left + offset;
							Complex factor = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
							factor *= timeDomainSignal[slice, right];
							timeDomainSignal[slice, right] = timeDomainSignal[slice, left] - factor;
							timeDomainSignal[slice, left] = timeDomainSignal[slice, left] + factor;
						}
					});
				}
			}

			return timeDomainSignal;
		}
		public static Complex[,] ParallelIterativeInverseDFT(Complex[,] frequencyDomainSignal)
		{
			frequencyDomainSignal = BitReverse.ReverseBits(frequencyDomainSignal);
			for (int slice = 0; slice < frequencyDomainSignal.GetLength(1); slice++)
			{
				for (int offset = 1; offset <= frequencyDomainSignal.GetLength(0) / 2; offset *= 2)
				{
					double precomputedFactor = Math.PI / offset;
					Parallel.For(0, offset, (pair) =>
					{
						double precomputedFactor2 = precomputedFactor * pair;
						for (int section = 0; section < frequencyDomainSignal.GetLength(0); section += 2 * offset)
						{
							int left = section + pair;
							int right = left + offset;
							Complex factor = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
							factor *= frequencyDomainSignal[right, slice];
							frequencyDomainSignal[right, slice] = frequencyDomainSignal[left, slice] - factor;
							frequencyDomainSignal[left, slice] = frequencyDomainSignal[left, slice] + factor;
						}
					});
				}
				for (int i = 0; i < frequencyDomainSignal.GetLength(0); i++)
					frequencyDomainSignal[i, slice] /= frequencyDomainSignal.GetLength(0);
			}
			for (int slice = 0; slice < frequencyDomainSignal.GetLength(0); slice++)
			{
				for (int offset = 1; offset <= frequencyDomainSignal.GetLength(1) / 2; offset *= 2)
				{
					double precomputedFactor = Math.PI / offset;
					Parallel.For(0, offset, (pair) =>
					{
						double precomputedFactor2 = precomputedFactor * pair;
						for (int section = 0; section < frequencyDomainSignal.GetLength(1); section += 2 * offset)
						{
							int left = section + pair;
							int right = left + offset;
							Complex factor = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
							factor *= frequencyDomainSignal[slice, right];
							frequencyDomainSignal[slice, right] = frequencyDomainSignal[slice, left] - factor;
							frequencyDomainSignal[slice, left] = frequencyDomainSignal[slice, left] + factor;
						}
					});
				}
				for (int i = 0; i < frequencyDomainSignal.GetLength(1); i++)
					frequencyDomainSignal[slice, i] /= frequencyDomainSignal.GetLength(1);
			}

			return frequencyDomainSignal;
		}
	}
}
