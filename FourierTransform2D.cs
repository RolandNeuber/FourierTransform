using System.Numerics;

namespace FourierTransforms
{
	public static class FourierTransform2D
	{
		public static Complex[,] DFT2D(Complex[,] timeDomainSignal)
		{
			return new Complex[timeDomainSignal.GetLength(0), timeDomainSignal.GetLength(1)];
		}
	}
}
