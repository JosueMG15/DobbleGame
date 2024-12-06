﻿using DobbleGame.Servidor;
using DobbleGame.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    public partial class VentanaMenu : Window
    {
        private readonly ControlDeUsuarioNotificacion _controlNotificacion;
        public VentanaMenu()
        {
            InitializeComponent();
            InicializarDatos();
            MarcoPrincipal.NavigationService.Navigate(new PaginaMenu());
            _controlNotificacion = new ControlDeUsuarioNotificacion();
            contenedorNotificacion.Content = _controlNotificacion;
        }

        private void InicializarDatos()
        {
            lbNombreUsuario.Content = Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario;
            ConvertirImagenPerfil(Dominio.CuentaUsuario.CuentaUsuarioActual.Foto);
            CallbackManager.Instance.NotificarCambioEvent += NotificarCambio;
            CallbackManager.Instance.NotificarSalidaEvent += NotificarSalida;
            CallbackManager.Instance.NotificarInvitacionCambioEvent += NotificarInvitacionCambio;
            CallbackManager.Instance.NotificarVentanaInvitacionEvent += NotificarVentanaInvitacion;

            MarcoPrincipal.Navigated += PaginaSalaActiva;
            this.Closing += VentanaMenuCierreAbrupto;
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
                Dispatcher.Invoke(() =>
                {
                    foreach (var child in ContenedorNotificaciones.Children)
                    {
                        if (child is Border border && border.Child is Grid grid)
                        {
                            foreach (var gridHijo in grid.Children)
                            {
                                if (gridHijo is StackPanel stackPanel)
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
                });                
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
                Dispatcher.Invoke(() =>
                {
                    foreach (var hijo in ContenedorNotificaciones.Children)
                    {
                        if (hijo is Border border && border.Child is Grid grid)
                        {
                            foreach (var gridHijo in grid.Children)
                            {
                                if (gridHijo is StackPanel stackPanel)
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
                });
            }
            catch (Exception)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, true);
            }
        }

        public void NotificarVentanaInvitacion(string nombreUsuarioInvitacion, string codigoSala)
        {
            var proxyGestionAmigos = new Servidor.GestionAmigosClient();

            try
            {
                if (!Application.Current.Windows.OfType<VentanaMenu>().Any())
                {
                    return;
                }

                string mensaje = string.Format(Properties.Resources.lb_TeEstaInvitando_, nombreUsuarioInvitacion);
                VentanaModalDecision ventanaModalDecision = new VentanaModalDecision(mensaje)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                bool? respuesta = ventanaModalDecision.ShowDialog();

                proxyGestionAmigos.ReestablecerInvitacionPendiente(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);

                if (respuesta == true)
                {
                    ManejarRespuestaInvitacion(codigoSala);
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionAmigos, ex, this);
            }
        }

        private void ManejarRespuestaInvitacion(string codigoSala)
        {
            if (Application.Current.Windows.OfType<VentanaMenu>().FirstOrDefault() is VentanaMenu ventanaMenu)
            {
                if (ventanaMenu.MarcoPrincipal.Content is PaginaSala paginaSalaActual)
                {
                    paginaSalaActual.AbandonarSala();
                }

                PaginaSala nuevaPaginaSala = new PaginaSala(false, codigoSala);

                if (!nuevaPaginaSala.ExisteSala())
                {
                    _controlNotificacion.MostrarNotificacion(Properties.Resources.lb_SalaInexistente);
                    return;
                }

                if (!nuevaPaginaSala.EsSalaDisponible())
                {
                    _controlNotificacion.MostrarNotificacion(Properties.Resources.lb_SalaEnPartida);
                    return;
                }

                if (!nuevaPaginaSala.HayEspacioEnSala())
                {
                    _controlNotificacion.MostrarNotificacion(Properties.Resources.lb_SalaLlena);
                    return;
                }

                if (nuevaPaginaSala.IniciarSesionSala())
                {
                    ventanaMenu.MarcoPrincipal.Navigate(nuevaPaginaSala);
                }
            }
        }

        private static void CambiarEstadoAusente(StackPanel stackPanel)
        {
            foreach ( var hijo in stackPanel.Children)   
            {
                if(hijo is StackPanel estadoPanel)
                {
                    foreach (var estadoHijo in estadoPanel.Children)
                    {
                        if(estadoHijo is Ellipse circulo){
                            circulo.Fill = Brushes.Red;
                        }
                        else if (estadoHijo is Label estadoTexto)
                        {
                            estadoTexto.Content = Properties.Resources.lb_Ausente;
                        }
                    }
                }
            }
        }

        private void CambiarInvitacion(Grid grid)
        {
            foreach (var hijo in grid.Children)
            {
                if (hijo is Panel panel)
                {
                    CambiarEstadoEnHijos(panel);
                }
            }
        }

        private void CambiarEstadoEnHijos(Panel parent)
        {
            foreach (var hijo in parent.Children)
            {
                if (hijo is Button botonInvitar && botonInvitar.Content?.ToString() == Properties.Resources.btn_Invitar)
                {
                    botonInvitar.Background = Brushes.Gray;
                    botonInvitar.IsEnabled = false;
                }

                if (hijo is Panel panel)
                {
                    CambiarEstadoEnHijos(panel); 
                }
            }
        }

        private void PaginaSalaActiva(object sender, NavigationEventArgs e)
        {
            if (e.Content is PaginaSala)
            {
                ContenedorNotificaciones.Children.Clear();
                CargarAmistades();
                return;
            }
            if (!(e.Content is PaginaSala))
            {
                ContenedorNotificaciones.Children.Clear();
                CargarAmistades();
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
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, this);

            VentanaModalDecision ventana = new VentanaModalDecision(Properties.Resources.lb_MensajeCerrarSesion,
                Properties.Resources.btn_CerrarSesión, Properties.Resources.global_Cancelar);
            bool? respuesta = ventana.ShowDialog();

            if (respuesta == true)
            {
                CerrarSesion();

                MainWindow mainWindow = new MainWindow();
                this.Close();
                mainWindow.Show();
            }
        }

        private void VentanaMenuCierreAbrupto(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var _proxyGestionJugador = new GestionJugadorClient();
            var _proxyGestionAmigos = new GestionAmigosClient();
            try
            {
                if (!string.IsNullOrEmpty(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario))
                {
                    _proxyGestionJugador.CerrarSesionJugador
                        (Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);

                    CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    _proxyGestionAmigos.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);                                    
                    _proxyGestionAmigos.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    _proxyGestionAmigos.NotificarBotonInvitacion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);

                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(_proxyGestionJugador, ex, this);
            }
        }

        private void CerrarSesion()
        {
            var _proxyGestionJugador = new GestionJugadorClient();
            var _proxyGestionAmigos = new GestionAmigosClient();
            try
            {
                _proxyGestionJugador.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                _proxyGestionAmigos.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                _proxyGestionAmigos.NotificarBotonInvitacion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);

            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(_proxyGestionJugador, ex, this);
            }
        }

        private void BtnSolicitudesAmistad(object sender, RoutedEventArgs e)
        {
            var _proxyGestionAmigos = new GestionAmigosClient();
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, this);

            var respuesta = _proxyGestionAmigos.ObtenerSolicitudesPendientes(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario);

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
            var _proxyGestionAmigos = new GestionAmigosClient();
            try
            {
                var respuesta = _proxyGestionAmigos.ObtenerAmistades(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario);

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
                 Utilidades.Utilidades.ManejarExcepciones(_proxyGestionAmigos, ex, this);
            }
        }

        private void MostrarAmigo(Dominio.Amistad solicitud, bool esAgeno)
        {
            var _proxyGestionAmigos = new GestionAmigosClient();
            try
            {
                Dominio.CuentaUsuarioAmigo cuentaUsuarioAmigo = new Dominio.CuentaUsuarioAmigo
                {
                    Usuario = UsuarioAmigo(solicitud, esAgeno).Usuario,
                    Puntaje = UsuarioAmigo(solicitud, esAgeno).Puntaje,
                    Foto = UsuarioAmigo(solicitud, esAgeno).Foto
                };

                var respuesta = _proxyGestionAmigos.UsuarioConectado(cuentaUsuarioAmigo.Usuario);

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

                 
                var stackNombreEstado = new StackPanel   
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

                if (respuesta.Resultado)   
                {
                    circulo.Fill = Brushes.LightGreen;
                    textoEstado.Content = Properties.Resources.lb_EnLínea;

                    foreach (Window ventana in Application.Current.Windows)
                    {
                        if(ventana is VentanaMenu ventanaMenu && ventanaMenu.MarcoPrincipal.Content is PaginaSala)
                        {
                            botonInvitar.Background = Brushes.Blue;
                            botonInvitar.IsEnabled = true;
                        }
                    }

                    botonInvitar.Click += (s, e) => InvitarAmistad(solicitud);
                }
                else   
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
                Utilidades.Utilidades.ManejarExcepciones(_proxyGestionAmigos, ex, this);
            }
        }

        private static DobbleGame.Servidor.CuentaUsuario UsuarioAmigo(Dominio.Amistad solicitud, bool esAgeno)
        {
            var _proxyGestionAmigos = new GestionAmigosClient();
            if (esAgeno == true)
            {
                var respuesta = _proxyGestionAmigos.ObtenerUsuario(solicitud.UsuarioPrincipalId);
                var cuenta = respuesta.Resultado;
                return cuenta;
            }
            else
            {
                var respuesta = _proxyGestionAmigos.ObtenerUsuario(solicitud.UsuarioAmigoId);
                var cuenta = respuesta.Resultado;
                return cuenta;
            }
        }

        private static ImageSource ConvertirBytesAImagen(byte[] fotoBytes)
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
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, this);

            VentanaEliminarAmigo ventanaEliminarAmigo = new VentanaEliminarAmigo(this, amistad, panelSolicitud);
            ventanaEliminarAmigo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaEliminarAmigo.ShowDialog();
        }

        private void InvitarAmistad(Dominio.Amistad solicitud)
        {
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, this);
            foreach (Window ventana in Application.Current.Windows)
            {
                if (ventana is VentanaMenu ventanaMenu && ventanaMenu.MarcoPrincipal.Content is PaginaSala paginaSala)
                {
                    ProcesarInvitacion(solicitud, paginaSala);
                }
            }
        }

        private void ProcesarInvitacion(Dominio.Amistad solicitud, PaginaSala paginaSala)
        {
            var proxyGestionAmigos = new Servidor.GestionAmigosClient();

            try
            {
                string nombreUsuario = ObtenerNombreUsuarioInvitacion(solicitud);

                if (paginaSala.UsuariosConectados.Any(j => j.Usuario == nombreUsuario))
                {
                    _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_JugadorEnSala);
                    return;
                }

                if (!proxyGestionAmigos.UsuarioConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario).Resultado)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, false);
                    return;
                }

                if (!proxyGestionAmigos.TieneInvitacionPendiente(nombreUsuario))
                {
                    proxyGestionAmigos.NotificarInvitacion(nombreUsuario, Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, paginaSala.CodigoSala);
                    _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_InvitaciónEnviada);
                }
                else
                {
                    _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_InvitaciónPendiente);
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionAmigos, ex, this);
            }
        }

        private string ObtenerNombreUsuarioInvitacion(Dominio.Amistad solicitud)
        {
            Dominio.CuentaUsuarioAmigo cuentaUsuarioPrincipal = new Dominio.CuentaUsuarioAmigo
            {
                Usuario = UsuarioAmigo(solicitud, true).Usuario,
            };
            Dominio.CuentaUsuarioAmigo cuentaUsuarioAmigo = new Dominio.CuentaUsuarioAmigo
            {
                Usuario = UsuarioAmigo(solicitud, false).Usuario,
            };

            return Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario != cuentaUsuarioPrincipal.Usuario
                ? cuentaUsuarioPrincipal.Usuario
                : cuentaUsuarioAmigo.Usuario;
        }
    }
}

