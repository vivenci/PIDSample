using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class CollectionBase<T> : Collection<T>
    {
        public PIDCollection InitCollection(PIDCollection c, PIDModel item, PIDConstant pidc, int maximum, ControllerType ct = ControllerType.Single, PIDAction action = PIDAction.None, double cumulativeErrRange = 0)
        {
            if (c == null)
            {
                c = new PIDCollection(pidc, item.Interval);
            }
            else
            {
                c.PIDConstants = pidc;
                c.Interval = item.Interval;
            }
            c.CumulativeErrRange = cumulativeErrRange;
            c.Maximum = maximum;
            c.E = i => c[i].Err;

            switch (ct)
            {
                case ControllerType.Single:
                    PIDModel m = new PIDModel(item.SV, item.PV, item.Interval);
                    c.Add(m);
                    break;
                case ControllerType.Cascade:
                    CascadePIDModel cm = item as CascadePIDModel;
                    PIDModel m2 = null;
                    if (action == PIDAction.Input)
                    {
                        m2 = new PIDModel(cm.InSV, cm.InPV, cm.Interval);
                    }
                    else if (action == PIDAction.Output)
                    {
                        m2 = new PIDModel(cm.OutSV, cm.OutPV, cm.Minimum, cm.Maximum, cm.Interval);
                    }
                    c.Add(m2);
                    break;
                case ControllerType.MultiCascade:
                    MultiCascadeModel mcm = item as MultiCascadeModel;
                    PIDModel m3 = null;
                    if (action == PIDAction.Input)
                    {
                        m3 = new PIDModel(mcm.InSV, mcm.InPV, mcm.Interval);
                    }
                    else if (action == PIDAction.Output)
                    {
                        m3 = new PIDModel(mcm.OutSV, mcm.OutPV, mcm.Minimum, mcm.Maximum, mcm.Interval);
                    }
                    c.Add(m3);
                    break;
                case ControllerType.Feed:
                    FeedModel fm = item as FeedModel;
                    PIDModel m4 = null;
                    if (action == PIDAction.Input)
                    {
                        m4 = new PIDModel(fm.InSV, fm.InPV, fm.Interval);
                    }
                    else if (action == PIDAction.Output)
                    {
                        m4 = new PIDModel(fm.OutSV + fm.CorrectionValue, fm.OutPV, fm.Minimum, fm.Maximum, fm.Interval);
                    }
                    c.Add(m4);
                    break;
                case ControllerType.Radio:
                    RadioModel rm = item as RadioModel;
                    PIDModel m5 = new PIDModel(rm.SV + rm.CorrectionValue, rm.PV, rm.Interval);
                    c.Add(m5);
                    break;
                case ControllerType.ComplexRadio:
                    ComplexRadioModel crm = item as ComplexRadioModel;
                    PIDModel m6 = null;
                    if (action == PIDAction.Input)
                    {
                        m6 = new PIDModel(crm.InSV, crm.InPV, crm.Interval);
                    }
                    else if (action == PIDAction.Output)
                    {
                        m6 = new PIDModel(crm.OutSV + crm.CorrectionValue, crm.OutPV, crm.Minimum, crm.Maximum, crm.Interval);
                    }
                    c.Add(m6);
                    break;
                case ControllerType.Furnace:
                    FurnaceModel fum = item as FurnaceModel;
                    PIDModel m7 = null;
                    if (action == PIDAction.Input)
                    {
                        m7 = new PIDModel(fum.InSV, fum.InPV, fum.Interval);
                    }
                    else if (action == PIDAction.Output)
                    {
                        m7 = new PIDModel(fum.OutSV, fum.OutPV, fum.Minimum, fum.Maximum, fum.Interval);
                    }
                    c.Add(m7);
                    break;
            }
            return c;

        }

        /// <summary>
        /// 是否进行检测
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="updFreq">更新频率</param>
        public bool IsCheck(ulong index, int updFreq)
        {
            return ((index > 0) && (index % (ulong)updFreq == 0)) ? true : false;
        }

        /// <summary>
        /// 比较目标值和基础值之差是否超过指定偏差量
        /// </summary>
        /// <param name="bsidpv">基础值</param>
        /// <param name="cmpdpv">比较值</param>
        /// <param name="differ">偏差量</param>
        public bool CheckDiff(double bsidpv, double cmpdpv, double differ)
        {
            double tmpDiff = cmpdpv - bsidpv;
            if (Math.Abs(tmpDiff) <= differ)
            {
                return false;
            }
            else
            {

                return true;
            }
        }

    }
}
