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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para PaginaRecuperarContraseña.xaml
    /// </summary>
    public partial class PaginaRecuperarContraseña : Page
    {
        VentanaRecuperarContraseña _marcoPrincipal;
        public PaginaRecuperarContraseña(VentanaRecuperarContraseña marcoPrincipal)
        {
            InitializeComponent();
            _marcoPrincipal = marcoPrincipal;   
        }

        private void BtnEnviarCodigo(object sender, RoutedEventArgs e)
        {
            String correo = tbCorreo.Text.Trim();
            EnviarCodigo(correo);
            var paginaIngresoCodigo = new PaginaIngresoCodigo(_marcoPrincipal);
            this.NavigationService.Navigate(paginaIngresoCodigo);
        }

        private void EnviarCodigo(string correo)
        {
            /*if (Utilidades.Utilidades.EsCampoVacio(correo))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return;
            }

            using (var proxy = new Servidor.GestionCorreosClient())
            {
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    var respuestaUsuario = proxy.EnviarCodigo(correo);
                    if (respuestaUsuario.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaUsuario.Resultado)
                    {
                        MostrarMensaje(Properties.Resources.lb_UsuarioExistente_);
                        return;
                    }

                    var respuestaModificarUsuario = proxy.ModificarNombreUsuario(CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario, nuevoNombre);
                    if (respuestaModificarUsuario.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaModificarUsuario.Resultado)
                    {
                        CuentaUsuario.cuentaUsuarioActual.Usuario = nuevoNombre;
                        CuentaUsuario.cuentaUsuarioActual = new CuentaUsuario
                        {
                            IdCuentaUsuario = Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario,
                            Usuario = nuevoNombre,
                            Correo = Dominio.CuentaUsuario.cuentaUsuarioActual.Correo,
                            Contraseña = Dominio.CuentaUsuario.cuentaUsuarioActual.Contraseña,
                            Foto = Dominio.CuentaUsuario.cuentaUsuarioActual.Foto,
                            Puntaje = Dominio.CuentaUsuario.cuentaUsuarioActual.Puntaje,
                            Estado = true,
                        };

                        this.Close();
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
            }*/
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            _marcoPrincipal.Close();
        }

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }
    }
}
