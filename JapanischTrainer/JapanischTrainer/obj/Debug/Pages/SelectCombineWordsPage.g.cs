﻿#pragma checksum "D:\Soi Fon\Visual Studio\Projects\Mobile Computing\JapanischTrainer\JapanischTrainer\Pages\SelectCombineWordsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "09E06E0EC7C3230CD07F2340C901FC88"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34209
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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
    
    
    public partial class SelectCombineWordsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.ListBox setsListbox;
        
        internal System.Windows.Controls.Button loadLessonsButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem selectAll;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem selectNone;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/JapanischTrainer;component/Pages/SelectCombineWordsPage.xaml", System.UriKind.Relative));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.setsListbox = ((System.Windows.Controls.ListBox)(this.FindName("setsListbox")));
            this.loadLessonsButton = ((System.Windows.Controls.Button)(this.FindName("loadLessonsButton")));
            this.selectAll = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("selectAll")));
            this.selectNone = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("selectNone")));
        }
    }
}

