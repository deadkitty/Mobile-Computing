using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NihongoSenpai.Pages.Controls;
using NihongoSenpai.Controller;
using NihongoSenpai.Data;
using NihongoSenpai.Database;

namespace NihongoSenpai.Pages
{
    public partial class PracticeFlashcardsPage : PhoneApplicationPage
    {
        #region Constructor
                
        public PracticeFlashcardsPage()
        {         
            InitializeComponent();

            UpdateView();
        }
        
        #endregion

        #region Events
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e); 
            
            DataManager.UpdateProgress();            
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            FlashcardsController.Deinitialize();
        }

        private void correct1Button_Click(object sender, RoutedEventArgs e)
        {
            FlashcardsController.EvaluateKanji(5);

            UpdateView();
        }

        private void correct2Button_Click(object sender, RoutedEventArgs e)
        {
            FlashcardsController.EvaluateKanji(4);

            UpdateView();
        }

        private void correct3Button_Click(object sender, RoutedEventArgs e)
        {
            FlashcardsController.EvaluateKanji(3);

            UpdateView();
        }
        
        private void wrong1Button_Click(object sender, RoutedEventArgs e)
        {
            FlashcardsController.EvaluateKanji(2);

            UpdateView();
        }

        private void wrong2Button_Click(object sender, RoutedEventArgs e)
        {
            FlashcardsController.EvaluateKanji(1);

            UpdateView();
        }

        private void wrong3Button_Click(object sender, RoutedEventArgs e)
        {
            FlashcardsController.EvaluateKanji(0);

            UpdateView();
        }
        
        private void UpdateView()
        {
            detailKanjiItem.FillKanjiItem(FlashcardsData.ActiveKanji, true);
        }

        #endregion
    }
}