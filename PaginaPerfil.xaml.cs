using DobbleGame.Servidor;
using DobbleGame.Utilidades;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.ServiceModel;
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
            lbCorreoElectronico.Content = Dominio.CuentaUsuario.CuentaUsuarioActual.Correo;
            lbNombreUsuario.Content = Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario;
            lbPuntaje.Content = Dominio.CuentaUsuario.CuentaUsuarioActual.Puntaje;
            ConvertirImagenPerfil(Dominio.CuentaUsuario.CuentaUsuarioActual.Foto);
        }

        private void BtnRegresar(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animacionDesvanecimiento = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            animacionDesvanecimiento.Completed += (s, a) =>
            {
                PaginaMenu paginaMenu = new PaginaMenu();
                this.NavigationService.Navigate(paginaMenu);

                AnimarElementos(paginaMenu);
            };
            this.BeginAnimation(Frame.OpacityProperty, animacionDesvanecimiento);
        }

        private void AnimarElementos(PaginaMenu paginaMenu)
        {
            if (paginaMenu.Content is Panel panel)
            {
                foreach (UIElement elemento in panel.Children)
                {
                    elemento.Opacity = 0;

                    DoubleAnimation animacionAparicion = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)))
                    {
                        BeginTime = TimeSpan.FromMilliseconds(200) 
                    };

                    elemento.BeginAnimation(UIElement.OpacityProperty, animacionAparicion);
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
            OpenFileDialog dialogo = new OpenFileDialog();

            dialogo.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            dialogo.Title = "Selecciona una imagen";

            dialogo.ShowDialog();
            string rutaArchivo = dialogo.FileName;

            if (EsArchivoImagen(rutaArchivo) == false)
            {
                MostrarMensaje(Properties.Resources.lb_FormatoInválido);
                return;
            }

            if (!string.IsNullOrEmpty(rutaArchivo) && File.Exists(rutaArchivo))
            {
                FileInfo fileInfo = new FileInfo(rutaArchivo);

                if (fileInfo.Length > 10 * 1024) // 10 KB en bytes
                {
                MostrarMensaje(Properties.Resources.lb_FormatoInválido);
                return;
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(rutaArchivo);
                bitmap.EndInit();

                GuardarFotoPerfil(rutaArchivo, bitmap);                          
            }
        }

        private void GuardarFotoPerfil(String rutaImagen, BitmapImage bitmap)
        {
            var proxyGestionJugador = new Servidor.GestionJugadorClient();
            try
            {
                byte[] foto = File.ReadAllBytes(rutaImagen);

                var respuestaModificarFoto = proxyGestionJugador.ModificarFotoUsuario(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, foto);

                var ventanaPrincipal = Window.GetWindow(this);

                if (respuestaModificarFoto.ErrorBD)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionBD(ventanaPrincipal);
                    return;
                }
                if (respuestaModificarFoto.Resultado)
                {
                    Dominio.CuentaUsuario.CuentaUsuarioActual.Foto = foto;
                    ImagenPerfil.Source = bitmap;
                    _ventanaMenu.ConvertirImagenPerfil(Dominio.CuentaUsuario.CuentaUsuarioActual.Foto);
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionJugador, ex, this);
            }
        }

        private bool EsArchivoImagen(string rutaArchivo)
        {
            try
            {
                using (var imagen = System.Drawing.Image.FromFile(rutaArchivo))
                {
                    return true;
                }
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }

        private void ConvertirImagenPerfil(byte[] fotoBytes)
        {
            if (fotoBytes == null || fotoBytes.Length == 0)
                return;

            using (var ms = new MemoryStream(fotoBytes))
            {
                BitmapImage imagen = new BitmapImage();
                imagen.BeginInit();
                imagen.StreamSource = ms;
                imagen.CacheOption = BitmapCacheOption.OnLoad;
                imagen.EndInit();
                imagen.Freeze();

                ImagenPerfil.Source = imagen;
            }
        }

        public void ActualizarNombreUsuario(string nuevoTexto)
        {
            lbNombreUsuario.Content = nuevoTexto;
        }

        private void MostrarMensaje(string mensaje)
        {
            panelMensaje.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void OcultarDialogo(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource != panelMensaje && panelMensaje.Visibility == Visibility.Visible)
            {
                panelMensaje.Visibility = Visibility.Hidden;
            }
        }
    }
}
