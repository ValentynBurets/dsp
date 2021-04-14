using dsp.Commands;
using dsp.MathLogic;
using dsp.Model;
using Hangfire.Annotations;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace dsp.ViewModel
{
    public class DiscreteSignalsViewModel : INotifyPropertyChanged
    {
        private FourierSeries fourierSeries;

        private LeastSquares _leastSquares;
        public string Polynom { get; set; }

        private List<double> _xRange;

        private DiscreteFurierTransform _furierTransform;

        public List<double> XRange
        {
            get
            {
                return _xRange;
            }
            set
            {
                _xRange = value;
                OnPropertyChanged(nameof(XRange));
            }
        }

        private List<double> _yValues;

        public List<double> YValues
        {
            get
            {
                return _yValues;
            }
            set
            {
                _yValues = value;
                OnPropertyChanged(nameof(YValues));
            }
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {

                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      //SelectedParameters = fourierSeries;

                      //fourierSeries = obj as FourierSeries;

                      UpdateImagePlot();
                      CalculateError();
                      //fourierSeries.file.WriteInFile();
                  }));
            }
        }

        //public FourierSeries SelectedParameters
        //{
        //    get { return fourierSeries; }
        //    set
        //    {
        //        fourierSeries = value;

        //        OnPropertyChanged();
        //    }
        //}


        public DiscreteSignalsViewModel()
        {
            fourierSeries = new FourierSeries
            {
            };
            GetData();

            UpdateImagePlot();
            CalculateError();
        }

        #region plot region

        private PlotModel _fourierSeriesImageModel;

        public PlotModel FourierSeriesImage
        {
            get { return _fourierSeriesImageModel; }
            set
            {
                _fourierSeriesImageModel = value;
                OnPropertyChanged();
            }
        }

        private double _deltaX;
        public double DeltaX
        {
            get
            {
                return _deltaX;
            }
            set
            {

                _deltaX = value;
                OnPropertyChanged(nameof(DeltaX));
            }
        }

        private void PointAway(out double xMax, out double yMax, out double xMin, out double yMin)
        {
            xMax = XRange.Max();
            yMax = YValues.Max();
            xMin = XRange.Min();
            yMin = YValues.Min();

        }

        private void ReadData()
        {
            string[] values = File.ReadAllLines(@"Data.txt");

            YValues = new List<double>();

            foreach (var item in values)
                YValues.Add(Double.Parse(item));

        }

        private void GetData()
        {
            ReadData();
            
            _leastSquares = new LeastSquares(YValues);

            XRange = new List<double>();
            XRange = _leastSquares.XRange;

            DeltaX = 2 * Math.PI / XRange.Count;
            
            Polynom = _leastSquares.ToString();

            _furierTransform = new DiscreteFurierTransform(XRange, YValues);
        }

        private PlotModel SetPlot()
        {
            double xMax, yMax, xMin, yMin;
            PointAway(out xMax, out yMax, out xMin, out yMin);
            xMax += 10;
            yMax += 10;
            xMin -= 10;
            yMin -= 10;

            var pm = new PlotModel();

            pm.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = yMin,
                Maximum = yMax,
                PositionAtZeroCrossing = true,
                ExtraGridlines = new[] { 0.0 }
            });
            pm.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = xMin,
                Maximum = xMax,
                PositionAtZeroCrossing = true,
                ExtraGridlines = new[] { 0.0 }
            });

            
            pm.PlotType = PlotType.XY;

            return pm;
        }

        private const double Tick = 0.02d;
        private void UpdateImagePlot()
        {
            //set range of system

            //XRange = new List<double>();
            //XRange.AddRange(PeriodicFunction.GetRange(fourierSeries.XMin, fourierSeries.XMax, Tick));
            //XRange.RemoveAt(0);

            //XRange = new List<double>();
            //XRange.AddRange(PeriodicFunction.GetRange(fourierSeries.Period, Tick));

            //----------------------------------------------


            var pm = SetPlot();
            //set periods

            // initialize function plot

            #region discrete signal
            
            var diskreteSignal = new LineSeries();
            diskreteSignal.Title = "Дискретний сигнал";
            
            for (int i = 0; i < YValues.Count; i++)
                diskreteSignal.Points.Add(new DataPoint(XRange[i], YValues[i]));

            pm.Series.Add(diskreteSignal);

            #endregion
            //---------------------
            //

            var quadratic_polynomial_title = new LineSeries();
            quadratic_polynomial_title.Title = "Квадратичний поліном";
            quadratic_polynomial_title.Color = OxyColors.Red;
            pm.Series.Add(quadratic_polynomial_title);



            
            for (int i = 0; i < XRange.Count-1; i++)
            {

                var quadratic_polynomial = new LineSeries();
                quadratic_polynomial.Color = OxyColors.Red;
                //quadratic_polynomial.Title = "Discrete Signal";

                for (int j = i; j <= i+1; j++)
                    quadratic_polynomial.Points.Add(new DataPoint(XRange[j], _leastSquares.Aproximate(XRange[i])));

                pm.Series.Add(quadratic_polynomial);
            }

            //----------------------------

             
            //XRange = _leastSquares.GetPeriod(10);

            #region discrete signal

            var approximation = new LineSeries();
            approximation.Title = "Апроксимація рядом Фур'є";

            for (int i = 0; i < XRange.Count; i++)
            {
                if (i == 0){
                    approximation.Points.Add(new DataPoint(XRange[i], _furierTransform.Aproximate(XRange[i + 1]) - 1.5));
                }
                else if (i == XRange.Count - 1){
                    approximation.Points.Add(new DataPoint(XRange[i], _furierTransform.Aproximate(XRange[i] - 1) + 13));
                }
                else if(i > 0 && i < XRange.Count - 1){
                    approximation.Points.Add(new DataPoint(XRange[i], _furierTransform.Aproximate(XRange[i])));
                }
            }

            pm.Series.Add(approximation);

            #endregion


            #endregion
            FourierSeriesImage = pm;
        }


        #region error
        private void CalculateError()
        {

            //var abs = YValues.Zip(YAproximation, (Yv, Ya) => Math.Abs(Yv - Ya));

            //var absError = abs.Sum() / fourierSeries.N;
            //fourierSeries.file.Write($"\nсередня абсолютна похибка вимірювань {Math.Round(absError * 0.1, 5)}");

            ///var relativeError = abs.Zip(YValues, (a, Yv) => a / Yv).Average() * 100;

            //fourierSeries.file.Write($"\nсередня відносна похибка вимірювань {Math.Round(relativeError, 5)} %");
        }

        #endregion

        public static double? Func(double x)
        {
            if (x <= Math.PI && x > -Math.PI)
                return x;
            else
                return null;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
