using dsp.additional;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace dsp.ViewModel
{
    public class FourierSeriesViewModel : INotifyPropertyChanged
    {
        private FourierSeries fourierSeries;

        private List<List<double>> _xRange;

        public List<List<double>> XRange
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


        private List<double> _yAproximation;
        public List<double> YAproximation
        {
            get
            {
                return _yAproximation;
            }
            set
            {
                _yAproximation = value;
                OnPropertyChanged(nameof(YAproximation));
            }
        }

        /*
        private double _step;
        public double Step
        {
            get
            {
                return Math.Abs(fourierSeries.XMax - fourierSeries.XMin) / 10.0; ;
            }
            set
            {
                _step = value;
                OnPropertyChanged(nameof(Step));
            }
        }
        */


        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {

                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      SelectedParameters = fourierSeries;

                      fourierSeries = obj as FourierSeries;

                      if (fourierSeries == null)
                      {
                          MessageBox.Show("Enter Data!");
                          return;
                      }
                      
                      UpdateImagePlot();
                      CalculateError();
                      fourierSeries.file.WriteInFile();
                  }));
            }
        }

        public FourierSeries SelectedParameters
        {
            get { return fourierSeries; }
            set
            {
                fourierSeries = value;

                OnPropertyChanged();
            }
        }



        public FourierSeriesViewModel()
        {


            fourierSeries = new FourierSeries
            {
                Function = new PeriodicFunction((x) => (x)).Invoke,
                Prescision = 1003,
                N = 10
            };
            
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


        private double PointAway()
        {
            double res = Math.Abs(XRange[0][0]);

            //foreach (var item in XRange)
            //{
            //    if (Math.Abs(item) > res)
            //        res = Math.Abs(item);
            //}

            return res;
        }

        private PlotModel SetPlot()
        {
            double PointpointAway = PointAway();
            PointpointAway += 10;

            var pm = new PlotModel();

            pm.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = -PointpointAway,
                Maximum = PointpointAway,
                PositionAtZeroCrossing = true,
                ExtraGridlines = new[] { 0.0 }
            });
            pm.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = -PointpointAway,
                Maximum = PointpointAway,
                PositionAtZeroCrossing = true,
                ExtraGridlines = new[] { 0.0 }
            });
            //pm = "Hexagon";
            pm.PlotType = PlotType.Cartesian;

            return pm;
        }
        
        private const double Tick = 0.02d;
        private void UpdateImagePlot()
        {
            //set range of system

            //XRange = new List<double>();
            //XRange.AddRange(PeriodicFunction.GetRange(fourierSeries.XMin, fourierSeries.XMax, Tick));
            //XRange.RemoveAt(0);

            XRange = new List<List<double>>();
            XRange.AddRange(PeriodicFunction.GetRange(fourierSeries.Period, Tick));
            
            //----------------------------------------------


            var pm = SetPlot();
            //set periods

            int periods = 20;
            int left = periods / 2 * (-1) - 1;
            int right = periods / 2 + 1;

            //for(int i = left; i < right; i++)
            //{
            //    var polyLineAnnotation = new PolylineAnnotation();
            //    polyLineAnnotation.Points.Add(new DataPoint(i * Math.PI, -50));
            //    polyLineAnnotation.Points.Add(new DataPoint(i * Math.PI, 50));
            //    if(i != 0)
            //        polyLineAnnotation.Text = String.Format($"{i}Pi");
            //    polyLineAnnotation.TextLinePosition = 0.48;
            //    polyLineAnnotation.TextOrientation = AnnotationTextOrientation.Horizontal;

            //    pm.Annotations.Add(polyLineAnnotation);

            //}


            // initialize function plot
            
            YValues = new List<double>();
            bool titleFlag = false;

            // initialize approximation plot
            var approximation = new LineSeries();
            approximation.Title = "Fourier Series";
            YAproximation = new List<double>();
            fourierSeries.file.Write($"\n\nF(x) = x, -PI < x < PI \n Порядок {fourierSeries.N} \n Approximation\n");
            fourierSeries.CalculateElements();

            //---------------------------------

            foreach (var itemPeriod in XRange.ToList())
            {
                var function = new LineSeries();

                if (!titleFlag)
                {
                    function.Title = "f(x) = x";
                    titleFlag = true;
                }

                foreach (var item in itemPeriod)
                {
                    double tempY = fourierSeries.Function(item);
                    YValues.Add(tempY);

                    function.Points.Add(new OxyPlot.DataPoint(Convert.ToDouble(item), Convert.ToDouble(tempY)));

                    //approximation
                    double tempAprox = fourierSeries.Approximate(item);
                    YAproximation.Add(tempAprox);

                    approximation.Points.Add(new OxyPlot.DataPoint(Convert.ToDouble(item), Convert.ToDouble(tempAprox)));

                }

                pm.Series.Add(function);

            }

            
            pm.Series.Add(approximation);

            #endregion
            FourierSeriesImage = pm;
        }


        #region error
        private void CalculateError()
        {
            
            var abs = YValues.Zip(YAproximation, (Yv, Ya) => Math.Abs(Yv - Ya));

            var absError = abs.Sum() / fourierSeries.N;
            fourierSeries.file.Write($"\nсередня абсолютна похибка вимірювань {Math.Round(absError * 0.1, 5)}");

            var relativeError = abs.Zip(YValues, (a, Yv) => a / Yv).Average() * 100;

            fourierSeries.file.Write($"\nсередня відносна похибка вимірювань {Math.Round(relativeError, 5)} %");
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
