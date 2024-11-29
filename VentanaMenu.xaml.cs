using DobbleGame.Servidor;
using DobbleGame.Utilidades;
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
            MarcoPrincipal.NavigationService.Navigate(new PaginaMenu());
        }

        private void InicializarDatos()
        {
            this.Closing += VentanaMenu_Closing;
            lbNombreUsuario.Content = Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario;
            btnEstadoUsuario.Background = Utilidades.Utilidades.StringABrush("#59B01E");
            lbEstadoUsuario.Content = Properties.Resources.lb_EnLínea;
            ConvertirImagenPerfil(Dominio.CuentaUsuario.CuentaUsuarioActual.Foto);
            CargarAmistades();
            CambiarBotonInvitar(ContenedorNotificaciones);

            CallbackManager.Instance.NotificarCambioEvent += NotificarCambio;
            CallbackManager.Instance.NotificarSalidaEvent += NotificarSalida;
            CallbackManager.Instance.NotificarInvitacionActivaEvent += NotificarInvitacionActiva;
            CallbackManager.Instance.NotificarVentanaInvitacionEvent += NotificarVentanaInvitacion;
        }


        public void NotificarCambio()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    ContenedorNotificaciones.Children.Clear();
                    CargarAmistades();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void NotificarSalida(string nombreUsuario)
        {
            try
            {
                foreach (var child in ContenedorNotificaciones.Children)
                {
                    if (child is Border border && border.Child is Grid grid)
                    {
                        foreach (var gridChild in grid.Children)
                        {
                            if (gridChild is StackPanel stackPanel)
                            {
                                foreach (var stackChild in stackPanel.Children)
                                {
                                    if (stackChild is TextBlock textBlock && textBlock.Text == nombreUsuario)
                                    {
                                        CambiarEstadoAAusente(stackPanel);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void NotificarInvitacionActiva(string nombreUsuario)
        {

        }

        public void NotificarVentanaInvitacion(string nombreUsuarioInvitacion)
        {
            try
            {
                bool sePuedeEnviar = true;

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow || window is VentanaPartida)
                    {
                        sePuedeEnviar = false;
                    }
                }

            if (sePuedeEnviar == true)
            {
                string mensaje = String.Format(Properties.Resources.lb_TeEstaInvitando_, nombreUsuarioInvitacion);
                VentanaModalDecision ventanaModalDecision = new VentanaModalDecision(mensaje);
                ventanaModalDecision.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ventanaModalDecision.ShowDialog();
            }
        }

        private void CambiarEstadoAAusente(StackPanel stackPanel)
        {
            foreach (var child in stackPanel.Children)
            {
                if (child is StackPanel estadoPanel)
                {
                    foreach (var estadoChild in estadoPanel.Children)
                    {
                        if (estadoChild is Ellipse circulo)
                        {
                            circulo.Fill = Brushes.Red;
                        }
                        else if (estadoChild is Label estadoTexto)
                        {
                            estadoTexto.Content = Properties.Resources.lb_Ausente;
                        }
                    }
                }
            }
        }

        private void CambiarBotonInvitar(StackPanel stackPanel)
        {
        }

        private void BtnIrPerfil_Click(object sender, RoutedEventArgs e)
        {
            if (MarcoPrincipal.Content is PaginaSala paginasala)
            {
                VentanaModalDecision ventanaModalDecision = new VentanaModalDecision(Properties.Resources.lb_ConfirmarIrPerfil);
                bool? respuesta = ventanaModalDecision.ShowDialog();

                if (respuesta == true)
                {
                    paginasala.AbandonarSala();
                    IrPaginaPerfil();
                }
                else
                {
                    return;
                }
            }

            if (!(MarcoPrincipal.Content is PaginaPerfil))
            {
                IrPaginaPerfil();
            }
        }

        private void IrPaginaPerfil()
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

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            VentanaModalDecision ventana = new VentanaModalDecision(Properties.Resources.lb_MensajeCerrarSesion,
                Properties.Resources.btn_CerrarSesión, Properties.Resources.global_Cancelar);
            bool? respuesta = ventana.ShowDialog();

            if (respuesta == true)
            {
                CerrarSesion();

                App.Current.Dispatcher.Invoke(() =>
                {
                    ((App)Application.Current).DetenerPing();
                });

                MainWindow mainWindow = new MainWindow();
                this.Close();
                mainWindow.Show();
            }
        }

        private void VentanaMenu_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var proxyUsuario = new Servidor.GestionAmigosClient();
            var proxy = new GestionJugadorClient();
            try
            {
                if (!string.IsNullOrEmpty(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario))
                {
                    proxy.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                    CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    proxyUsuario.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
            }
        }

        private void CerrarSesion()
        {
            var proxy = new GestionJugadorClient();
            var proxyUsuario = new GestionAmigosClient();
            try
            {
                proxy.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                proxyUsuario.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                Utilidades.Utilidades.ManejarExcepciones(proxyUsuario, ex, this);   
            }
        }

        private void BtnSolicitudesAmistad(object sender, RoutedEventArgs e)
        {
            var proxy = new GestionAmigosClient();
            var respuesta = proxy.ObtenerSolicitudesPendientes(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario);

            if (respuesta.ErrorBD)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                return;
            }
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
            if (Dominio.CuentaUsuario.CuentaUsuarioActual.Estado == true)
            {
                btnEstadoUsuario.Background = Utilidades.Utilidades.StringABrush("#F44545");
                lbEstadoUsuario.Content = Properties.Resources.lb_Ausente;
                Dominio.CuentaUsuario.CuentaUsuarioActual.Estado = false;
            }
            else
            {
                btnEstadoUsuario.Background = Utilidades.Utilidades.StringABrush("#59B01E");
                lbEstadoUsuario.Content = Properties.Resources.lb_EnLínea;
                Dominio.CuentaUsuario.CuentaUsuarioActual.Estado = true;
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
            var proxy = new Servidor.GestionAmigosClient();
            try
            {
                if (proxy.State == CommunicationState.Faulted)
                {
                    proxy.Abort();
                    throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                }

                var respuesta = proxy.ObtenerAmistades(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario);

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
                            if (amistad.UsuarioPrincipalId != Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario)
                            {
                                MostrarAmigo(amistad, true);
                            }
                            else
                            {
                                MostrarAmigo(amistad, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
        }

        private void MostrarAmigo(Dominio.Amistad solicitud, bool esAgeno)
        {
            var proxy = new Servidor.GestionAmigosClient();
            try
            {
                Dominio.CuentaUsuarioAmigo cuentaUsuarioAmigo = new Dominio.CuentaUsuarioAmigo
                {
                    Usuario = UsuarioAmigo(solicitud, esAgeno).Usuario,
                    Puntaje = UsuarioAmigo(solicitud, esAgeno).Puntaje,
                    Foto = UsuarioAmigo(solicitud, esAgeno).Foto
                };

                var panelSolicitud = new Border
                {
                    Background = new SolidColorBrush(Colors.Transparent),
                    Padding = new Thickness(10),
                    BorderBrush = new SolidColorBrush(Colors.LightGray),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(5)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100, GridUnitType.Pixel) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100, GridUnitType.Pixel) });

                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // Imagen circular del usuario
                var fotoUsuario = new Image
                {
                    Width = 60,
                    Height = 60,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(0, 0, 15, 0),
                    Clip = new EllipseGeometry { Center = new Point(30, 30), RadiusX = 30, RadiusY = 30 }
                };
                byte[] fotoBytes = cuentaUsuarioAmigo.Foto;
                if (fotoBytes != null)
                {
                    fotoUsuario.Source = ConvertirBytesAImagen(fotoBytes);
                }
                Grid.SetColumn(fotoUsuario, 0);
                Grid.SetRowSpan(fotoUsuario, 2);
                grid.Children.Add(fotoUsuario);

                // Nombre de usuario y Estado 
                var stackNombreEstado = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var nombreUsuario = new TextBlock
                {
                    Text = cuentaUsuarioAmigo.Usuario,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold
                };
                stackNombreEstado.Children.Add(nombreUsuario);

                var estado = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                var respuesta = proxy.UsuarioConectado(cuentaUsuarioAmigo.Usuario);
                if (proxy.State == CommunicationState.Faulted)
                {
                    proxy.Abort();
                    throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                }
                if (respuesta.ErrorBD)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                    return;
                }

                //Si esta en línea
                if (respuesta.Resultado)
                {
                    var circulo = new Ellipse
                    {
                        Width = 15,
                        Height = 15,
                        Fill = Brushes.LightGreen,
                        Margin = new Thickness(0, 0, 5, 0)
                    };
                    estado.Children.Add(circulo);

                    var textoEnLinea = new Label
                    {
                        Content = Properties.Resources.lb_EnLínea,
                        FontSize = 12
                    };
                    estado.Children.Add(textoEnLinea);
                }
                //Si esta ausente
                else
                {
                    var circulo = new Ellipse
                    {
                        Width = 15,
                        Height = 15,
                        Fill = Brushes.Red,
                        Margin = new Thickness(0, 0, 5, 0)
                    };
                    estado.Children.Add(circulo);

                    var textoEnLinea = new Label
                    {
                        Content = Properties.Resources.lb_Ausente,
                        FontSize = 12
                    };
                    estado.Children.Add(textoEnLinea);
                }

                stackNombreEstado.Children.Add(estado);
                Grid.SetColumn(stackNombreEstado, 1);
                Grid.SetRow(stackNombreEstado, 0);
                grid.Children.Add(stackNombreEstado);

                // Puntos del usuario
                var puntosUsuario = new TextBlock
                {
                    Text = $"Puntos: {cuentaUsuarioAmigo.Puntaje}",
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                Grid.SetColumn(puntosUsuario, 1);
                Grid.SetRow(puntosUsuario, 1);
                grid.Children.Add(puntosUsuario);

                // Botones de "EliminarAmistad" e "Invitar"
                var botonesPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var botonEliminar = new Button
                {
                    Background = Brushes.Transparent,
                    Foreground = Brushes.Black,
                    BorderBrush = Brushes.Transparent,
                    Padding = new Thickness(5),
                    Margin = new Thickness(0, 0, 5, 0)
                };
                var imagenEliminar = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Imagenes/BotonEliminarAmigo.png")),
                    Width = 30,
                    Height = 30
                };
                botonEliminar.Content = imagenEliminar;
                botonEliminar.Click += (s, e) => EliminarAmistad(solicitud, panelSolicitud);
                botonesPanel.Children.Add(botonEliminar);

                var botonInvitar = new Button
                {
                    Content = "Invitar",
                    Background = Brushes.Gray,
                    Foreground = Brushes.White,
                    Padding = new Thickness(5)
                };

                if (respuesta.Resultado)
                {
                    //if(validarQueEstaEnSala)
                    botonInvitar.Click += (s, e) => InvitarAmistad(solicitud);
                }

                botonesPanel.Children.Add(botonInvitar);

                Grid.SetColumn(botonesPanel, 2);
                Grid.SetRowSpan(botonesPanel, 2);
                grid.Children.Add(botonesPanel);

                panelSolicitud.Child = grid;

                // Añadir la notificación al StackPanel de notificaciones principal
                ContenedorNotificaciones.Children.Add(panelSolicitud);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
            }
        }

        private DobbleGame.Servidor.CuentaUsuario UsuarioAmigo(Dominio.Amistad solicitud, bool esAgeno)
        {
            var proxy = new Servidor.GestionAmigosClient();
            if (esAgeno == true)
            {
                var respuesta = proxy.ObtenerUsuario(solicitud.UsuarioPrincipalId);
                var cuenta = respuesta.Resultado;
                return cuenta;
            }
            else
            {
                var respuesta = proxy.ObtenerUsuario(solicitud.UsuarioAmigoId);
                var cuenta = respuesta.Resultado;
                return cuenta;
            }
        }

        private ImageSource ConvertirBytesAImagen(byte[] fotoBytes)
        {
            if (fotoBytes == null || fotoBytes.Length == 0)
                return null;

            using (var stream = new MemoryStream(fotoBytes))
            {
                var imagen = new BitmapImage();
                imagen.BeginInit();
                imagen.CacheOption = BitmapCacheOption.OnLoad;
                imagen.StreamSource = stream;
                imagen.EndInit();
                return imagen;
            }
        }

        private void EliminarAmistad(Dominio.Amistad amistad, Border panelSolicitud)
        {
            VentanaEliminarAmigo ventanaEliminarAmigo = new VentanaEliminarAmigo(this, amistad, panelSolicitud);
            ventanaEliminarAmigo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaEliminarAmigo.ShowDialog();
        }

        private void InvitarAmistad(Dominio.Amistad solicitud)
        {
            var proxy = new Servidor.GestionAmigosClient();
            try
            {
                string nombreUsuario;

                Dominio.CuentaUsuarioAmigo cuentaUsuarioPrincipal = new Dominio.CuentaUsuarioAmigo
                {
                    Usuario = UsuarioAmigo(solicitud, true).Usuario,
                };
                Dominio.CuentaUsuarioAmigo cuentaUsuarioAmigo = new Dominio.CuentaUsuarioAmigo
                {
                    Usuario = UsuarioAmigo(solicitud, false).Usuario,
                };

                if (Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario != cuentaUsuarioPrincipal.Usuario)
                {
                    nombreUsuario = cuentaUsuarioPrincipal.Usuario;
                }
                else
                {
                    nombreUsuario = cuentaUsuarioAmigo.Usuario;
                }

                var estaConectado = proxy.UsuarioConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                if (!estaConectado.Resultado)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, false);
                    return;
                }

                if (proxy.State == CommunicationState.Faulted)
                {
                    proxy.Abort();
                    throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                }

                proxy.NotificarInvitacion(nombreUsuario, Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
            }
        }
    }
}
