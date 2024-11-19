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
    /// Lógica de interacción para PaginaNuevaContraseña.xaml
    /// </summary>
    public partial class PaginaNuevaContraseña : Page
    {
        VentanaRecuperarContraseña _marcoPrincipal;
        string _correo;
        public PaginaNuevaContraseña(VentanaRecuperarContraseña marcoPrincipal, string correo)
        {
            InitializeComponent();
            _marcoPrincipal = marcoPrincipal;
            _correo = correo;
        }

        private void BtnActualizar(object sender, RoutedEventArgs e)
        {
            String nuevaContraseña = tbNuevaContraseña.Password.Trim();
            String confirmarNuevaContraseña = tbConfirmarContraseña.Password.Trim();
            ActualizarContraseña(nuevaContraseña, confirmarNuevaContraseña, _correo);
        }

        private void ActualizarContraseña(String nuevaContraseña, String confirmarNuevaContraseña, string correo)
        {
            if (Utilidades.Utilidades.EsCampoVacio(nuevaContraseña) || Utilidades.Utilidades.EsCampoVacio(confirmarNuevaContraseña))
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

                    if (nuevaContraseña != confirmarNuevaContraseña)
                    {
                        MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide_);
                        return;
                    }

                    if (Utilidades.Utilidades.ValidarContraseña(nuevaContraseña) && Utilidades.Utilidades.ValidarContraseña(confirmarNuevaContraseña))
                    {
                        string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(tbNuevaContraseña.Password);
                        var respuestaUsuario = proxy.ObtenerUsuarioPorCorreo(correo);
                        var usuario = respuestaUsuario.Resultado;

                        Dominio.CuentaUsuario.CuentaUsuarioActual = new Dominio.CuentaUsuario
                        {
                            IdCuentaUsuario = usuario.IdCuentaUsuario,
                            Contraseña = usuario.Contraseña
                        };

                        var respuestaModificarContraseña = proxy.ModificarContraseñaUsuario(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, contraseñaHasheada);


                        if (respuestaModificarContraseña.ErrorBD)
                        {
                            Utilidades.Utilidades.MostrarVentanaErrorConexionBD(_marcoPrincipal);
                            return;
                        }
                        if (nuevaContraseña == usuario.Contraseña)
                        {
                            MostrarMensaje(Properties.Resources.global_MismaContraseña_);
                            return;
                        }
                        if (respuestaModificarContraseña.Resultado)
                        {
                            _marcoPrincipal.Close();
                        }
                        else
                        {
                            MostrarMensaje(Properties.Resources.global_MismaContraseña_);
                            return;
                        }
                    }
                    else
                    {
                        MostrarMensaje(Properties.Resources.lb_DatosInválidos);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            _marcoPrincipal.Close();

        }
        private void PasswordBox_CambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }
    }
}

