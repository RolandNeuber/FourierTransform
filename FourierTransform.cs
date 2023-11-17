using System.Numerics;
using FourierTransforms.Operations;

namespace FourierTransforms
{
    public static class FourierTransforms
	{
		/// <summary>
		/// Computes the DFT of a given Time-Domain-Signal.
		/// Time-Domain-Signal can be of any size, but is very slow for non-powers of 2.
		/// Try to avoid Time-Domain-Signals with non-power of two sizes.
		/// </summary>
		/// <param name="timeDomainSignal">The Time-Domain-Signal.</param>
		/// <returns>Returns the corresponding Frequency-Domain-Signal.</returns>
		public static Complex[] DFT(IList<Complex> timeDomainSignal)
		{
			if (timeDomainSignal.Count == 0)
				return timeDomainSignal.ToArray();
			if (BitOperations.IsPow2(timeDomainSignal.Count))
				return FourierTransformOperations.ParallelIterativeDFT(timeDomainSignal);
			return FourierTransformOperations.DFT(timeDomainSignal);
			//Complex[] extendedTimeDomainSignal = new Complex[BitOperations.RoundUpToPowerOf2((uint)timeDomainSignal.Count)];
			//Array.Copy(timeDomainSignal.ToArray(), extendedTimeDomainSignal, timeDomainSignal.Count);

			//for (int i = timeDomainSignal.Count; i < extendedTimeDomainSignal.Length; i++)
			//{
			//	extendedTimeDomainSignal[i] = Complex.Zero;
			//}
			
			//Complex[] frequencyDomainSignal = FourierTransformOperations.ParallelIterativeDFT(extendedTimeDomainSignal);
			//Complex[] truncatedFrequencyDomainSignal = new Complex[timeDomainSignal.Count];
			//Array.Copy(frequencyDomainSignal, truncatedFrequencyDomainSignal, timeDomainSignal.Count);
			
			//return truncatedFrequencyDomainSignal;
		}
	}
}
