using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class SwitchController : ControllerBase
    {
        public SwitchController()
        {

        }

        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.Switch;
            }
        }

        public SwitchCollection SwitchCollection { get; set; }

        public SwitchModel Current
        {
            get
            {
                if (SwitchCollection != null)
                {
                    return this.SwitchCollection.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public override void GetInstance(double baseInterval)
        {
            this.SwitchCollection = new SwitchCollection(baseInterval);
        }

        public void AddModel(SwitchModel m)
        {
            if (this.SwitchCollection != null)
            {
                this.SwitchCollection.Add(m);
            }
        }
    }
}
