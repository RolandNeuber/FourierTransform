using System.Numerics;

namespace FourierTransforms.Operations
{
	public static class FourierTransformOperations
	{
		public static Complex[] DFT(IList<Complex> timeDomainSignal)
		{
			Complex[] frequencyDomainSignal = new Complex[timeDomainSignal.Count];
			//Parallel.For(0, timeDomainSignal.Count, (m) =>
			//{
			//	for (int k = 0; k < timeDomainSignal.Count; k++)
			//	{
			//		frequencyDomainSignal[m] += timeDomainSignal[k] * Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne * m * k / timeDomainSignal.Count);
			//	}
			//});
			for (int m = 0; m < timeDomainSignal.Count; m++)
			{
				for (int k = 0; k < timeDomainSignal.Count; k++)
				{
					frequencyDomainSignal[m] += timeDomainSignal[k] * Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne * m * k / timeDomainSignal.Count);
				}
			}
			return frequencyDomainSignal;
		}
		public static Complex[] InverseDFT(IList<Complex> frequencyDomainSignal)
		{
			Complex[] timeDomainSignal = new Complex[frequencyDomainSignal.Count];
			for (int m = 0; m < frequencyDomainSignal.Count; m++)
			{
				for (int k = 0; k < frequencyDomainSignal.Count; k++)
				{
					timeDomainSignal[m] += frequencyDomainSignal[k] * Complex.Exp(2 * Math.PI * Complex.ImaginaryOne * m * k / frequencyDomainSignal.Count);
				}
				timeDomainSignal[m] /= frequencyDomainSignal.Count;
			}
			return timeDomainSignal;
		}
		public static Complex[] FastDFT(IList<Complex> timeDomainSignal)
		{
			if (timeDomainSignal.Count == 1)
			{
				return [.. timeDomainSignal];
			}
			else
			{
				Complex[] right = new Complex[timeDomainSignal.Count / 2];
				Complex[] left = new Complex[timeDomainSignal.Count / 2];
				for (int i = 0; i < timeDomainSignal.Count; i++)
				{
					if (i % 2 == 0)
					{
						right[i / 2] = timeDomainSignal[i];
					}
					else
					{
						left[(i - 1) / 2] = timeDomainSignal[i];
					}
				}
				right = FastDFT(right);
				left = FastDFT(left);
				Complex[] frequencyDomainSignal = new Complex[timeDomainSignal.Count];
				for (int m = 0; m < right.Length; m++)
				{
					Complex factor = Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne * m / timeDomainSignal.Count);
					frequencyDomainSignal[m] = right[m] + left[m] * factor;
					frequencyDomainSignal[m + timeDomainSignal.Count / 2] = right[m] - left[m] * factor;
				}
				return frequencyDomainSignal;
			}
		}
		public static Complex[] FastInverseDFT(IList<Complex> frequencyDomainSignal)
		{
			static Complex[] fastInverseDFT(IList<Complex> frequencyDomainSignal)
			{
				if (frequencyDomainSignal.Count == 1)
				{
					return [.. frequencyDomainSignal];
				}
				else
				{
					Complex[] right = new Complex[frequencyDomainSignal.Count / 2];
					Complex[] left = new Complex[frequencyDomainSignal.Count / 2];
					for (int i = 0; i < frequencyDomainSignal.Count; i++)
					{
						if (i % 2 == 0)
						{
							right[i / 2] = frequencyDomainSignal[i];
						}
						else
						{
							left[(i - 1) / 2] = frequencyDomainSignal[i];
						}
					}
					right = fastInverseDFT(right);
					left = fastInverseDFT(left);
					Complex[] timeDomainSignal = new Complex[frequencyDomainSignal.Count];
					for (int m = 0; m < right.Length; m++)
					{
						Complex factor = Complex.Exp(2 * Math.PI * Complex.ImaginaryOne * m / frequencyDomainSignal.Count);
						timeDomainSignal[m] = right[m] + left[m] * factor;
						timeDomainSignal[m + right.Length] = right[m] - left[m] * factor;
					}
					return timeDomainSignal;
				}
			}
			Complex[] timeDomainSignal = fastInverseDFT(frequencyDomainSignal);
			for (int i = 0; i < timeDomainSignal.Length; i++)
			{
				timeDomainSignal[i] /= timeDomainSignal.Length;
			}
			return timeDomainSignal;
		}
		public static Complex[] ParallelDFT(IList<Complex> timeDomainSignal)
		{
			if (timeDomainSignal.Count == 1)
			{
				return [.. timeDomainSignal];
			}
			else
			{
				Complex[] right = new Complex[timeDomainSignal.Count / 2];
				Complex[] left = new Complex[timeDomainSignal.Count / 2];
				//239.6ms for 2^19 Samples
				//for (int i = 0; i < timeDomainSignal.Count; i++)
				//{
				//	if (i % 2 == 0)
				//	{
				//		right[i / 2] = timeDomainSignal[i];
				//	}
				//	else
				//	{
				//		left[(i - 1) / 2] = timeDomainSignal[i];
				//	}
				//}
				//233.4ms for 2^19 Samples
				for (int i = 0; i < right.Length; i++)
				{
					right[i] = timeDomainSignal[2 * i];
					left[i] = timeDomainSignal[2 * i + 1];
				}
				Task<Complex[]> task1 = Task.Run(() =>
				{
					return ParallelDFT(right);
				});
				Task<Complex[]> task2 = Task.Run(() =>
				{
					return ParallelDFT(left);
				});
				right = task1.Result;
				left = task2.Result;
				Complex[] frequencyDomainSignal = new Complex[timeDomainSignal.Count];
				double precomputedFactor = -Math.Tau / timeDomainSignal.Count;
				for (int m = 0; m < right.Length; m++)
				{
					//Complex factor = Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne * m / timeDomainSignal.Count);
					//Complex factor = new(Math.Cos(-2 * Math.PI * m / timeDomainSignal.Count), Math.Sin(-2 * Math.PI * m / timeDomainSignal.Count));
					//~243ms for 2^19 Samples
					//Complex factor = new(Math.Cos(precomputedFactor * m), Math.Sin(precomputedFactor * m));
					//~234.8ms for 2^19 Samples
					double sinFactor;
					double cosFactor;
					(sinFactor, cosFactor) = Math.SinCos(precomputedFactor * m);
					Complex factor = new Complex(cosFactor, sinFactor) * left[m];
					frequencyDomainSignal[m] = right[m] + factor;
					frequencyDomainSignal[m + timeDomainSignal.Count / 2] = right[m] - factor;
				}
				return frequencyDomainSignal;
			}
		}
		public static Complex[] ParallelInverseDFT(IList<Complex> frequencyDomainSignal)
		{
			static Complex[] parallelInverseDFT(IList<Complex> frequencyDomainSignal)
			{
				if (frequencyDomainSignal.Count == 1)
				{
					return [.. frequencyDomainSignal];
				}
				else
				{
					Complex[] right = new Complex[frequencyDomainSignal.Count / 2];
					Complex[] left = new Complex[frequencyDomainSignal.Count / 2];
					for (int i = 0; i < right.Length; i++)
					{
						right[i] = frequencyDomainSignal[2 * i];
						left[i] = frequencyDomainSignal[2 * i + 1];
					}
					Task<Complex[]> task1 = Task.Run(() =>
					{
						return parallelInverseDFT(right);
					});
					Task<Complex[]> task2 = Task.Run(() =>
					{
						return parallelInverseDFT(left);
					});
					right = task1.Result;
					left = task2.Result;
					Complex[] timeDomainSignal = new Complex[frequencyDomainSignal.Count];
					double precomputedFactor = Math.Tau / frequencyDomainSignal.Count;
					for (int m = 0; m < right.Length; m++)
					{
						double sinFactor;
						double cosFactor;
						(sinFactor, cosFactor) = Math.SinCos(precomputedFactor * m);
						Complex factor = new Complex(cosFactor, sinFactor) * left[m];
						timeDomainSignal[m] = right[m] + factor;
						timeDomainSignal[m + frequencyDomainSignal.Count / 2] = right[m] - factor;
					}
					return timeDomainSignal;
				}
			}
			Complex[] timeDomainSignal = parallelInverseDFT(frequencyDomainSignal);
			for (int i = 0; i < timeDomainSignal.Length; i++)
			{
				timeDomainSignal[i] /= timeDomainSignal.Length;
			}
			return timeDomainSignal;
		}
		public static Complex[] IterativeDFT(IList<Complex> timeDomainSignal)
		{
			timeDomainSignal = BitReverse.ReverseBits(timeDomainSignal);
			//i is the offset of the elements (left and right) to perform the butterfly operation on
			for (int offset = 1; offset <= timeDomainSignal.Count / 2; offset *= 2)
			{
				//494.7ms for 2^19 Samples
				double precomputedFactor = -Math.PI / offset;
				//k counts through all pairs of butterfly operations in section i
				for (int pair = 0; pair < offset; pair++)
				{
					//356.2ms for 2^19 Samples
					double precomputedFactor2 = precomputedFactor * pair;
					//j counts through every section of size i
					for (int section = 0; section < timeDomainSignal.Count; section += 2 * offset)
					{
						int left = section + pair;
						int right = left + offset;
						Complex factor = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
						factor *= timeDomainSignal[right];
						timeDomainSignal[right] = timeDomainSignal[left] - factor;
						timeDomainSignal[left] = timeDomainSignal[left] + factor;
					}
				}
			}
			return [.. timeDomainSignal];
		}
		public static Complex[] IterativeInverseDFT(IList<Complex> frequencyDomainSignal)
		{
			frequencyDomainSignal = BitReverse.ReverseBits(frequencyDomainSignal);
			//i is the offset of the elements (left and right) to perform the butterfly operation on
			for (int offset = 1; offset <= frequencyDomainSignal.Count / 2; offset *= 2)
			{
				//494.7ms for 2^19 Samples
				double precomputedFactor = Math.PI / offset;
				//k counts through all pairs of butterfly operations in section i
				for (int pair = 0; pair < offset; pair++)
				{
					//356.2ms for 2^19 Samples
					double precomputedFactor2 = precomputedFactor * pair;
					//j counts through every section of size i
					for (int section = 0; section < frequencyDomainSignal.Count; section += 2 * offset)
					{
						int left = section + pair;
						int right = left + offset;
						Complex factor = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
						factor *= frequencyDomainSignal[right];
						frequencyDomainSignal[right] = frequencyDomainSignal[left] - factor;
						frequencyDomainSignal[left] = frequencyDomainSignal[left] + factor;
					}
				}
			}
			for (int i = 0; i < frequencyDomainSignal.Count; i++)
				frequencyDomainSignal[i] /= frequencyDomainSignal.Count;
			return [.. frequencyDomainSignal];
		}
		public static Complex[] ParallelIterativeDFT(IList<Complex> timeDomainSignal)
		{
			timeDomainSignal = BitReverse.ReverseBits(timeDomainSignal);
			//i is the offset of the elements (left and right) to perform the butterfly operation on
			for (int offset = 1; offset < timeDomainSignal.Count; offset *= 2)
			{
				//494.7ms for 2^19 Samples
				double precomputedFactor = -Math.PI / offset;
				//k counts through all pairs of butterfly operations in section i
				Parallel.For(0, offset, (pair) =>
				{
					//356.2ms for 2^19 Samples
					double precomputedFactor2 = precomputedFactor * pair;
					Complex precomputedFactor3 = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
					//j counts through every section of size i
					for (int section = 0; section < timeDomainSignal.Count; section += 2 * offset)
					{
						int left = section + pair;
						int right = left + offset;
						Complex factor = precomputedFactor3 * timeDomainSignal[right];
						timeDomainSignal[right] = timeDomainSignal[left] - factor;
						timeDomainSignal[left] = timeDomainSignal[left] + factor;
					}
				});
			}
			return [.. timeDomainSignal];
		}
		public static Complex[] ParallelIterativeInverseDFT(IList<Complex> frequencyDomainSignal)
		{
			frequencyDomainSignal = BitReverse.ReverseBits(frequencyDomainSignal);
			//i is the offset of the elements (left and right) to perform the butterfly operation on
			for (int offset = 1; offset < frequencyDomainSignal.Count; offset *= 2)
			{
				//494.7ms for 2^19 Samples
				double precomputedFactor = Math.PI / offset;
				//k counts through all pairs of butterfly operations in section i
				Parallel.For(0, offset, (pair) =>
				{
					//356.2ms for 2^19 Samples
					double precomputedFactor2 = precomputedFactor * pair;
					Complex precomputedFactor3 = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
					//j counts through every section of size i
					for (int section = 0; section < frequencyDomainSignal.Count; section += 2 * offset)
					{
						int left = section + pair;
						int right = left + offset;
						Complex factor = precomputedFactor3 * frequencyDomainSignal[right];
						frequencyDomainSignal[right] = frequencyDomainSignal[left] - factor;
						frequencyDomainSignal[left] = frequencyDomainSignal[left] + factor;
					}
				});
			}
			for (int i = 0; i < frequencyDomainSignal.Count; i++)
				frequencyDomainSignal[i] /= frequencyDomainSignal.Count;
			return [.. frequencyDomainSignal];
		}

		public static Complex[] BitReverseParallelIterativeDFT(IList<Complex> timeDomainSignal)
		{
			timeDomainSignal = BitReverse.UnsafeReverseBits(timeDomainSignal);
			//i is the offset of the elements (left and right) to perform the butterfly operation on
			for (int offset = 1; offset < timeDomainSignal.Count; offset *= 2)
			{
				//494.7ms for 2^19 Samples
				double precomputedFactor = -Math.PI / offset;
				//k counts through all pairs of butterfly operations in section i
				Parallel.For(0, offset, (pair) =>
				{
					//356.2ms for 2^19 Samples
					double precomputedFactor2 = precomputedFactor * pair;
					Complex precomputedFactor3 = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
					//j counts through every section of size i
					for (int section = 0; section < timeDomainSignal.Count; section += 2 * offset)
					{
						int left = section + pair;
						int right = left + offset;
						Complex factor = precomputedFactor3 * timeDomainSignal[right];
						timeDomainSignal[right] = timeDomainSignal[left] - factor;
						timeDomainSignal[left] = timeDomainSignal[left] + factor;
					}
				});
			}
			return [.. timeDomainSignal];
		}
		public static Complex[] BitReverseParallelIterativeInverseDFT(IList<Complex> frequencyDomainSignal)
		{
			frequencyDomainSignal = BitReverse.UnsafeReverseBits(frequencyDomainSignal);
			//i is the offset of the elements (left and right) to perform the butterfly operation on
			for (int offset = 1; offset < frequencyDomainSignal.Count; offset *= 2)
			{
				//494.7ms for 2^19 Samples
				double precomputedFactor = Math.PI / offset;
				//k counts through all pairs of butterfly operations in section i
				Parallel.For(0, offset, (pair) =>
				{
					//356.2ms for 2^19 Samples
					double precomputedFactor2 = precomputedFactor * pair;
					Complex precomputedFactor3 = new(Math.Cos(precomputedFactor2), Math.Sin(precomputedFactor2));
					//j counts through every section of size i
					for (int section = 0; section < frequencyDomainSignal.Count; section += 2 * offset)
					{
						int left = section + pair;
						int right = left + offset;
						Complex factor = precomputedFactor3 * frequencyDomainSignal[right];
						frequencyDomainSignal[right] = frequencyDomainSignal[left] - factor;
						frequencyDomainSignal[left] = frequencyDomainSignal[left] + factor;
					}
				});
			}
			for (int i = 0; i < frequencyDomainSignal.Count; i++)
				frequencyDomainSignal[i] /= frequencyDomainSignal.Count;
			return [.. frequencyDomainSignal];
		}

		/*
		 https://hackmd.io/@akshayk07/ryn-yR7qr
		 1  1  1  1
		 1 −j −1  j
		 1 −1  1 −1
		 1  j −1 −j
		 */
		public static Complex[] Radix4DFT(IList<Complex> timeDomainSignal)
		{
			timeDomainSignal = BitReverse.ReverseBits(timeDomainSignal);
			//i is the offset of the elements (left and right) to perform the butterfly operation on
			for (int offset = 1; offset <= timeDomainSignal.Count / 4; offset *= 4)
			{
				//k counts through all pairs of butterfly operations in section i
				for (int pair = 0; pair < offset; pair++)
				{
					//j counts through every section of size i
					for (int section = 0; section < timeDomainSignal.Count; section += 4 * offset)
					{
						Complex factor1 = new(Math.Cos(Math.PI / offset * pair), Math.Sin(Math.PI / offset * pair));
						Complex factor2 = new(Math.Cos(Math.PI / offset * pair * 2), Math.Sin(Math.PI / offset * pair * 2));
						Complex factor3 = new(Math.Cos(Math.PI / offset * pair * 3), Math.Sin(Math.PI / offset * pair * 3));
						Complex factor4 = new(Math.Cos(Math.PI / offset * pair * 4), Math.Sin(Math.PI / offset * pair * 4));
						int elem1 = section + pair;
						int elem2 = elem1 + offset;
						int elem3 = elem2 + offset;
						int elem4 = elem3 + offset;
						//Complex factor = new(Math.Cos(-Math.PI / offset * pair), Math.Sin(-Math.PI / offset * pair));
						//factor *= timeDomainSignal[elem2];
						//Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne * m * k / timeDomainSignal.Count # N)
						Complex[,] factor = new Complex[,]
						{
							{ 1, 1, 1, 1 },
							{ 1, -Complex.ImaginaryOne, -1, Complex.ImaginaryOne },
							{ 1, -1, 1, -1 },
							{ 1, Complex.ImaginaryOne, -1, -Complex.ImaginaryOne },
						};
						timeDomainSignal[elem1] = 
							timeDomainSignal[elem1] + 
							factor[0, 1] * timeDomainSignal[elem2] * factor1 + 
							factor[0, 2] * timeDomainSignal[elem3] * factor2 + 
							factor[0, 3] * timeDomainSignal[elem4] * factor3;
						timeDomainSignal[elem2] = 
							timeDomainSignal[elem1] + 
							factor[1, 1] * timeDomainSignal[elem2] * factor1 + 
							factor[1, 2] * timeDomainSignal[elem3] * factor2 + 
							factor[1, 3] * timeDomainSignal[elem4] * factor3;
						timeDomainSignal[elem3] = 
							timeDomainSignal[elem1] + 
							factor[2, 1] * timeDomainSignal[elem2] * factor1 + 
							factor[2, 2] * timeDomainSignal[elem3] * factor2 + 
							factor[2, 3] * timeDomainSignal[elem4] * factor3;
						timeDomainSignal[elem4] = 
							timeDomainSignal[elem1] + 
							factor[3, 1] * timeDomainSignal[elem2] * factor1 + 
							factor[3, 2] * timeDomainSignal[elem3] * factor2 + 
							factor[3, 3] * timeDomainSignal[elem4] * factor3;
					}
				}
			}
			return [.. timeDomainSignal];
		}
	}
}