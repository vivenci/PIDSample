using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class SplitRangingCollection : CollectionBase<SplitRangingModel>
    {

        UInt64 serialIndex = 0;


        public SplitRangingCollection(PIDConstant pidcIn, PIDConstant pidcOut, double interval = 1)
        {
            this.Interval = interval;
        }

        public int Maximum { get; set; }

        /// <summary>
        /// 当前时刻对应的序号
        /// </summary>
        public int K { get; set; }


        public double Interval { get; set; }

        /// <summary>
        /// 输入部分累计误差
        /// </summary>
        public double InCumulativeErr { get; set; }

        /// <summary>
        /// 输出部分累计误差
        /// </summary>
        public double OutCumulativeErr { get; set; }
    }
}
