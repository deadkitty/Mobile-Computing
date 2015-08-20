using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NihongoSenpai.Data;
using NihongoSenpai.Database;

namespace NihongoSenpai.Pages.Controls
{
    public partial class StatisticsItem : UserControl
    {
        int correctWrongRelation = 0;

        public static bool useJapToCompare = false;

        public StatisticsItem()
        {
            InitializeComponent();
        }

        public StatisticsItem(Word word, bool showPercentage, bool showTranslation)
        {
            InitializeComponent();

            Update(word, showPercentage, showTranslation);
        }

        public void HideProgressBar()
        {
            valueGrid.Visibility   = System.Windows.Visibility.Collapsed;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Update(Word word, bool showPercentage, bool showTranslation)
        {
            wordTextblock.Text = word.ToDetailString();

            if((word.CorrectWrongCountTranslation == 0 && !showTranslation) ||
               (word.CorrectWrongCountJapanese    == 0 &&  showTranslation))
            {
                HideProgressBar();
                return;
            }

            if(showTranslation)
            {
                correctWrongRelation = (int)(100.0f * word.CorrectWrongRelationTranslation);

                gerJapTextblock.Text = "Deutsch -> Japanisch";
                correctTextblock.Text = word.correctTranslation.ToString();
                wrongTextblock.Text = word.wrongTranslation.ToString();
            }
            else
            {
                correctWrongRelation = (int)(100.0f * word.CorrectWrongRelationJapanese);

                gerJapTextblock.Text = "Japanisch -> Deutsch";
                correctTextblock.Text = word.correctJapanese.ToString();
                wrongTextblock.Text = word.wrongJapanese.ToString();
            }

            progressBar.Value = correctWrongRelation;

            if (showPercentage)
            {
                correctTextblock.Text = correctWrongRelation + "%";
                wrongTextblock.Text   = 100 - correctWrongRelation + "%";
            }
        }
    }
}
