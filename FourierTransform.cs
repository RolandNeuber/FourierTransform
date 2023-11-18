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
				return [.. timeDomainSignal];
			if (BitOperations.IsPow2(timeDomainSignal.Count))
				return FourierTransformOperations.ParallelIterativeDFT(timeDomainSignal);
			return FourierTransformOperations.DFT(timeDomainSignal);
		}

		public static Complex[] InverseDFT(IList<Complex> frequencyDomainSignal)
		{
			if (frequencyDomainSignal.Count == 0)
				return [.. frequencyDomainSignal];
			if (BitOperations.IsPow2(frequencyDomainSignal.Count))
				return FourierTransformOperations.ParallelIterativeInverseDFT(frequencyDomainSignal);
			return FourierTransformOperations.InverseDFT(frequencyDomainSignal);
		}
	}
}
