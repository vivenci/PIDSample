using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 串级PID模型
    /// </summary>
    public class CascadePIDModel : PIDModel
    {

        public CascadePIDModel()
        {

        }

        public CascadePIDModel(double inSv, double inPv, double outBsv, double outPv, double interval = 1)
        {
            this.InSV = inSv;
            this.InPV = inPv;
            this.OutBasicSV = outBsv;
            this.OutPV = outPv;
            this.Interval = interval;
        }

        public CascadePIDModel(double inSv, double inPv, double outBsv, double outPv, double min, double max, double interval = 1) : this(inSv, inPv, outBsv, outPv, interval)
        {
            this.Minimum = min;
            this.Maximum = max;
            this.EnableAdjust = true;
        }

        #region 输入部分

        public double InSV { get; set; }

        public double InPV { get; set; }


        public double InErr
        {
            get
            {
                return InPV - InSV;
            }
        }

        public double InP { get; set; }

        public double InI { get; set; }

        public double InD { get; set; }

        public double InDD { get; set; }

        public double InDPV { get; set; }

        public double InDDPV { get; set; }

        public double InVariation { get; set; }


        public new double SV
        {
            get
            {
                return this.InSV;
            }
        }

        public new double PV
        {
            get
            {
                return this.InPV;
            }
        }

        public new double DPV
        {
            get
            {
                return this.InDPV;
            }
        }

        public new double DDPV
        {
            get
            {
                return this.InDDPV;
            }
        }
        #endregion



        #region 输出部分

        public double OutBasicSV { get; set; }

        public double OutSV
        {
            get
            {
                return this.OutBasicSV + this.InVariation;
            }
        }

        public double OutPV { get; set; }

        public new double Err
        {
            get
            {
                return OutPV - OutSV;
            }
        }

        public double OutDPV { get; set; }

        public double OutDDPV { get; set; }

        #endregion

    }
}
