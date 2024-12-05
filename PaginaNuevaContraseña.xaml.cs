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
            string nuevaContraseña = tbNuevaContraseña.Password.Trim();
            string confirmarNuevaContraseña = tbConfirmarContraseña.Password.Trim();
            ActualizarContraseña(nuevaContraseña, confirmarNuevaContraseña, _correo);
        }

        private void ActualizarContraseña(string nuevaContraseña, string confirmarNuevaContraseña, string correo)
        {
            if (Utilidades.Utilidades.EsCampoVacio(nuevaContraseña) || Utilidades.Utilidades.EsCampoVacio(confirmarNuevaContraseña))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return;
            }

            var proxyGestionJugador = new Servidor.GestionJugadorClient();
            try
            {
                if (nuevaContraseña != confirmarNuevaContraseña)
                {
                    MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide_);
                    return;
                }

                if (Utilidades.Utilidades.ValidarContraseña(nuevaContraseña) && Utilidades.Utilidades.ValidarContraseña(confirmarNuevaContraseña))
                {
                    string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(tbNuevaContraseña.Password);
                    var respuestaUsuario = proxyGestionJugador.ObtenerUsuarioPorCorreo(correo);
                    var usuario = respuestaUsuario.Resultado;

                    if (respuestaUsuario.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(_marcoPrincipal);
                        return;
                    }

                    Dominio.CuentaUsuario.CuentaUsuarioActual = new Dominio.CuentaUsuario
                    {
                        IdCuentaUsuario = usuario.IdCuentaUsuario,
                        Contraseña = usuario.Contraseña
                    };

                    var respuestaModificarContraseña = proxyGestionJugador.ModificarContraseñaUsuario
                        (Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, contraseñaHasheada);


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
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionJugador, ex, this);
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            IconoAdvertencia.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            _marcoPrincipal.Close();

        }
        private void PbCambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }
    }
}

