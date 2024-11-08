using DobbleGame.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
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
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Proxies;
using Dominio;
using System.Windows.Navigation;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.ServiceModel;

namespace DobbleGame
{
    public partial class VentanaCambioNombre : Window
    {
        private PaginaPerfil _paginaPerfil;
        private VentanaMenu _ventanaMenu;
        public VentanaCambioNombre(PaginaPerfil paginaPerfil, VentanaMenu ventanaMenu)
        {
            InitializeComponent();
            _paginaPerfil = paginaPerfil;
            _ventanaMenu = ventanaMenu;
        }

        private void BtnActualizarUsuario(object sender, RoutedEventArgs e)
        {
            String nuevoNombre = tbNuevoNombre.Text.Trim();
            GuardarCambioNombre(nuevoNombre);

            _paginaPerfil.ActualizarNombreUsuario(CuentaUsuario.cuentaUsuarioActual.Usuario);
            _ventanaMenu.ActualizarNombreUsuario(CuentaUsuario.cuentaUsuarioActual.Usuario);
        }

        private void GuardarCambioNombre(string nuevoNombre)
        {
            if (Utilidades.Utilidades.EsCampoVacio(nuevoNombre))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return;
            }

            using (var proxy = new Servidor.GestionJugadorClient())
            {
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    var respuestaUsuario = proxy.ExisteNombreUsuario(nuevoNombre);
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
            }
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void TbNuevoNombre(object sender, TextChangedEventArgs e)
        {

        }

    }
}
