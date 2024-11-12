using DobbleGame.Servidor;
using DobbleGame.Utilidades;
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
    /// Lógica de interacción para VentanaErrorConexion.xaml
    /// </summary>
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

            try
            {
                if (proxy != null && proxy.State == CommunicationState.Opened)
                {
                    proxy.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                    proxy.Close();
                }
                
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != inicioSesion)
                    {
                        window.Close();
                    }
                }
            }
            catch (CommunicationException ex)
            {
                Registro.Error($"Estado del proxy: {proxy.State}. \nExcepción de CommunicationException: {ex.Message}." +
                               $"\nTraza: {ex.StackTrace}. \nFuente: {ex.Source}");
                proxy.Abort();
            }
            catch (TimeoutException ex)
            {
                Registro.Error($"Estado del proxy: {proxy.State}. \nExcepción de TimeoutException: {ex.Message}." +
                               $"\nTraza: {ex.StackTrace}. \nFuente: {ex.Source}");
                proxy.Abort();
            }
            catch (Exception ex)
            {
                Registro.Error($"Estado del proxy: {proxy.State}. \nExcepción no manejada: {ex.Message}." +
                               $"\nTraza: {ex.StackTrace}. \nFuente: {ex.Source}");
                proxy.Abort();
            }

            
            inicioSesion.Show();
        }
    }
}
