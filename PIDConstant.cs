using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class PIDConstant
    {
        public PIDConstant()
        {

        }

        public PIDConstant(double cp, double ti, double td, PIDControlAlgorithm algor = PIDControlAlgorithm.PID, bool liveUpdEnable = false, bool reverse = false, double tcoef = 1)
        {
            this.CP = cp;
            this.TI = ti;
            this.TD = td;
            this.ControlAlgorithm = algor;
            this.LiveUpdateEnable = liveUpdEnable;
            this.Reverse = reverse;
            this.TargetCoefficient = tcoef;
        }

        public PIDConstant(ComputeParams cps, PIDControlAlgorithm algor = PIDControlAlgorithm.PID, bool liveUpdEnable = false, bool reverse = false, double tcoef = 1)
        {
            this.ComputeParams = cps;
            this.ControlAlgorithm = algor;
            this.LiveUpdateEnable = liveUpdEnable;
            this.Reverse = reverse;
            this.TargetCoefficient = tcoef;
        }

        public double CP { get; set; }

        public double KP
        {
            get
            {
                if (this.CP == 0)
                {
                    return double.PositiveInfinity;
                }
                else
                {
                    return 100 / this.CP;
                }
            }
        }

        public double TI { get; set; }

        public double TD { get; set; }


        public ComputeParams ComputeParams { get; set; }


        /// <summary>
        /// PID控制算法
        /// 通常情况下,输入部分使用IPD,输出部分使用PID
        /// </summary>
        public PIDControlAlgorithm ControlAlgorithm { get; set; }

        /// <summary>
        /// 是否控制反转
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// 目标系数
        /// </summary>
        public double TargetCoefficient { get; set; }

        /// <summary>
        /// 开启实时更新PID
        /// </summary>
        public bool LiveUpdateEnable { get; set; }

        public PIDConstant GetPID(PIDConstant cst, PIDModel item)
        {
            if (cst != null && cst.ComputeParams != null)
            {
                var cps = cst.ComputeParams;
                double tmpP = cps.P0 - cps.a * item.D * cps.b * item.DD;
                double tmpI = cps.I0 - cps.m * item.Err * cps.n * item.DD;
                double tmpD = cps.D0 + cps.k * Math.Abs(item.DD);

                this.CP = Math.Min(cps.PMax, Math.Max(cps.PMin, tmpP));
                this.TI = Math.Min(cps.IMax, Math.Max(cps.IMin, tmpI));
                this.TD = Math.Min(cps.DMax, Math.Max(cps.DMin, tmpD));
            }

            return this;
        }

    }


    public class ComputeParams
    {
        public ComputeParams()
        {

        }

        public ComputeParams(double p0, double i0, double d0, double a, double b, double m, double n, double k, double pMin, double pMax, double iMin, double iMax, double dMin, double dMax)
        {
            this.P0 = p0;
            this.I0 = i0;
            this.D0 = d0;
            this.a = a;
            this.b = b;
            this.m = m;
            this.n = n;
            this.k = k;
            this.PMin = PMin;
            this.PMax = PMax;
            this.IMin = iMin;
            this.IMax = IMax;
            this.DMin = DMin;
            this.DMax = dMax;
        }

        public double P0 { get; set; }
        public double I0 { get; set; }
        public double D0 { get; set; }

        public double a { get; set; }
        public double b { get; set; }
        public double m { get; set; }
        public double n { get; set; }
        public double k { get; set; }

        public double PMin { get; set; }
        public double PMax { get; set; }

        public double IMin { get; set; }
        public double IMax { get; set; }

        public double DMin { get; set; }
        public double DMax { get; set; }
    }

    public enum PIDControlAlgorithm
    {
        IPD,        //使用DeltaPV
        PID         //使用DeltaDV
    }
}
