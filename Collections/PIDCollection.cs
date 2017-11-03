using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class PIDCollection : CollectionBase<PIDModel>
    {
        UInt64 serialIndex = 0;
        double cumulativeErr = 0;

        public PIDCollection(PIDConstant pidc, double interval = 1)
        {
            this.Interval = interval;
            this.PIDConstants = pidc;
            if (pidc != null)
            {
                this.UpdatePIDFunc = pidc.GetPID;
            }
        }

        public PIDCollection(PIDConstant pidc, int k, double interval)
        {
            this.K = k;
            this.Interval = interval;
            this.PIDConstants = pidc;
            if (pidc != null)
            {
                this.UpdatePIDFunc = pidc.GetPID;
            }
        }

        protected override void InsertItem(int index, PIDModel item)
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

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            //this.CumulativeErr -= E(index);//若允许移除点,则启用此语句
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
        public double CumulativeErr
        {
            get
            {
                if (this.CumulativeErrRange > 0)
                {
                    //超出范围
                    if (Math.Abs(cumulativeErr) > this.CumulativeErrRange)
                    {
                        return cumulativeErr > 0 ? this.CumulativeErrRange : this.CumulativeErrRange * (-1);
                    }
                    else
                    {
                        return cumulativeErr;
                    }
                }
                else
                {
                    return cumulativeErr;
                }
            }
            set
            {
                cumulativeErr = value;
            }
        }

        /// <summary>
        /// 累计误差范围
        /// 累计误差CumulativeErr的绝对值应不大于该值
        /// </summary>
        public double CumulativeErrRange { get; set; }

        /// <summary>
        /// 累计计算变化量
        /// </summary>
        public double CumulativeVariation { get; set; }

        /// <summary>
        /// e(k)
        /// 传入时间点K,输出函数在该时间点K处的值
        /// </summary>
        public Func<int, double> E
        {
            get;
            set;
        }

        /// <summary>
        /// 根据最新的PIDModel的结果计算得到新的PIDConstant对象
        /// </summary>
        public Func<PIDConstant, PIDModel, PIDConstant> UpdatePIDFunc
        {
            get;
            set;
        }

        private double GetP()
        {
            return E(K);
        }

        private double GetI()
        {
            double ir = 0;

            this.CumulativeErr += E(this.K);
            ir = this.CumulativeErr;

            return ir;
        }

        //private double GetI()
        //{
        //    double ir = 0;

        //    if (this.Count < this.Maximum)
        //    {
        //        for (int i = 0; i <= this.K; i++)
        //        {
        //            ir += E(i);
        //        }
        //    }
        //    else if (this.Count == this.Maximum)
        //    {
        //        for (int i = 0; i <= this.K; i++)
        //        {
        //            ir += E(i);
        //        }
        //        this.CumulativeErr = ir;
        //    }
        //    else
        //    {
        //        this.CumulativeErr += E(this.K);
        //        ir = this.CumulativeErr;
        //    }
        //    return ir;
        //}

        private double GetD()
        {
            if (this.K > 0)
            {
                return (E(K) - E(K - 1)) / this.Interval;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 时间间隔和默认不同时,使用此方法计算微分量
        /// </summary>
        /// <param name="interval">某时间点处和上次的间隔时间</param>
        private double GetD(double interval)
        {
            if (this.K > 0)
            {
                return (E(K) - E(K - 1)) / interval;
            }
            else
            {
                return 0;
            }
        }

        private double GetDD()
        {
            if (this.K > 1)
            {
                return this[K].D - this[K - 1].D;
            }
            else
            {
                return 0;
            }
        }

        private double GetDeltaPV()
        {
            if (this.K == 0)
            {
                return 0;
            }
            else
            {
                return (this[K].PV - this[K - 1].PV) / this.Interval;
            }
        }

        private double GetDDeltaPV()
        {
            if (this.K == 0)
            {
                return 0;
            }
            else
            {
                return this[K].DPV - this[K - 1].DPV;
            }
        }

        public double Formula()
        {
            if (this.PIDConstants != null)
            {
                double delta = 0;
                if (this.PIDConstants.ControlAlgorithm == PIDControlAlgorithm.IPD)
                {
                    delta = this.GetDeltaPV();
                }
                else if (this.PIDConstants.ControlAlgorithm == PIDControlAlgorithm.PID)
                {
                    delta = this.GetD();
                }

                double pid = PIDConstants.KP * (delta + this.Interval * this.GetP() / PIDConstants.TI + PIDConstants.TD * this.GetDDeltaPV());
                pid = pid * PIDConstants.TargetCoefficient;
                if (!PIDConstants.Reverse)
                {
                    pid = pid * (-1);
                }
                return pid;
            }
            else
            {
                return 0;
            }
        }

        private void Standardize(int k)
        {
            this.K = k;
            PIDModel item = this[K];
            item.Index = this.serialIndex;
            item.P = this.GetP();
            item.I = this.GetI();
            if (item.Interval > 0)
            {
                item.D = this.GetD(item.Interval);
            }
            else
            {
                item.D = this.GetD();
            }
            item.DD = this.GetDD();
            item.DPV = this.GetDeltaPV();
            item.DDPV = this.GetDDeltaPV();

            //更新PID
            if (this.UpdatePIDFunc != null)
            {
                if (this.PIDConstants.LiveUpdateEnable)
                {
                    this.PIDConstants = this.UpdatePIDFunc(this.PIDConstants, item);
                }
            }

            item.Variation = Formula();
            this.CumulativeVariation += item.Variation;
            this[K] = item;
        }

        /// <summary>
        /// 标准化处理整个集合
        /// </summary>
        //public void Standardize()
        //{
        //    if (this.Count > 0)
        //    {
        //        for (int i = 0; i < this.Count; i++)
        //        {
        //            this.Standardize(i);
        //        }
        //    }
        //}
    }
}
