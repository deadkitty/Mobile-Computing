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
using NihongoSenpai.Controller;
using NihongoSenpai.Database;
using NihongoSenpai.Settings;

namespace NihongoSenpai.Pages
{
    public partial class PracticeVocabPage : PhoneApplicationPage
    {
        #region Constructor

        public PracticeVocabPage()
            : base()
        {
            InitializeComponent();
            
            pageScrollViewer.IsEnabled = true;

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
            if (VocabData.IncorrectWords.Count > 0)
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
            if (VocabData.ItemsLeft == 0)
            {
                EndPractice();

                return;
            }

            wordsLeftValueTextblock   .Text = VocabData.ItemsLeft   .ToString();
            wordsCorrectValueTextblock.Text = VocabData.ItemsCorrect.ToString();
            wordsWrongValueTextblock  .Text = VocabData.ItemsWrong  .ToString();

            visibleTextblock    .Text = VocabData.ShownText;
            hiddenTextblock     .Text = VocabData.AnswerText;
            descriptionTextblock.Text = VocabData.DescriptionText;
        }


        public void EndPractice()
        {
            MessageBox.Show("Keine Wörter mehr zum Lernen verfügbar");

            NavigationService.GoBack();
        }

        #endregion
    }
}