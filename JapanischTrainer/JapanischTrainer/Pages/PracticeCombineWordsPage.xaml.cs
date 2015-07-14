using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using JapanischTrainer.Controller;
using JapanischTrainer.Data;
using System.Windows.Media;

namespace JapanischTrainer.Pages
{
    public partial class PracticeCombineWordsPage : PhoneApplicationPage
    {
        #region Fields

        private List<Button> selectedButtons = new List<Button>();
        private List<Button> correctButtons = new List<Button>();

        private bool showAnswer = false;

        private SolidColorBrush normalBrush;
        private SolidColorBrush selectedBrush;
        private SolidColorBrush correctBrush;
        private SolidColorBrush correctAnswerBrush;
        private SolidColorBrush wrongBrush;
        
        #endregion

        #region Constructor

        public PracticeCombineWordsPage()
        {
            InitializeComponent();

            CombineWordsController.GetNext(combineButtonsGrid.Children.Count);

            normalBrush        = new SolidColorBrush(Colors.White);
            selectedBrush      = new SolidColorBrush(Color.FromArgb(0xff, 0x33, 0x66, 0xcc));
            correctBrush       = new SolidColorBrush(Color.FromArgb(0xff, 0x55, 0xcc, 0x77));
            correctAnswerBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x77, 0xff, 0xaa));
            wrongBrush         = new SolidColorBrush(Color.FromArgb(0xff, 0xdd, 0x55, 0x33));

            UpdateView();
        }
        
        #endregion

        #region Events

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            okButton.Visibility = System.Windows.Visibility.Visible;
            clearButton.Visibility = System.Windows.Visibility.Visible;
            backspaceButton.Visibility = System.Windows.Visibility.Visible;

            nextButton.Visibility = System.Windows.Visibility.Collapsed;

            CombineWordsController.GetNext(combineButtonsGrid.Children.Count);

            foreach (Button b in selectedButtons)
            {
                b.Background = normalBrush;
            }

            foreach(Button b in correctButtons)
            {
                b.Background = normalBrush;
            }

            selectedButtons.Clear();
            correctButtons.Clear();

            showAnswer = false;

            UpdateView();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            okButton.Visibility = System.Windows.Visibility.Collapsed;
            clearButton.Visibility = System.Windows.Visibility.Collapsed;
            backspaceButton.Visibility = System.Windows.Visibility.Collapsed;

            nextButton.Visibility = System.Windows.Visibility.Visible;

            CombineWordsController.CheckAnswer(targetWordTextbox.Text);

            showAnswer = true;

            UpdateView();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            targetWordTextbox.Text = "";

            foreach(Button b in selectedButtons)
            {
                b.Background = new SolidColorBrush(Colors.White);
            }

            selectedButtons.Clear();
        }

        private void backspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (targetWordTextbox.Text.Length > 0)
            {
                targetWordTextbox.Text = targetWordTextbox.Text.Substring(0, targetWordTextbox.Text.Length - 1);
                selectedButtons.Last().Background = normalBrush;
                selectedButtons.Remove(selectedButtons.Last());
                //selectedButtons.RemoveAt(selectedButtons.Count - 1);
            }
        }

        private void combineButton_Click(object sender, RoutedEventArgs e)
        {
            Button sourceButton = sender as Button;

            sourceButton.Background = selectedBrush;

            selectedButtons.Add(sourceButton);

            //int buttonID = Convert.ToInt32(sourceButton.Name.Substring(1, 2));

            //int row = buttonID / 10;
            //int column = buttonID - 10 * row;

            targetWordTextbox.Text += sourceButton.Content;
        }

        #endregion

        #region Public Methods

        private void UpdateView()
        {
            if(showAnswer)
            {
                String answer = targetWordTextbox.Text;
                String correctAnswer = CombineWordsData.ActiveWord.ToJString().Split('、')[0];

                int minLength = Math.Min(answer.Length, correctAnswer.Length);
                int maxLength = Math.Max(answer.Length, correctAnswer.Length);

                targetWordTextbox.Text = "";

                //at first i go over each button and print the buttons for the correct answer in green
                foreach(Button b in combineButtonsGrid.Children)
                {
                    String content = b.Content.ToString();
                    if(correctAnswer.Contains(content[0]))
                    {
                        b.Background = correctAnswerBrush;
                        correctButtons.Add(b);
                    }
                }

                //after that i go other the buttons the user selected
                //the buttons in the list and the answers in data should match
                for(int i = 0; i < minLength; ++i)
                {
                    //if i correct answered it i draw the button with a darker green
                    if(CombineWordsData.Answers[i])
                    {
                        targetWordTextbox.Text += answer[i];
                        selectedButtons[i].Background = correctBrush;
                    }
                    else //otherwise i draw the button in read and add the correct answer in brackets behind the sign
                    {
                        targetWordTextbox.Text += answer[i] + "(" + correctAnswer[i] + ")";
                        selectedButtons[i].Background = wrongBrush;
                    }
                }

                //if i select more buttons than the word has make the last buttons
                //in the list red
                if(selectedButtons.Count > minLength)
                {
                    for (int i = minLength; i < maxLength; ++i)
                    {
                        targetWordTextbox.Text += answer[i];
                        selectedButtons[i].Background = wrongBrush;
                    }

                    //TODO: think about what i can do users typed in word is to long
                    //i can't print the last part in red thats the problem =/
                    targetWordTextbox.Text += "()";
                }
                //if the word length is to short write the rest of the answer in brackets behind the word
                else if (selectedButtons.Count < maxLength)
                {
                    targetWordTextbox.Text += "(";

                    for (int i = minLength; i < maxLength; ++i)
                    {
                        targetWordTextbox.Text += correctAnswer[i];
                    }
                    
                    targetWordTextbox.Text += ")";
                }
            }
            else
            {
                targetWordTextbox.Text = "";
                sourceWordTextblock.Text = CombineWordsData.ActiveWord.translation;

                if(CombineWordsData.ActiveWord.showFlags == 1 ||CombineWordsData.ActiveWord.showFlags == 3)
                {
                    descriptionTextblock.Text = CombineWordsData.ActiveWord.ToDescriptionString();
                }

                for (int i = 0; i < combineButtonsGrid.Children.Count; ++i )
                {
                    (combineButtonsGrid.Children[i] as Button).Content = CombineWordsData.CurrentSigns[i];
                }
            }
        }
        
        #endregion
    }
}