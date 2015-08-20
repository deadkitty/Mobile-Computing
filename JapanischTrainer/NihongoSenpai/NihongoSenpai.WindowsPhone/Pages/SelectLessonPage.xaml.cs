using NihongoSenpai.Database;
using NihongoSenpai.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

namespace NihongoSenpai.Pages
{
    public sealed partial class SelectLessonPage : BasePage
    {
        #region Constructor

        public SelectLessonPage()
            : base()
        {
            this.InitializeComponent();
        }
        
        #endregion
        
        #region Events
        
        private async void loadLessonsButton_Click(object sender, RoutedEventArgs e)
        {
            if (setsListbox.SelectedItem != null)
            {
                Lesson selectedLesson = setsListbox.SelectedItem as Lesson;

                switch (selectedLesson.Type)
                {
                    case Lesson.EType.vocabulary:
                        
                        DataManager.LoadWords (selectedLesson);
                        Frame.Navigate(typeof(LessonDetailPage));
                        
                        break;
                        
                    case Lesson.EType.kanji:
                        
                        DataManager.LoadKanjis(selectedLesson);
                        Frame.Navigate(typeof(SelectKanjiPage));
                        
                        break;
                }
                
                setsListbox.SelectedItem = null;
            }
            else
            {
                MessageDialog msg = new MessageDialog("Keine Lektion ausgewählt!");

                await msg.ShowAsync();
            }
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataManager.ConnectToDatabase();

            if((e.Parameter as String) == "kanji")
            {
                DataManager.LoadLessons(Lesson.EType.kanji);
            }
            else
            {
                DataManager.LoadLessons(Lesson.EType.vocabulary);
            }
            
            foreach (Lesson l in AppData.Lessons)
            {
                setsListbox.Items.Add(l);
            }
            
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if(e.NavigationMode == NavigationMode.Back)
            {
                DataManager.CloseConnection();
            }

            base.OnNavigatedFrom(e);
        }

        #endregion
    }
}