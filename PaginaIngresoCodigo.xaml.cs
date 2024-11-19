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
    /// Lógica de interacción para PaginaIngresoCodigo.xaml
    /// </summary>
    public partial class PaginaIngresoCodigo : Page
    {
        VentanaRecuperarContraseña _marcoPrincipal;
        string _correo;
        String _codigo;
        public PaginaIngresoCodigo(VentanaRecuperarContraseña marcoPrincipal, string correo, String codigo)
        {
            InitializeComponent();
            _marcoPrincipal = marcoPrincipal;
            _correo = correo;
            _codigo = codigo;
        }

        private void BtnAceptar(object sender, RoutedEventArgs e)
        {
            using (var proxy = new Servidor.GestionCorreosClient())
            {
                try
                {
                    String codigo = tbCodigoSala.Text.Trim();
                    if (Utilidades.Utilidades.EsCampoVacio(codigo))
                    {
                        MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                        return;
                    }

                    if (codigo == _codigo)
                    {
                        var paginaNuevaContraseña = new PaginaNuevaContraseña(_marcoPrincipal, _correo);
                        this.NavigationService.Navigate(paginaNuevaContraseña);
                    }
                    else
                    {
                        MostrarMensaje(Properties.Resources.lb_CodigoInvalido);
                    }

                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }

            }
        }

        private void BtnReintentar(object sender, RoutedEventArgs e)
        {
            EnviarCodigo(_correo);
        }

        private void EnviarCodigo(string correo)
        {
            using (var proxy = new Servidor.GestionCorreosClient())
            {
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    string codigo = GenerarCodigo();
                    _codigo = codigo;
                    var respuesta = proxy.EnviarCodigo(correo, codigo);
                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(_marcoPrincipal);
                        return;
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
