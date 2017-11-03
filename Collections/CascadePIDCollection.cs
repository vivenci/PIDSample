using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 串级PID对象集合
    /// </summary>
    public class CascadePIDCollection : CollectionBase<CascadePIDModel>
    {
        UInt64 serialIndex = 0;
        PIDCollection cIn = null;
        PIDCollection cOut = null;

        public CascadePIDCollection(PIDConstant pidcIn, PIDConstant pidcOut, double interval = 1)
        {
            this.Interval = interval;
            this.InPIDConstants = pidcIn;
            this.OutPIDConstants = pidcOut;
        }

        public CascadePIDCollection(PIDConstant pidcIn, PIDConstant pidcOut, int k, double interval)
        {
            this.K = k;
            this.Interval = interval;
            this.InPIDConstants = pidcIn;
            this.OutPIDConstants = pidcOut;
        }

        protected override void InsertItem(int index, CascadePIDModel item)
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
        }


        public PIDConstant InPIDConstants { get; set; }

        public PIDConstant OutPIDConstants { get; set; }


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

        //public Func<CascadePIDModel, PIDConstant> UpdatePIDFunc
        //{
        //    get;
        //    set;
        //}

        //private void Standardize(int k)
        //{
        //    this.K = k;
        //    CascadePIDModel item = this[K];

        //    PIDCollection c = new PIDCollection(this.InPIDConstants, item.Interval);
        //    c.Maximum = this.Maximum;
        //    c.E = i => c[i].Err;
        //    PIDModel inItem = new PIDModel(item.InSV, item.InPV, item.Interval);
        //    c.Add(inItem);

        //    inItem = c.Last();
        //    item.InP = inItem.P;
        //    item.InI = inItem.I;
        //    item.InD = inItem.D;
        //    item.InDD = inItem.DD;
        //    item.InVariation = inItem.Variation;
        //    this.InCumulativeErr = c.CumulativeErr;

        //    PIDCollection cOut = new PIDCollection(this.OutPIDConstants, item.Interval);
        //    cOut.Maximum = this.Maximum;
        //    cOut.E = i => c[i].Err;
        //    PIDModel outItem = new PIDModel(item.OutSV, item.OutPV, item.Interval);
        //    cOut.Add(outItem);
        //    outItem = cOut.Last();
        //    item.P = outItem.P;
        //    item.I = outItem.I;
        //    item.D = outItem.D;
        //    item.DD = outItem.DD;
        //    item.Variation = outItem.Variation;
        //    this.OutCumulativeErr = cOut.CumulativeErr;

        //    this[K] = item;
        //}

        private void Standardize(int k)
        {
            this.K = k;
            CascadePIDModel item = this[K];

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
        }

        //private PIDCollection InitInCollection(CascadePIDModel item, PIDConstant pidc)
        //{
        //    if (cIn == null)
        //    {
        //        cIn = new PIDCollection(pidc, item.Interval);
        //    }
        //    else
        //    {
        //        cIn.PIDConstants = pidc;
        //        cIn.Interval = item.Interval;
        //    }

        //    cIn.Maximum = this.Maximum;
        //    cIn.E = i => cIn[i].Err;
        //    PIDModel m = new PIDModel(item.InSV, item.InPV, item.Interval);
        //    cIn.Add(m);
        //    return cIn;
        //}

        //private PIDCollection InitOutCollection(CascadePIDModel item, PIDConstant pidc)
        //{
        //    if (cOut == null)
        //    {
        //        cOut = new PIDCollection(pidc, item.Interval);
        //    }
        //    else
        //    {
        //        cOut.PIDConstants = pidc;
        //        cOut.Interval = item.Interval;
        //    }

        //    cOut.Maximum = this.Maximum;
        //    cOut.E = i => cOut[i].Err;
        //    PIDModel m = new PIDModel(item.OutSV, item.OutPV, item.Interval);
        //    cOut.Add(m);
        //    return cOut;
        //}

    }

}
