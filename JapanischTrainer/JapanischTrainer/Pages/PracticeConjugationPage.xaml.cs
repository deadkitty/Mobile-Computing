using System;
using System.Linq;
using System.Windows;
using NihongoSenpai.Pages.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using NihongoSenpai.Data;
using NihongoSenpai.Controller;

namespace NihongoSenpai.Pages
{
    public partial class PracticeConjugationPage : PhoneApplicationPage
    {   
        #region Fields
	    
        PracticeConjugationItem1[] conjugationItems;
 
	    #endregion

        #region Constructor

        public PracticeConjugationPage()
        {
            InitializeComponent();

            conjugationItems = new PracticeConjugationItem1[ConjugationData.maxActiveConjugationWordsCount];

            for (int i = 0; i < conjugationItems.Length; ++i)
            {
                conjugationItems[i] = new PracticeConjugationItem1();
                conjugationItems[i].Initialize(i, this);

                itemsStackPanel.Children.Insert(i, conjugationItems[i]);
            }

            ConjugationController.GetNextWords();

            UpdateView();
        }
        
        #endregion

        #region Events

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            List<String> answers = new List<String>();

            for (int i = 0; i < ConjugationData.ActiveWords.Length; ++i)
            {
                if (conjugationItems[i].Visibility == System.Windows.Visibility.Visible)
                {
                    answers.Add(conjugationItems[i].targetWordTextbox.Text);
                }
            }

            bool[] correctAnswered = ConjugationController.CheckWords(answers.ToArray());

            for (int i = 0; i < correctAnswered.Length; ++i)
            {
                if (!correctAnswered[i])
                {
                    conjugationItems[i].targetWordTextbox.Foreground = new SolidColorBrush(Colors.Red);
                    conjugationItems[i].targetWordTextbox.Text += "（" + ConjugationData.TargetWords[i] + "）";
                }
                else
                {
                    conjugationItems[i].targetWordTextbox.Foreground = new SolidColorBrush(Colors.Green);
                }
            }

            okButton.Visibility = System.Windows.Visibility.Collapsed;
            nextButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            ConjugationController.GetNextWords();
            
            okButton.Visibility = System.Windows.Visibility.Visible;
            nextButton.Visibility = System.Windows.Visibility.Collapsed;

            UpdateView();
        }

        public void UpdateView()
        {
            for (int i = 0; i < ConjugationData.ActiveWords.Length; ++i)
            {
                if (ConjugationData.ActiveWords[i] == null)
                {
                    conjugationItems[i].Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    conjugationItems[i].Visibility = System.Windows.Visibility.Visible;
                    conjugationItems[i].UpdateItem(ConjugationData.ActiveWords[i].ToJString(), Util.TargetFormToString(ConjugationData.TargetForms[i]));
                }
            }
        }
        
        public void ChangeFocus(int itemID)
        {
            if(itemID + 1 == ConjugationData.maxActiveConjugationWordsCount)
            {
                this.Focus();
            }
            else
            {
                conjugationItems[itemID + 1].Focus();
            }
        }

        #endregion
    }
}