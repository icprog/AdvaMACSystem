﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ComCtrls;
using DataPool;

namespace AdvaMACSystem
{
    public partial class PageViewMAC : UIControlbase
    {
        public PageViewMAC()
        {
            InitializeComponent();

            _candatapool = CDataPool.GetDataPoolObject();

            cylinderList = new List<CylinderCellBlock>();
            //new cylinders
            for (int i = 0; i < 8; i++)
            {
                CylinderCellBlock cylinder = new CylinderCellBlock();
                cylinderList.Add(cylinder);
            }
            this.SuspendLayout();
            for (int i = 0; i < 8; i++)
            {
                CylinderCellBlock cylinder = cylinderList[i];
                cylinder.Size = new Size(CBWidth, CBHeight);
                cylinder.Location = new Point(CBMarginLeft + (i % 2) * (CBWidth + CBSpacingX), CBMarginTop + (i / 2) * (CBHeight + CBSpacingY));
                cylinder.OnCylinderClicked += new OnCylinderClickHandler(cylinder_OnCylinderClicked);
                //cylinder.Font = currentFont;
                this.Controls.Add(cylinder);
            }
            progressBarImages = new CProgressBarImagesContainer();
            progressBarImages.BgImage = AdvaMACSystemRes.graybar;
            progressBarImages.FrontImage = AdvaMACSystemRes.greenbar;
            progressBarImages.FrontImage_Unpass = AdvaMACSystemRes.redbar;
            progressBarImages.WarningImage = AdvaMACSystemRes.yellowbar;
            progressBarImages.SettingImage = AdvaMACSystemRes.bluebar;

            foreach (CylinderCellBlock ccb in cylinderList)
            {
                ccb.IMGContainer = progressBarImages;
            }
        }

        private void cylinder_OnCylinderClicked(int cylinderIndex)
        {
            if (selectedCylinderIndex >= 0 && selectedCylinderIndex < cylinderList.Count)
                cylinderList[selectedCylinderIndex].Selected = false;
            if (cylinderList[cylinderIndex].Selected)
                selectedCylinderIndex = cylinderIndex;
            else
                selectedCylinderIndex = -1;
            
        }

        #region 布局
        private Font currentFont = null;//字体
        private int CBMarginTop = 3;//第一行CylinderBlock与顶端方向间距
        private int CBMarginLeft = 200;//第一列CylinderBlock与左端方向间距
        private int CBWidth = 310;//CylinderBlock宽度
        private int CBHeight = 145;//CylinderBlock高度
        private int CBSpacingX = 5;//CylinderBlock之间X方向间距
        private int CBSpacingY = 5;//CylinderBlock之间Y方向间距

        private int ButtonWidth = 235;
        private int ButtonHeight = 50;
        private int BUttonSpacingX = 15;
        #endregion

        private List<CylinderCellBlock> cylinderList = null;

        private CProgressBarImagesContainer progressBarImages = null;
        double x = 0;
        private void timer_RefreshMac_Tick(object sender, EventArgs e)
        {
            //x += 0.1;
            for (int i = 0; i < cylinderList.Count; i++)
            {
                if (cylinderList[i].InUse)
                {
                    //cylinderList[i].CurrentPressureValue = 20 * Math.Sin(x) + 30;
                    cylinderList[i].CurrentPressureValue = _candatapool.GetRealValue(pumpIndex, i, CmdDataType.cdtPressure_Real);
                    cylinderList[i].CurrentDistanceValue = _candatapool.GetRealValue(pumpIndex, i, CmdDataType.cdtPosition_Real);
                }
                else
                {
                    cylinderList[i].CurrentPressureValue = 0;
                    cylinderList[i].CurrentDistanceValue = 0;
                }
            }
        }

        private int pumpIndex = 0;//泵的编号
        private int selectedCylinderIndex = -1;

        private CDataPool _candatapool = null;
        public CDataPool CanDatapool
        {
            get { return _candatapool; }
            set
            {
                if (_candatapool != value)
                    _candatapool = value;
            }
        }

        public override void DoEnter()
        {
            RefreshCylinderList();
            this.timer_RefreshMac.Enabled = true;
            base.DoEnter();
        }

        private void RefreshCylinderList()
        {
            if (cylinderList == null)
                return;

            for (int i = 0; i < cylinderList.Count; i++)
            {
                cylinderList[i].InitialInstance(pumpIndex, i);
            }

            if (_candatapool == null)
                return;

            for (int i = 0; i < cylinderList.Count; i++)
            {
                //设定压力条单位
                cylinderList[i].PressureUnit = "kN";
                //设定压力条下限值
                cylinderList[i].MinPressureValue = 0;
                //设定压力条上限值
                cylinderList[i].MaxPressureValue = 60;
                //设定压力条下限报警值
                cylinderList[i].WarningPressureValue = 30;
                //设定压力条设定值
                cylinderList[i].SettingPressureValue = 50;

                //设定长度条单位
                cylinderList[i].DistanceUnit = "mm";
                //设定长度条下限值
                cylinderList[i].MinDistanceValue = 0;
                //设定长度条上限值
                cylinderList[i].MaxDistanceValue = 600;
                //设定长度条下限报警值
                cylinderList[i].WarningDistanceValue = 300;
                //设定长度条设定值
                cylinderList[i].SettingDistanceValue = 500;

                //检测油缸运行状态
                cylinderList[i].InUse = true;
                //cylinderList[i].InUse = _candatapool.GetintValue(pumpIndex, i, CmdDataType.cdtcylinderState_Real) != 0;
            }
            cylinderList[1].InUse = false;
        }

    }
}
