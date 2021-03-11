using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsp.MathLogic
{
	public static class MathIntegration
	{
		public static double MethodLeftRectangle(Func<double, double> MyFunction, double a, double b, int n)
		{
			double I = 0;
			double h = (b - a) / n;

			double x = a;

			for (int i = 0; i < n - 1; i++)
			{
				I += MyFunction(x);
				x += h;
			}

			I *= h;
			return I;
		}

		public static double MethodRightRectangle(Func<double, double> MyFunction, double a, double b, int n)
		{
			double I = 0;
			double h = (b - a) / n;

			double x = a;

			for (int i = 1; i < n; i++)
			{
				I += MyFunction(x);
				x += h;
			}

			I *= h;
			return I;
		}


		public static double MethodMiddleRectangle(Func<double, double> MyFunction, double a, double b, int n)
		{
			double I = 0;
			double h = (b - a) / n;

			double x = a;

			for (int i = 1; i < n; i++)
			{
				I += MyFunction(x + h / 2);
				x += h;
			}

			I *= h;
			return I;
		}

        public static double MethodTrapezoid(Func<double, double> MyFunction, double a, double b, int n)
        {

            double h = (b - a) / n;
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
        //public static double MethodTrapezoid(Func<double, double> f, double a, double b, int n)
        //{
        //	double sum = 0.0;
        //	double h = (b - a) / n;
        //	for (int i = 0; i < n; i++)
        //		sum += 0.5 * h * (f(a + i * h) + f(a + (i + 1) * h));
        //	//var e = Enumerable.Range(0, n)
        //	//        .Select(i => 0.5 * h * (f(a + i * h) + f(a + (i + 1) * h)))
        //	//        .Sum();
        //	return sum;
        //}

        public static double SimpsonMethod(Func<double, double> MyFunction, double a, double b, int n)
        {
            double h = (b - a) / n;
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


    }
}
