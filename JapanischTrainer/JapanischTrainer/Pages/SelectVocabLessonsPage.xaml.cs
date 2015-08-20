using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NihongoSenpai.Database;
using NihongoSenpai.Data;
using NihongoSenpai.Settings;
using NihongoSenpai.Controller;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NihongoSenpai.Pages
{
    public partial class SelectVocabLessonsPage : PhoneApplicationPage
    {
        #region Fields

        private BitmapImage japGerIconSelectedBitmap;
        private BitmapImage japGerIconBitmap;

        private BitmapImage gerIconSelectedBitmap;
        private BitmapImage gerIconBitmap;

        private BitmapImage japIconSelectedBitmap;
        private BitmapImage japIconBitmap;

        #endregion

        #region Constructor
        
        public SelectVocabLessonsPage()
        {
            InitializeComponent();

            japGerIconSelectedBitmap = new BitmapImage(new Uri("/Assets/AppBar/japaneseGermanSelected.png", UriKind.Relative));
            japGerIconBitmap = new BitmapImage(new Uri("/Assets/AppBar/japaneseGerman.png", UriKind.Relative));
            
            gerIconSelectedBitmap = new BitmapImage(new Uri("/Assets/AppBar/germanSelected.png", UriKind.Relative));
            gerIconBitmap = new BitmapImage(new Uri("/Assets/AppBar/german.png", UriKind.Relative));

            japIconSelectedBitmap = new BitmapImage(new Uri("/Assets/AppBar/japaneseSelected.png", UriKind.Relative));
            japIconBitmap = new BitmapImage(new Uri("/Assets/AppBar/japanese.png", UriKind.Relative));

            SelectAppbarIcons();

            SetLoadOptions();
            SetSortOrder();
        }

        #endregion

        #region Events
        
        private void loadLessonsButton_Click(object sender, RoutedEventArgs e)
        {
            if (setsListbox.SelectedItems.Count > 0)
            {
                GetLoadOptions();
                GetSortOrder();

                Lesson[] selectedLessons = new Lesson[setsListbox.SelectedItems.Count];

                for (int i = 0; i < selectedLessons.Length; ++i)
                {
                    selectedLessons[i] = setsListbox.SelectedItems[i] as Lesson;
                }

                VocabController.LoadLessons(selectedLessons);

                if (VocabData.Words.Count == 0)
                {
                    MessageBox.Show("Keine Wörter in den gewählten Lektionen mehr verfügbar!");
                    VocabController.Deinitialize();

                    return;
                }

                NavigationService.Navigate(new Uri("/Pages/PracticeVocabPage.xaml", UriKind.Relative));

                setsListbox.SelectedItems.Clear();
            }
            else
            {
                MessageBox.Show("Keine Lektion ausgewählt!");
            }
        }

        #region Load Options

        private void SelectAppbarIcons()
        {
            switch (AppSettings.WordPracticeMethod)
            {
                case 0:

                    japGerIcon.Source = japGerIconSelectedBitmap;
                    japIcon   .Source = japIconBitmap;
                    gerIcon   .Source = gerIconBitmap;

                    break;

                case 1:

                    japGerIcon.Source = japGerIconBitmap;
                    gerIcon   .Source = gerIconSelectedBitmap;
                    japIcon   .Source = japIconBitmap;

                    break;

                case 2:

                    japGerIcon.Source = japGerIconBitmap;
                    gerIcon   .Source = gerIconBitmap;
                    japIcon   .Source = japIconSelectedBitmap;

                    break;
            }
        }

        private void japGerIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AppSettings.WordPracticeMethod = 0;
            SelectAppbarIcons();
        }

        private void gerIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AppSettings.WordPracticeMethod = 1;
            SelectAppbarIcons();
        }

        private void japIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AppSettings.WordPracticeMethod = 2;
            SelectAppbarIcons();
        }

        private void SetLoadOptions()
        {
            int loadOptions = AppSettings.LoadOptions;

            otherCheckbox.IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            phrCheckbox  .IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            suffCheckbox .IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            prevCheckbox .IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            partCheckbox .IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            nounCheckbox .IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            advCheckbox  .IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            naAdjCheckbox.IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            iAdjCheckbox .IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            verb3Checkbox.IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            verb2Checkbox.IsChecked = loadOptions % 2 == 1 ? true : false;
            loadOptions >>= 1;
            verb1Checkbox.IsChecked = loadOptions % 2 == 1 ? true : false;
        }

        private void GetLoadOptions()
        {
            int loadOptions = 0;

            loadOptions += verb1Checkbox.IsChecked == true ? 1 : 0;
            loadOptions <<= 1;
            loadOptions += verb2Checkbox.IsChecked == true ? 1 : 0;
            loadOptions <<= 1;
            loadOptions += verb3Checkbox.IsChecked == true ? 1 : 0;
            loadOptions <<= 1;
            loadOptions += iAdjCheckbox .IsChecked == true ? 1 : 0;
            loadOptions <<= 1;
            loadOptions += naAdjCheckbox.IsChecked == true ? 1 : 0;
            loadOptions <<= 1;
            loadOptions += advCheckbox  .IsChecked == true ? 1 : 0;
            loadOptions <<= 1;
            loadOptions += nounCheckbox .IsChecked == true ? 1 : 0;
            loadOptions <<= 1;          
            loadOptions += partCheckbox .IsChecked == true ? 1 : 0;
            loadOptions <<= 1;          
            loadOptions += prevCheckbox .IsChecked == true ? 1 : 0;
            loadOptions <<= 1;          
            loadOptions += suffCheckbox .IsChecked == true ? 1 : 0;
            loadOptions <<= 1;          
            loadOptions += phrCheckbox  .IsChecked == true ? 1 : 0;
            loadOptions <<= 1;
            loadOptions += otherCheckbox.IsChecked == true ? 1 : 0;

            AppSettings.LoadOptions = loadOptions;
        }
        
        private void SetSortOrder()
        {
            switch (AppSettings.SortOrder)
            {
                case 0: radio0.IsChecked = true; break;
                case 1: radio1.IsChecked = true; break;
                case 2: radio2.IsChecked = true; break;
                case 3: radio3.IsChecked = true; break;
            }
        }

        private void GetSortOrder()
        {
            if (radio0.IsChecked.Value)
            {
                AppSettings.SortOrder = 0;
            }
            else if (radio1.IsChecked.Value)
            {
                AppSettings.SortOrder = 1;
            }
            else if (radio2.IsChecked.Value)
            {
                AppSettings.SortOrder = 2;
            }
            else if (radio3.IsChecked.Value)
            {
                AppSettings.SortOrder = 3;
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
            
            AppSettings.SaveSettings();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            //if i come from main Page open database connection and initialize lessons
            if(e.NavigationMode == NavigationMode.New)
            {
                DataManager.ConnectToLocalStorageDatabase();

                DataManager.LoadLessons(Lesson.EType.vocabulary);

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
            foreach(object item in setsListbox.Items)
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