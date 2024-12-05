using DobbleGame.Servidor;
using DobbleGame.Utilidades;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
    /// Lógica de interacción para VentanaErrorBD.xaml
    /// </summary>
    public partial class VentanaErrorBD : Window
    {
        public VentanaErrorBD(string titulo, string mensaje)
        {
            InitializeComponent();
            lbTitulo.Content = titulo;
            tbMensaje.Text = mensaje;
        }

        private void BtnReintentar_Click(object sender, RoutedEventArgs e)
        {
            var proxy = new GestionAmigosClient();
            try
            {
                var respuestaUsuario = proxy.ObtenerSolicitud();
                if (respuestaUsuario.ErrorBD)
                {
                    return;
                }
                this.Close();
            }
            catch(Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
            }
        }

        private void BtnInicioSesion_Click(object sender, RoutedEventArgs e)
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
                    break; // Salir del bucle al encontrar la ventana
                }
            }

            try
            {
                if (!isLoginWindowOpen)
                {
                    proxy.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                    CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    proxyUsuario.NotificarDesconexion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
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
