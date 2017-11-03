using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class FurnaceController : ControllerBase
    {
        private int updateIndex = 0;
        private PIDConstant constantIn = null;
        private PIDConstant constantOut = null;
        FurnaceManage furnaceManage = null;

        public FurnaceController()
        {
            this.Maximun = DefaultMaximun;
            this.E = i => this.FurnaceCollection[i].Err;
        }

        public FurnaceController(int maximum)
        {
            this.Maximun = maximum;
            this.E = i => this.FurnaceCollection[i].Err;
        }



        public new ControllerType ControllerType
        {
            get
            {
                return ControllerType.Furnace;
            }
        }

        public PIDConstant ConstantIn
        {
            get
            {
                if (this.FurnaceCollection != null)
                {
                    return this.FurnaceCollection.InPIDConstants;
                }
                else
                {
                    return constantIn;
                }
            }

            set
            {
                constantIn = value;
                if (this.FurnaceCollection != null)
                {
                    this.FurnaceCollection.InPIDConstants = value;
                }
            }
        }

        public PIDConstant ConstantOut
        {
            get
            {
                if (this.FurnaceCollection != null)
                {
                    return this.FurnaceCollection.OutPIDConstants;
                }
                else
                {
                    return constantOut;
                }
            }

            set
            {
                constantOut = value;
                if (this.FurnaceCollection != null)
                {
                    this.FurnaceCollection.OutPIDConstants = value;
                }
            }
        }

        public FurnaceCollection FurnaceCollection { get; set; }

        public CascadePIDModel Current
        {
            get
            {
                if (FurnaceCollection != null)
                {
                    return this.FurnaceCollection.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public FurnaceManage FurnaceManage
        {
            get
            {
                if (this.FurnaceCollection != null)
                {
                    return this.FurnaceCollection.FurnaceManage;
                }
                else
                {
                    return furnaceManage;
                }
            }
            set
            {
                this.furnaceManage = value;
                if (this.FurnaceCollection != null)
                {
                    this.FurnaceCollection.FurnaceManage = value;
                }
            }
        }


        public override void GetInstance(List<PIDConstant> constants, FurnaceManage manage, double baseInterval)
        {
            this.FurnaceManage = manage;
            if (constants != null && constants.Count == 2)
            {
                this.ConstantIn = constants[0];
                this.ConstantOut = constants[1];
                this.FurnaceCollection = new FurnaceCollection(this.ConstantIn, this.ConstantOut, manage, baseInterval);
                this.FurnaceCollection.Maximum = this.Maximun;
            }
        }

        public void AddModel(FurnaceModel m)
        {
            if (this.FurnaceCollection != null)
            {
                this.FurnaceCollection.Add(m);
            }
        }

    }

    /// <summary>
    /// 加热炉时间控制
    /// </summary>
    public class FurnaceManage
    {

        public FurnaceManage(double increaseRange, int changeCnt)
        {
            this.IncreaseRange = increaseRange;
            this.ChangeCount = changeCnt;
            this.UpdateIndex = 0;
            this.LastUpdateTime = DateTime.MinValue;//初始设置为最小值
        }

        /// <summary>
        /// 输入部分目标值每天可增加的上限
        /// </summary>
        public double IncreaseRange { get; set; }

        /// <summary>
        /// 每天可改变的次数
        /// </summary>
        public int ChangeCount { get; set; }

        /// <summary>
        /// 更新索引
        /// </summary>
        public int UpdateIndex { get; set; }

        /// <summary>
        /// 每天每次的更新量
        /// </summary>
        public double IncreaseValue
        {
            get
            {
                if (this.ChangeCount > 0)
                {
                    return this.IncreaseRange / this.ChangeCount;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 上次的更新时间
        /// 用于判断日期更新
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
    }
}
