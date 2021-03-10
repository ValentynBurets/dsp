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

        private double _accuracy;

        private int _fi0Analytical;
        private int _n;
        private double _anAnalytical;
        public Func<double, double> Function { get; set; }

        private Func<Func<double, double>, double, double, int, double> IntegrationMethod { get; set; }

        public FourierSeries()
        {
            IntegrationMethod = MathIntegration.MethodTrapezoid;
        }
        public double Accuracy
        {
            get { return _accuracy; }
            set
            {
                _accuracy = value;
                OnPropertyChanged("Accuracy");
            }
        }

        public int Fi0Analytical
        {
            get { return _fi0Analytical; }
            set
            {
                _fi0Analytical = value;
                OnPropertyChanged("Fi0Analytical");
            }
        }

        public int N
        {
            get { return _n; }
            set
            {
                _n = value;
                OnPropertyChanged("N");
            }
        }
        public double AnAnalytical
        {
            get { return _anAnalytical; }
            set
            {
                _anAnalytical = value;
                OnPropertyChanged("AnAnalytical");
            }
        }

        //Код програми для визначення кількості доданків у сумі:

        public int FindN()
        {
            double firstValueAnalytically, secondValueAnalytically;
            int n = 1;
            do
            {
                firstValueAnalytically = GetGAnalitycally(Math.PI / 4, n);
                n += 2;
                secondValueAnalytically = GetGAnalitycally(Math.PI / 4, n);
            } while (Math.Abs(firstValueAnalytically - secondValueAnalytically) > _accuracy);
            _n = n - 2;
            return _n;
        }

        //Код для пошуку абсолютної похибки:

        public double FindCommonAbsoluteError()
        {
            double testValue = Math.PI * 5 / 6;
            double firstValue = GetGAnalitycally(testValue, _n);
            double secondValue = GetGAnalitycally(testValue, _n + 2);
            return Math.Abs(firstValue - secondValue);
        }

        //Yes
        //Код для обрахунку значення за допомогою ряду Фур'є:
        public double GetGAnalitycally(double t, int n)
        {
            double rezult = Fi0Analytical / 2;
            double suma = 0;
            for (int i = 1; i <= n; ++i)
            {
                suma += AnAnalytical * Math.Cos(i * t) + GetBnAnalytically(i) * Math.Sin(i * t);
            }
            return rezult + suma;
        }

        public double GetBnAnalytically(int i)
        {
            throw new NotImplementedException();
        }

        //yes
        //Код для обрахунку коефіцієнтів ряду Фур'є:
        public double ComputeAn(int n)
        {
            return (1.0 - Math.Cos(Math.PI * n)) / (Math.PI * n * n);
        }

        public double ComputeBn(int n)
        {
            return -1.0 * (Math.Cos(Math.PI * n) / n);
        }

        //Код для обрахунку абсолютної похибки:
        public double ABSError(int n)
        {
            double suma = 0;
            for (int i = 0; i < n; i++)
            {
                suma += Math.Abs(Function(i) - GetGAnalitycally(i, n));
            }
            return suma / n;
        }

        //Код для обрахунку відносної похибки:
        public double RelativeError(int n)
        {
            double suma = 0;
            for (int i = 0; i < n; i++)
            {
                suma += (Math.Abs(Function(i) - GetGAnalitycally(i, n))) / Function(i);
            }
            return suma / n;
        }
        
        //Код для середньої квадратичної похибки:
        public double AverageSquareError(int n)
        {
            double suma = 0;
            for (int i = 0; i < n; i++)
            {
                suma += (Function(i) - GetGAnalitycally(i, n)) *
                    (Function(i) - GetGAnalitycally(i, n));
            }
            return Math.Sqrt(suma) / n;
        }

        //код що обчислюватиме наближення рядом
        //Фур’є з точністю до порядку N(брати цей параметр, як аргумент функції).

        private CalculateA0() => 2.0/ (2 * Math.PI) * IntegrationMethod

        public double Aproximate(double x)
        {
            if(Function(x) == Function(-x))
            {
                return CalculateA0() / 2.0
            }
            if(Function(-x) == -Function(x))
            {

            }
            else
            {

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

