using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 加热炉回路模型
    /// 
    /// </summary>
    public class FurnaceModel : CascadePIDModel
    {
        public FurnaceModel()
        {

        }

        public FurnaceModel(double inSv, double inPv, double outBsv, double outPv, double interval = 1)
        {
            this.InSV = inSv;
            this.InPV = inPv;
            this.OutBasicSV = outBsv;
            this.OutPV = outPv;
            this.Interval = interval;
        }

        public FurnaceModel(double inSv, double inPv, double outBsv, double outPv, double min, double max, double interval = 1) : this(inSv, inPv, outBsv, outPv, interval)
        {
            this.Minimum = min;
            this.Maximum = max;
            this.EnableAdjust = true;
        }




    }
}
