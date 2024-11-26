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
                var proxy = new Servidor.GestionAmigosClient();
                try
                {
                    var estaConectado = proxy.UsuarioConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    if (!estaConectado.Resultado)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this, false);
                        return;
                    }

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
                    Utilidades.Utilidades.ManejarExcepciones(proxyGestionJugador, ex, this);
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
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
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxyGestionAmigos, ex, this);
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
