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
    public partial class VentanaEnviarSolicitudAmistad : Window
    {
        public VentanaEnviarSolicitudAmistad()
        {
            InitializeComponent();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            String nombreUsuario = tbNombreUsuario.Text.Trim();
            EnviarSolicitudAmistad(nombreUsuario);
        }

        private void EnviarSolicitudAmistad(string nombreUsuario)
        {
            if (Utilidades.Utilidades.EsCampoVacio(nombreUsuario))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return;
            }

            using (var proxyGestionJugador = new Servidor.GestionJugadorClient())
            {
                try
                {
                    var respuestaUsuario = proxyGestionJugador.ExisteNombreUsuario(nombreUsuario);
                    if (respuestaUsuario.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (!respuestaUsuario.Resultado)
                    {
                        MostrarMensaje(Properties.Resources.lb_UsuarioInexistente_);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error inesperado en verificar usuario: {ex.Message}");
                    return;
                }
            }

            using (var proxyGestionAmigos = new Servidor.GestionAmigosClient())
            {
                try
                {
                    if (proxyGestionAmigos.State == CommunicationState.Faulted)
                    {
                        proxyGestionAmigos.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    var respuestaAmistadYaExiste = proxyGestionAmigos.AmistadYaExiste(
                        Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, nombreUsuario);

                    if (respuestaAmistadYaExiste.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaAmistadYaExiste.Resultado)
                    {
                        MostrarMensaje(Properties.Resources.lb_SolicitudYaEnviada_);
                        return;
                    }
                    if(nombreUsuario == Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario)
                    {
                        MostrarMensaje(Properties.Resources.lb_SolicitudATiMismo_);
                        return;
                    }


                    var respuestaEnviarSolicitudAmistad = proxyGestionAmigos.EnviarSolicitudAmistad(
                        Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, nombreUsuario);

                    if (respuestaEnviarSolicitudAmistad.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaEnviarSolicitudAmistad.Resultado)
                    {
                        MostrarMensaje("Solicitud de amistad enviada exitosamente.");
                        this.Close();
                    }
                    else
                    {
                        MostrarMensaje("No se pudo enviar la solicitud de amistad. Inténtalo nuevamente.");
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
                    if (proxyGestionAmigos.State == CommunicationState.Faulted)
                    {
                        proxyGestionAmigos.Abort();
                    }
                    else
                    {
                        proxyGestionAmigos.Close();
                    }
                }
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }
    }
}
