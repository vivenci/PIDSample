using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 复杂串级模型
    /// 
    /// 首次:    [输入] PV=输入位号测量值  SV=输入位号目标值
    ///             计算得deltaSV
    ///          [输出] PV=输出位号测量值  SV= 输出位号测量值 + deltaSV
    ///             计算得deltaMV
    /// 第二次:  [输入] PV=输入位号测量值  SV=输入位号目标值
    ///             计算得deltaSV
    ///          [输出] PV=输出位号测量值  SV= 前次输出SV+当前deltaSV
    ///             计算得deltaMV
    /// 第i次:    ……………………………………………………………………
    /// 
    /// </summary>
    public class MultiCascadeModel : CascadePIDModel
    {

        public MultiCascadeModel()
        {
            this.AuxiliaryModels = new List<PIDModel>();
        }

        public MultiCascadeModel(double inSv, double inPv, double outBsv, double outPv, double interval = 1)
        {
            this.AuxiliaryModels = new List<PIDModel>();
            this.InSV = inSv;
            this.InPV = inPv;
            this.OutBasicSV = outBsv;
            this.OutPV = outPv;
            this.Interval = interval;
        }

        public MultiCascadeModel(double inSv, double inPv, double outBsv, double outPv, double min, double max, double interval = 1) : this(inSv, inPv, outBsv, outPv, interval)
        {
            this.Minimum = min;
            this.Maximum = max;
            this.EnableAdjust = true;
        }

        /// <summary>
        /// 输入部分附加PID控制模块列表
        /// </summary>
        public List<PIDModel> AuxiliaryModels { get; set; }

        public new double OutSV
        {
            get
            {
                double ret = this.OutBasicSV + this.InVariation;
                if (this.AuxiliaryModels != null && this.AuxiliaryModels.Count > 0)
                {
                    ret = ret + this.AuxiliaryModels.Sum(m => m.Variation);
                }
                return ret;
            }
        }

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
}
