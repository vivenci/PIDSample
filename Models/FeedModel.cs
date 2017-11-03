using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 进料PID模型
    /// 
    /// 首次:    [输入] PVS=输入监测位号测量值组  PV=输入回路测量值 SV=输入回路测量值组
    ///             由PVS求得APVS,由PV,SV计算得deltaSV
    ///          [输出] PV=输出位号测量值  SV= 本次APVS + deltaSV
    ///             计算得deltaMV
    /// 第二次:  [输入] PVS=输入监测位号测量值组  PV=输入回路测量值 SV=输入回路测量值组
    ///             由PVS求得APVS,由PV,SV计算得deltaSV
    ///          [输出] PV=输出位号测量值  SV=本次APVS + 前次deltaSV + 当前deltaSV
    ///             计算得deltaMV
    ///             
    /// 第 i 次:  [输入] PVS=输入监测位号测量值组  PV=输入回路测量值 SV=输入回路测量值组
    ///             由PVS求得APVS,由PV,SV计算得deltaSV
    ///          [输出] PV=输出位号测量值  SV=本次APVS + 第0次deltaSV + 第1次deltaSV + …… + 第i-1次deltaSV + 当前deltaSV
    ///             计算得deltaMV
    ///  
    /// </summary>
    public class FeedModel : PIDModel
    {
        public FeedModel()
        {

        }

        public FeedModel(List<double> dpvs, double inSv, double inPv, double outPv, double correctionValue, double unitFactor = 1, double interval = 1)
        {
            this.DynamicPV = dpvs.Average();
            this.InSV = inSv;
            this.InPV = inPv;
            this.OutPV = outPv;
            this.Interval = interval;
            this.CorrectionValue = correctionValue;
            this.UniFactor = unitFactor;
        }

        public FeedModel(List<double> dpvs, double inSv, double inPv, double outPv, double correctionValue, double min, double max, double unitFactor = 1, double interval = 1) : this(dpvs, inSv, inPv, outPv, correctionValue, unitFactor, interval)
        {
            this.Minimum = min;
            this.Maximum = max;
            this.EnableAdjust = true;
        }

        #region 输入部分

        /// <summary>
        /// 从监测位号输入值组取得的平均值
        /// </summary>
        public double DynamicPV { get; protected set; }

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
        /// 矫正偏差量
        /// </summary>
        public double CorrectionValue { get; set; }

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

        /// <summary>
        /// 单位换算系数
        /// </summary>
        public double UniFactor { get; set; }
    }
}
