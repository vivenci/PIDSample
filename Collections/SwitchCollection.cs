using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class SwitchCollection : CollectionBase<SwitchModel>
    {
        public SwitchCollection(double interval = 1)
        {
            this.Interval = interval;
        }

        protected override void InsertItem(int index, SwitchModel item)
        {
            this.Clear();
            base.InsertItem(0, item);
            this.Standardize();
        }

        public double Interval { get; set; }


        private void Standardize()
        {
            if (this.Count > 0)
            {
                SwitchModel item = this[0];

                if (item.PV > item.BSV + item.Offset)
                {
                    item.Variation = item.Reverse ? 0 : 2;
                }
                else
                {
                    item.Variation = item.Reverse ? 2 : 0;
                }
            }
        }
    }
}
