using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class SingleController : ControllerBase
    {
        private PIDConstant singleConstant = null;

        public SingleController()
        {
            this.Maximun = DefaultMaximun;
            this.E = i => this.SingleCollection[i].Err;
        }

        public SingleController(int maximum)
        {
            this.Maximun = maximum;
            this.E = i => this.SingleCollection[i].Err;
        }

        public SingleController(int maximum, Func<int, double> e)
        {
            this.Maximun = maximum;
            this.E = e;
        }

        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.Single;
            }
        }

        //public PIDConstant SingleConstant { get; set; }

        public PIDConstant SingleConstant
        {
            get
            {
                if (this.SingleCollection != null)
                {
                    return this.SingleCollection.PIDConstants;
                }
                else
                {
                    return singleConstant;
                }
            }

            set
            {
                singleConstant = value;
                if (this.SingleCollection != null)
                {
                    this.SingleCollection.PIDConstants = value;
                }
            }
        }

        public PIDCollection SingleCollection { get; set; }

        public PIDModel Current
        {
            get
            {
                if (SingleCollection != null)
                {
                    return this.SingleCollection.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public override void GetInstance(List<PIDConstant> constants, double baseInterval)
        {
            if (constants != null && constants.Count > 0)
            {
                this.SingleConstant = constants[0];
                this.SingleCollection = new PIDCollection(this.SingleConstant, baseInterval);
                this.SingleCollection.E = this.E;
                this.SingleCollection.Maximum = this.Maximun;
            }
        }

        public void AddModel(PIDModel m)
        {
            if (this.SingleCollection != null)
            {
                this.SingleCollection.Add(m);
            }
        }
    }
}
