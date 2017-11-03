using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class SplitRangingModel : CascadePIDModel
    {
        public SplitRangingModel()
        {
            this.MultipleOutputModels = new List<PIDModel>();
        }


        #region 输出部分

        /// <summary>
        /// 多路输出模型列表
        /// </summary>
        public List<PIDModel> MultipleOutputModels { get; set; }


        #region 不再使用原输出部分的属性,重置为NaN
        public new double OutBasicSV
        {
            get
            {
                return double.NaN;
            }
        }

        public new double OutSV
        {
            get
            {
                return double.NaN;
            }
        }

        public new double OutPV
        {
            get
            {
                return double.NaN;
            }
        }

        public new double Err
        {
            get
            {
                return double.NaN;
            }
        }

        public new double OutDPV
        {
            get
            {
                return double.NaN;
            }
        }

        public new double OutDDPV
        {
            get
            {
                return double.NaN;
            }
        }
        #endregion


        #endregion
    }
}
