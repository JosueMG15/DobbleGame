using DobbleGame.Servidor;
using System;
using System.Windows;


namespace DobbleGame
{
    public partial class VentanaEnviarSolicitudAmistad : Window
    {
        private GestionJugadorClient _proxyGestionJugador = new GestionJugadorClient();
        private GestionAmigosClient _proxyGestionAmigos = new GestionAmigosClient();

        public VentanaEnviarSolicitudAmistad()
        {
            InitializeComponent();
        }

        private void BtnRegresar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEnviar(object sender, RoutedEventArgs e)
        {
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, this);
            EnviarSolicitudAmistad();
        }

        private void EnviarSolicitudAmistad()
        {
            try
            {
                String nombreUsuario = tbNombreUsuario.Text.Trim();

                if (Utilidades.Utilidades.EsCampoVacio(nombreUsuario))
                {
                    MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                    return;
                }

                var respuestaUsuario = _proxyGestionJugador.ExisteNombreUsuario(nombreUsuario);

                if (respuestaUsuario.ErrorBD)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                    return;
                }
                if (!respuestaUsuario.Resultado)
                {
                    MostrarMensaje(Properties.Resources.lb_UsuarioInexistente);
                    return;
                }

                var respuestaAmistadYaExiste = _proxyGestionAmigos.AmistadYaExiste(Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, nombreUsuario);

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

                if (nombreUsuario == Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario)
                {
                    MostrarMensaje(Properties.Resources.lb_SolicitudATiMismo_);
                    return;
                }

                var respuestaEnviarSolicitudAmistad = _proxyGestionAmigos.EnviarSolicitudAmistad(
                    Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, nombreUsuario);

                if (respuestaEnviarSolicitudAmistad.Resultado)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(_proxyGestionJugador, ex, this);
            }

        }

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }
    }
}
