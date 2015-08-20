using NihongoSenpai.Database;
using NihongoSenpai.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NihongoSenpai.Resources.Controls
{
    public sealed partial class EditWordItem : UserControl
    {        
        private Word word;

        public IPageUpdater pageUpdater = null;

        public EditWordItem()
        {
            this.InitializeComponent();
        }

        public void FillControl(Word word)
        {        
            //this.word = word;

            //kanaTextbox       .Text = word.kana;
            //kanjiTextbox      .Text = word.kanji == null ? "" : word.kanji;
            //translationTextbox.Text = word.translation;
            //descriptionTextbox.Text = word.description == null ? "" : word.description;
            //showFlagListbox.SelectedIndex = word.showFlags;

            //if (word.type == -1)
            //{
            //    typeListbox.SelectedIndex = (int)Word.EType.other;
            //}
            //else
            //{
            //    typeListbox.SelectedIndex = word.type;
            //}
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            //word.kana        = kanaTextbox.Text;
            //word.kanji       = kanjiTextbox.Text == "" ? null : kanjiTextbox.Text;
            //word.translation = translationTextbox.Text;
            //word.description = descriptionTextbox.Text == "" ? null : descriptionTextbox.Text;
            //word.showFlags   = showFlagListbox.SelectedIndex;
            //word.type        = typeListbox.SelectedIndex;

            //if(pageUpdater != null)
            //{
            //    pageUpdater.UpdatePage();
            //}

            //DataManager.UpdateProgress();
            
            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
