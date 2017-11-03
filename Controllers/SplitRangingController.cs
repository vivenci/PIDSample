using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class SplitRangingController : ControllerBase
    {

        public SplitRangingController()
        {
            this.Maximun = DefaultMaximun;
            this.E = i => this.SplitRangingCollection[i].Err;
        }

        public SplitRangingController(int maximum)
        {
            this.Maximun = maximum;
            this.E = i => this.SplitRangingCollection[i].Err;
        }



        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.SplitRanging;
            }
        }

        public SplitRangingCollection SplitRangingCollection { get; set; }

        public SplitRangingModel Current
        {
            get
            {
                if (SplitRangingCollection != null)
                {
                    return this.SplitRangingCollection.Last();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
