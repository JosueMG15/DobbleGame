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
    /// Lógica de interacción para PaginaNuevaContraseña.xaml
    /// </summary>
    public partial class PaginaNuevaContraseña : Page
    {
        VentanaRecuperarContraseña _marcoPrincipal;
        public PaginaNuevaContraseña(VentanaRecuperarContraseña marcoPrincipal)
        {
            InitializeComponent();
            _marcoPrincipal = marcoPrincipal;
        }

        private void BtnActualizar(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            _marcoPrincipal.Close();

        }
    }
}

