using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using JapanischTrainer.Data;
using JapanischTrainer.Database;

namespace JapanischTrainer.Pages.Controls
{
    public partial class StatisticsWordItem : UserControl
    {
        private Word word;

        int correct = 0;
        int correctKanji = 0;

        public StatisticsWordItem()
        {
            InitializeComponent();
        }

        public StatisticsWordItem(Word word, bool showPercentage)
        {
            InitializeComponent();

            this.word = word;

            wordTextblock.Text = word.ToDetailString();

            if ((word.correctTranslation | word.wrongTranslation) == 0)
            {
                kanaGrid.Visibility = System.Windows.Visibility.Collapsed;
                kanaProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                correct = 100 * word.correctTranslation / (word.correctTranslation + word.wrongTranslation);
                kanaProgressBar.Value = correct;

                if (showPercentage)
                {
                    correctKanaTextblock.Text = correct + "%";
                    wrongKanaTextblock.Text = 100 - correct + "%";
                }
                else
                {
                    correctKanaTextblock.Text = word.correctTranslation.ToString();
                    wrongKanaTextblock.Text = word.wrongTranslation.ToString();
                }
            }

            if ((word.correctJapanese | word.wrongJapanese) > 0)
            {
                correctKanji = 100 * word.correctJapanese / (word.correctJapanese + word.wrongJapanese);
                kanjiProgressBar.Value = correctKanji;

                if (showPercentage)
                {
                    correctKanjiTextblock.Text = correctKanji + "%";
                    wrongKanjiTextblock.Text = 100 - correctKanji + "%";
                }
                else
                {
                    correctKanjiTextblock.Text = word.correctJapanese.ToString();
                    wrongKanjiTextblock.Text = word.wrongJapanese.ToString();
                }
            }
            else
            {
                kanjiGrid.Visibility = System.Windows.Visibility.Collapsed;
                kanjiProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void UpdateAfterReset()
        {
            kanaGrid.Visibility = System.Windows.Visibility.Collapsed;
            kanjiGrid.Visibility = System.Windows.Visibility.Collapsed;

            kanaProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            kanjiProgressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Update(bool showPercentage)
        {
            if (showPercentage)
            {
                correctKanaTextblock.Text = correct + "%";
                correctKanjiTextblock.Text = correctKanji + "%";

                wrongKanaTextblock.Text = 100 - correct + "%";
                wrongKanjiTextblock.Text = 100 - correctKanji + "%";
            }
            else
            {
                correctKanaTextblock.Text = word.correctTranslation.ToString();
                correctKanjiTextblock.Text = word.correctJapanese.ToString();

                wrongKanaTextblock.Text = word.wrongTranslation.ToString();
                wrongKanjiTextblock.Text = word.wrongJapanese.ToString();
            }
        }
    }
}
