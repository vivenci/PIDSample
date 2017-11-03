using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 基础PID模型
    /// </summary>
    public class PIDModel
    {
        private double variation;

        public PIDModel()
        {

        }

        public PIDModel(double sv, double pv, double interval = 1)
        {
            this.SV = sv;
            this.PV = pv;
            this.Interval = interval;
        }

        public PIDModel(double sv, double pv, double min, double max, double interval = 1) : this(sv, pv, interval)
        {
            this.Minimum = min;
            this.Maximum = max;
            this.EnableAdjust = true;
        }

        /// <summary>
        /// 目标值
        /// </summary>
        public double SV { get; set; }

        /// <summary>
        /// 测量值
        /// </summary>
        public double PV { get; set; }

        /// <summary>
        /// 目标值的测量值的偏差
        /// </summary>
        public double Err
        {
            get
            {
                return PV - SV;
            }
        }

        public double Interval { get; set; }

        public double P { get; set; }

        public double I { get; set; }

        /// <summary>
        /// DV变化量
        /// </summary>
        public double D { get; set; }

        /// <summary>
        /// D的变化量(DV变化量的变化量)
        /// </summary>
        public double DD { get; set; }

        /// <summary>
        /// PV的变化量
        /// </summary>
        public double DPV { get; set; }

        /// <summary>
        /// DPV的变化量(PV变化量的变化量)
        /// </summary>
        public double DDPV { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public double Minimum { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public double Maximum { get; set; }

        /// <summary>
        /// 计算所得的目标变化量
        /// </summary>
        public double Variation
        {
            get
            {
                return variation;

            }
            set
            {
                double tmp = value;
                if (this.EnableAdjust)
                {
                    tmp = this.Minimum == 0 ? value : Math.Max(this.Minimum, value);
                    tmp = this.Maximum == 0 ? tmp : Math.Min(this.Maximum, tmp);
                }
                variation = tmp;
            }
        }

        /// <summary>
        /// 在集合中的序号
        /// </summary>
        public UInt64 Index { get; set; }

        /// <summary>
        /// 是否启动校验
        /// </summary>
        public bool EnableAdjust { get; set; }


        /// <summary>
        /// Variation经过取值范围[Minimum , Maximum]校准后取得的值
        /// </summary>
        //public double AdjustValue
        //{
        //    get
        //    {
        //        if (this.EnableAdjust)
        //        {
        //            double v = this.Variation;
        //            double tmp = this.Minimum == 0 ? v : Math.Max(this.Minimum, v);
        //            tmp = this.Maximum == 0 ? tmp : Math.Min(this.Maximum, tmp);
        //            return tmp;
        //        }
        //        else
        //        {
        //            return this.Variation;
        //        }
        //    }
        //}
    }
}
