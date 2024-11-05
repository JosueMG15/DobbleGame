using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Lógica de interacción para VentanaGestionarSolicitudesAmistad.xaml
    /// </summary>
    public partial class VentanaGestionarSolicitudesAmistad : Window
    {
        private VentanaMenu _ventanaMenu;

        public VentanaGestionarSolicitudesAmistad(VentanaMenu ventanaMenu)
        {
            _ventanaMenu = ventanaMenu;
            InitializeComponent();
            CargarSolicitudesAmistad();
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

                    var respuesta = proxy.ObtenerSolicitudesPendientes(Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
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
                Background = new SolidColorBrush(Colors.Transparent), // Fondo transparente
                Padding = new Thickness(10), // Espaciado interno en lugar de margen externo
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
            botonAceptar.Click += (s, e) => AceptarSolicitud(solicitud, panelSolicitud);

            var botonRechazar = new Button
            {
                Content = "Rechazar",
                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White)
            };
            botonRechazar.Click += (s, e) => RechazarSolicitud(solicitud, panelSolicitud);

            stackBotones.Children.Add(botonAceptar);
            stackBotones.Children.Add(botonRechazar);
            Grid.SetColumn(stackBotones, 1);

            grid.Children.Add(nombreUsuario);
            grid.Children.Add(stackBotones);

            panelSolicitud.Child = grid;

            ContenedorNotificaciones.Children.Add(panelSolicitud);
        }

        private void AceptarSolicitud(Dominio.Amistad solicitud, Border panelSolicitud)
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

                    var respuesta = proxy.AceptarSolicitud(solicitud.IdAmistad);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }

                    if (respuesta.Resultado)
                    {
                        ContenedorNotificaciones.Children.Remove(panelSolicitud);
                        _ventanaMenu.CargarAmistad(solicitud.IdAmistad);
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

                    var respuesta = proxy.EliminarAmistad(solicitud.IdAmistad);

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

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
