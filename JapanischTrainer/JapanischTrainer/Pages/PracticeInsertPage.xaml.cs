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
using JapanischTrainer.Controller;
using System.Windows.Media;

namespace JapanischTrainer.Pages
{
    public partial class PracticeInsertPage : PhoneApplicationPage
    {
        private bool checkAnswer = false;

        private SolidColorBrush normalBrush;
        private SolidColorBrush correctBrush;
        private SolidColorBrush wrongBrush;

        public PracticeInsertPage()
        {
            InitializeComponent();

            normalBrush  = new SolidColorBrush(Colors.Black);
            correctBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x55, 0xcc, 0x77));
            wrongBrush   = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x22, 0x44));

            UpdateView();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            checkAnswer = false;

            InsertController.GetNextSentence();
            
            UpdateView();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            checkAnswer = true;
            
            UpdateView();
        }

        private void targetWordTextbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                sentenceTextblock.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                Focus();
            }
        }
        
        private void targetWordTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            sentenceTextblock.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
        }

        private void UpdateView()
        {
            if(checkAnswer)
            {
                if(InsertController.CheckAnswer(targetWordTextbox.Text))
                {
                    targetWordTextbox.Foreground = correctBrush;   
                }
                else
                {
                    targetWordTextbox.Foreground = wrongBrush;
                    targetWordTextbox.Text += "（" + InsertData.SentenceAnswer + "）";
                }

                okButton.Visibility = System.Windows.Visibility.Collapsed;
                nextButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                sentenceTextblock.Text = InsertData.SentenceText;

                targetWordTextbox.Foreground = normalBrush;
                targetWordTextbox.Text = "";

                okButton.Visibility = System.Windows.Visibility.Visible;
                nextButton.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}