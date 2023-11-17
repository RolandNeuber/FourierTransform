using FourierTransforms.Operations;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;

namespace FourierTransforms.Benchmarks
{
    public static class FourierTransformBenchmarks
    {
        public static void AllBenchmark()
        {
            uint[] counts = new uint[20];
            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = (uint)Math.Pow(2, i);
            }
            long[,] times = new long[counts.Length, 7];
            const int padding = -15;
            Console.WriteLine($"{"Count",padding}" +
                $"{nameof(FourierTransformOperations.DFT),padding}" +
                $"{nameof(FourierTransformOperations.FastDFT),padding}" +
                $"{nameof(FourierTransformOperations.ParallelDFT),padding}" +
                $"{nameof(FourierTransformOperations.IterativeDFT),padding}" +
                $"{nameof(FourierTransformOperations.InverseDFT),padding}" +
                $"{nameof(FourierTransformOperations.FastInverseDFT),padding}" +
                $"{nameof(FourierTransformOperations.ParallelInverseDFT)}");
            for (int i = 0; i < counts.Length; i++)
            {
                uint count = counts[i];
                //times[i, 0] = FourierTransformBenchmark(count, FourierTransforms.DFT);
                times[i, 1] = FourierTransformBenchmark(count, FourierTransformOperations.FastDFT);
                times[i, 2] = FourierTransformBenchmark(count, FourierTransformOperations.ParallelDFT);
                times[i, 3] = FourierTransformBenchmark(count, FourierTransformOperations.IterativeDFT);
                //times[i, 4] = FourierTransformBenchmark(count, FourierTransforms.InverseDFT);
                times[i, 5] = FourierTransformBenchmark(count, FourierTransformOperations.FastInverseDFT);
                times[i, 6] = FourierTransformBenchmark(count, FourierTransformOperations.ParallelInverseDFT);
                Console.Write($"{count,padding}");
                Console.Write($"{times[i, 0],padding}");
                Console.Write($"{times[i, 1],padding}");
                Console.Write($"{times[i, 2],padding}");
                Console.Write($"{times[i, 3],padding}");
                Console.Write($"{times[i, 4],padding}");
                Console.Write($"{times[i, 5],padding}");
                Console.WriteLine($"{times[i, 6]}");
            }
        }
        public static void AllBenchmark(Func<Complex[], Complex[]>[] functions, uint counts = 15, uint iterations = 1)
        {
            long[,] times = new long[counts, functions.Length];
            const int padding = -22;

            Console.WriteLine();
            Console.Write($"{"Count",padding}");
            foreach (var function in functions)
            {
                Console.Write($"{function.GetMethodInfo().Name,padding}");
            }
            Console.WriteLine();

            for (int count = 1; count < counts; count++)
            {
                Console.Write($"{1 << count,padding}");
                for (int i = 0; i < functions.Length; i++)
                {
                    times[count, i] = FourierTransformBenchmark((uint)1 << count, functions[i], iterations);
                    Console.Write($"{times[count, i],padding}");
                }
                Console.WriteLine();
            }
        }
        public static long FourierTransformBenchmark(uint count, Func<Complex[], Complex[]> FourierTransform)
        {
            Complex[] input = new Complex[count];
            Random random = new();
            for (int i = 0; i < count; i++)
            {
                input[i] = new Complex(random.NextDouble(), random.NextDouble());
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            FourierTransform(input);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        public static long FourierTransformBenchmark(uint count, Func<Complex[], Complex[]> FourierTransform, uint iterations)
        {
            Stopwatch stopwatch = new();
            for (int i = 0; i < iterations; i++)
            {
                Complex[] input = new Complex[count];
                Random random = new();
                for (int j = 0; j < count; j++)
                {
                    input[j] = new Complex(random.NextDouble(), random.NextDouble());
                }
                stopwatch.Start();
                FourierTransform(input);
                stopwatch.Stop();
                //Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
            return stopwatch.ElapsedMilliseconds / iterations;
        }
    }
}
