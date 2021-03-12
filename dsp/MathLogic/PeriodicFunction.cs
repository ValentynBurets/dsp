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
    }
}