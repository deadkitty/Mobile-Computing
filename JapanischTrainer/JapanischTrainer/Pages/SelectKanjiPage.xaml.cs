using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using JapanischTrainer.Database;

namespace JapanischTrainer.Pages
{
    public partial class SelectKanjiPage : PhoneApplicationPage
    {
        private int columnCount;
        private int rowCount;
        private int lastRowEmptyFieldsCount;

        //loaded flag to know that the kanjis are already in the grid, otherwise if i come back to this side
        //the page will try to add new grids for the kanjis every time
        private bool loaded = false;

        public SelectKanjiPage()
        {
            InitializeComponent();

            columnCount = LayoutRoot.ColumnDefinitions.Count;

            //for JLPT5 we have 103 kanji so we need 21 rows, the last row has 3 kanjis, without lastRowCount we would just get 20 rows
            //so i add the empty fields of the last row to the Kanjis array length
            lastRowEmptyFieldsCount = columnCount - (AppData.Kanjis.Length % columnCount);
            rowCount = (AppData.Kanjis.Length + lastRowEmptyFieldsCount) / columnCount;

            setNameTextblock.Text = AppData.SelectedLesson.name;
        }
        
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(!loaded)
            {
                //add new rows to the grid
                double rowHeight = LayoutRoot.ActualWidth / columnCount;
                for (int i = 0; i < rowCount; ++i)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength(rowHeight);
                    LayoutRoot.RowDefinitions.Add(rd);
                }

                //add kanji´s to the rows
                for (int i = 0; i < rowCount - 1; ++i)
                {
                    for (int j = 0; j < columnCount; ++j)
                    {
                        Button b = new Button();
                        b.Foreground = new SolidColorBrush(Colors.Black);
                        b.Content = AppData.Kanjis[i * columnCount + j].kanji;
                        b.Click += new RoutedEventHandler(kanjiButton_Click);

                        //i have no need for a tab index in a phone application, so i use it to give
                        //my buttons some more informations like and ID so that i can navigate to the
                        //correct kanji on the DetailKanjiPage
                        b.TabIndex = i * columnCount + j;
                        LayoutRoot.Children.Add(b);
                        Grid.SetColumn(b, j);
                        Grid.SetRow(b, i);
                    }
                }

                //add last row kanjis to the grid
                for (int i = 0; i < columnCount - lastRowEmptyFieldsCount; ++i)
                {
                    Button b = new Button();
                    b.Foreground = new SolidColorBrush(Colors.Black);
                    b.Content = AppData.Kanjis[(rowCount - 1) * columnCount + i].kanji;
                    b.Click += new RoutedEventHandler(kanjiButton_Click);
                    b.TabIndex = (rowCount - 1) * columnCount + i;
                    LayoutRoot.Children.Add(b);
                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, rowCount - 1);
                }

                loaded = true;
            }
        }

        private void kanjiButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            AppData.SelectedKanjiIndex = b.TabIndex;
            NavigationService.Navigate(new Uri("/Pages/KanjiDetailPage.xaml", UriKind.Relative));
        }

        private void resetLessonIcon_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Lektion wirklich zurücksetzen???", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DataManager.ResetKanji(false);
            }
        }
    }
}