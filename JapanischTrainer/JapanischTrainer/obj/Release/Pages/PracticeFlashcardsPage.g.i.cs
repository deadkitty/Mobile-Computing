﻿#pragma checksum "D:\Soi Fon\Visual Studio\Projects\Mobile Computing\JapanischTrainer\JapanischTrainer\Pages\PracticeFlashcardsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C45EBDD20E2C2E2DBAE86150DE1680D9"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34209
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using JapanischTrainer.Pages.Controls;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace JapanischTrainer.Pages {
    
    
    public partial class PracticeFlashcardsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal JapanischTrainer.Pages.Controls.DetailKanjiItem detailKanjiItem;
        
        internal System.Windows.Controls.Button correct1Button;
        
        internal System.Windows.Controls.Button correct2Button;
        
        internal System.Windows.Controls.Button correct3Button;
        
        internal System.Windows.Controls.Button wrong1Button;
        
        internal System.Windows.Controls.Button wrong2Button;
        
        internal System.Windows.Controls.Button wrong3Button;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/JapanischTrainer;component/Pages/PracticeFlashcardsPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.detailKanjiItem = ((JapanischTrainer.Pages.Controls.DetailKanjiItem)(this.FindName("detailKanjiItem")));
            this.correct1Button = ((System.Windows.Controls.Button)(this.FindName("correct1Button")));
            this.correct2Button = ((System.Windows.Controls.Button)(this.FindName("correct2Button")));
            this.correct3Button = ((System.Windows.Controls.Button)(this.FindName("correct3Button")));
            this.wrong1Button = ((System.Windows.Controls.Button)(this.FindName("wrong1Button")));
            this.wrong2Button = ((System.Windows.Controls.Button)(this.FindName("wrong2Button")));
            this.wrong3Button = ((System.Windows.Controls.Button)(this.FindName("wrong3Button")));
        }
    }
}

