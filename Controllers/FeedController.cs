using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class FeedController : ControllerBase
    {
        double cumulativeErrRange;
        private PIDConstant constantIn = null;
        private PIDConstant constantOut = null;

        public FeedController()
        {
            this.Maximun = DefaultMaximun;
            this.E = i => this.FeedCollection[i].Err;
        }

        public FeedController(int maximum)
        {
            this.Maximun = maximum;
            this.E = i => this.FeedCollection[i].Err;
        }

        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.Feed;
            }
        }

        public double BaseInterval { get; set; }

        public DynamicObject DynamicObject { get; set; }

        /// <summary>
        /// 策略开关
        /// 该值和DynamicObject中UpdateFrequency共同决定是否启用策略
        /// </summary>
        public bool StrategySwitch
        {
            get
            {
                if (this.FeedCollection != null)
                {
                    return this.FeedCollection.StrategySwitch;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (this.FeedCollection != null)
                {
                    this.FeedCollection.StrategySwitch = value;
                }
            }
        }


        public PIDConstant ConstantIn
        {
            get
            {
                if (this.FeedCollection != null)
                {
                    return this.FeedCollection.InPIDConstants;
                }
                else
                {
                    return constantIn;
                }
            }

            set
            {
                constantIn = value;
                if (this.FeedCollection != null)
                {
                    this.FeedCollection.InPIDConstants = value;
                }
            }
        }

        public PIDConstant ConstantOut
        {
            get
            {
                if (this.FeedCollection != null)
                {
                    return this.FeedCollection.OutPIDConstants;
                }
                else
                {
                    return constantOut;
                }
            }

            set
            {
                constantOut = value;
                if (this.FeedCollection != null)
                {
                    this.FeedCollection.OutPIDConstants = value;
                }
            }
        }

        public FeedCollection FeedCollection { get; set; }

        public FeedModel Current
        {
            get
            {
                if (FeedCollection != null)
                {
                    return this.FeedCollection.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<double> DynamicList
        {
            get
            {
                if (this.DynamicObject != null)
                {
                    return this.DynamicObject.DynamicList;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 累计偏差范围
        /// </summary>
        public double CumulativeErrRange
        {
            get
            {
                if (this.FeedCollection != null)
                {
                    return this.FeedCollection.CumulativeErrRange;
                }
                else
                {
                    return cumulativeErrRange;
                }
            }

            set
            {
                cumulativeErrRange = value;
                if (this.FeedCollection != null)
                {
                    this.FeedCollection.CumulativeErrRange = value;
                }
            }
        }

        public void UpdateDynamic(double value)
        {
            if (this.DynamicObject != null)
            {
                this.DynamicObject.UpdateDynamic(value);
            }
        }


        public override void GetInstance(List<PIDConstant> constants, DynamicObject dynObj, double baseInterval, double cumulativeErrRange)
        {
            //if (dynObj != null)
            //{
            //    //参数匹配检测
            //    if (dynObj.Interval != baseInterval)
            //    {
            //        string msg = InnerArgumentException.GenInfo(nameof(dynObj.Interval), dynObj, nameof(baseInterval), baseInterval);
            //        throw new InnerArgumentException(msg);
            //    }
            //}
            this.DynamicObject = dynObj;

            if (constants != null && constants.Count == 2 && dynObj != null)
            {
                this.BaseInterval = baseInterval;
                this.ConstantIn = constants[0];
                this.ConstantOut = constants[1];
                this.FeedCollection = new FeedCollection(this.ConstantIn, this.ConstantOut, dynObj.UpdateFrequency, dynObj.WriteTimes, dynObj.FetchType, dynObj.Difference, this.BaseInterval, cumulativeErrRange);
                this.FeedCollection.Maximum = this.Maximun;
                this.StrategySwitch = true;
            }
        }

        public void AddModel(FeedModel m)
        {
            if (this.FeedCollection != null)
            {
                this.FeedCollection.Add(m);
            }
        }

    }

    public enum FetchType
    {
        Normal,
        Min,
        Max
    }
}
