using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class RadioCollection : CollectionBase<RadioModel>
    {
        UInt64 serialIndex = 0;
        PIDCollection c = null;

        public RadioCollection(PIDConstant pidc, int updateFreq, int writeTimes, FetchType fetchType, double diff, double interval = 1)
        {
            this.Interval = interval;
            this.UpdateFrequency = updateFreq;
            this.WriteTimes = writeTimes;
            this.FetchType = fetchType;
            this.Difference = diff;
            this.PIDConstants = pidc;
            this.StrategyEnable = false;
            this.StrategyPeriod = StrategyPeriod.Rest;
        }

        protected override void InsertItem(int index, RadioModel item)
        {
            //若超出最大容量,则删除最开始的元素
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

        public PIDConstant PIDConstants { get; set; }


        /// <summary>
        /// 最大容量
        /// </summary>
        public int Maximum { get; set; }

        /// <summary>
        /// 当前时刻对应的序号
        /// </summary>
        public int K { get; set; }

        /// <summary>
        /// 默认采样时间间隔
        /// </summary>
        public double Interval { get; set; }

        /// <summary>
        /// 累计误差
        /// </summary>
        public double CumulativeErr { get; set; }

        /// <summary>
        /// 累计计算变化量
        /// </summary>
        public double CumulativeVariation { get; set; }

        /// <summary>
        /// 根据最新的PIDModel的结果计算得到新的PIDConstant对象
        /// </summary>
        //public Func<PIDConstant, PIDModel, PIDConstant> UpdatePIDFunc
        //{
        //    get;
        //    set;
        //}

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
            RadioModel item = this[K];

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

                if (pcheck)
                {
                    //执行策略
                    if (doStrategy)
                    {
                        if (this.StrategyObject != null)
                        {
                            if (this.StrategyObject.Index < this.StrategyObject.ComputeCount)
                            {
                                item.SV = (this.StrategyObject.BasicValue + this.StrategyObject.Diff * (this.StrategyObject.Index + 1) / this.StrategyObject.ComputeCount) * item.UniFactor;
                            }
                            this.StrategyObject.GoNext();
                        }
                    }
                    else
                    {
                        item.SV = item.RPV * item.ScaleFactor * item.UniFactor;
                    }
                }
                //不执行检测
                else
                {
                    if (this.K > 0)
                    {
                        item.SV = this[K - 1].SV;
                    }
                    else
                    {
                        item.SV = 0;
                    }
                }

            }
            else
            {
                item.SV = item.RPV * item.ScaleFactor * item.UniFactor;
            }


            c = this.InitCollection(c, item, this.PIDConstants, this.Maximum, ControllerType.Radio, PIDAction.None);
            PIDModel nItem = c.Last();

            item.P = nItem.P;
            item.I = nItem.I;
            item.D = nItem.D;
            item.DD = nItem.DD;
            item.DPV = nItem.DPV;
            item.DDPV = nItem.DDPV;
            item.Variation = nItem.Variation;
            this.CumulativeErr = c.CumulativeErr;

            this.PIDConstants = c.PIDConstants;
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
