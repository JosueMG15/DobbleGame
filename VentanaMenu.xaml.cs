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
using System.Windows.Navigation;
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
            this.Closing += VentanaMenuCierreAbrupto;
            lbNombreUsuario.Content = Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario;
            ConvertirImagenPerfil(Dominio.CuentaUsuario.CuentaUsuarioActual.Foto);
            CargarAmistades();

            if (ControlDeUsuarioNotificacion.Instancia.Parent is Panel parentPanel)
            {
                parentPanel.Children.Remove(ControlDeUsuarioNotificacion.Instancia);
            }

            this.GridPrincipal.Children.Add(ControlDeUsuarioNotificacion.Instancia);

            CallbackManager.Instance.NotificarCambioEvent += NotificarCambio;
            CallbackManager.Instance.NotificarSalidaEvent += NotificarSalida;
            CallbackManager.Instance.NotificarInvitacionCambioEvent += NotificarInvitacionCambio;
            CallbackManager.Instance.NotificarVentanaInvitacionEvent += NotificarVentanaInvitacion;

            MarcoPrincipal.Navigated += PaginaSalaActiva;
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
            catch (Exception)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, true);
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
                                        CambiarEstadoAusente(stackPanel);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, true);
            }
        }

        public void NotificarInvitacionCambio(string nombreUsuario)
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
                                        CambiarInvitacion(grid);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, true);
            }
        }

        public void NotificarVentanaInvitacion(string nombreUsuarioInvitacion, string codigoSala)
        {
            var proxy = new Servidor.GestionAmigosClient();

            try
            {
                bool sePuedeEnviar = true;

                foreach (Window ventana in Application.Current.Windows)
                {
                    if (!(ventana is VentanaMenu ventanaMenu))
                    {
                        sePuedeEnviar = false;
                        return;
                    }

                    if (sePuedeEnviar == true)
                    {

                        string mensaje = String.Format(Properties.Resources.lb_TeEstaInvitando_, nombreUsuarioInvitacion);
                        VentanaModalDecision ventanaModalDecision = new VentanaModalDecision(mensaje);
                        ventanaModalDecision.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        bool? respuesta = ventanaModalDecision.ShowDialog();

                        proxy.ReestablecerInvitacionPendiente(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);

                        if (respuesta == true)
                        {
                            if (ventanaMenu.MarcoPrincipal.Content is PaginaSala paginaSalaActual)
                            {
                                paginaSalaActual.AbandonarSala();
                            }
                            PaginaSala paginaSala = new PaginaSala(false, codigoSala);
                            if (paginaSala.ExisteSala())
                            {
                                if (paginaSala.HayEspacioEnSala())
                                {
                                    if (paginaSala.IniciarSesionSala())
                                    {
                                        MarcoPrincipal.Navigate(paginaSala);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
            }
        }

        private void CambiarEstadoAusente(StackPanel stackPanel)
        {
            foreach ( var child in stackPanel.Children)
            {
                if(child is StackPanel estadoPanel)
                {
                    foreach (var estadoChild in estadoPanel.Children)
                    {
                        if(estadoChild is Ellipse circulo){
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

        private void CambiarInvitacion(Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is Panel panel)
                {
                    CambiarEstadoEnHijos(panel);
                }
            }
        }

        private void CambiarEstadoEnHijos(Panel parent)
        {
            foreach (var child in parent.Children)
            {
                if (child is Button botonInvitar && botonInvitar.Content?.ToString() == Properties.Resources.btn_Invitar)
                {
                    botonInvitar.Background = Brushes.Gray;
                    botonInvitar.IsEnabled = false;
                }

                if (child is Panel panel)
                {
                    CambiarEstadoEnHijos(panel); 
                }
            }
        }

        private void PaginaSalaActiva(object sender, NavigationEventArgs e)
        {
            if (e.Content is PaginaSala paginaSala)
            {
                var proxyGestionAmigos = new GestionAmigosClient();
                proxyGestionAmigos.NotificarCambios();
                return;
            }
            if (!(e.Content is PaginaSala paginaSala1))
            {
                var proxyGestionAmigos = new GestionAmigosClient();
                proxyGestionAmigos.NotificarCambios(); 
                return;
            }
        }

        private void BtnIrPerfil(object sender, RoutedEventArgs e)
        {
            if (MarcoPrincipal.Content is PaginaSala paginasala)
            {
                VentanaModalDecision ventanaModalDecision = new 
                    VentanaModalDecision(Properties.Resources.lb_ConfirmarIrPerfil);
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
            DoubleAnimation animacionDesvanecimiento = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            animacionDesvanecimiento.Completed += (s, a) =>
            {
                PaginaPerfil paginaPerfil = new PaginaPerfil(this);
                MarcoPrincipal.Navigate(paginaPerfil);

                DoubleAnimation animacionAparicion = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                MarcoPrincipal.BeginAnimation(Frame.OpacityProperty, animacionAparicion);
            };
            MarcoPrincipal.BeginAnimation(Frame.OpacityProperty, animacionDesvanecimiento);
        }

        private void BtnCerrarSesion(object sender, RoutedEventArgs e)
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

        private void VentanaMenuCierreAbrupto(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var proxyGestionAmigos = new Servidor.GestionAmigosClient();
            var proxyGestionJugador = new GestionJugadorClient();
            try
            {
                var estaConectado = proxyGestionAmigos.UsuarioConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                if (!estaConectado.Resultado)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, false);
                    return;
                }
                if (!string.IsNullOrEmpty(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario))
                {
                    proxyGestionJugador.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                    CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    proxyUsuario.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ((App)Application.Current).DetenerPing();
                    });
                    
                    proxyGestionAmigos.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    proxyGestionAmigos.NotificarBotonInvitacion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionJugador, ex, this);
            }
        }

        private void CerrarSesion()
        {
            var proxyGestionJugador = new GestionJugadorClient();
            var proxyGestionAmigos = new GestionAmigosClient();
            try
            {
                proxyGestionJugador.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                proxyGestionAmigos.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                proxyGestionAmigos.NotificarBotonInvitacion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);

                App.Current.Dispatcher.Invoke(() =>
                {
                    ((App)Application.Current).DetenerPing();
                });
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionJugador, ex, this);
            }
        }

        private void BtnSolicitudesAmistad(object sender, RoutedEventArgs e)
        {
            var proxyGestionAmigos = new GestionAmigosClient();
            var respuesta = proxyGestionAmigos.ObtenerSolicitudesPendientes(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario);

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

        public void ActualizarNombreUsuario(string nuevoTexto)
        {
            lbNombreUsuario.Content = nuevoTexto;
        }

        public void ConvertirImagenPerfil(byte[] fotoBytes)
        {
            if (fotoBytes == null || fotoBytes.Length == 0)
                return;

            using (var flujoMemoria = new MemoryStream(fotoBytes))
            {
                BitmapImage imagen = new BitmapImage();
                imagen.BeginInit();
                imagen.StreamSource = flujoMemoria;
                imagen.CacheOption = BitmapCacheOption.OnLoad;
                imagen.EndInit();
                imagen.Freeze();

                ImagenPerfil.Source = imagen;
            }
        }

        public void CargarAmistades()
        {
            var proxyGestionAmigos = new Servidor.GestionAmigosClient();
            try
            {
                var respuesta = proxyGestionAmigos.ObtenerAmistades(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario);

                if (respuesta.ErrorBD)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                    return;
                }

                if (respuesta.Resultado != null && respuesta.Resultado.Length > 0)
                {
                    List<Dominio.Amistad> amistades = respuesta.Resultado
                        .Select(amistad => new Dominio.Amistad
                        {
                            IdAmistad = amistad.idAmistad,
                            EstadoSolicitud = amistad.estadoSolicitud,
                            UsuarioPrincipalId = amistad.UsuarioPrincipalId,
                            UsuarioAmigoId = amistad.UsuarioAmigoId
                        })
                        .ToList();

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
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionAmigos, ex, this);
            }
        }

        private void MostrarAmigo(Dominio.Amistad solicitud, bool esAgeno)
        {
            var proxyGestionAmigos = new Servidor.GestionAmigosClient();
            try
            {
                Dominio.CuentaUsuarioAmigo cuentaUsuarioAmigo = new Dominio.CuentaUsuarioAmigo
                {
                    Usuario = UsuarioAmigo(solicitud, esAgeno).Usuario,
                    Puntaje = UsuarioAmigo(solicitud, esAgeno).Puntaje,
                    Foto = UsuarioAmigo(solicitud, esAgeno).Foto
                };

                var respuesta = proxyGestionAmigos.UsuarioConectado(cuentaUsuarioAmigo.Usuario);

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

                
                var fotoUsuario = new Image    // Foto del usuario
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

                 
                var stackNombreEstado = new StackPanel    // Nombre de usuario y Estado
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var nombreUsuario = new TextBlock
                {
                    Text = cuentaUsuarioAmigo.Usuario,
                    FontSize = 13,
                    FontWeight = FontWeights.Bold
                };
                stackNombreEstado.Children.Add(nombreUsuario);

                var estado = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                
                var puntosUsuario = new TextBlock    // Puntos del usuario
                {
                    Text = $"Puntos: {cuentaUsuarioAmigo.Puntaje}",
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                Grid.SetColumn(puntosUsuario, 1);
                Grid.SetRow(puntosUsuario, 1);
                grid.Children.Add(puntosUsuario);

                
                var botonesPanel = new StackPanel    // Botones de "EliminarAmistad" e "Invitar"
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
                    Margin = new Thickness(0, 0, 5, 0),
                    ToolTip = Properties.Resources.btn_EliminarAmigo
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


                var circulo = new Ellipse
                {
                    Width = 15,
                    Height = 15,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                estado.Children.Add(circulo);

                var textoEstado = new Label
                {
                    FontSize = 12
                };
                estado.Children.Add(textoEstado);

                var botonInvitar = new Button
                {
                    Content = Properties.Resources.btn_Invitar,
                    Background = Brushes.Gray,
                    Foreground = Brushes.White,
                    IsEnabled = false,
                    Padding = new Thickness(5)
                };
                botonesPanel.Children.Add(botonInvitar);

                if (respuesta.Resultado)    //Si esta en línea
                {
                    circulo.Fill = Brushes.LightGreen;
                    textoEstado.Content = Properties.Resources.lb_EnLínea;

                    foreach (Window ventana in Application.Current.Windows)
                    {
                        if(ventana is VentanaMenu ventanaMenu && ventanaMenu.MarcoPrincipal.Content is PaginaSala paginaSala)
                        {
                            botonInvitar.Background = Brushes.Blue;
                            botonInvitar.IsEnabled = true;
                        }
                    }

                    botonInvitar.Click += (s, e) => InvitarAmistad(solicitud);
                }
                else    //Si esta ausente
                {
                    circulo.Fill = Brushes.Red;
                    textoEstado.Content = Properties.Resources.lb_Ausente;
                }

                stackNombreEstado.Children.Add(estado);
                Grid.SetColumn(stackNombreEstado, 1);
                Grid.SetRow(stackNombreEstado, 0);
                grid.Children.Add(stackNombreEstado);

                Grid.SetColumn(botonesPanel, 2);
                Grid.SetRowSpan(botonesPanel, 2);
                grid.Children.Add(botonesPanel);

                panelSolicitud.Child = grid;

                ContenedorNotificaciones.Children.Add(panelSolicitud);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionAmigos, ex, this);
            }
        }

        private DobbleGame.Servidor.CuentaUsuario UsuarioAmigo(Dominio.Amistad solicitud, bool esAgeno)
        {
            var proxyGestionAmigos = new Servidor.GestionAmigosClient();
            if (esAgeno == true)
            {
                var respuesta = proxyGestionAmigos.ObtenerUsuario(solicitud.UsuarioPrincipalId);
                var cuenta = respuesta.Resultado;
                return cuenta;
            }
            else
            {
                var respuesta = proxyGestionAmigos.ObtenerUsuario(solicitud.UsuarioAmigoId);
                var cuenta = respuesta.Resultado;
                return cuenta;
            }
        }

        private ImageSource ConvertirBytesAImagen(byte[] fotoBytes)
        {
            if (fotoBytes == null || fotoBytes.Length == 0)
                return null;

            using (var flujoMemoria = new MemoryStream(fotoBytes))
            {
                var imagen = new BitmapImage();
                imagen.BeginInit();
                imagen.CacheOption = BitmapCacheOption.OnLoad;
                imagen.StreamSource = flujoMemoria;
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
            foreach (Window ventana in Application.Current.Windows)
            {
                if (ventana is VentanaMenu ventanaMenu && ventanaMenu.MarcoPrincipal.Content is PaginaSala paginaSala)
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

                        var jugadorEnSala = paginaSala.UsuariosConectados.FirstOrDefault(j => j.Usuario == nombreUsuario);

                        if (jugadorEnSala != null)
                        {
                            ControlDeUsuarioNotificacion.Instancia.MostrarNotificacion(Properties.Resources.msg_JugadorEnSala);
                            return;
                        }

                        var estaConectado = proxy.UsuarioConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                        if (!estaConectado.Resultado)
                        {
                            Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, false);
                            return;
                        }

                        if (!proxy.TieneInvitacionPendiente(nombreUsuario))
                        {
                            proxy.NotificarInvitacion(nombreUsuario, Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, paginaSala.CodigoSala);
                            ControlDeUsuarioNotificacion.Instancia.MostrarNotificacion(Properties.Resources.msg_InvitaciónEnviada);
                        }
                        else
                        {
                            ControlDeUsuarioNotificacion.Instancia.MostrarNotificacion(Properties.Resources.msg_InvitaciónPendiente);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                    }
                }
                else
                {
                    ControlDeUsuarioNotificacion.Instancia.MostrarNotificacion("Debes estar en una sala para invitar a un amigo");
                }
            }
        }
    }
}

