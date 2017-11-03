using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class RadioModel : PIDModel
    {
        public RadioModel()
        {
        }

        public RadioModel(double pv, double rpv, double scale, double correctionValue, double unitFactor = 1, double interval = 1)
        {
            this.PV = pv;
            this.RPV = rpv;
            this.ScaleFactor = scale;
            this.Interval = interval;
            this.UniFactor = unitFactor;
        }

        public RadioModel(double pv, double rpv, double scale, double correctionValue, double min, double max, double unitFactor = 1, double interval = 1) : this(pv, rpv, scale, correctionValue, unitFactor, interval)
        {
            this.Minimum = min;
            this.Maximum = max;
            this.EnableAdjust = true;
        }

        /// <summary>
        /// 和比例系数组成目标值的PV
        /// </summary>
        public double RPV { get; set; }

        /// <summary>
        /// 比例系数
        /// </summary>
        public double ScaleFactor { get; set; }

        public new double SV { get; set; }

        /// <summary>
        /// 矫正偏差量
        /// </summary>
        public double CorrectionValue { get; set; }

        /// <summary>
        /// 单位换算系数
        /// </summary>
        public double UniFactor { get; set; }
    }
}
