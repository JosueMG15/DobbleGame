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
    /// Lógica de interacción para PaginaRecuperarContraseña.xaml
    /// </summary>
    public partial class PaginaRecuperarContraseña : Page
    {
        VentanaRecuperarContraseña _marcoPrincipal;
        public PaginaRecuperarContraseña(VentanaRecuperarContraseña marcoPrincipal)
        {
            InitializeComponent();
            _marcoPrincipal = marcoPrincipal;   
        }

        private void BtnEnviarCodigo(object sender, RoutedEventArgs e)
        {
            var paginaIngresoCodigo = new PaginaIngresoCodigo(_marcoPrincipal);
            this.NavigationService.Navigate(paginaIngresoCodigo);

        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            _marcoPrincipal.Close();
        }
    }
}
