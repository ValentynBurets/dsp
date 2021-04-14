using dsp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsp.MathLogic
{
    public class PeriodicFunction
    {
        public Func<double, double> Function { get; set; }
        public PeriodicFunction(Func<double, double> function)
        {
            Function = function;
        }

        public double Invoke(double x)
        {
            while (x <= -Math.PI) x += 2 * Math.PI;
            while (x > Math.PI) x -= 2 * Math.PI;
            return Function(x);
        }

        public static IEnumerable<double> GetRange(double min, double max, double step)
        {
            double diff = max - min;
            int steps = (int)(diff / step) - 1;

            return Enumerable.Range(0, (int)((max - min) / step))
                .Select(i => min + diff * i / steps);
        }

        public static List<List<double>> GetRange(int Period, double step)
        {
            int leftSide;
            int rightSide;

            List<List<double>> res = new List<List<double>>();

            rightSide = Period / 2;
            leftSide = rightSide * (-1);

            if (Period % 2 == 1)
                rightSide++;


            //calculate left side 
            for(int i = leftSide; i < rightSide; i++)
            {
                res.Add(GetPeriod(i, step));
            }


            return res;
        }

   

        public static List<double> GetPeriod(int period, double step)
        {
            period *= 2;
            double left = Math.PI * period - Math.PI;
            double right = Math.PI * period + Math.PI;

            List<double> res = new List<double>();

            for (double i = left + step; i < right; i += step)
            {
                res.Add(i);
            }
         
            return res;
        }

    }
}