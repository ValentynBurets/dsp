using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace dsp.Model
{
    public class DiscreteFurierTransform
    {
        public DiscreteFurierTransform(List<double> x, List<double> y)
        {
            N = y.Count-1;
            Data = new List<Point>();

            for (int i = 0; i < N; i++)
                Data.Add(new Point(x[i], y[i]));
            
            Period = 2 * Math.PI;
            Logger = new StringBuilder();
        }

        public int N { get; set; }

        public double Period { get; set; }
        public List<Point> Data { get; set; }
        public StringBuilder Logger { get; set; }

        private double CalculateA_0()
        {
            double a0 = Data.Skip(1).Select(point => point.Y).Sum() / (N + 1);
            Logger.Append($"A0 =\t {a0}\n");
            return a0;
        }

        private double CalculateA_n(int k)
        {
            double an = 2.0 / (N + 1) * Data.Skip(1).Select(p => p.Y * Math.Cos(k * p.X)).Sum();
            Logger.Append($"A{k + 1} =\t {an.ToString("F6")}\n");
            return an;
        }

        private double CalculateB_n(int k)
        {
            double bn = 2.0 / (N+1) * Data.GetRange(1, N-1).Select(p => p.Y * Math.Sin(k * p.X)).Sum();
            Logger.Append($"B{k} =\t {bn.ToString("F6")}\n");
            return bn;
        }

        public double Aproximate(double x)
        {
            Logger.Clear();
            double result = CalculateA_0() / 2 + Enumerable.Range(0, N)
                .Select(i => CalculateA_n(i) * Math.Cos(i * x) + CalculateB_n(i) * Math.Sin(i * x))
                .Sum();

            return result / 2.25;
        }

        public void Save()
        {
            Aproximate(0d);
            File.WriteAllText(@"log.TXT", Logger.ToString());
        }

        public double[] AproximateComplex(double[] x)
        {
            int N = x.Length;
            double[] y = new double[N];
            Complex[] X = new Complex[N];
            for (int k = 0; k < N; k++)
            {
                for (int n = 0; n < N; n++)
                {
                    X[k] += new Complex
                    (
                        x[n] * Math.Cos(2 * Math.PI * k * n / N),
                        x[n] * Math.Sin(2 * Math.PI * k * n / N)
                    );
                }
                y[k] += X[k].Real;
            }
            return y;
        }

        public double[] AproximateComplexMatrix(IEnumerable<double> y)
        {
            int N = y.Count();
            double[] yRes = new double[N];
            Complex w = Complex.Pow(Math.E, -2 * Math.PI * Complex.ImaginaryOne / N);
            Complex[] F = new Complex[N];
            Complex[,] W = new Complex[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    W[i, j] = Complex.Pow(w, i * j);
                    F[i] += W[i, j] * y.ElementAt(j);
                }
                yRes[i] = F[i].Magnitude;
            }

            return yRes;
        }

    }
}