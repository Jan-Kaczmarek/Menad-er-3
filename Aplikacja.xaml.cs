using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Menadżer_3
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Aplikacja : Window
    {
        public Aplikacja()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            string filePath = "C:\\Users\\jan.kaczmarek\\source\\repos\\Menadżer 3\\Dane\\DaneLogowania.txt";
            DataTable dt = new DataTable();

            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length > 0)
            {
                // Pierwsza linia to nagłówki
                string firstLine = lines[0];
                string[] headerLabels = firstLine.Split('\t');

                foreach (string header in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(header));
                }

                // Pozostałe linie to dane
                for (int r = 1; r < lines.Length; r++)
                {
                    string[] items = lines[r].Split('\t');
                    dt.Rows.Add(items);
                }
            }

            // Przypisz DataTable do DataGrid
            dataGrid.ItemsSource = dt.DefaultView;
        }

        private void Nteczka_Click(object sender, RoutedEventArgs e)
        {
            Nowateczka Nowateczka = new Nowateczka();
            Nowateczka.Show();
        }
    }
}