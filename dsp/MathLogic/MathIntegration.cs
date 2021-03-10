using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsp.MathLogic
{
	public static class MathIntegration
	{

		public static double FindH(double a, double b, double n)
		{
			return (b - a) / n;
		}

		public static double MethodLeftRectangle(double a, double b, int n)
		{
			double I = 0;
			double h = FindH(a, b, n);

			double x = a;

			for (int i = 0; i < n - 1; i++)
			{
				I += MyFunction(x);
				x += h;
			}

			I *= h;
			return I;
		}

		public static double MethodRightRectangle(double a, double b, int n)
		{
			double I = 0;
			double h = FindH(a, b, n);

			double x = a;

			for (int i = 1; i < n; i++)
			{
				I += MyFunction(x);
				x += h;
			}

			I *= h;
			return I;
		}


		public static double MethodMiddleRectangle(double a, double b, int n)
		{
			double I = 0;
			double h = FindH(a, b, n);

			double x = a;

			for (int i = 1; i < n; i++)
			{
				I += MyFunction(x + h / 2);
				x += h;
			}

			I *= h;
			return I;
		}

		public static double MethodTrapezoid(double a, double b, int n)
		{

			double h = FindH(a, b, n);
			double Res = 0;
			double x = a;

			for (int i = 0; i < n - 1; i++)
			{
				Res += MyFunction(x);
				x += h;
			}

			Res += ((MyFunction(a) + MyFunction(x)) / 2);

			Res *= h;

			return Res;
		}


		public static double SimpsonMethod(double a, double b, int n)
		{
			double h = FindH(a, b, n);
			double FourSum = 0, TwoSum = 0, Res;
			double index;
			double i = 0;

			while (i <= n)
			{
				FourSum += 4 * MyFunction(a + (2 * i - 1) * h);
				i++;
			}

			i = 0;
			while (i <= n - 1)
			{
				TwoSum += 2 * MyFunction(a + (2 * i) * h);
				i++;
			}

			index = (double)(2 * n);
			Res = (h / 4) * (MyFunction(a) + MyFunction(a + index * h) + FourSum + TwoSum);

			return Res;
		}

		public static double MyFunction(double x)
		{
			return x;
		}
		//}
		//public static double? Func(double x)
		//{
		//	if (x <= 0 && x > -Math.PI)
		//		return x;
		//	else
		//		return null;
		//}
	}
}
