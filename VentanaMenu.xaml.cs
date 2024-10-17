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

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para VentanaMenu.xaml
    /// </summary>
    public partial class VentanaMenu : Window
    {
        public VentanaMenu()
        {
            InitializeComponent();
            MarcoPrincipal.NavigationService.Navigate(new PaginaMenu());
        }

        private void BtnIrPerfil_Click(object sender, RoutedEventArgs e)
        {
            MarcoPrincipal.NavigationService.Navigate(new PaginaPerfil());
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void GestionUsuario_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSolicitudesAmistad(object sender, RoutedEventArgs e)
        {
            VentanaGestionarSolicitudesAmistad ventanaGestionarSolicitudesAmistad = new VentanaGestionarSolicitudesAmistad();
            ventanaGestionarSolicitudesAmistad.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaGestionarSolicitudesAmistad.ShowDialog();
        }

        private void BtnAgregarAmistad(object sender, RoutedEventArgs e)
        {
            VentanaEnviarSolicitudAmistad ventanaEnviarSolicitudAmistad = new VentanaEnviarSolicitudAmistad();
            ventanaEnviarSolicitudAmistad.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaEnviarSolicitudAmistad.ShowDialog();
        }
    }
}
