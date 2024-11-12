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
            enviarSolicitudAmistad(nombreUsuario);
        }

        private void enviarSolicitudAmistad(String nombreUsuario)
        {
            if (Utilidades.Utilidades.EsCampoVacio(nombreUsuario))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return;
            }

            using (var proxy = new Servidor.GestionAmigosClient())
            {
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    var proxyGestionJugador = new Servidor.GestionJugadorClient();

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

                    var respuestaAmistadYaExiste = proxy.AmistadYaExiste(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, nombreUsuario);
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

                    var respuestaEnviarSolicitudAmistad = proxy.EnviarSolicitudAmistad(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, nombreUsuario);
                    if (respuestaEnviarSolicitudAmistad.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaEnviarSolicitudAmistad.Resultado)
                    {
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

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }
    }
}
