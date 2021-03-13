using dsp.additional;
using dsp.MathLogic;
using Hangfire.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace dsp.Model
{

    public class FourierSeries : INotifyPropertyChanged
    {        
        public int Prescision { get; set; }

        public Func<double, double> Function { get; set; }

        private double A0;
        private List<double> Ans;
        private List<double> Bns;

        private Func<Func<double, double>, double, double, int, double> IntegrationMethod { get; set; }

        private int _period = 1;
        public int Period
        {
            get
            {
                return _period;
            }
            set
            {
                _period = value;
                XMin = -1 * value * Math.PI;
                XMax = value * Math.PI;
                OnPropertyChanged(nameof(Period));
            }
        }

        private double _Xmin;
        public double XMin
        {
            get
            {
                return _Xmin;
            }
            set
            {
                _Xmin = value;
                OnPropertyChanged(nameof(XMin));
            }
        }

        private double _Xmax;
        public double XMax
        {
            get
            {
                return _Xmax;
            }
            set
            {
                _Xmax = value;
                OnPropertyChanged(nameof(XMax));
            }
        }


        public FourierSeries() 
        {
            IntegrationMethod = MathIntegration.MethodTrapezoid;
            //IntegrationMethod = MathIntegration.SimpsonMethod;
            //IntegrationMethod = MathIntegration.MethodMiddleRectangle;
            Period = 1;
            file = new Writer();
        }

        public void CalculateElements()
        {
            CalculateA0();
            Ans = new List<double>();
            Bns = new List<double>();

            for(int i = 1; i <= N; i++)
            {
                Ans.Add(CalculateAn(i));
                Bns.Add(CalculateBn(i));
            }
        }

        public Writer file;
        

        private int _n;

        public int N
        {
            get { return _n; }
            set
            {
                _n = value;
                OnPropertyChanged("N");
            }
        }


        private double CalculateA0()
        {
            var res = 2.0 / (2 * Math.PI) * IntegrationMethod(Function, -Math.PI, Math.PI, Prescision);
            file.Write($"A0 {res} \n");
            A0 = res;
            return res;
        }
        private double CalculateAn(int n)
        {
            var res = 2.0 / (2 * Math.PI) * IntegrationMethod((x) => Function(x) * Math.Cos(n * x), -Math.PI, Math.PI, Prescision);
            file.Write($"A{n} {res} \n");
            return res;
        }

        private double CalculateBn(int n)
        {
            var res = 2.0 / (2 * Math.PI) * IntegrationMethod((x) => Function(x) * Math.Sin(n * x), -Math.PI, Math.PI, Prescision);
            file.Write($"B{n} {res} \n");
            return res;
        }



        public double Approximate(double x)
        {
            if(Function(x) == Function(-x))
            {
                return A0 / 2.0 + Enumerable.Range(1, N)
                    .Select(i => Ans[i - 1] * Math.Cos(i * x))
                    .Sum();
            }
            if (Function(-x) == -Function(x))
            {
                return Enumerable.Range(1, N)
                    .Select(i => Bns[i - 1] * Math.Sin(i * x))
                    .Sum();
            }
            else
            {
                return A0 / 2.0 + Enumerable.Range(1, N)
                    .Select(i => Ans[i - 1] * Math.Cos(i * x) + Bns.ElementAt(i) * Math.Sin(i * x))
                    .Sum();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

