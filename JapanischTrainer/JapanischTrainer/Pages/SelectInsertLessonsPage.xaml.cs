﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NihongoSenpai.Database;
using NihongoSenpai.Controller;

namespace NihongoSenpai.Pages
{
    public partial class SelectInsertLessonsPage : PhoneApplicationPage
    {
        #region Constructor

        public SelectInsertLessonsPage()
        {
            InitializeComponent();
        }
        
        #endregion

        #region Events

        #region Load Lessons

        private void loadLessonsButton_Click(object sender, RoutedEventArgs e)
        {
            if (setsListbox.SelectedItems.Count > 0)
            {
                Lesson[] selectedLessons = new Lesson[setsListbox.SelectedItems.Count];

                for (int i = 0; i < selectedLessons.Length; ++i)
                {
                    selectedLessons[i] = setsListbox.SelectedItems[i] as Lesson;
                }

                InsertController.LoadLessons(selectedLessons);

                NavigationService.Navigate(new Uri("/Pages/PracticeInsertPage.xaml", UriKind.Relative));

                setsListbox.SelectedItems.Clear();
            }
            else
            {
                MessageBox.Show("Keine Lektion ausgewählt!");
            }
        }
        
        #endregion

        #region Back, NavigatedFrom/To

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
            if (e.NavigationMode == NavigationMode.New)
            {
                DataManager.ConnectToLocalStorageDatabase();

                DataManager.LoadLessons(Lesson.EType.insert);

                foreach (Lesson l in AppData.Lessons)
                {
                    setsListbox.Items.Add(l);
                }
            }
        }

        #endregion

        #region ApplicationBar

        private void selectAll_Click(object sender, EventArgs e)
        {
            foreach (object item in setsListbox.Items)
            {
                setsListbox.SelectedItems.Add(item);
            }
        }

        private void selectNone_Click(object sender, EventArgs e)
        {
            setsListbox.SelectedItems.Clear();
        }

        #endregion

        #endregion
    }
}