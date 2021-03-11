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
using System.Linq;
using System.Runtime.CompilerServices;
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


        private double _Xmin = -Math.PI * 2;
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

        private double _Xmax = Math.PI * 2;
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



        private double _step;
        public double Step
        {
            get
            {
                return Math.Abs(XMax - XMin) / 10.0; ;
            }
            set
            {
                _step = value;
                OnPropertyChanged(nameof(Step));
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
                      SelectedParameters = fourierSeries;

                      fourierSeries = obj as FourierSeries;

                      if (fourierSeries == null)
                      {
                          MessageBox.Show("Enter Data!");
                          return;
                      }
                      
                      UpdateImagePlot();

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
            //pm.Title = "Hexagon";
            pm.PlotType = PlotType.Cartesian;

            return pm;
        }

        private void UpdateImagePlot()
        {
            //set range of system

            XRange = new List<double>();
            XRange.AddRange(PeriodicFunction.GetRange(XMin, XMax, Tick));

            //----------------------------------------------

            
            var pm = SetPlot();

            #region intialize function plot
            
            var function = new LineSeries();
            YValues = new List<double>();
            
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

            YAproximation = new List<double>();

            foreach (var item in XRange.ToList())
            {
                double tempY = fourierSeries.Aproximate(item);
                YAproximation.Add(tempY);
                
                approximation.Points.Add(new OxyPlot.DataPoint(Convert.ToDouble(item), Convert.ToDouble(tempY)));
            }
            pm.Series.Add(approximation);

            #endregion
            FourierSeriesImage = pm;
        }


        #endregion


        private const double Tick = 0.02d;
        //public ObservableCollection<DataPoint> CalculatePointsFunc()
        //{
        //    fourierSeries.Function = new PeriodicFunction(x => Math.Sign(x)).Invoke;

        //    ObservableCollection<DataPoint> _points = new ObservableCollection<DataPoint>();

        //    double start_x = -Math.PI;
        //    double finish = 0;

        //    double temp = 0.1;

        //    if (fourierSeries != null && fourierSeries.N != 0)
        //        temp = (finish - start_x) / fourierSeries.N;

        //    while (start_x <= finish)
        //    {
        //        double? tempY = Func(start_x);
        //        if (tempY != null)
        //            _points.Add(new DataPoint(start_x, (double)tempY));
        //        start_x += temp;
        //    }

        //    return _points;
        //}

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
