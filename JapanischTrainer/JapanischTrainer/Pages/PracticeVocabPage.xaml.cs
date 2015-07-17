using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using JapanischTrainer.Data;
using JapanischTrainer.Controller;
using JapanischTrainer.Database;
using JapanischTrainer.Settings;

namespace JapanischTrainer.Pages
{
    public partial class PracticeVocabPage : PhoneApplicationPage
    {
        #region Constructor

        public PracticeVocabPage()
            : base()
        {
            InitializeComponent();

            //for the sake of non complexity^^ and because of i made part lessons to learn wrong words in
            //near future again, i disable the option to learn wrong words in the middle of a part lesson
            if(AppSettings.PartLessons)
            {
                (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = false;
            }
            
            UpdateView();
        }

        #endregion

        #region Events

        private void wrongButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            correctButton.Visibility = Visibility.Collapsed;
            wrongButton  .Visibility = Visibility.Collapsed;
            showButton   .Visibility = Visibility.Visible;

            hiddenTextblock.Visibility = Visibility.Collapsed;

            VocabController.WordWrong();
            VocabController.GetNext();

            UpdateView();
        }

        private void correctButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            correctButton.Visibility = Visibility.Collapsed;
            wrongButton  .Visibility = Visibility.Collapsed;
            showButton   .Visibility = Visibility.Visible;

            hiddenTextblock.Visibility = Visibility.Collapsed;

            VocabController.WordCorrect();
            VocabController.GetNext();

            UpdateView();
        }

        private void showButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            correctButton.Visibility = Visibility.Visible;
            wrongButton  .Visibility = Visibility.Visible;
            showButton   .Visibility = Visibility.Collapsed;

            hiddenTextblock.Visibility = Visibility.Visible;
            
            descriptionTextblock.Text = VocabData.ActiveWord.ToDescriptionString();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            DataManager.UpdateProgress();
            VocabController.Deinitialize();
        }

        private void learnWrongWordsItem_Click(object sender, EventArgs e)
        {
            if (VocabData.WrongAnsweredWords.Count > 0)
            {
                VocabController.LearnWrongWords();

                UpdateView();
            }
            else
            {
                MessageBox.Show("Noch keine falschen Wörter verfügbar die wiederholt werden könnten!");
            }
        }

        private void editIcon_Click(object sender, System.EventArgs e)
        {
            editWordsControl.Visibility = System.Windows.Visibility.Visible;
            editWordsControl.FillControl(VocabData.ActiveWord);
        }
        
        public void UpdateView()
        {
            if(VocabData.ItemsLeft == 0)
            {
                EndPractice();

                return;
            }

            wordsLeftValueTextblock   .Text = VocabData.ItemsLeft   .ToString();
            wordsCorrectValueTextblock.Text = VocabData.ItemsCorrect.ToString();
            wordsWrongValueTextblock  .Text = VocabData.ItemsWrong  .ToString();

            descriptionTextblock.Text = null;

            if (AppSettings.ShowDescription)
            {
                switch (VocabData.ActiveWord.showFlags)
                {
                    case 1: if (!VocabData.ActiveWord.showJWord) descriptionTextblock.Text = VocabData.ActiveWord.ToDescriptionString(); break;
                    case 2: if ( VocabData.ActiveWord.showJWord) descriptionTextblock.Text = VocabData.ActiveWord.ToDescriptionString(); break;
                    case 3:                                      descriptionTextblock.Text = VocabData.ActiveWord.ToDescriptionString(); break;
                }
            }

            if (VocabData.ActiveWord.showJWord)
            {
                if (VocabData.ActiveWord.kanji != null)
                {
                    visibleTextblock.Text = VocabData.ActiveWord.kanji;
                    hiddenTextblock.Text = VocabData.ActiveWord.kana + ",\n" + VocabData.ActiveWord.translation;
                }
                else
                {
                    visibleTextblock.Text = VocabData.ActiveWord.kana;
                    hiddenTextblock.Text = VocabData.ActiveWord.translation;
                }
            }
            else
            {
                visibleTextblock.Text = VocabData.ActiveWord.translation;

                if (VocabData.ActiveWord.kanji != null)
                {
                    hiddenTextblock.Text = VocabData.ActiveWord.kanji + ",\n" + VocabData.ActiveWord.kana;
                }
                else
                {
                    hiddenTextblock.Text = VocabData.ActiveWord.kana;
                }
            }

            if (pageScrollViewer.ScrollableHeight > pageScrollViewer.Height)
            {
                pageScrollViewer.IsEnabled = false;
            }
            else
            {
                pageScrollViewer.IsEnabled = true;
            }
        }

        public void EndPractice()
        {
            MessageBox.Show("Keine Wörter mehr zum Lernen verfügbar");

            NavigationService.GoBack();
        }

        #endregion
    }
}