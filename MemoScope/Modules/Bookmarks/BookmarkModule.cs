﻿using BrightIdeasSoftware;
using MemoScope.Core;
using MemoScope.Core.Bookmark;
using MemoScope.Core.Helpers;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFwk.UIMessages;

namespace MemoScope.Modules.Bookmarks
{
    public partial class BookmarkModule : UIClrDumpModule, IMessageListener<BookmarkMessage>
    {
        public BookmarkModule()
        {
            InitializeComponent();
        }

        public void SetUp(ClrDump clrDump)
        {
            ClrDump = clrDump;
            dlvBookmarks.InitColumns<Bookmark>();
            dlvBookmarks.SetUpAddressColumn<Bookmark>(this);
            dlvBookmarks.SetUpTypeColumn<Bookmark>(this);
            var col = dlvBookmarks[nameof(Bookmark.Comment)];
            col.CellEditUseWholeCell = true;
            dlvBookmarks.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
            dlvBookmarks.CellEditFinished += (o, e) => {
                ClrDump.BookmarkMgr.SaveBookmarks();
            };

            var colColor = dlvBookmarks.AllColumns.FirstOrDefault(c => c.Name == nameof(Bookmark.Color));
            dlvBookmarks.FormatCell += (o, e) =>
            {
                if ( e.Column != colColor)
                {
                    return;
                }
                var rowObj = e.Model;
                var bookmark = rowObj as Bookmark;
                if (bookmark != null)
                {
                    e.SubItem.BackColor = bookmark.Color;
                    e.SubItem.Text = bookmark.Color != Color.Empty ? null : "Select Color...";
                }
            };
            var colPick = dlvBookmarks.GetColumn(nameof(Bookmark.ColorPick));
            colPick.IsButton = true;
            colPick.ButtonSizing = OLVColumn.ButtonSizingMode.CellBounds;

            dlvBookmarks.ButtonClick += (o, e) =>
            {
                var rowObj = e.Model;
                var bookmark = rowObj as Bookmark;
                if (bookmark != null)
                {
                    ColorDialog colDiag = new ColorDialog();
                    colDiag.Color = bookmark.Color;
                    if (colDiag.ShowDialog() == DialogResult.OK)
                    {
                        bookmark.Color = colDiag.Color;
                        ClrDump.BookmarkMgr.SaveBookmarks();
                    }
                }
            };
            dlvBookmarks.UseCellFormatEvents = true;
            LoadBookmarks();
        }

        private void LoadBookmarks()
        {
            dlvBookmarks.Objects = ClrDump.BookmarkMgr.GetBookmarks();
            dlvBookmarks.BuildGroups(nameof(Bookmark.TypeName), SortOrder.Ascending);
        }

        public override void PostInit()
        {
            Icon = Properties.Resources.award_star_gold_blue;
            Name = $"#{ClrDump.Id} - Bookmarks";
        }

        public void HandleMessage(BookmarkMessage message)
        {
            LoadBookmarks();
        }
    }
}
