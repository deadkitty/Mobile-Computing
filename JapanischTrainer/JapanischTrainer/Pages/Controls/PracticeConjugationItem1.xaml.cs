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
using System.Windows.Input;
using System.Windows.Media;

namespace NihongoSenpai.Pages.Controls
{
    public partial class PracticeConjugationItem1 : UserControl
    {
        private int itemID;
        private PracticeConjugationPage parentPage;

        public TextBox TargetWordTextbox
        {
            get { return targetWordTextbox; }
        }

        public PracticeConjugationItem1()
        {
            InitializeComponent();
        }

        public void Initialize(int itemID, PracticeConjugationPage parentPage)
        {
            this.itemID = itemID;
            this.parentPage = parentPage;
        }

        public void UpdateItem(String sourceWord, String targetForm)
        {
            sourceWordTextblock.Text = sourceWord;
            targetFormTextblock.Text = targetForm;
            targetWordTextbox  .Text = "";
            targetWordTextbox  .Foreground = new SolidColorBrush(Colors.Black);
        }

        private void targetWordTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                parentPage.ChangeFocus(itemID);
            }
        }
    }
}
