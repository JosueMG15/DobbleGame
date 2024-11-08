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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para PaginaIngresoCodigo.xaml
    /// </summary>
    public partial class PaginaIngresoCodigo : Page
    {
        VentanaRecuperarContraseña _marcoPrincipal;
        public PaginaIngresoCodigo(VentanaRecuperarContraseña marcoPrincipal)
        {
            InitializeComponent();
            _marcoPrincipal = marcoPrincipal;
        }

        private void BtnAceptar(object sender, RoutedEventArgs e)
        {
            var paginaNuevaContraseña = new PaginaNuevaContraseña(_marcoPrincipal);
            this.NavigationService.Navigate(paginaNuevaContraseña);
        }

        private void BtnReintentar(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            _marcoPrincipal.Close();
        }
    }
}
