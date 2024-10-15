using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
    public partial class PaginaSala : Page, ServidorDobble.IGestionSalaCallback
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
                InstanceContext contexto = new InstanceContext(this);
                ServidorDobble.GestionSalaClient proxy = new ServidorDobble.GestionSalaClient(contexto);
                proxy.EnviarMensajeSala(mensaje);
                tbChat.Text = String.Empty;
            }
        }

        private void tbContenedor_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbChat_GotFocus(object sender, RoutedEventArgs e)
        {
            tbContenedor.Visibility = Visibility.Visible;
        }

        public void SalaResponse(string respuesta)
        {
            tbContenedor.Text += $"{respuesta}{Environment.NewLine}";
            tbContenedor.ScrollToEnd();
            
        }
    }
}
