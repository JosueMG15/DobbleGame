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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para PaginaRecuperarContraseña.xaml
    /// </summary>
    public partial class PaginaRecuperarContraseña : Page
    {
        VentanaRecuperarContraseña _marcoPrincipal;
        public PaginaRecuperarContraseña(VentanaRecuperarContraseña marcoPrincipal)
        {
            InitializeComponent();
            _marcoPrincipal = marcoPrincipal;   
        }

        private void BtnEnviarCodigo(object sender, RoutedEventArgs e)
        {
            String correo = tbCorreo.Text.Trim();
            EnviarCodigo(correo);
        }

        private void EnviarCodigo(string correo)
        {
            using (var proxy = new Servidor.GestionCorreosClient())
            {
                if (Utilidades.Utilidades.EsCampoVacio(correo))
                {
                    MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                    return;
                }
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    if (ValidarCorreo(correo) == false)
                    {
                        MostrarMensaje(Properties.Resources.lb_CorreoNoExiste_);
                        return;
                    }

                    string codigo = GenerarCodigo();
                    var respuesta = proxy.EnviarCodigo(correo, codigo);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(_marcoPrincipal);
                        return;
                    }

                    if (respuesta.Resultado)
                    {
                        PaginaIngresoCodigo paginaIngresoCodigo = new PaginaIngresoCodigo(_marcoPrincipal, correo, codigo);
                        this.NavigationService.Navigate(paginaIngresoCodigo);
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
        }

        public string GenerarCodigo()
        {
            return new Random().Next(100000, 999999).ToString(); // Código de 6 dígitos
        }

        public bool ValidarCorreo(string correo)
        {
            using (var proxy = new Servidor.GestionJugadorClient())
            {
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    var respuesta = proxy.ExisteCorreoAsociado(correo);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(_marcoPrincipal);
                        return false;
                    }

                    if (respuesta.Resultado)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                    return false;
                }
            }
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            _marcoPrincipal.Close();
        }

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }
    }
}
