using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using JapanischTrainer.Settings;
using Microsoft.Phone.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Activation;
using JapanischTrainer.Database;
using JapanischTrainer.Pages.Controls;

namespace JapanischTrainer.Pages
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Fields

        PhotoChooserTask photoChooser;

        bool updateDatabase = false;
        bool exportDatabase = false;

        #endregion

        #region Constructor

        public MainPage()
            : base()
        {
            InitializeComponent();

            if (AppSettings.BackgroundImage != null)
            {
                LoadBackgroundImage();
            }

            loadAllWordsCheckBox.IsChecked = AppSettings.LoadAllWords;
            partLessonsCheckBox .IsChecked = AppSettings.PartLessons;
            showDescCheckBox    .IsChecked = AppSettings.ShowDescription;

            correctWrongRelationSlider.ValueChanged += correctWrongRelationSlider_ValueChanged;
            minimumCountSlider        .ValueChanged += minimumCountSlider_ValueChanged;
            partLessonsCountSlider    .ValueChanged += partLessonsCountSlider_ValueChanged;

            correctWrongRelationSlider.Value = AppSettings.CorrectWrongRelation * 100.0f;
            minimumCountSlider        .Value = AppSettings.MinimumWordCount;
            partLessonsCountSlider    .Value = AppSettings.PartLessonWordsCount;
        }

        #endregion

        #region Events

        #region General

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            AppSettings.SaveSettings();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            AppSettings.SaveSettings();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App app = App.Current as App;

            if (app.FileOpenPickerContinuationArgs != null && updateDatabase)
            {
                this.ContinueFileOpenPicker(app.FileOpenPickerContinuationArgs);
            }
            if (app.FileSavePickerContinuationArgs != null && exportDatabase)
            {
                this.ContinueFileSavePicker(app.FileSavePickerContinuationArgs);
            }
        }

        #endregion

        #region Exercise Menu

        private void practiceVocabButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectVocabLessonsPage.xaml", UriKind.Relative));
        }

        private void practiceInsertButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectInsertLessonsPage.xaml", UriKind.Relative));
        }

        private void practiceConjuctionButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectConjugationLessonsPage.xaml", UriKind.Relative));
        }

        private void practiceCombineWordsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectCombineWordsPage.xaml", UriKind.Relative));
        }

        private void practiceFlashcardsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectFlashcardLessonsPage.xaml", UriKind.Relative));
        }

        #endregion

        #region Explanation Menu

        private void showWordsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectLessonPage.xaml?pageType=vocab", UriKind.Relative));
        }

        private void showKanjiButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectLessonPage.xaml?pageType=kanji", UriKind.Relative));
        }

        private void showGrammarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ShowGrammarPage.xaml", UriKind.Relative));
        }

        private void searchWordsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SearchWordsPage.xaml", UriKind.Relative));
        }

        #endregion

        #endregion
        
        #region Settings Menu

        #region Checkboxes

        private void loadAllWordsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.LoadAllWords = true;
            correctWrongRelationSlider.IsEnabled = false;
            minimumCountSlider.IsEnabled = false;
        }

        private void loadAllWordsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.LoadAllWords = false;
            correctWrongRelationSlider.IsEnabled = true;
            minimumCountSlider.IsEnabled = true;
        }

        private void partLessonsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.PartLessons = true;
            partLessonsCountSlider.IsEnabled = true;
        }

        private void partLessonsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.PartLessons = false;
            partLessonsCountSlider.IsEnabled = false;
        }

        private void showDescCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.ShowDescription = true;
        }

        private void showDescCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.ShowDescription = false;
        }

        #endregion

        #region Sliders

        private void correctWrongRelationSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            AppSettings.CorrectWrongRelation = (float)e.NewValue / 100.0f;
            correctWrongRelationTextblock.Text = "Richtig/Falsch Relation: " + (int)correctWrongRelationSlider.Value + "%";
        }

        private void minimumCountSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            AppSettings.MinimumWordCount = (int)e.NewValue;
            minimumWordCountTextblock.Text = "Wörter Mindestens Gelernt: " + (int)minimumCountSlider.Value;
        }

        private void partLessonsCountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AppSettings.PartLessonWordsCount = (int)e.NewValue;
            partLessonsCountTextblock.Text = "Wörter pro Teillektion: " + (int)partLessonsCountSlider.Value;
        }

        #endregion

        #region Change Background Image

        private void changeBackgroundButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            photoChooser = new PhotoChooserTask();
            photoChooser.Completed += new EventHandler<PhotoResult>(photoChooser_Completed);
            photoChooser.Show();
        }

        void photoChooser_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                String filename = "bg.png";

                using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isoStorage.FileExists(filename))
                    {
                        isoStorage.DeleteFile(filename);
                    }

                    IsolatedStorageFileStream fs = isoStorage.CreateFile(filename);
                    BitmapImage bmp = new BitmapImage();
                    bmp.SetSource(e.ChosenPhoto);
                    WriteableBitmap wBmp = new WriteableBitmap(bmp);
                    wBmp.SaveJpeg(fs, wBmp.PixelWidth, wBmp.PixelHeight, 0, 100);
                    fs.Close();
                }

                AppSettings.BackgroundImage = filename;
                LoadBackgroundImage();
            }
        }

        private void LoadBackgroundImage()
        {
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                BitmapImage bmp = new BitmapImage();
                IsolatedStorageFileStream fs = isoStorage.OpenFile(AppSettings.BackgroundImage, System.IO.FileMode.Open);
                bmp.SetSource(fs);

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = bmp;
                panoramaMenu.Background = brush;

                fs.Close();
            }
        }

        #endregion

        #region Reset Learnsets

        private void resetLearnsetsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Wirklich?", "Alle Wörter Zurücksetzen", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DataManager.ResetWords(true);
            }
        }

        #endregion

        #region Inport/Export Database

        private void addContentButton_Click(object sender, RoutedEventArgs e)
        {
            updateDatabase = true;

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".txt");
            picker.ContinuationData["Operation"] = "UpdateDatabase";
            picker.PickSingleFileAndContinue();
        }

        private void exportDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            exportDatabase = true;

            FileSavePicker picker = new FileSavePicker();
            picker.ContinuationData["Operation"] = "ExportDatabase";
            picker.FileTypeChoices.Add("TextFile", new List<string>() { ".txt" });
            picker.SuggestedFileName = "NihongoSenpaiExport";
            picker.PickSaveFileAndContinue();
        }

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if ((args.ContinuationData["Operation"] as string) == "UpdateDatabase" &&
                 args.Files != null &&
                 args.Files.Count > 0)
            {
                StorageFile file = args.Files[0];

                if (file.Name.EndsWith("txt"))
                {
                    IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);

                    DataManager.ConnectToLocalStorageDatabase();
                    String updateStatus = DataManager.UpdateDatabase(fileStream);
                    DataManager.CloseConnection();

                    fileStream.Dispose();

                    MessageBox.Show(updateStatus);
                }
            }

            updateDatabase = false;
        }

        public async void ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        {
            if ((args.ContinuationData["Operation"] as string) == "ExportDatabase" && args.File != null)
            {
                StorageFile file = args.File;

                IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);

                DataManager.ConnectToLocalStorageDatabase();
                String exportStatus = DataManager.ExportDatabase(fileStream);
                DataManager.CloseConnection();

                fileStream.Dispose();

                MessageBox.Show(exportStatus);
            }
            else
            {
                MessageBox.Show("Export fehlgeschlagen!");
            }

            exportDatabase = false;
        }

        #endregion

        #endregion
    }
}