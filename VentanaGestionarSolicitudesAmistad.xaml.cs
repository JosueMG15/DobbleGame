using DobbleGame.Servidor;
using DobbleGame.Utilidades;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DobbleGame
{
    public partial class VentanaGestionarSolicitudesAmistad : Window
    {
        private VentanaMenu _ventanaMenu;

        public VentanaGestionarSolicitudesAmistad(VentanaMenu ventanaMenu)
        {
            _ventanaMenu = ventanaMenu;
            InitializeComponent();
            CargarSolicitudesAmistad();

            // Suscribirse a eventos de NotificacionesManager
            CallbackManager.Instance.NotificarSolicitudAmistadEvent += NotificarSolicitudAmistad;
        }

        private void VentanaGestionarSolicitudesAmistad_Closed(object sender, EventArgs e)
        {
            CallbackManager.Instance.NotificarSolicitudAmistadEvent -= NotificarSolicitudAmistad;
        }


        // Métodos de callback 
        public void NotificarSolicitudAmistad()
        {
            Dispatcher.Invoke(() =>
            {
                CargarSolicitud();
            });
        }

        private void CargarSolicitudesAmistad()
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

                    var respuesta = proxy.ObtenerSolicitudesPendientes(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(_ventanaMenu);
                        return;
                    }

                    if (respuesta.Resultado != null && respuesta.Resultado.Length > 0)
                    {
                        // Mapeo de objetos del servicio a la capa de dominio
                        List<Dominio.Amistad> solicitudesAmistad = respuesta.Resultado
                            .Select(solicitud => new Dominio.Amistad
                            {
                                IdAmistad = solicitud.idAmistad,
                                EstadoSolicitud = solicitud.estadoSolicitud,
                                UsuarioPrincipalId = solicitud.UsuarioPrincipalId,
                                UsuarioAmigoId = solicitud.UsuarioAmigoId
                            })
                            .ToList();

                        // Mostrar las notificaciones
                        foreach (var solicitud in solicitudesAmistad)
                        {
                            MostrarNotificacionSolicitud(solicitud);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
        }

        public void CargarSolicitud()
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

                    var respuesta = proxy.ObtenerSolicitud();
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
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
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
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.25, GridUnitType.Star) }); 
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.75, GridUnitType.Star) }); 

            // Crear la imagen del usuario y hacerla circular
            var fotoUsuario = new Image
            {
                Width = 70, 
                Height = 70, 
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(0, 0, 15, 0) 
            };

            byte[] fotoBytes = cuenta.Foto; 
            if (fotoBytes != null)
            {
                fotoUsuario.Source = ConvertirBytesAImagen(fotoBytes);
            }

            fotoUsuario.Clip = new EllipseGeometry
            {
                Center = new Point(35, 35), 
                RadiusX = 35,
                RadiusY = 35
            };
            Grid.SetColumn(fotoUsuario, 0);

            // Crear el TextBlock con el nombre de usuario
            var nombreUsuario = new TextBlock
            {
                Text = cuentaUsuarioAmigo.Usuario,
                FontSize = 21, 
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center, 
                Margin = new Thickness(0, 0, 0, 0) 
            };
            Grid.SetColumn(nombreUsuario, 1);

            // Crear los botones de aceptar y rechazar
            var stackBotones = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var botonAceptar = new Button
            {
                Content = "Aceptar",
                Margin = new Thickness(0, 0, 5, 0),
                Background = new SolidColorBrush(Colors.Green),
                Foreground = new SolidColorBrush(Colors.White)
            };
            botonAceptar.Click += (s, e) => AceptarSolicitud(solicitud, panelSolicitud, cuentaUsuarioAmigo.Usuario);

            var botonRechazar = new Button
            {
                Content = "Rechazar",
                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White)
            };
            botonRechazar.Click += (s, e) => RechazarSolicitud(solicitud, panelSolicitud);

            stackBotones.Children.Add(botonAceptar);
            stackBotones.Children.Add(botonRechazar);

            var contenidoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center // Centrar verticalmente todo el contenido
            };
            contenidoStackPanel.Children.Add(nombreUsuario);
            contenidoStackPanel.Children.Add(stackBotones);

            Grid.SetColumn(contenidoStackPanel, 1);

            grid.Children.Add(fotoUsuario);
            grid.Children.Add(contenidoStackPanel);

            panelSolicitud.Child = grid;

            ContenedorNotificaciones.Children.Add(panelSolicitud);
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

        private void AceptarSolicitud(Dominio.Amistad solicitud, Border panelSolicitud, String nombreUsuario)
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

                    var respuesta = proxy.AceptarSolicitud(solicitud.IdAmistad, nombreUsuario);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }

                    if (respuesta.Resultado)
                    {
                        ContenedorNotificaciones.Children.Remove(panelSolicitud);
                        _ventanaMenu.ContenedorNotificaciones.Children.Clear();
                        _ventanaMenu.CargarAmistades();
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
        }

        private void RechazarSolicitud(Dominio.Amistad solicitud, Border panelSolicitud)
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

                    var respuesta = proxy.EliminarAmistad(solicitud.IdAmistad, null, null);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }

                    if (respuesta.Resultado)
                    {
                        ContenedorNotificaciones.Children.Remove(panelSolicitud);
                    }

                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
