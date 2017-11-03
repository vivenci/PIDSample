using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class FurnaceCollection : CollectionBase<FurnaceModel>
    {
        UInt64 serialIndex = 0;
        PIDCollection cIn = null;
        PIDCollection cOut = null;

        public FurnaceCollection(PIDConstant pidcIn, PIDConstant pidcOut, FurnaceManage manage, double interval = 1)
        {
            this.Interval = interval;
            this.InPIDConstants = pidcIn;
            this.OutPIDConstants = pidcOut;
            this.FurnaceManage = manage;
        }

        protected override void InsertItem(int index, FurnaceModel item)
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

        public PIDConstant InPIDConstants { get; set; }

        public PIDConstant OutPIDConstants { get; set; }

        public FurnaceManage FurnaceManage { get; set; }


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
        /// 输入部分累计误差
        /// </summary>
        public double InCumulativeErr { get; set; }

        /// <summary>
        /// 输出部分累计误差
        /// </summary>
        public double OutCumulativeErr { get; set; }

        private void Standardize(int k)
        {
            this.K = k;
            FurnaceModel item = this[K];

            double inceaseVal = this.GetIncreaseValue();
            item.InSV = item.InSV + inceaseVal;

            //输入部分
            cIn = this.InitCollection(cIn, item, this.InPIDConstants, this.Maximum, ControllerType.Cascade, PIDAction.Input);
            PIDModel inItem = cIn.Last();

            item.InP = inItem.P;
            item.InI = inItem.I;
            item.InD = inItem.D;
            item.InDD = inItem.DD;
            item.InDPV = inItem.DPV;
            item.InDDPV = inItem.DDPV;
            item.InVariation = inItem.Variation;
            this.InCumulativeErr = cIn.CumulativeErr;

            //输出部分
            cOut = this.InitCollection(cOut, item, this.OutPIDConstants, this.Maximum, ControllerType.Cascade, PIDAction.Output);
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

            this.FurnaceManage.LastUpdateTime = DateTime.Now;
            this.FurnaceManage.UpdateIndex++;
        }

        /// <summary>
        /// 获得当前应该增加的量
        /// </summary>
        private double GetIncreaseValue()
        {
            if (this.FurnaceManage == null)
            {
                return 0;
            }
            else
            {
                //首次运行
                if (this.FurnaceManage.LastUpdateTime == DateTime.MinValue)
                {
                    this.FurnaceManage.UpdateIndex = 0;
                }
                else
                {
                    TimeSpan ts = DateTime.Now.Date - this.FurnaceManage.LastUpdateTime.Date;
                    //跨日,重置更新次数
                    if (ts.Days >= 1)
                    {
                        this.FurnaceManage.UpdateIndex = 0;
                    }
                }
                //当日更新次数没用完,则返回当次应该增加的量
                if (this.FurnaceManage.UpdateIndex < this.FurnaceManage.ChangeCount)
                {
                    return this.FurnaceManage.IncreaseValue;
                }
                //当日更新次数用完
                else
                {
                    return 0;
                }
            }

        }

    }
}
