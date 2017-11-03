using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class RadioController : ControllerBase
    {
        private PIDConstant radioConstant = null;

        public RadioController()
        {
            this.Maximun = DefaultMaximun;
            this.E = i => this.RadioCollection[i].Err;
        }

        public RadioController(int maximum)
        {
            this.Maximun = maximum;
            this.E = i => this.RadioCollection[i].Err;
        }

        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.Radio;
            }
        }

        public double BaseInterval { get; set; }

        public PIDConstant RadioConstant
        {
            get
            {
                if (this.RadioCollection != null)
                {
                    return this.RadioCollection.PIDConstants;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                radioConstant = value;
                if (this.RadioCollection != null)
                {
                    this.RadioCollection.PIDConstants = value;
                }
            }
        }

        public SimpleDynamicObject DynamicObject { get; set; }

        /// <summary>
        /// 策略开关
        /// 该值和DynamicObject中UpdateFrequency共同决定是否启用策略
        /// </summary>
        public bool StrategySwitch
        {
            get
            {
                if (this.RadioCollection != null)
                {
                    return this.RadioCollection.StrategySwitch;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (this.RadioCollection != null)
                {
                    this.RadioCollection.StrategySwitch = value;
                }
            }
        }

        public RadioCollection RadioCollection { get; set; }

        public RadioModel Current
        {
            get
            {
                if (RadioCollection != null)
                {
                    return this.RadioCollection.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public override void GetInstance(PIDConstant constant, SimpleDynamicObject dynObj, double baseInterval)
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

            if (constant != null && dynObj != null)
            {
                this.BaseInterval = baseInterval;
                this.RadioConstant = constant;
                this.RadioCollection = new RadioCollection(this.RadioConstant, dynObj.UpdateFrequency, dynObj.WriteTimes, dynObj.FetchType, dynObj.Difference, this.BaseInterval);
                this.RadioCollection.Maximum = this.Maximun;
                this.StrategySwitch = true;
            }
        }

        public void AddModel(RadioModel m)
        {
            if (this.RadioCollection != null)
            {
                this.RadioCollection.Add(m);
            }
        }

    }
}
