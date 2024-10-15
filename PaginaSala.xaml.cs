using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    public partial class PaginaSala : Page
    {
        public PaginaSala()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PaginaMenu());
        }

        private void BtnEnviar_Mnesaje(object sender, RoutedEventArgs e)
        {
            String mensaje = tbChat.Text.Trim();

            if (!string.IsNullOrEmpty(mensaje))
            {
                tbContenedor.Text += $"{mensaje}{Environment.NewLine}";
                tbContenedor.ScrollToEnd();
                tbChat.Text = String.Empty;
            }
        }

        private void TbChat(object sender, TextChangedEventArgs e)
        {

        }

        private void tbContenedor_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
