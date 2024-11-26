using DobbleGame.Servidor;
using DobbleGame.Utilidades;
using Dominio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
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

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para VentanaCambioContraseña.xaml
    /// </summary>
    public partial class VentanaCambioContraseña : Window
    {
        public VentanaCambioContraseña()
        {
            InitializeComponent();
        }


        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnActualizarContraseña(object sender, RoutedEventArgs e)
        {
            String contraseñaActual = pbContraseñaActual.Password.Trim();
            String nuevaContraseña = pbNuevaContraseña.Password.Trim();
            String confirmarNuevaContraseña = pbConfirmarNuevaContraseña.Password.Trim();

            ActualizarContraseña(contraseñaActual, nuevaContraseña, confirmarNuevaContraseña);
        }

        private void ActualizarContraseña(string contraseñaActual, string nuevaContraseña, string confirmarNuevaContraseña)
        {
            if (ValidarEntradas(contraseñaActual, nuevaContraseña, confirmarNuevaContraseña))
            {
                return;
            }

            using (var proxy = new Servidor.GestionJugadorClient())
            {
                var proxyUsuario = new Servidor.GestionAmigosClient();
                try
                {
                    var estaConectado = proxyUsuario.UsuarioConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
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

                    if (!ValidarContraseñaActual(proxy, contraseñaActual))
                    {
                        return;
                    }

                    string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(nuevaContraseña);
                    var respuestaModificarContraseña = proxy.ModificarContraseñaUsuario(
                        Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario,
                        contraseñaHasheada);

                    if (respuestaModificarContraseña.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }

                    if (respuestaModificarContraseña.Resultado)
                    {
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                    Utilidades.Utilidades.ManejarExcepciones(proxyUsuario, ex, this);
                }
            }
        }

        private bool ValidarEntradas(string contraseñaActual, string nuevaContraseña, string confirmarNuevaContraseña)
        {
            if (Utilidades.Utilidades.EsCampoVacio(contraseñaActual) ||
                Utilidades.Utilidades.EsCampoVacio(nuevaContraseña) ||
                Utilidades.Utilidades.EsCampoVacio(confirmarNuevaContraseña))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return true;
            }

            if (nuevaContraseña != confirmarNuevaContraseña)
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide_);
                return true;
            }

            if (contraseñaActual == nuevaContraseña)
            {
                MostrarMensaje(Properties.Resources.global_MismaContraseña_);
                return true;
            }

            if (!Utilidades.Utilidades.ValidarContraseña(contraseñaActual) ||
                !Utilidades.Utilidades.ValidarContraseña(nuevaContraseña) ||
                !Utilidades.Utilidades.ValidarContraseña(confirmarNuevaContraseña))
            {
                MostrarMensaje(Properties.Resources.lb_DatosInválidos);
                return true;
            }

            return false;
        }

        private bool ValidarContraseñaActual(Servidor.GestionJugadorClient proxy, string contraseñaActual)
        {
            var respuestaUsuario = proxy.ValidarContraseña(
                Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario,
                Utilidades.EncriptadorContraseña.GenerarHashSHA512(contraseñaActual));

            if (respuestaUsuario.ErrorBD)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                return false;
            }

            if (!respuestaUsuario.Resultado)
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaActualInvalida);
                return false;
            }

            return true;
        }


        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void PasswordBox_CambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

    }
}
