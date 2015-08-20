using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using NihongoSenpai.Data;
using NihongoSenpai.Pages.Controls;
using NihongoSenpai.Database;

namespace NihongoSenpai.Pages
{
    public partial class SearchWordsPage : PhoneApplicationPage, IPageUpdater
    {
        public SearchWordsPage()
        {
            InitializeComponent();

            editWordsControl.pageUpdater = this;
        }

        private void searchTextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataManager.FindWords(searchTextbox.Text);

                matchedItemsListbox.Items.Clear();

                if (AppData.Words != null)
                {
                    foreach (Word w in AppData.Words)
                    {
                        matchedItemsListbox.Items.Add(new DetailWordItem(w));
                    }
                }

                //hide keyboard
                Focus();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.NavigationMode == NavigationMode.New)
            {
                DataManager.ConnectToLocalStorageDatabase();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            DataManager.CloseConnection();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (matchedItemsListbox.SelectedItem == null)
            {
                MessageBox.Show("Kein Wort ausgewählt!");
            }
            else
            {
                DetailWordItem wordItem = matchedItemsListbox.SelectedItem as DetailWordItem;

                editWordsControl.Visibility = System.Windows.Visibility.Visible;
                editWordsControl.FillControl(wordItem.value);
            }
        }

        private void showLessonButton_Click(object sender, RoutedEventArgs e)
        {
            if(matchedItemsListbox.SelectedItem == null)
            {
                MessageBox.Show("Kein Wort ausgewählt!");
            }
            else
            {
                DetailWordItem wordItem = matchedItemsListbox.SelectedItem as DetailWordItem;

                DataManager.LoadWords(wordItem.value.lessonID);

                NavigationService.Navigate(new Uri("/Pages/LessonDetailPage.xaml", UriKind.Relative));

                matchedItemsListbox.SelectedItem = null;
            }
        }

        private void matchedItemsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (DetailWordItem item in e.AddedItems)
            {
                item.Selected();
            }

            foreach (DetailWordItem item in e.RemovedItems)
            {
                item.Deselected();
            }
        }
        
        public void UpdatePage()
        {
            DetailWordItem wordItem = matchedItemsListbox.SelectedItem as DetailWordItem;

            wordItem.Update();
        }
    }
}