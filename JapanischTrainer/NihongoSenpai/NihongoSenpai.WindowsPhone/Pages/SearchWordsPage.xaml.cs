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

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace NihongoSenpai.Pages
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class SearchWordsPage : BasePage
    {
        #region Constructor

        public SearchWordsPage()
            : base()
        {
            this.InitializeComponent();
        }
        
        #endregion
        
        #region Events

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            //if (matchedItemsListbox.SelectedItem == null)
            //{                
            //    //MessageBox.Show("Kein Wort ausgewählt!");
            //}
            //else
            //{
            //DetailWordItem wordItem = matchedItemsListbox.SelectedItem as DetailWordItem;

            editWordItem.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //editWordItem.FillControl(wordItem.value);
            //}
        }

        private void showLessonButton_Click(object sender, RoutedEventArgs e)
        {
            //if(matchedItemsListbox.SelectedItem == null)
            //{
            //    MessageBox.Show("Kein Wort ausgewählt!");
            //}
            //else
            //{
            //    DetailWordItem wordItem = matchedItemsListbox.SelectedItem as DetailWordItem;

            //    DataManager.LoadWords(wordItem.value.lessonID);

            //    NavigationService.Navigate(new Uri("/Pages/LessonDetailPage.xaml", UriKind.Relative));

            //    matchedItemsListbox.SelectedItem = null;
            //}
        }

        private void matchedItemsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (DetailWordItem item in e.AddedItems)
            //{
            //    item.Selected();
            //}

            //foreach (DetailWordItem item in e.RemovedItems)
            //{
            //    item.Deselected();
            //}
        }

        private void searchTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                //DataManager.FindWords(searchTextbox.Text);

                //matchedItemsListbox.Items.Clear();

                //if (AppData.Words != null)
                //{
                //    foreach (Word w in AppData.Words)
                //    {
                //        matchedItemsListbox.Items.Add(new DetailWordItem(w));
                //    }
                //}

                //hide keyboard TODO: find better solution
                searchTextbox.IsEnabled = false;
                searchTextbox.IsEnabled = true;

                //works but textbox has still focus and i have to unfocus it somehow, otherwise 
                //the keyboard doesn't show up again if i click in the textbox and i have to first unfocus 
                //it manually
                //InputPane.GetForCurrentView().TryHide();
            }
        }
        
        #endregion
    }
}
