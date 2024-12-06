using DobbleGame.Utilidades;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DobbleGame
{
    public partial class PaginaNuevaContraseña : Page
    {
        private Servidor.GestionJugadorClient _proxyGestionJugador = new Servidor.GestionJugadorClient();
        private VentanaRecuperarContraseña _marcoPrincipal;
        private string _correo;

        public PaginaNuevaContraseña(VentanaRecuperarContraseña marcoPrincipal, string correo)
        {
            InitializeComponent();
            _correo = correo;
            _marcoPrincipal = marcoPrincipal;
        }

        private void BtnActualizar(object sender, RoutedEventArgs e)
        {
            ActualizarContraseña(_correo);
        }

        private void ActualizarContraseña(string correo)
        {
            try
            {
                string nuevaContraseña = tbNuevaContraseña.Password.Trim();
                string confirmarNuevaContraseña = tbConfirmarContraseña.Password.Trim();

                if (Utilidades.Utilidades.EsCampoVacio(nuevaContraseña) || Utilidades.Utilidades.EsCampoVacio(confirmarNuevaContraseña))
                {
                    MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                    return;
                }

                if (nuevaContraseña != confirmarNuevaContraseña)
                {
                    MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide);
                    return;
                }

                if (Utilidades.Utilidades.ValidarContraseña(nuevaContraseña) && Utilidades.Utilidades.ValidarContraseña(confirmarNuevaContraseña))
                {
                    string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(tbNuevaContraseña.Password);
                    var respuestaUsuario = _proxyGestionJugador.ObtenerUsuarioPorCorreo(correo);
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

                    var respuestaModificarContraseña = _proxyGestionJugador.ModificarContraseñaUsuario
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
                Utilidades.Utilidades.ManejarExcepciones(_proxyGestionJugador, ex, this);
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
            _proxyGestionJugador.Close();
        }

        private void PbCambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }
    }
}

