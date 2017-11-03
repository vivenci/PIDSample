using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class ComplexRadioCollection : CollectionBase<ComplexRadioModel>
    {
        UInt64 serialIndex = 0;
        PIDCollection cIn = null;
        PIDCollection cOut = null;
        List<PIDConstant> auxiliaryConstants = null;
        List<PIDCollection> cAcs = null;

        public ComplexRadioCollection(PIDConstant pidcIn, PIDConstant pidcOut, List<PIDConstant> pidcAuxs, int updateFreq, int writeTimes, FetchType fetchType, double diff, List<double> cumulativeErrRanges, double interval = 1)
        {
            this.cAcs = new List<PIDCollection>();

            this.Interval = interval;
            this.UpdateFrequency = updateFreq;
            this.WriteTimes = writeTimes;
            this.FetchType = fetchType;
            this.Difference = diff;
            this.InPIDConstants = pidcIn;
            this.OutPIDConstants = pidcOut;
            this.StrategyEnable = false;
            this.StrategyPeriod = StrategyPeriod.Rest;

            this.InPIDConstants = pidcIn;
            this.OutPIDConstants = pidcOut;
            this.AuxiliaryConstants = pidcAuxs;

            this.CumulativeErrRanges = cumulativeErrRanges;
        }

        protected override void InsertItem(int index, ComplexRadioModel item)
        {
            if (this.Count > 0)
            {
                if (this.Count + 1 > this.Maximum)
                {
                    this.Remove(this.First());
                    index = index - 1;
                }
            }

            base.InsertItem(index, item);
            this.Standardize(index);
            serialIndex++;
        }

        public PIDConstant InPIDConstants { get; set; }

        public PIDConstant OutPIDConstants { get; set; }

        public List<PIDConstant> AuxiliaryConstants
        {
            get
            {
                return this.auxiliaryConstants;
            }
            set
            {
                this.auxiliaryConstants = value;
                if (value.Count > 0)
                {
                    if (this.cAcs != null && this.cAcs.Count < value.Count)
                    {
                        int diff = value.Count - this.cAcs.Count;
                        for (int i = 0; i < diff; i++)
                        {
                            this.cAcs.Add(new PIDCollection(null));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 最大容量
        /// </summary>
        public int Maximum { get; set; }

        public List<double> CumulativeErrRanges { get; set; }

        /// <summary>
        /// 当前时刻对应的序号
        /// </summary>
        public int K { get; set; }

        /// <summary>
        /// 默认采样时间间隔
        /// </summary>
        public double Interval { get; set; }

        /// <summary>
        /// 输入部分累计误差
        /// </summary>
        public double InCumulativeErr { get; set; }

        /// <summary>
        /// 输出部分累计误差
        /// </summary>
        public double OutCumulativeErr { get; set; }

        public int UpdateFrequency { get; set; }

        public int WriteTimes { get; set; }

        public FetchType FetchType { get; set; }

        public double Difference { get; set; }

        /// <summary>
        /// 是否启用策略
        /// </summary>
        public bool StrategyEnable { get; set; }

        public bool StrategySwitch { get; set; }

        /// <summary>
        /// 策略对象
        /// </summary>
        public StrategyObject StrategyObject { get; set; }

        /// <summary>
        /// 策略周期
        /// </summary>
        public StrategyPeriod StrategyPeriod { get; set; }

        private void Standardize(int k)
        {
            this.K = k;
            ComplexRadioModel item = this[K];

            //输入部分
            cIn = this.InitCollection(cIn, item, this.InPIDConstants, this.Maximum, ControllerType.ComplexRadio, PIDAction.Input, this.CumulativeErrRanges[0]);
            PIDModel inItem = cIn.Last();

            item.InP = inItem.P;
            item.InI = inItem.I;
            item.InD = inItem.D;
            item.InDD = inItem.DD;
            item.InDPV = inItem.DPV;
            item.InDDPV = inItem.DDPV;
            item.InVariation = inItem.Variation;
            this.InCumulativeErr = cIn.CumulativeErr;

            //附加部分
            if (this.AuxiliaryConstants != null)
            {

                for (int i = 0; i < this.AuxiliaryConstants.Count; i++)
                {
                    PIDConstant ct = this.AuxiliaryConstants[i];

                    PIDCollection ac = this.cAcs[i];
                    PIDModel cam = item.AuxiliaryModels[i];//取附加PID模型
                    ac = this.InitCollection(ac, cam, ct, this.Maximum, ControllerType.Single, PIDAction.None, this.CumulativeErrRanges[i + 1]);
                    cam = ac.Last();

                    item.AuxiliaryModels[i] = cam;
                    this.cAcs[i] = ac;
                }
            }

            double axVals = 0;//附加输入回路计算变化量
            if (item.AuxiliaryModels != null && item.AuxiliaryModels.Count > 0)
            {
                axVals = item.AuxiliaryModels.Sum(m => m.Variation);
            }

            //开关启动
            if (this.StrategySwitch)
            {
                bool pcheck = IsCheck(this.serialIndex, this.UpdateFrequency); //该轮是否进行检测

                bool doStrategy = false;

                //UpdateFrequency为1时,不启用策略
                if (this.UpdateFrequency == 1)
                {
                    doStrategy = false;
                }
                else if (this.StrategyPeriod == StrategyPeriod.Rest)
                {
                    //当前为休息周期,且进行检测
                    if (pcheck)
                    {
                        this.InitStrategy();
                        //满足条件,启用策略
                        if (this.StrategyEnable)
                        {
                            this.StrategyPeriod = StrategyPeriod.Running;//转换状态为运行
                            doStrategy = true;
                        }
                        //不满足条件,不启用策略,且保持休息周期状态
                        else
                        {
                            doStrategy = false;
                        }
                    }
                    //当前为休息周期,且不进行检测
                    else
                    {
                        doStrategy = false;
                    }

                }
                else if (this.StrategyPeriod == StrategyPeriod.Running)
                {
                    //当前为运行周期,且进行检测
                    if (pcheck)
                    {
                        //紧邻运行周期的下一周期进行休息
                        this.StrategyPeriod = StrategyPeriod.Rest;
                        this.StrategyObject = null;
                        doStrategy = false;
                    }
                    //当前为运行周期,且不进行检测
                    else
                    {
                        doStrategy = true;
                    }
                }

                //执行检测
                if (pcheck)
                {
                    //执行策略
                    if (doStrategy)
                    {
                        if (this.StrategyObject != null)
                        {
                            if (this.StrategyObject.Index < this.StrategyObject.ComputeCount)
                            {
                                item.OutSV = cIn.CumulativeVariation + axVals + (this.StrategyObject.BasicValue + this.StrategyObject.Diff * (this.StrategyObject.Index + 1) / this.StrategyObject.ComputeCount) * item.UniFactor;
                            }
                            this.StrategyObject.GoNext();
                        }
                    }
                    else
                    {
                        item.OutSV = cIn.CumulativeVariation + axVals + item.RPV * item.ScaleFactor * item.UniFactor;
                    }
                }
                //不执行检测
                else
                {
                    if (this.K > 0)
                    {
                        item.OutSV = this[K - 1].OutSV;
                    }
                    else
                    {
                        item.OutSV = 0;
                    }
                }

            }
            //开关未启动
            else
            {
                item.OutSV = cIn.CumulativeVariation + axVals + item.RPV * item.ScaleFactor * item.UniFactor;
            }


            //输出部分
            cOut = this.InitCollection(cOut, item, this.OutPIDConstants, this.Maximum, ControllerType.ComplexRadio, PIDAction.Output);
            PIDModel outItem = cOut.Last();

            item.P = outItem.P;
            item.I = outItem.I;
            item.D = outItem.D;
            item.DD = outItem.DD;
            item.OutDPV = outItem.DPV;
            item.OutDDPV = outItem.DDPV;
            item.Variation = outItem.Variation;
            this.OutCumulativeErr = cOut.CumulativeErr;

            this.InPIDConstants = cIn.PIDConstants;
            this.OutPIDConstants = cOut.PIDConstants;
            this[K] = item;
        }


        private void InitStrategy()
        {
            //获取最近一个更新周期的数据
            List<double> dpvList = new List<double>();
            int freq = this.UpdateFrequency;
            for (int i = 0; i <= freq; i++)
            {
                dpvList.Add(this[K - freq + i].RPV * this[K - freq + i].ScaleFactor);
            }

            double bsidpv = dpvList.First();//判断基值

            double cmpdpv = dpvList.Last();//根据选项取得比较的目标值
            if (this.FetchType == FetchType.Min)
            {
                cmpdpv = dpvList.Min();
            }
            else if (this.FetchType == FetchType.Max)
            {
                cmpdpv = dpvList.Max();
            }

            //判断基值和目标值之差的绝对值是否大于设定的额定差值
            bool flag = CheckDiff(bsidpv, cmpdpv, this.Difference);
            if (flag)
            {
                this.StrategyObject = new StrategyObject(bsidpv, cmpdpv - bsidpv, this.WriteTimes);
            }
            else
            {
                this.StrategyObject = null;
            }
            this.StrategyEnable = flag;

        }


    }
}
