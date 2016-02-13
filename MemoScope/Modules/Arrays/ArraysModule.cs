﻿using MemoScope.Core;
using System.Collections.Generic;
using System.Windows.Forms;
using WinFwk.UICommands;
using MemoScope.Core.Helpers;
using BrightIdeasSoftware;

namespace MemoScope.Modules.Arrays
{
    public partial class ArraysModule : UIClrDumpModule, UIDataProvider<ArraysAddressList>
    {
        private List<ArraysInformation> Arrays { get; set; }

        public ArraysModule()
        {
            InitializeComponent();
        }

        public void Setup(ClrDump clrDump)
        {
            ClrDump = clrDump;
            Icon = Properties.Resources.text_rotate_small;
            Name = $"#{clrDump.Id} - Arrays";

            dlvArrays.InitColumns<ArraysInformation>();
            dlvArrays.SetUpTypeColumn<ArraysInformation>();
            dlvArrays.RegisterDataProvider(() => Data, this);
            dlvArrays.CellClick += OnCellClick;
        }

        private void OnCellClick(object sender, CellClickEventArgs e)
        {
            if( e.ClickCount == 2 && e.Model != null)
            {
                // TODO
            }
        }

        public override void Init()
        {
            base.Init();
            Arrays = ArraysAnalysis.Analyse(ClrDump, MessageBus);
        }

        public override void PostInit()
        {
            base.PostInit();
            Summary = $"{Arrays.Count} Array types";
            dlvArrays.Objects = Arrays;
            dlvArrays.Sort(nameof(ArraysInformation.TotalLength), SortOrder.Descending);
        }

        public ArraysAddressList Data
        {
            get
            {
                var arraysInfo = dlvArrays.SelectedObject<ArraysInformation>();
                if(arraysInfo ==null)
                {
                    return null;
                }
                return new ArraysAddressList(arraysInfo.ClrDumpType);
            }
        }
    }
}
