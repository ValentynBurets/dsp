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
            if (x <= -Math.PI)
            {
                return Function(x + 2 * Math.PI);
            }
            if (x >= Math.PI)
            {
                return Function(x - 2 * Math.PI);
            }
            else
            {
                return Function(x);
            }
        }
    }
}