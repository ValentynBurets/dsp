using dsp.MathLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace dspUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            PeriodicFunction func = new PeriodicFunction((x) => (x));
            int period = 0;
            double step = 0.1;
            
            List<double> expected = new List<double>();

            for (double i = -Math.PI; i < Math.PI; i+=step)
            {
                expected.Add(i);
            }

            List<double> lists = PeriodicFunction.GetPeriod(period, step);

            //Assert.Collection(expected, lists);
        }
    }
}
