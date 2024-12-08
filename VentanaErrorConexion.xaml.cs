using DobbleGame.Servidor;
using DobbleGame.Utilidades;
using System;
using System.Windows;

namespace DobbleGame
{
    public partial class VentanaErrorConexion : Window
    {
        public VentanaErrorConexion(string titulo, string mensaje)
        {
            InitializeComponent();
            lbTitulo.Content = titulo;
            tbMensaje.Text = mensaje;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            MainWindow inicioSesion = new MainWindow();
            var proxy = new GestionJugadorClient();
            var proxyUsuario = new GestionAmigosClient();

            bool isLoginWindowOpen = false;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    isLoginWindowOpen = true;
                    break;
                }
            }

            try
            {
                if (!isLoginWindowOpen)
                {
                    proxy.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                    if (!Dominio.CuentaUsuario.CuentaUsuarioActual.EsInvitado)
                    {
                        CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                        proxyUsuario.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    }
                }

                proxy.Close();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != inicioSesion)
                    {
                        window.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepcionErrorConexion(ex, proxy);
            }

            inicioSesion.Show();
        }
    }
}
