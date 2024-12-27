using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureCalibration.Data
{
    public class MeasureModel
    {
        public MeasureModel(double time, double val)
        {
            Time = time;
            Value = val;
        }

        public double Time { get; set; }
        public double Value { get; set; }
    }
}
