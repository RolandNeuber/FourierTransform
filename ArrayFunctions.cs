namespace FourierTransforms
{
	public static class ArrayFunctions
	{
		public static void ToConsole<T>(T[] array)
		{
			int maxLength = 0;
			foreach (T item in array)
			{
				maxLength = Math.Max(maxLength, (item?.ToString() ?? "").Length);
			}

			foreach (T item in array)
			{
				int length = maxLength - (item?.ToString() ?? "").Length + 1;
				Console.Write(new string(' ', length) + item?.ToString());
			}
		}
		public static void ToConsole<T>(T[] array, Func<T, string> decorator)
		{
			int maxLength = 0;
			foreach (T item in array)
			{
				maxLength = Math.Max(maxLength, decorator(item).Length);
			}

			foreach (T item in array)
			{
				int length = maxLength - decorator(item).Length + 1;
				Console.Write(new string(' ', length) + decorator(item));
			}
		}

		public static void ToConsole<T>(T[,] array)
		{
			int maxLength = 0;
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					maxLength = Math.Max(maxLength, (array[i, j]?.ToString() ?? "").Length);
				}
			}

			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					int length = maxLength - (array[i, j]?.ToString() ?? "").Length + 1;
					Console.Write(new string(' ', length) + array[i, j]?.ToString());
				}
				Console.WriteLine();
			}
		}

		public static void ToConsole<T>(T[,] array, Func<T, string> decorator)
		{
			int maxLength = 0;
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					maxLength = Math.Max(maxLength, decorator(array[i, j]).Length);
				}
			}

			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					int length = maxLength - decorator(array[i, j]).Length + 1;
					Console.Write(new string(' ', length) + decorator(array[i, j]));
				}
				Console.WriteLine();
			}
		}
	}
}
