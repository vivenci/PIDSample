using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public enum ControllerType
    {
        Single,         //单回路
        Cascade,        //串级
        MultiCascade,   //复杂串级
        Feed,           //进料
        Radio,           //比例
        ComplexRadio,    //复杂比例
        Switch,         //间隔
        Furnace,        //加热炉
        SplitRanging    //分程
    }


    public enum PIDAction
    {
        None,
        Input,
        Output
    }
}
