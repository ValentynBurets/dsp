using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsp.Model
{
    class LeastSquares
    {
        public double[] Coeficients { get; set; }
        public int N { get; set; }

        public List<double> XRange { get; set; }

        public LeastSquares(List<double> y)
        {
            N = y.Count;
            XRange = GetPeriod();
            FindCoeficients(XRange, y);
        }

        private void FindCoeficients(List<double> x, List<double> y)
        {
            double[,] matr = new double[N,N];
            double[] b = new double[N];
            FormMatrix(x, y, ref matr, ref b, N);
            Coeficients = SolveUsingLU(matr, b, N);
        }

        private void FormMatrix(List<double> x, List<double> y, ref double[,] matr, ref double[] b, int n)
        {
            for(int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    matr[i, j] = Enumerable.Range(0, x.Count).Select(k => Math.Pow(x[k], i + j)).Sum();
                }
                b[i] += Enumerable.Range(0, y.Count).Select(k => y[k] * Math.Pow(x[k], i)).Sum();
            }
        }

        public double[] SolveUsingLU(double[,] matrix, double[] rightPart, int n)
        {
            double[,] lu = new double[n, n];
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                        sum += lu[i, k] * lu[k, j];
                    lu[i, j] = matrix[i, j] - sum;
                }
                for (int j = i + 1; j < n; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                        sum += lu[j, k] * lu[k, i];
                    lu[j, i] = (1 / lu[i, i]) * (matrix[j, i] - sum);
                }
            }

            double[] y = new double[n];
            for (int i = 0; i < n; i++)
            {
                sum = 0;
                for (int k = 0; k < i; k++)
                    sum += lu[i, k] * y[k];
                y[i] = rightPart[i] - sum;
            }

            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                sum = 0;
                for (int k = i + 1; k < n; k++)
                    sum += lu[i, k] * x[k];
                x[i] = (1 / lu[i, i]) * (y[i] - sum);
            }
            return x;
        }

        public List<double> GetPeriod(int n)
        {
            double left = -Math.PI;
            double right = Math.PI;

            List<double> res = new List<double>();

            
            double step = (right - left) / (n-1);
            double temp = left;
            for (double i = 0; i < n; i++)
            {
                res.Add(temp);
                temp += step;
            }

            return res;
        }

        public List<double> GetPeriod()
        {
            double left = -Math.PI;
            double right = Math.PI;

            List<double> res = new List<double>();

            double temp = left;
            double step = (right - left) / (N-1);

            for (double i = 0; i < N; i++)
            {
                res.Add(temp);
                temp += step;
            }

            return res;
        }

        public double Aproximate(double x)
        {
            double result = 0;
            for (int i = 0; i < N; i++)
            {
                result += Coeficients[i] * Math.Pow(x, i);
            }
            return result;
        }
    }
}
