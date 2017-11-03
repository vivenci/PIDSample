using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    /// <summary>
    /// 串级控制器
    /// </summary>
    public class CascadeController : ControllerBase
    {
        private PIDConstant constantIn = null;
        private PIDConstant constantOut = null;

        public CascadeController()
        {
            this.Maximun = DefaultMaximun;
            this.E = i => this.CascadeCollection[i].Err;
        }

        public CascadeController(int maximum)
        {
            this.Maximun = maximum;
            this.E = i => this.CascadeCollection[i].Err;
        }



        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.Cascade;
            }
        }

        public PIDConstant ConstantIn
        {
            get
            {
                if (this.CascadeCollection != null)
                {
                    return this.CascadeCollection.InPIDConstants;
                }
                else
                {
                    return constantIn;
                }
            }

            set
            {
                constantIn = value;
                if (this.CascadeCollection != null)
                {
                    this.CascadeCollection.InPIDConstants = value;
                }
            }
        }

        public PIDConstant ConstantOut
        {
            get
            {
                if (this.CascadeCollection != null)
                {
                    return this.CascadeCollection.OutPIDConstants;
                }
                else
                {
                    return constantOut;
                }
            }

            set
            {
                constantOut = value;
                if (this.CascadeCollection != null)
                {
                    this.CascadeCollection.OutPIDConstants = value;
                }
            }
        }

        public CascadePIDCollection CascadeCollection { get; set; }

        public CascadePIDModel Current
        {
            get
            {
                if (CascadeCollection != null)
                {
                    return this.CascadeCollection.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public override void GetInstance(List<PIDConstant> constants, double baseInterval)
        {
            if (constants != null && constants.Count == 2)
            {
                this.ConstantIn = constants[0];
                this.ConstantOut = constants[1];
                this.CascadeCollection = new CascadePIDCollection(this.ConstantIn, this.ConstantOut, baseInterval);
                this.CascadeCollection.Maximum = this.Maximun;
            }
        }

        public void AddModel(CascadePIDModel m)
        {
            if (this.CascadeCollection != null)
            {
                this.CascadeCollection.Add(m);
            }
        }

    }
}
