using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class ControllerFactory
    {

        private ControllerFactory()
        {

        }

        public static T Create<T>(int maximun = 100) where T : ControllerBase, new()
        {
            ControllerBase controller = null;
            var props = new T().GetType().GetProperties();
            var p = props.Single(pro => pro.Name == "ControllerType");
            ControllerType ct = (ControllerType)p.GetValue(new T(), null);

            if (ct == ControllerType.Single)
            {
                controller = new SingleController(maximun);
            }
            else if (ct == ControllerType.Cascade)
            {
                controller = new CascadeController(maximun);
            }
            else if (ct == ControllerType.MultiCascade)
            {
                controller = new MultiCascadeController(maximun);
            }
            else if (ct == ControllerType.Feed)
            {
                controller = new FeedController(maximun);
            }
            else if (ct == ControllerType.Radio)
            {
                controller = new RadioController(maximun);
            }
            else if (ct == ControllerType.ComplexRadio)
            {
                controller = new ComplexRadioController(maximun);
            }
            else if (ct == ControllerType.Switch)
            {
                controller = new SwitchController();
            }
            else if (ct == ControllerType.Furnace)
            {
                controller = new FurnaceController(maximun);
            }
            else if (ct == ControllerType.SplitRanging)
            {
                //controller = new ComplexRadioController();
            }
            return controller as T;
        }

    }
}
