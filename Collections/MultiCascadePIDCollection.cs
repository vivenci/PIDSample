using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class MultiCascadePIDCollection : CollectionBase<MultiCascadeModel>
    {
        UInt64 serialIndex = 0;
        PIDCollection cIn = null;
        PIDCollection cOut = null;
        List<PIDConstant> auxiliaryConstants = null;
        List<PIDCollection> cAcs = null;

        public MultiCascadePIDCollection(PIDConstant pidcIn, PIDConstant pidcOut, List<PIDConstant> pidcAuxs, double interval = 1)
        {
            this.cAcs = new List<PIDCollection>();

            this.Interval = interval;
            this.InPIDConstants = pidcIn;
            this.OutPIDConstants = pidcOut;
            this.AuxiliaryConstants = pidcAuxs;
        }

        protected override void InsertItem(int index, MultiCascadeModel item)
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
            MultiCascadeModel item = this[K];

            //输入部分
            cIn = this.InitCollection(this.cIn, item, this.InPIDConstants, this.Maximum, ControllerType.MultiCascade, PIDAction.Input);
            PIDModel inItem = cIn.Last();
            item.InP = inItem.P;
            item.InI = inItem.I;
            item.InD = inItem.D;
            item.InDD = inItem.DD;
            item.InDPV = inItem.DPV;
            item.InDDPV = inItem.DDPV;
            item.InVariation = inItem.Variation;

            //附加部分
            if (this.AuxiliaryConstants != null)
            {

                for (int i = 0; i < this.AuxiliaryConstants.Count; i++)
                {
                    PIDConstant ct = this.AuxiliaryConstants[i];

                    PIDCollection ac = this.cAcs[i];
                    PIDModel cam = item.AuxiliaryModels[i];//取附加PID模型
                    ac = this.InitCollection(ac, cam, ct, this.Maximum, ControllerType.Single, PIDAction.None);
                    cam = ac.Last();

                    item.AuxiliaryModels[i] = cam;
                    this.cAcs[i] = ac;
                }
            }

            //输出部分
            cOut = this.InitCollection(this.cOut, item, this.OutPIDConstants, this.Maximum, ControllerType.MultiCascade, PIDAction.Output);
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


        //public PIDCollection InitCollection(PIDCollection c, PIDModel item, PIDConstant pidc, int maximum)
        //{
        //    if (c == null)
        //    {
        //        c = new PIDCollection(pidc, item.Interval);
        //    }
        //    else
        //    {
        //        c.PIDConstants = pidc;
        //        c.Interval = item.Interval;
        //    }
        //    c.Maximum = maximum;
        //    c.E = i => c[i].Err;
        //    PIDModel m = new PIDModel(item.SV, item.PV, item.Interval);
        //    c.Add(m);
        //    return c;
        //}

    }
}
