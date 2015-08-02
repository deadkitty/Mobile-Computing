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

namespace JapanischTrainer.Pages.Controls
{
    public partial class EditWordControl : UserControl
    {
        private Word word;

        public IPageUpdater pageUpdater = null;

        public EditWordControl()
        {
            InitializeComponent();
        }

        public void FillControl(Word word)
        {
            //if(word.original == null)
            //{
            //    this.word = word;
            //}
            //else
            //{
            //    this.word = word.original;
            //}

            this.word = word;

            kanaTextbox.Text = word.kana;
            kanjiTextbox.Text = word.kanji == null ? "" : word.kanji;
            translationTextbox.Text = word.translation;
            descriptionTextbox.Text = word.description == null ? "" : word.description;
            showFlagListbox.SelectedIndex = word.showFlags;

            if (word.type == -1)
            {
                typeListbox.SelectedIndex = (int)Word.EType.other;
            }
            else
            {
                typeListbox.SelectedIndex = word.type;
            }
        }

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Visibility = System.Windows.Visibility.Collapsed;
        }

        private void editButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (word.original == null)
            //{
                word.kana = kanaTextbox.Text;
                word.kanji = kanjiTextbox.Text == "" ? null : kanjiTextbox.Text;
                word.translation = translationTextbox.Text;
                word.description = descriptionTextbox.Text == "" ? null : descriptionTextbox.Text;
                word.showFlags = showFlagListbox.SelectedIndex;
                word.type = typeListbox.SelectedIndex;
            //}
            //else
            //{
            //    word.original.kana = kanaTextbox.Text;
            //    word.original.kanji = kanjiTextbox.Text == "" ? null : kanjiTextbox.Text;
            //    word.original.translation = translationTextbox.Text;
            //    word.original.description = descriptionTextbox.Text == "" ? null : descriptionTextbox.Text;
            //    word.original.showFlags = showFlagListbox.SelectedIndex;
            //    word.original.type = typeListbox.SelectedIndex;
            //}

            if(pageUpdater != null)
            {
                pageUpdater.UpdatePage();
            }

            DataManager.UpdateProgress();
            Visibility = System.Windows.Visibility.Collapsed;
        }
    }

}
