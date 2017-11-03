using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 间隔PID模型
    /// 
    /// 观测值大于基准值+偏移量时,开阀
    /// 观测值回路到基准值时,关阀
    /// Reverse为true时此关系反转
    /// 观测值大于基准值+偏移量时,关阀
    /// 观测值回路到基准值时,开阀
    /// </summary>
    public class SwitchModel
    {

        public SwitchModel(double bsv, double pv, double offset, double interval = 1, bool reverse = false)
        {
            this.BSV = bsv;
            this.PV = pv;
            this.Offset = offset;
            this.Interval = interval;
            this.Reverse = reverse;
        }

        /// <summary>
        /// 观测值
        /// </summary>
        public double PV { get; set; }

        /// <summary>
        /// 基准值
        /// </summary>
        public double BSV { get; set; }

        /// <summary>
        /// 变化量
        /// 该模型下此值只能为0或2
        /// 2:开阀;0:关阀
        /// </summary>
        public double Variation { get; set; }

        /// <summary>
        /// 基于基准值的最大偏移量
        /// </summary>
        public double Offset { get; set; }

        public double Interval { get; set; }

        /// <summary>
        /// 反向关系
        /// </summary>
        public bool Reverse { get; set; }

    }
}
