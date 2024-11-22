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

            if (!string.IsNullOrEmpty(selectedFilePath) && File.Exists(selectedFilePath))
            {
                FileInfo fileInfo = new FileInfo(selectedFilePath);

                if (fileInfo.Length > 10 * 1024) // 10 KB en bytes
                {
                MostrarMensaje(Properties.Resources.lb_FormatoInválido);
                return;
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFilePath);
                bitmap.EndInit();

                guardarFotoPerfil(selectedFilePath, bitmap);                          
            }
        }

        private void guardarFotoPerfil(String rutaImagen, BitmapImage bitmap)
        {
            using (var proxy = new Servidor.GestionJugadorClient())
            {
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    byte[] foto = File.ReadAllBytes(rutaImagen);
                    byte[] fotoRedimencionada = RedimensionarImagen(foto, 800, 600);
                    var respuestaModificarFoto = proxy.ModificarFotoUsuario(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, fotoRedimencionada);

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
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
        }

        private byte[] RedimensionarImagen(byte[] datosImagen, int anchoMaximo, int altoMaximo)
        {
            using (MemoryStream memoriaEntrada = new MemoryStream(datosImagen))
            {
                System.Drawing.Image imagenOriginal = System.Drawing.Image.FromStream(memoriaEntrada);
                int nuevoAncho = imagenOriginal.Width;
                int nuevoAlto = imagenOriginal.Height;

                if (imagenOriginal.Width > anchoMaximo || imagenOriginal.Height > altoMaximo)
                {
                    float proporcionAncho = (float)anchoMaximo / imagenOriginal.Width;
                    float proporcionAlto = (float)altoMaximo / imagenOriginal.Height;
                    float proporcion = Math.Min(proporcionAncho, proporcionAlto); 

                    nuevoAncho = (int)(imagenOriginal.Width * proporcion);
                    nuevoAlto = (int)(imagenOriginal.Height * proporcion);
                }

                Bitmap imagenRedimensionada = new Bitmap(imagenOriginal, new System.Drawing.Size(nuevoAncho, nuevoAlto));

                using (MemoryStream memoriaSalida = new MemoryStream())
                {
                    imagenRedimensionada.Save(memoriaSalida, ImageFormat.Jpeg);

                    return memoriaSalida.ToArray();
                }
            }
        }

        private void ConvertirImagenPerfil(byte[] fotoBytes)
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

        private void MostrarMensaje(string mensaje)
        {
            panelMensaje.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void Window_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource != panelMensaje && panelMensaje.Visibility == Visibility.Visible)
            {
                panelMensaje.Visibility = Visibility.Hidden;
            }
        }
    }
}
