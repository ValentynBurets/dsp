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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace dsp.ViewModel
{
    public class FourierSeriesViewModel : INotifyPropertyChanged
    {
        private FourierSeries fourierSeries;
        private ObservableCollection<DataPoint> _points;

        public ObservableCollection<DataPoint> Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                OnPropertyChanged(nameof(Points));
            }
        }

        private ObservableCollection<DataPoint> _approximationPoints;

        public ObservableCollection<DataPoint> ApproxiamationPoints
        {
            get
            {
                return _approximationPoints;
            }
            set
            {
                _points = value;
                OnPropertyChanged(nameof(ApproxiamationPoints));
            }
        }

        private double _Xmin = -Math.PI;
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

        private double _Xmax = Math.PI;
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
                      if (fourierSeries.Accuracy == 0 && fourierSeries.AnAnalytical == 0 && fourierSeries.Fi0Analytical == 0 && fourierSeries.N == 0)
                      {
                          MessageBox.Show("Something went wrong!");
                          return;
                      }

                      if (Points == null)
                          Points = new ObservableCollection<DataPoint>();
                      if (Points.Count > 1)
                          Points.Clear();

                      Points = CalculatePointsFunc();

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
            Points = CalculatePointsFunc();
            fourierSeries = new FourierSeries
            {
                Accuracy = 0,
                Fi0Analytical = 0,
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
            double res = Math.Abs(Points[0].X);

            foreach (var item in Points)
            {
                if (Math.Abs(item.X) > res)
                    res = Math.Abs(item.X);
                if (Math.Abs(item.Y) > res)
                    res = Math.Abs(item.Y);
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
            pm.Title = "Hexagon";
            pm.PlotType = PlotType.Cartesian;

            return pm;
        }

        public Func<double, double> Function { get; set; }

        private void UpdateImagePlot()
        {
            Function = new PeriodicFunction(x => Math.Sign(x)).Invoke;


            var pm = SetPlot();
            var function = new LineSeries();

            foreach (DataPoint points in Points.ToList())
            {

                var pointAnnotation = new PointAnnotation()
                {
                    X = Convert.ToDouble(points.X),
                    Y = Convert.ToDouble(points.Y),
                };
                function.Points.Add(new OxyPlot.DataPoint(Convert.ToDouble(points.X), Convert.ToDouble(points.Y)));
                pm.Annotations.Add(pointAnnotation);
            }
            pm.Series.Add(function);

            FourierSeriesImage = pm;
        }


        #endregion
        public ObservableCollection<DataPoint> CalculatePointsFunc()
        {
            ObservableCollection<DataPoint> _points = new ObservableCollection<DataPoint>();

            double start_x = -Math.PI;
            double finish = 0;

            double temp = 0.1;

            if (fourierSeries != null && fourierSeries.N != 0)
                temp = (finish - start_x) / fourierSeries.N;

            while (start_x <= finish)
            {
                double? tempY = Func(start_x);
                if (tempY != null)
                    _points.Add(new DataPoint(start_x, (double)tempY));
                start_x += temp;
            }

            return _points;
        }

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
