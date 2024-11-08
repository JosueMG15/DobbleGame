using DobbleGame.Servidor;
using Dominio;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            CargarAmistades();
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
            CerrarSesion();
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void CerrarSesion()
        {
            var proxy = new GestionJugadorClient();
            proxy.CerrarSesionJugador(Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario);
        }

        private void BtnSolicitudesAmistad(object sender, RoutedEventArgs e)
        {
            VentanaGestionarSolicitudesAmistad ventanaGestionarSolicitudesAmistad = new VentanaGestionarSolicitudesAmistad(this);
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
            if (Dominio.CuentaUsuario.cuentaUsuarioActual.Estado == true)
            {
                btnEstadoUsuario.Background = Utilidades.Utilidades.StringABrush("#F44545");
                lbEstadoUsuario.Content = Properties.Resources.lb_Ausente;
                Dominio.CuentaUsuario.cuentaUsuarioActual.Estado = false;
            }
            else
            {
                btnEstadoUsuario.Background = Utilidades.Utilidades.StringABrush("#59B01E");
                lbEstadoUsuario.Content = Properties.Resources.lb_EnLínea;
                Dominio.CuentaUsuario.cuentaUsuarioActual.Estado = true;
            }
      
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

        public void CargarAmistades()
        {
            using (var proxy = new Servidor.GestionAmigosClient())
            {
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    var respuesta = proxy.ObtenerAmistades(Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }

                    if (respuesta.Resultado != null && respuesta.Resultado.Length > 0)
                    {
                        // Mapeo de objetos del servicio a la capa de dominio
                        List<Dominio.Amistad> amistades = respuesta.Resultado
                            .Select(amistad => new Dominio.Amistad
                            {
                                IdAmistad = amistad.idAmistad,
                                EstadoSolicitud = amistad.estadoSolicitud,
                                UsuarioPrincipalId = amistad.UsuarioPrincipalId,
                                UsuarioAmigoId = amistad.UsuarioAmigoId
                            })
                            .ToList();

                        // Mostrar las notificaciones
                        foreach (var amistad in amistades)
                        {
                            if(amistad.UsuarioPrincipalId != Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario)
                            {
                                MostrarNotificacionSolicitud(amistad);
                            }
                        }
                    }
                }
                catch (CommunicationObjectFaultedException faultEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error en el objeto de comunicación: {faultEx.Message}");
                }
                catch (CommunicationException commEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error de comunicación: {commEx.Message}");
                }
                catch (TimeoutException timeoutEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error de tiempo de espera: {timeoutEx.Message}");
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }
                finally
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                    }
                    else
                    {
                        proxy.Close();
                    }
                }
            }
        }

        public void CargarAmistad(int idAmistad)
        {
            using (var proxy = new Servidor.GestionAmigosClient())
            {
                try
                {
                    // Verificar si el canal de comunicación está en estado Faulted
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    // Llamar al método del servidor para obtener la amistad por id
                    var respuesta = proxy.ObtenerAmistad(idAmistad);
                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }

                    // Si no hay error, cargar la amistad
                    if (respuesta.Resultado != null)
                    {
                        var amistadDominio = new Dominio.Amistad
                        {
                            IdAmistad = respuesta.Resultado.idAmistad,
                            EstadoSolicitud = respuesta.Resultado.estadoSolicitud,
                            UsuarioPrincipalId = respuesta.Resultado.UsuarioPrincipalId,
                            UsuarioAmigoId = respuesta.Resultado.UsuarioAmigoId
                        };

                        // Crear y mostrar la notificación con el objeto mapeado
                        MostrarNotificacionSolicitud(amistadDominio);

                    }
                }
                catch (CommunicationObjectFaultedException faultEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error en el objeto de comunicación: {faultEx.Message}");
                }
                catch (CommunicationException commEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error de comunicación: {commEx.Message}");
                }
                catch (TimeoutException timeoutEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error de tiempo de espera: {timeoutEx.Message}");
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }
                finally
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                    }
                    else
                    {
                        proxy.Close();
                    }
                }
            }
        }



        private void MostrarNotificacionSolicitud(Dominio.Amistad solicitud)
        {
            var proxy = new Servidor.GestionAmigosClient();
            var respuesta = proxy.ObtenerUsuario(solicitud.UsuarioPrincipalId);
            var cuenta = respuesta.Resultado;

            Dominio.CuentaUsuarioAmigo cuentaUsuarioAmigo = new Dominio.CuentaUsuarioAmigo
            {
                Usuario = cuenta.Usuario,
            };

            var panelSolicitud = new Border
            {
                Background = new SolidColorBrush(Colors.Transparent), 
                Padding = new Thickness(10), 
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(1)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.4, GridUnitType.Star) });

            var nombreUsuario = new TextBlock
            {
                Text = cuentaUsuarioAmigo.Usuario,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetColumn(nombreUsuario, 0);

            grid.Children.Add(nombreUsuario);

            panelSolicitud.Child = grid;

            ContenedorNotificaciones.Children.Add(panelSolicitud);
        }

    }
}
