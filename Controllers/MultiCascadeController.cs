using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 复杂串级控制器
    /// </summary>
    public class MultiCascadeController : ControllerBase
    {
        private PIDConstant constantIn = null;
        private PIDConstant constantOut = null;
        private List<PIDConstant> auxiliaryConstants = null;

        public MultiCascadeController()
        {
            this.Maximun = DefaultMaximun;
            this.E = i => this.MultiCascadeCollection[i].Err;
            this.AuxiliaryConstants = new List<PIDConstant>();
        }

        public MultiCascadeController(int maximum)
        {
            this.Maximun = maximum;
            this.E = i => this.MultiCascadeCollection[i].Err;
            this.AuxiliaryConstants = new List<PIDConstant>();
        }

        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.MultiCascade;
            }
        }

        public PIDConstant ConstantIn
        {
            get
            {
                if (this.MultiCascadeCollection != null)
                {
                    return this.MultiCascadeCollection.InPIDConstants;
                }
                else
                {
                    return constantIn;
                }
            }

            set
            {
                constantIn = value;
                if (this.MultiCascadeCollection != null)
                {
                    this.MultiCascadeCollection.InPIDConstants = value;
                }
            }
        }

        public PIDConstant ConstantOut
        {
            get
            {
                if (this.MultiCascadeCollection != null)
                {
                    return this.MultiCascadeCollection.OutPIDConstants;
                }
                else
                {
                    return constantOut;
                }
            }

            set
            {
                constantOut = value;
                if (this.MultiCascadeCollection != null)
                {
                    this.MultiCascadeCollection.OutPIDConstants = value;
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
                if (this.MultiCascadeCollection != null)
                {
                    this.MultiCascadeCollection.AuxiliaryConstants = value;
                }
            }
        }

        public MultiCascadePIDCollection MultiCascadeCollection { get; set; }

        public MultiCascadeModel Current
        {
            get
            {
                if (MultiCascadeCollection != null)
                {
                    return this.MultiCascadeCollection.Last();
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
            if (this.MultiCascadeCollection != null && this.MultiCascadeCollection.AuxiliaryConstants != null)
            {
                this.MultiCascadeCollection.AuxiliaryConstants.Add(constant);
            }
        }

        public void RemoveAuxiliaryConstant(PIDConstant constant)
        {
            if (this.AuxiliaryConstants != null)
            {
                this.AuxiliaryConstants.Remove(constant);
            }
            if (this.MultiCascadeCollection != null && this.MultiCascadeCollection.AuxiliaryConstants != null)
            {
                this.MultiCascadeCollection.AuxiliaryConstants.Remove(constant);
            }
        }

        public void ClearAuxiliaryConstants()
        {
            if (this.AuxiliaryConstants != null)
            {
                this.AuxiliaryConstants.Clear();
            }
            if (this.MultiCascadeCollection != null && this.MultiCascadeCollection.AuxiliaryConstants != null)
            {
                this.MultiCascadeCollection.AuxiliaryConstants.Clear();
            }
        }

        public override void GetInstance(List<PIDConstant> constants, double baseInterval)
        {
            if (constants != null && constants.Count >= 2)
            {
                this.ConstantIn = constants[0];
                this.ConstantOut = constants[1];

                //初始化附加PID列表
                if (constants.Count > 2)
                {
                    this.AuxiliaryConstants = constants.GetRange(2, constants.Count - 2);
                }
                this.MultiCascadeCollection = new MultiCascadePIDCollection(this.ConstantIn, this.ConstantOut, this.AuxiliaryConstants, baseInterval);
                this.MultiCascadeCollection.Maximum = this.Maximun;
            }
        }

        public void AddModel(MultiCascadeModel m)
        {
            if (this.MultiCascadeCollection != null)
            {
                this.MultiCascadeCollection.Add(m);
            }
        }
    }
}
