using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class SimpleDynamicObject
    {
        private int wTimes;
        public SimpleDynamicObject(int cycle, int updFreq, int writeTimes, double diff, FetchType ft)
        {
            this.Cycle = cycle;
            this.UpdateFrequency = updFreq;
            this.WriteTimes = writeTimes;
            this.Difference = diff;
            this.FetchType = ft;
        }

        /// <summary>
        /// 观测值容量
        /// DynamicList的列表长度Length
        /// </summary>
        public int Cycle { get; set; }

        /// <summary>
        /// 观测位号更新周期
        /// 设置为1时更新频率和回路周期一致,不启用更新逻辑
        /// 否则该值为回路周期的倍数,此时启用更新逻辑
        /// </summary>
        public int UpdateFrequency { get; set; }

        /// <summary>
        /// 写数次数
        /// 指定偏差值分多少次写入
        /// 该值应小于等于UpdateFrequency
        /// </summary>
        public int WriteTimes
        {
            get
            {
                return wTimes;
            }
            set
            {
                wTimes = Math.Min(value, this.UpdateFrequency);
            }
        }

        /// <summary>
        /// 启用更新逻辑时的取值策略
        /// 取Normal时表示取当前更新周期末尾值
        /// 取Min时表示取当前更新周期中最小值
        /// 取Max时表示取当前更新周期中最大值
        /// </summary>
        public FetchType FetchType { get; set; }

        /// <summary>
        /// 更新逻辑中的判断差值
        /// 该值将与更新逻辑中基准值与使用取值策略所取值的差值的绝对值进行比较
        /// 如果大于该值,将在接下来的更新周期中使用分段更新策略
        /// 否则正常更新
        /// </summary>
        public double Difference { get; set; }
    }

    public class DynamicObject : SimpleDynamicObject
    {
        public DynamicObject(int cycle, int updFreq, int writeTimes, double diff, FetchType ft) : base(cycle, updFreq, writeTimes, diff, ft)
        {
            this.DynamicList = new List<double>();
        }

        /// <summary>
        /// 观测位号值列表
        /// </summary>
        public List<double> DynamicList { get; set; }

        /// <summary>
        /// 更新列表
        /// </summary>
        public void UpdateDynamic(double value)
        {
            if (this.DynamicList != null)
            {
                if (this.DynamicList.Count >= Cycle)
                {
                    this.DynamicList.Remove(this.DynamicList.First());
                }
                this.DynamicList.Add(value);
            }
        }

    }


}
