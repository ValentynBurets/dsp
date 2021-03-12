using dsp.additional;
using dsp.Commands;
using dsp.MathLogic;
using dsp.Model;
using Hangfire.Annotations;
using OxyPlot;
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

        private List<double> _xRange;

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
            double res = Math.Abs(XRange[0]);

            foreach (var item in XRange)
            {
                if (Math.Abs(item) > res)
                    res = Math.Abs(item);
            }

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

            XRange = new List<double>();
            XRange.AddRange(PeriodicFunction.GetRange(fourierSeries.XMin, fourierSeries.XMax, Tick));

            //----------------------------------------------

            
            var pm = SetPlot();

            #region intialize function plot
            
            var function = new LineSeries();
            YValues = new List<double>();
            function.Title = "f(x) = x";
            
            foreach (var item in XRange.ToList())
            {
                double tempY = fourierSeries.Function(item);
                YValues.Add(tempY);

                function.Points.Add(new OxyPlot.DataPoint(Convert.ToDouble(item), Convert.ToDouble(tempY)));
            }

            pm.Series.Add(function);
            #endregion

            #region intialize approximation plot
            var approximation = new LineSeries();
            approximation.Title = "Fourier Series";
            YAproximation = new List<double>();

            fourierSeries.file.Write($"F(x) = x, -PI < x < PI \n Порядок {fourierSeries.N} \n Approximation\n");
            fourierSeries.CalculateElements();

            foreach (var item in XRange.ToList())
            {
                double tempY = fourierSeries.Approximate(item);
                YAproximation.Add(tempY);
                
                approximation.Points.Add(new OxyPlot.DataPoint(Convert.ToDouble(item), Convert.ToDouble(tempY)));
            }
            pm.Series.Add(approximation);

            #endregion
            FourierSeriesImage = pm;
        }


        #endregion

        #region error
        private void CalculateError()
        {
            
            var abs = YValues.Zip(YAproximation, (Yv, Ya) => Math.Abs(Yv - Ya));
            var relativeError = abs.Zip(YValues, (a, Yv) => a / Yv).Average() * 100;

            fourierSeries.file.Write($"відносна похибка вимірювань {Math.Round(relativeError, 5)} %");
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
