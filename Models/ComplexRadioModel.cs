using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 复杂比例PID模型
    /// </summary>
    public class ComplexRadioModel : RadioModel
    {

        public ComplexRadioModel(double inSv, double inPv, double outPv, double rpv, double scale, double correctionValue, double unitFactor = 1, double interval = 1)
        {
            this.InSV = inSv;
            this.InPV = inPv;
            this.OutPV = outPv;
            this.RPV = rpv;
            this.ScaleFactor = scale;
            this.Interval = interval;
            this.CorrectionValue = correctionValue;
            this.UniFactor = unitFactor;
        }

        public ComplexRadioModel(double inSv, double inPv, double outPv, double rpv, RadioFormula formula, double correctionValue, double unitFactor = 1, double interval = 1)
        {
            this.InSV = inSv;
            this.InPV = inPv;
            this.OutPV = outPv;
            this.RPV = rpv;
            this.Interval = interval;
            this.CorrectionValue = correctionValue;
            this.UniFactor = unitFactor;
            this.FormulaEnable = true;
            this.RadioFormula = formula;
            if (this.RadioFormula != null)
            {
                this.ScaleFactor = this.RadioFormula.RFun(this.InPV);
            }
        }

        public ComplexRadioModel(double inSv, double inPv, double outPv, double rpv, double scale, double correctionValue, double min, double max, double unitFactor = 1, double interval = 1) : this(inSv, inPv, outPv, rpv, scale, correctionValue, unitFactor, interval)
        {
            this.Minimum = min;
            this.Maximum = max;
            this.EnableAdjust = true;
        }

        public ComplexRadioModel(double inSv, double inPv, double outPv, double rpv, RadioFormula formula, double correctionValue, double min, double max, double unitFactor = 1, double interval = 1) : this(inSv, inPv, outPv, rpv, formula, correctionValue, unitFactor, interval)
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

        /// <summary>
        /// 输入部分附加PID控制模块列表
        /// </summary>
        public List<PIDModel> AuxiliaryModels { get; set; }

        /// <summary>
        /// 矫正偏差量
        /// </summary>
        public new double CorrectionValue { get; set; }

        #endregion


        #region 输出部分

        public double OutSV { get; set; }

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

        public bool FormulaEnable { get; set; }

        public RadioFormula RadioFormula { get; set; }


        public void AddAuxiliaryModel(PIDModel m)
        {
            if (this.AuxiliaryModels != null)
            {
                this.AuxiliaryModels.Add(m);
            }
        }

        public void RemoveAuxiliaryModel(PIDModel m)
        {
            if (this.AuxiliaryModels != null)
            {
                this.AuxiliaryModels.Remove(m);
            }
        }

        public void ClearAuxiliaryModels()
        {
            if (this.AuxiliaryModels != null)
            {
                this.AuxiliaryModels.Clear();
            }
        }
    }

    public class RadioFormula
    {
        public RadioFormula(double a, double b, double c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.RFun = (x) => this.A * x * x + this.B * x + this.C;
        }

        /// <summary>
        /// 二次项系数
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// 一次项系数
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// 常数项系数
        /// </summary>
        public double C { get; set; }

        public Func<double, double> RFun { get; set; }
    }
}
