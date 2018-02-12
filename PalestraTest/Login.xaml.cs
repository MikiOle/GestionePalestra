using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PalestraTest
{
    /// <summary>
    /// Logica di interazione per Window1.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        string pass = "ragno";

        private void btnInvio_Click(object sender, RoutedEventArgs e)
        {
            if (pass == password.Password)
            {
                DialogResult = true;
                this.Close();
            }
        }

        private void btnAnnulla_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
