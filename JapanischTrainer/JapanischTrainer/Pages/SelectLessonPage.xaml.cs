using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using JapanischTrainer.Database;

namespace JapanischTrainer.Pages
{
    public partial class SelectLessonPage : PhoneApplicationPage
    {
        #region Fields

        private String navigationPageString;
        private String pageTypeString;

        #endregion

        #region Constructor
        
        public SelectLessonPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Events
        
        private void loadLessonsButton_Click(object sender, RoutedEventArgs e)
        {
            if (setsListbox.SelectedItem != null)
            {
                
                switch (pageTypeString)
                {
                    case "vocab": DataManager.LoadWords (setsListbox.SelectedItem as Lesson); break;
                    case "kanji": DataManager.LoadKanjis(setsListbox.SelectedItem as Lesson); break;
                }

                NavigationService.Navigate(new Uri(navigationPageString, UriKind.Relative));

                setsListbox.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Keine Lektion ausgewählt!");
            }
        }
        
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //if i go back to main page, close the database connection
            if (e.NavigationMode == NavigationMode.Back)
            {
                DataManager.CloseConnection();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            //if i come from main Page open database connection and initialize lessons
            if(e.NavigationMode == NavigationMode.New)
            {
                NavigationContext.QueryString.TryGetValue("pageType", out pageTypeString);

                DataManager.ConnectToLocalStorageDatabase();

                switch (pageTypeString)
                {
                    case "vocab":
                        
                        navigationPageString = "/Pages/LessonDetailPage.xaml";
                        DataManager.LoadLessons(Lesson.EType.vocabulary);     
   
                        break;

                    case "kanji": 
                        
                        navigationPageString = "/Pages/SelectKanjiPage.xaml";                        
                        DataManager.LoadLessons(Lesson.EType.kanji);
                
                        break;
                }

                foreach (Lesson l in AppData.Lessons)
                {
                    setsListbox.Items.Add(l);
                }
            }
        }

        #endregion
    }
}