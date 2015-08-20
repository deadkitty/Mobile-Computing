using NihongoSenpai.Common;
using NihongoSenpai.Database;
using NihongoSenpai.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace NihongoSenpai.Pages
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : BasePage
    {
        #region Fields

        //PhotoChooserTask photoChooser;

        bool updateDatabase = false;
        bool exportDatabase = false;
        bool choosePhoto = false;

        #endregion

        #region Constructor

        public MainPage()
            : base()
        {
            this.InitializeComponent();
                        
            CoreApplicationView view = CoreApplication.GetCurrentView();
            view.Activated += ViewActivated;

            if (AppSettings.BackgroundImage != null)
            {
                LoadBackgroundImage();
            }
        }
        
        #endregion

        #region General
        
        private void ViewActivated(CoreApplicationView sender, IActivatedEventArgs args)
        {
            if(updateDatabase)
            {
                ContinueFileOpenPicker(args as FileOpenPickerContinuationEventArgs);
            }
            else if(exportDatabase)
            {
                ContinueFileSavePicker(args as FileSavePickerContinuationEventArgs);
            }
            else if(choosePhoto)
            {
                ContinueFileOpenPicker(args as FileOpenPickerContinuationEventArgs);
            }
        }

        public void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if ((args.ContinuationData["Operation"] as string) == "UpdateDatabase"
                && args.Files != null
                && args.Files.Count > 0)
            {
                ImportFromFile(args.Files[0]);
            }
            else if ((args.ContinuationData["Operation"] as string) == "ChooseBackgroundImage"
                && args.Files != null
                && args.Files.Count > 0)
            {
                ChooseBackgroundImage(args.Files[0]);
            }
        }
        
        public async void ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        {
            if ((args.ContinuationData["Operation"] as string) == "ExportDatabase" && args.File != null)
            {
                ExportToFile(args.File);
            }
            else
            {
                MessageDialog msg = new MessageDialog("Export fehlgeschlagen!");

                await msg.ShowAsync();
            }
        }

        #endregion

        #region Exercise Menu

        private void practiceVocabButton_Click(object sender, RoutedEventArgs e)
        {
            ////NavigationService.Navigate(new Uri("/Pages/SelectVocabLessonsPage.xaml", UriKind.Relative));
        }

        private void practiceInsertButton_Click(object sender, RoutedEventArgs e)
        {
            ////NavigationService.Navigate(new Uri("/Pages/SelectInsertLessonsPage.xaml", UriKind.Relative));
        }

        private void practiceConjuctionButton_Click(object sender, RoutedEventArgs e)
        {
            ////NavigationService.Navigate(new Uri("/Pages/SelectConjugationLessonsPage.xaml", UriKind.Relative));
        }

        private void practiceCombineWordsButton_Click(object sender, RoutedEventArgs e)
        {
            ////NavigationService.Navigate(new Uri("/Pages/SelectCombineWordsPage.xaml", UriKind.Relative));
        }

        private void practiceFlashcardsButton_Click(object sender, RoutedEventArgs e)
        {
            ////NavigationService.Navigate(new Uri("/Pages/SelectFlashcardLessonsPage.xaml", UriKind.Relative));
        }

        #endregion

        #region Explanation Menu

        private void showWordsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SelectLessonPage), "vocab");
        }

        private void showKanjiButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SelectLessonPage), "kanji");
        }

        private void showGrammarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowGrammarPage));
        }

        private void searchWordsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchWordsPage));
        }

        #endregion

        #region Others Menu

        #region Import/Export

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            updateDatabase = true;

            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");
            picker.ContinuationData["Operation"] = "UpdateDatabase";
            picker.PickSingleFileAndContinue();
        }

        private async void ImportFromFile(StorageFile file)
        {
            IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);

            DataManager.ConnectToDatabase();
            String updateStatus = DataManager.ImportFromFile(fileStream);
            DataManager.CloseConnection();

            fileStream.Dispose();

            MessageDialog msg = new MessageDialog(updateStatus);

            await msg.ShowAsync();
            
            updateDatabase = false;
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            exportDatabase = true;

            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ContinuationData["Operation"] = "ExportDatabase";
            picker.FileTypeChoices.Add("TextFile", new List<string>() { ".txt" });

            String time = DateTime.Now.ToString();
            time = time.Replace(' ', '_');
            time = time.Replace(":", "");
            time = time.Replace(".", "");

            String fileName = "NihongoSenpaiExport_" + time;

            picker.SuggestedFileName = fileName;
            picker.PickSaveFileAndContinue();
        }
        
        private async void ExportToFile(StorageFile file)
        {
            IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);

            DataManager.ConnectToDatabase();
            String exportStatus = DataManager.ExportToFile(fileStream);
            DataManager.CloseConnection();

            fileStream.Dispose();

            MessageDialog msg = new MessageDialog(exportStatus);

            await msg.ShowAsync();

            exportDatabase = false;
        }

        #endregion

        #region Reset Senpai

        private async void resetAllButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msg = new MessageDialog("Alle Wörter Zurücksetzen", "Wirklich?");

            msg.Commands.Add(new UICommand("Ok", ResetWords));
            msg.Commands.Add(new UICommand("Abbrechen"));

            await msg.ShowAsync();
        }

        private void ResetWords(IUICommand command)
        {
            //DataManager.ResetWords(true);
        }
        
        #endregion

        #region Choose Background Image

        private void changeBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            choosePhoto = true;

            FileOpenPicker photoPicker = new FileOpenPicker();
            photoPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            photoPicker.ViewMode = PickerViewMode.Thumbnail;
            photoPicker.FileTypeFilter.Add(".png");
            photoPicker.FileTypeFilter.Add(".jpg");
            photoPicker.FileTypeFilter.Add(".jpeg");
            photoPicker.ContinuationData["Operation"] = "ChooseBackgroundImage";
            photoPicker.PickSingleFileAndContinue();
        }

        private async void ChooseBackgroundImage(StorageFile file)
        {
            String filename = "menuBackground.png";
            
            await file.CopyAsync(ApplicationData.Current.LocalFolder as IStorageFolder, filename, NameCollisionOption.ReplaceExisting);

            AppSettings.BackgroundImage = filename;

            LoadBackgroundImage();
        }

        private async void LoadBackgroundImage()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(AppSettings.BackgroundImage);
            
            using(IRandomAccessStream fs = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bmp = new BitmapImage();
                
                bmp.SetSource(fs);
            
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = bmp;
                hubMenu.Background = brush;
            }

            choosePhoto = false;
        }
        
        #endregion

        #endregion
    }
}
