using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace prsv02
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            txtDescription.Text = "Ova aplikacija je nastala\nu sklopu projektnog zadatka\nna predmetu:\nParalelni Racunarski Sistemi.";
            txtDescription.Text += "\n\nAutori:\n  Aleksandar Radic\n  Vesna Bjeloglav";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
