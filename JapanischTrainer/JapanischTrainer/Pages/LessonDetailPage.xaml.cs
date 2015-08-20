using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NihongoSenpai.Data;
using NihongoSenpai.Pages.Controls;
using NihongoSenpai.Settings;
using NihongoSenpai.Database;
using System.Diagnostics;

namespace NihongoSenpai.Pages
{
    public partial class LessonDetailPage : PhoneApplicationPage
    {
        private bool showStats = false;
        
        private bool showPercentage = false;
        private bool showTranslationStats = false;
        
        private StatisticsItem[] statisticItems;
        private DetailWordItem[] detailItems;

        public LessonDetailPage()
        {
            InitializeComponent();

            //appbar icons and items are no part of silverlight and i have to assign them 
            //manually otherwise they are null
            showPercentageIcon  = ApplicationBar.Buttons[1] as ApplicationBarIconButton;            
            showJapStatsItem    = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            showTranslStatsItem = ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
            sortAccendingItem   = ApplicationBar.MenuItems[2] as ApplicationBarMenuItem;
            sortDeccendingItem  = ApplicationBar.MenuItems[3] as ApplicationBarMenuItem;

            setnameTextblock.Text = AppData.SelectedLesson.name;

            statisticItems = new StatisticsItem[AppData.Words.Length];
            detailItems = new DetailWordItem[AppData.Words.Length];

            AddDetailItems();
        }

        private void editIcon_Click(object sender, System.EventArgs e)
        {
            //NavigateTo(EPageType.VocabEditPage);
        }
        
        private void showStatsIcon_Click(object sender, System.EventArgs e)
        {
            showStats = !showStats;

            showPercentageIcon.IsEnabled = showStats;
            
            showJapStatsItem.IsEnabled    = showStats;
            showTranslStatsItem.IsEnabled = showStats;
            sortAccendingItem.IsEnabled   = showStats;
            sortDeccendingItem.IsEnabled  = showStats;

            wordsPanel.Children.Clear();

            if (showStats)
            {
                AddStatisticItems(showTranslationStats);
            }
            else
            {
                AddDetailItems();
            }
        }

        private void resetLessonIcon_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Lektion wirklich zurücksetzen???", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DataManager.ResetWords(false);

                for (int i = 0; i < wordsPanel.Children.Count; ++i)
                {
                    (wordsPanel.Children[i] as StatisticsItem).HideProgressBar();
                }
            }
        }

        private void showPercentageIcon_Click(object sender, EventArgs e)
        {
            showPercentage = !showPercentage;
            UpdateStatisticItems(showTranslationStats);
        }

        private void showJapStatsItem_Click(object sender, EventArgs e)
        {
            if (!showTranslationStats)
                return;

            showTranslationStats = false;
            UpdateStatisticItems(false);
        }

        private void showTranslStatsItem_Click(object sender, EventArgs e)
        {
            if (showTranslationStats)
                return;

            showTranslationStats = true;
            UpdateStatisticItems(true);
        }

        private void sortAccendingItem_Click(object sender, EventArgs e)
        {
            wordsPanel.Children.Clear();
                        
            DataManager.SortWords(true, showTranslationStats);

            AddStatisticItems(showTranslationStats);
        }

        private void sortDeccendingItem_Click(object sender, EventArgs e)
        {
            wordsPanel.Children.Clear();

            DataManager.SortWords(false, showTranslationStats);

            AddStatisticItems(showTranslationStats);
        }

        private void AddStatisticItems(bool showTranslation)
        {
            foreach (Word w in AppData.Words)
            {
                wordsPanel.Children.Add(new StatisticsItem(w, showPercentage, showTranslation));
                statisticItems[wordsPanel.Children.Count - 1] = wordsPanel.Children[wordsPanel.Children.Count - 1] as StatisticsItem;
            }
        }

        private void AddDetailItems()
        {
            foreach (Word w in AppData.Words)
            {
                Debug.WriteLine("Add Item: " + w.ToExportString());

                wordsPanel.Children.Add(new DetailWordItem(w));
                detailItems[wordsPanel.Children.Count - 1] = wordsPanel.Children[wordsPanel.Children.Count - 1] as DetailWordItem;
            }
        }

        private void UpdateStatisticItems(bool showTranslation)
        {
            for (int i = 0; i < wordsPanel.Children.Count; ++i)
            {
                statisticItems[i].Update(AppData.Words[i], showPercentage, showTranslation);
            }
        }
    }
}