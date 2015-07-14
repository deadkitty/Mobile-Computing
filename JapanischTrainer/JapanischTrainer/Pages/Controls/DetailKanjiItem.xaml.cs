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
using System.Windows.Media;

namespace JapanischTrainer.Pages.Controls
{
    public partial class DetailKanjiItem : UserControl
    {
        private Kanji kanji;

        private bool kanjiHidden = false;
        private bool onyomiHidden = false;
        private bool kunyomiHidden = false;
        private bool exampleHidden = false;

        public DetailKanjiItem()
        {
            InitializeComponent();

            kanjiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            onyomiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            kunyomiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            exampleTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
        }

        public DetailKanjiItem(Kanji kanji)
        {
            InitializeComponent();

            this.kanji = kanji;

            kanjiTextblock.Text   = kanji.kanji;
            onyomiTextblock.Text  = kanji.onyomi;
            kunyomiTextblock.Text = kanji.kunyomi;
            meaningTextblock.Text = kanji.meaning;
            exampleTextblock.Text = kanji.example;

            kanjiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            onyomiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            kunyomiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            exampleTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void FillKanjiItem(Kanji kanji)
        {
            this.kanji = kanji;

            kanjiTextblock.Text   = kanji.kanji;
            onyomiTextblock.Text  = kanji.onyomi;
            kunyomiTextblock.Text = kanji.kunyomi;
            meaningTextblock.Text = kanji.meaning;
            exampleTextblock.Text = kanji.example;
        }

        public void FillKanjiItem(Kanji kanji, bool hideText)
        {
            FillKanjiItem(kanji);

            kanjiHidden   = hideText;
            onyomiHidden  = hideText;
            kunyomiHidden = hideText;
            exampleHidden = hideText;

            if(hideText)
            {
                kanjiTextblock.Visibility = System.Windows.Visibility.Collapsed;
                onyomiTextblock.Visibility = System.Windows.Visibility.Collapsed;
                kunyomiTextblock.Visibility = System.Windows.Visibility.Collapsed;
                exampleTextblock.Visibility = System.Windows.Visibility.Collapsed;
                
                kanjiTabTextblock.Visibility = System.Windows.Visibility.Visible;
                onyomiTabTextblock.Visibility = System.Windows.Visibility.Visible;
                kunyomiTabTextblock.Visibility = System.Windows.Visibility.Visible;
                exampleTabTextblock.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void kanjiGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            kanjiHidden = !kanjiHidden;

            if(kanjiHidden)
            {
                kanjiTextblock.Visibility = System.Windows.Visibility.Collapsed;
                kanjiTabTextblock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                kanjiTextblock.Visibility = System.Windows.Visibility.Visible;
                kanjiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void onyomiGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            onyomiHidden = !onyomiHidden;

            if (onyomiHidden)
            {
                onyomiTextblock.Visibility = System.Windows.Visibility.Collapsed;
                onyomiTabTextblock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                onyomiTextblock.Visibility = System.Windows.Visibility.Visible;
                onyomiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void kunyomiGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            kunyomiHidden = !kunyomiHidden;

            if (kunyomiHidden)
            {
                kunyomiTextblock.Visibility = System.Windows.Visibility.Collapsed;
                kunyomiTabTextblock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                kunyomiTextblock.Visibility = System.Windows.Visibility.Visible;
                kunyomiTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void exampleGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            exampleHidden = !exampleHidden;

            if (exampleHidden)
            {
                exampleTextblock.Visibility = System.Windows.Visibility.Collapsed;
                exampleTabTextblock.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                exampleTextblock.Visibility = System.Windows.Visibility.Visible;
                exampleTabTextblock.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
