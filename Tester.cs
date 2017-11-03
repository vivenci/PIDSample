using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    class Tester
    {
        Random r = new Random();

        void TestSingle()
        {
            PIDConstant pidc = new PIDConstant(1, 1, 1);

            PIDCollection c = new PIDCollection(pidc);
            c.Maximum = 5;
            c.E = i => c[i].Err;

            PIDModel m1 = new PIDModel(50, 70);
            PIDModel m2 = new PIDModel(50, 60);
            PIDModel m3 = new PIDModel(50, 53);
            PIDModel m4 = new PIDModel(50, 49);
            PIDModel m5 = new PIDModel(50, 51);
            PIDModel m6 = new PIDModel(50, 48);
            PIDModel m7 = new PIDModel(50, 52);
            PIDModel m8 = new PIDModel(50, 51);
            PIDModel m9 = new PIDModel(50, 46);
            PIDModel m10 = new PIDModel(50, 52);
            PIDModel m11 = new PIDModel(50, 50.2);
            PIDModel m12 = new PIDModel(50, 49);
            PIDModel m13 = new PIDModel(50, 50.5);
            PIDModel m14 = new PIDModel(50, 49.2);

            c.Add(m1);
            c.Add(m2);
            c.Add(m3);
            c.Add(m4);
            c.Add(m5);
            c.Add(m6);
            c.Add(m7);
            c.Add(m8);
            c.Add(m9);
            c.Add(m10);
            c.Add(m11);
            c.Add(m12);
            c.Add(m13);
            c.Add(m14);

            //var v = from t in c
            //        select t.Variation;
            //double s = v.Sum();
            //c.Standardize();
        }

        void TestCascade()
        {
            PIDConstant pidcIn = new PIDConstant(120, 200, 30, PIDControlAlgorithm.IPD);
            PIDConstant pidcOut = new PIDConstant(300, 30, 0, PIDControlAlgorithm.PID);

            CascadePIDCollection cc = new CascadePIDCollection(pidcIn, pidcOut, 1);
            cc.Maximum = 100;

            CascadePIDModel m1 = new CascadePIDModel(50, 52, 2000, 1950);
            cc.Add(m1);
        }


        void TestFactory()
        {
            //构造单回路
            SingleController sc = ControllerFactory.Create<SingleController>(5);
            List<PIDConstant> pidcs = new List<PIDConstant>();

            pidcs.Add(new PIDConstant(1, 1, 1, PIDControlAlgorithm.IPD));

            sc.GetInstance(pidcs, 1);

            PIDModel m1 = new PIDModel(50, 49);
            PIDModel m2 = new PIDModel(50, 49.5);
            PIDModel m3 = new PIDModel(50, 50.5);
            PIDModel m4 = new PIDModel(50, 50.8);

            sc.AddModel(m1);
            sc.AddModel(m2);
            sc.AddModel(m3);
            sc.AddModel(m4);

            //构造串级
            CascadeController cc = ControllerFactory.Create<CascadeController>(5);
            List<PIDConstant> pidcs2 = new List<PIDConstant>();
            PIDConstant pidcIn = new PIDConstant(120, 200, 30, PIDControlAlgorithm.IPD);
            PIDConstant pidcOut = new PIDConstant(300, 30, 0, PIDControlAlgorithm.PID);
            pidcs2.Add(pidcIn);
            pidcs2.Add(pidcOut);
            cc.GetInstance(pidcs2, 1);

            CascadePIDModel cm1 = new CascadePIDModel(50, 52, 2000, 1950);
            cc.AddModel(cm1);


            //构造复杂串级
            MultiCascadeController mcc = ControllerFactory.Create<MultiCascadeController>(5);
            List<PIDConstant> pidcs3 = new List<PIDConstant>();
            PIDConstant pidcIn2 = new PIDConstant(120, 200, 30, PIDControlAlgorithm.IPD);
            PIDConstant pidcOut2 = new PIDConstant(300, 30, 0, PIDControlAlgorithm.PID);
            PIDConstant pidcAu1 = new PIDConstant(100, 150, 20, PIDControlAlgorithm.IPD);//附加PID
            PIDConstant pidcAu2 = new PIDConstant(100, 150, 20, PIDControlAlgorithm.IPD);
            pidcs3.Add(pidcIn2);
            pidcs3.Add(pidcOut2);
            pidcs3.Add(pidcAu1);
            pidcs3.Add(pidcAu2);

            mcc.GetInstance(pidcs3, 1);

            MultiCascadeModel mcm1 = new MultiCascadeModel(50, 52, 2000, 1950, 1);
            PIDModel aum1 = new PIDModel(20, 19, 1);//附加回路模型
            PIDModel aum2 = new PIDModel(20, 19.8, 1);
            mcm1.AddAuxiliaryModel(aum1);
            mcm1.AddAuxiliaryModel(aum2);

            mcc.AddModel(mcm1);

        }

        void TestFactory2()
        {
            //构造进料回路
            FeedController fc = ControllerFactory.Create<FeedController>(10);

            List<PIDConstant> pidcs = new List<PIDConstant>();
            PIDConstant pidcIn = new PIDConstant(120, 200, 30, PIDControlAlgorithm.IPD);
            PIDConstant pidcOut = new PIDConstant(300, 30, 0, PIDControlAlgorithm.PID);
            pidcs.Add(pidcIn);
            pidcs.Add(pidcOut);

            double interval = 1;
            DynamicObject dynObj = new DynamicObject(20, 5, 3, 0.01, FetchType.Max);
            for (int i = 0; i < dynObj.Cycle; i++)
            {
                dynObj.UpdateDynamic(10 + r.NextDouble() * 2);
            }

            fc.GetInstance(pidcs, dynObj, interval, 100);

            //添加项
            for (int i = 0; i < 12; i++)
            {
                fc.UpdateDynamic(10 + r.NextDouble() * 2);
                FeedModel fm = new FeedModel(fc.DynamicList, 50, 52 + r.NextDouble(), 51, 0, 100, 1);
                fc.AddModel(fm);
            }


        }

        void TestFactory3()
        {
            //构造间隔回路
            SwitchController sc = ControllerFactory.Create<SwitchController>();
            sc.GetInstance(3);

            SwitchModel sm1 = new SwitchModel(50, 40, 10, 2);
            SwitchModel sm2 = new SwitchModel(50, 45, 10, 2);
            SwitchModel sm3 = new SwitchModel(50, 52, 10, 2);
            SwitchModel sm4 = new SwitchModel(50, 61, 10, 2);
            SwitchModel sm5 = new SwitchModel(50, 56, 10, 2);

            sc.AddModel(sm1);
            sc.AddModel(sm2);
            sc.AddModel(sm3);
            sc.AddModel(sm4);
            sc.AddModel(sm5);


            //构造加热炉回路
            FurnaceController fc = ControllerFactory.Create<FurnaceController>(100);
            List<PIDConstant> pidcs2 = new List<PIDConstant>();
            PIDConstant pidcIn = new PIDConstant(120, 200, 30, PIDControlAlgorithm.IPD);
            PIDConstant pidcOut = new PIDConstant(300, 30, 0, PIDControlAlgorithm.PID);
            pidcs2.Add(pidcIn);
            pidcs2.Add(pidcOut);
            FurnaceManage manage = new FurnaceManage(20, 5);
            fc.GetInstance(pidcs2, manage, 1);

            FurnaceModel fm1 = new FurnaceModel(50, 51, 52, 53, 1);
            FurnaceModel fm2 = new FurnaceModel(50, 52, 52, 55, 1);
            FurnaceModel fm3 = new FurnaceModel(50, 50.5, 52, 54, 1);
            FurnaceModel fm4 = new FurnaceModel(50, 50, 52, 52, 1);
            FurnaceModel fm5 = new FurnaceModel(50, 51.2, 52, 53, 1);
            FurnaceModel fm6 = new FurnaceModel(50, 53, 52, 56, 1);
            FurnaceModel fm7 = new FurnaceModel(50, 52.2, 52, 54, 1);
            FurnaceModel fm8 = new FurnaceModel(50, 51.6, 52, 55, 1);

            fc.AddModel(fm1);
            fc.AddModel(fm2);
            fc.AddModel(fm3);
            fc.AddModel(fm4);
            fc.AddModel(fm5);
            fc.AddModel(fm6);
            fc.AddModel(fm7);
            fc.AddModel(fm8);
        }
    }
}
