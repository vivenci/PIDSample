using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PIDSample
{
    public abstract class ControllerBase
    {
        public const int DefaultMaximun = 100;

        public ControllerType ControllerType;

        public int Maximun;

        public Func<int, double> E;

        /// <summary>
        /// 用于间隔控制器
        /// </summary>
        /// <param name="baseInterval">执行间隔</param>
        public virtual void GetInstance(double baseInterval)
        {
        }

        /// <summary>
        /// 用于单回路,串级,复杂串级控制器
        /// </summary>
        /// <param name="constants">PID回路参数列表</param>
        /// <param name="baseInterval">执行间隔</param>
        public virtual void GetInstance(List<PIDConstant> constants, double baseInterval)
        {
        }

        /// <summary>
        /// 用于比例控制器
        /// </summary>
        /// <param name="constant">PID回路参数</param>
        /// <param name="dynObj">基础动态修正对象</param>
        /// <param name="baseInterval">执行间隔</param>
        public virtual void GetInstance(PIDConstant constant, SimpleDynamicObject dynObj, double baseInterval)
        {
        }

        /// <summary>
        /// 用于复杂比例控制器
        /// </summary>
        /// <param name="constants">PID回路参数列表</param>
        /// <param name="dynObj">基础动态修正对象</param>
        /// <param name="baseInterval">执行间隔</param>
        /// <param name="cumulativeErrRange">累计误差范围列表</param>
        public virtual void GetInstance(List<PIDConstant> constants, SimpleDynamicObject dynObj, double baseInterval, List<double> cumulativeErrRange)
        {
        }

        /// <summary>
        /// 用于进料控制器
        /// </summary>
        /// <param name="constants">PID回路参数列表</param>
        /// <param name="dynObj">动态修正对象</param>
        /// <param name="baseInterval">执行间隔</param>
        /// <param name="cumulativeErrRange">累计误差范围列表</param>
        public virtual void GetInstance(List<PIDConstant> constants, DynamicObject dynObj, double baseInterval, double cumulativeErrRange)
        {
        }

        /// <summary>
        /// 用于加热炉控制器
        /// </summary>
        /// <param name="constants">PID回路参数列表</param>
        /// <param name="manage">加热炉时间控制对象</param>
        /// <param name="baseInterval">执行间隔</param>
        public virtual void GetInstance(List<PIDConstant> constants, FurnaceManage manage, double baseInterval)
        {
        }
    }
}
