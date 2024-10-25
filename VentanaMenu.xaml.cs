using DobbleGame.Servidor;
using Dominio;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            InicializarDatos();
            MarcoPrincipal.NavigationService.Navigate(new PaginaMenu());
        }

        private void InicializarDatos()
        {
            lbNombreUsuario.Content = Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario;
            btnEstadoUsuario.Background = Utilidades.Utilidades.StringABrush("#59B01E");
            lbEstadoUsuario.Content = Properties.Resources.lb_EnLínea;
            ConvertirImagenPerfil(Dominio.CuentaUsuario.cuentaUsuarioActual.Foto);
        }

        private void BtnIrPerfil_Click(object sender, RoutedEventArgs e)
        {
            if(!(MarcoPrincipal.Content is PaginaPerfil))
            {
                DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
                fadeOutAnimation.Completed += (s, a) =>
                {
                    PaginaPerfil paginaPerfil = new PaginaPerfil(this);
                    MarcoPrincipal.Navigate(paginaPerfil);

                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                    MarcoPrincipal.BeginAnimation(Frame.OpacityProperty, fadeInAnimation);
                };
                MarcoPrincipal.BeginAnimation(Frame.OpacityProperty, fadeOutAnimation);
            }
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
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

        private void BtnCambiarEstado_Click(object sender, RoutedEventArgs e)
        {
            btnEstadoUsuario.Background = Utilidades.Utilidades.StringABrush("#F44545");
            lbEstadoUsuario.Content = Properties.Resources.lb_Ausente;
        }

        public void ActualizarNombreUsuario(string nuevoTexto)
        {
            lbNombreUsuario.Content = nuevoTexto;
        }

        public void ConvertirImagenPerfil(byte[] fotoBytes)
        {
            if (fotoBytes == null || fotoBytes.Length == 0)
                return;

            using (var ms = new MemoryStream(fotoBytes))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();

                ImagenPerfil.Source = image;
            }
        }
    }
}
