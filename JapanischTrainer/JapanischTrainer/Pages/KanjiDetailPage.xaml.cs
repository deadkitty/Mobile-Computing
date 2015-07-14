using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using JapanischTrainer.Pages.Controls;
using JapanischTrainer.Data;
using JapanischTrainer.Database;

namespace JapanischTrainer.Pages
{
    public partial class KanjiDetailPage : PhoneApplicationPage
    {
        #region Fields

        int lastPivotIndex = 0;
        int currKanjiIndex = 0;

        PivotItem[] items;
        DetailKanjiItem[] kanjiItems;
        
        #endregion

        #region Properties

        private int PrevKanjiIndex
        {
            get
            {
                if (currKanjiIndex == 0)
                {
                    return AppData.Kanjis.Length - 1;
                }
                return currKanjiIndex - 1;
            }
        }

        private int NextKanjiIndex
        {
            get 
            {
                return (currKanjiIndex + 1) % AppData.Kanjis.Length; 
            }
        }
        
        #endregion

        #region Constructor

        public KanjiDetailPage()
        {
            InitializeComponent();

            items = new PivotItem[]
            {
                privotCtrl.Items[0] as PivotItem,
                privotCtrl.Items[1] as PivotItem,
                privotCtrl.Items[2] as PivotItem,
            };

            kanjiItems = new DetailKanjiItem[]
            {
                (items[0].Content as Grid).Children[0] as DetailKanjiItem,
                (items[1].Content as Grid).Children[0] as DetailKanjiItem,
                (items[2].Content as Grid).Children[0] as DetailKanjiItem,
            };
        }
        
        #endregion

        #region Events

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            currKanjiIndex = AppData.SelectedKanjiIndex;

            kanjiItems[0].FillKanjiItem(AppData.Kanjis[currKanjiIndex]);
            kanjiItems[1].FillKanjiItem(AppData.Kanjis[NextKanjiIndex]);
            kanjiItems[2].FillKanjiItem(AppData.Kanjis[PrevKanjiIndex]);            
        }

        /// <summary>
        /// when the page is created SelectionChanged is called one time, but i
        /// can´t do that the first, and i didn´t want to use an if clause just for
        /// one time, so i just exchange the SelectionChanged handler ;D
        /// </summary>
        private void privotCtrl_FirstSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            privotCtrl.SelectionChanged -= privotCtrl_FirstSelectionChanged;
            privotCtrl.SelectionChanged += privotCtrl_SelectionChanged;
        }

        private void privotCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (lastPivotIndex)
            {
                case 0: currKanjiIndex = privotCtrl.SelectedIndex == 1 ? NextKanjiIndex : PrevKanjiIndex; break;
                case 1: currKanjiIndex = privotCtrl.SelectedIndex == 2 ? NextKanjiIndex : PrevKanjiIndex; break;
                case 2: currKanjiIndex = privotCtrl.SelectedIndex == 0 ? NextKanjiIndex : PrevKanjiIndex; break;
            }

            switch (privotCtrl.SelectedIndex)
            {
                case 0:
                    kanjiItems[2].FillKanjiItem(AppData.Kanjis[PrevKanjiIndex]);
                    kanjiItems[1].FillKanjiItem(AppData.Kanjis[NextKanjiIndex]);

                    break;

                case 1:
                    kanjiItems[0].FillKanjiItem(AppData.Kanjis[PrevKanjiIndex]);
                    kanjiItems[2].FillKanjiItem(AppData.Kanjis[NextKanjiIndex]);

                    break;

                case 2:
                    kanjiItems[1].FillKanjiItem(AppData.Kanjis[PrevKanjiIndex]);
                    kanjiItems[0].FillKanjiItem(AppData.Kanjis[NextKanjiIndex]);

                    break;
            }
            lastPivotIndex = privotCtrl.SelectedIndex;
        }

        #endregion
    }
}