using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class ComplexRadioController : ControllerBase
    {
        private PIDConstant constantIn = null;
        private PIDConstant constantOut = null;
        private List<PIDConstant> auxiliaryConstants = null;
        private List<double> cumulativeErrRanges = null;

        public ComplexRadioController()
        {
            this.Maximun = DefaultMaximun;
            this.E = i => this.ComplexRadioCollection[i].Err;
        }

        public ComplexRadioController(int maximum)
        {
            this.Maximun = maximum;
            this.E = i => this.ComplexRadioCollection[i].Err;
        }

        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.ComplexRadio;
            }
        }

        public double BaseInterval { get; set; }

        public SimpleDynamicObject DynamicObject { get; set; }

        /// <summary>
        /// 策略开关
        /// 该值和DynamicObject中UpdateFrequency共同决定是否启用策略
        /// </summary>
        public bool StrategySwitch
        {
            get
            {
                if (this.ComplexRadioCollection != null)
                {
                    return this.ComplexRadioCollection.StrategySwitch;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (this.ComplexRadioCollection != null)
                {
                    this.ComplexRadioCollection.StrategySwitch = value;
                }
            }
        }

        public PIDConstant ConstantIn
        {
            get
            {
                if (this.ComplexRadioCollection != null)
                {
                    return this.ComplexRadioCollection.InPIDConstants;
                }
                else
                {
                    return constantIn;
                }
            }

            set
            {
                constantIn = value;
                if (this.ComplexRadioCollection != null)
                {
                    this.ComplexRadioCollection.InPIDConstants = value;
                }
            }
        }

        public PIDConstant ConstantOut
        {
            get
            {
                if (this.ComplexRadioCollection != null)
                {
                    return this.ComplexRadioCollection.OutPIDConstants;
                }
                else
                {
                    return constantOut;
                }
            }

            set
            {
                constantOut = value;
                if (this.ComplexRadioCollection != null)
                {
                    this.ComplexRadioCollection.OutPIDConstants = value;
                }
            }
        }

        public List<PIDConstant> AuxiliaryConstants
        {
            get
            {
                return auxiliaryConstants;
            }

            set
            {
                auxiliaryConstants = value;
                if (this.ComplexRadioCollection != null)
                {
                    this.ComplexRadioCollection.AuxiliaryConstants = value;
                }
            }
        }

        public List<double> CumulativeErrRanges
        {
            get
            {
                if (this.ComplexRadioCollection != null)
                {
                    return this.ComplexRadioCollection.CumulativeErrRanges;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                cumulativeErrRanges = value;
                if (this.ComplexRadioCollection != null)
                {
                    this.ComplexRadioCollection.CumulativeErrRanges = value;
                }
            }
        }

        public ComplexRadioCollection ComplexRadioCollection { get; set; }

        public ComplexRadioModel Current
        {
            get
            {
                if (ComplexRadioCollection != null)
                {
                    return this.ComplexRadioCollection.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public void AddAuxiliaryConstant(PIDConstant constant)
        {
            if (this.AuxiliaryConstants != null)
            {
                this.AuxiliaryConstants.Add(constant);
            }
            if (this.ComplexRadioCollection != null && this.ComplexRadioCollection.AuxiliaryConstants != null)
            {
                this.ComplexRadioCollection.AuxiliaryConstants.Add(constant);
            }
        }

        public void RemoveAuxiliaryConstant(PIDConstant constant)
        {
            if (this.AuxiliaryConstants != null)
            {
                this.AuxiliaryConstants.Remove(constant);
            }
            if (this.ComplexRadioCollection != null && this.ComplexRadioCollection.AuxiliaryConstants != null)
            {
                this.ComplexRadioCollection.AuxiliaryConstants.Remove(constant);
            }
        }

        public void ClearAuxiliaryConstants()
        {
            if (this.AuxiliaryConstants != null)
            {
                this.AuxiliaryConstants.Clear();
            }
            if (this.ComplexRadioCollection != null && this.ComplexRadioCollection.AuxiliaryConstants != null)
            {
                this.ComplexRadioCollection.AuxiliaryConstants.Clear();
            }
        }


        public void UpdateCumulativeErrRanges(int index, double range)
        {
            if (this.CumulativeErrRanges != null)
            {
                this.CumulativeErrRanges[index] = range;

            }
        }

        public override void GetInstance(List<PIDConstant> constants, SimpleDynamicObject dynObj, double baseInterval, List<double> cumulativeErrRanges)
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

            if (constants != null && constants.Count >= 2 && dynObj != null)
            {
                this.BaseInterval = baseInterval;
                this.ConstantIn = constants[0];
                this.ConstantOut = constants[1];

                if (constants.Count > 2)
                {
                    this.AuxiliaryConstants = constants.GetRange(2, constants.Count - 2);
                }
                this.ComplexRadioCollection = new ComplexRadioCollection(this.ConstantIn, this.ConstantOut, this.AuxiliaryConstants, dynObj.UpdateFrequency, dynObj.WriteTimes, dynObj.FetchType, dynObj.Difference, cumulativeErrRanges, this.BaseInterval);
                this.ComplexRadioCollection.Maximum = this.Maximun;
                this.StrategySwitch = true;
            }
        }

        public void AddModel(ComplexRadioModel m)
        {
            if (this.ComplexRadioCollection != null)
            {
                this.ComplexRadioCollection.Add(m);
            }
        }
    }
}
