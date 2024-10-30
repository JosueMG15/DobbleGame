using DobbleGame.Utilidades;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    public partial class PaginaPerfil : Page
    {
        private VentanaMenu _ventanaMenu;
        public PaginaPerfil(VentanaMenu ventanaMenu)
        {
            InitializeComponent();
            InicializarDatos();
            _ventanaMenu = ventanaMenu; 
        }

        private void InicializarDatos()
        {
            lbCorreoElectronico.Content = Dominio.CuentaUsuario.cuentaUsuarioActual.Correo;
            lbNombreUsuario.Content = Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario;
            ConvertirImagenPerfil(Dominio.CuentaUsuario.cuentaUsuarioActual.Foto);
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            fadeOutAnimation.Completed += (s, a) =>
            {
                PaginaMenu paginaMenu = new PaginaMenu();
                this.NavigationService.Navigate(paginaMenu);

                AnimateElementsInPaginaMenu(paginaMenu);
            };
            this.BeginAnimation(Frame.OpacityProperty, fadeOutAnimation);

        }

        private void AnimateElementsInPaginaMenu(PaginaMenu paginaMenu)
        {
            if (paginaMenu.Content is Panel panel)
            {
                foreach (UIElement element in panel.Children)
                {
                    element.Opacity = 0;

                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)))
                    {
                        BeginTime = TimeSpan.FromMilliseconds(200) 
                    };

                    element.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                }
            }
        }

        private void BtnCambiarUsuario(object sender, RoutedEventArgs e)
        {
            VentanaCambioNombre ventanaCambioNombre = new VentanaCambioNombre(this, _ventanaMenu);
            ventanaCambioNombre.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaCambioNombre.ShowDialog();
        }

        private void BtnCambiarContraseña(object sender, RoutedEventArgs e)
        {
            VentanaCambioContraseña ventanaCambioContraseña = new VentanaCambioContraseña();
            ventanaCambioContraseña.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaCambioContraseña.ShowDialog();

        }

        private void BtnActualizarFoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            openFileDialog.Title = "Selecciona una imagen";

            openFileDialog.ShowDialog();
            string selectedFilePath = openFileDialog.FileName;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();

            if (!string.IsNullOrEmpty(selectedFilePath) && File.Exists(selectedFilePath))
            {
                bitmap.UriSource = new Uri(selectedFilePath);
                bitmap.EndInit();

                ImagenPerfil.Source = bitmap;

                Servidor.GestionJugadorClient proxy = new Servidor.GestionJugadorClient();
                byte[] foto = File.ReadAllBytes(selectedFilePath);
                proxy.ModificarFotoUsuario(Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario, foto);
                Dominio.CuentaUsuario.cuentaUsuarioActual.Foto = foto;


            }
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

        public void ActualizarNombreUsuario(string nuevoTexto)
        {
            lbNombreUsuario.Content = nuevoTexto;
        }
    }
}
