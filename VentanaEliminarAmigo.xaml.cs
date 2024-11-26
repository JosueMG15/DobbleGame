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
    /// Lógica de interacción para VentanaEliminarAmigo.xaml
    /// </summary>
    public partial class VentanaEliminarAmigo : Window
    {
        VentanaMenu _ventanaMenu;
        Dominio.Amistad _amistad;
        Border _panelSolicitud;
        public VentanaEliminarAmigo(VentanaMenu ventanaMenu, Dominio.Amistad amistad, Border panelSolicitud)
        {
            _ventanaMenu = ventanaMenu;
            _amistad = amistad;
            _panelSolicitud = panelSolicitud;
            InitializeComponent();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            using (var proxy = new Servidor.GestionAmigosClient())
            {
                try
                {
                    var estaConectado = proxy.UsuarioConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
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

                    var respuestaUsuarioPrincipal = proxy.ObtenerUsuario(_amistad.UsuarioPrincipalId);
                    if (respuestaUsuarioPrincipal.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }

                    var cuentaPrincipal = respuestaUsuarioPrincipal.Resultado;


                    //Usuario amigo de la amistad
                    var respuestaUsuarioAmigo = proxy.ObtenerUsuario(_amistad.UsuarioAmigoId);
                    var cuentaAmigo = respuestaUsuarioAmigo.Resultado;

                    var respuesta = proxy.EliminarAmistad(_amistad.IdAmistad, cuentaPrincipal.Usuario, cuentaAmigo.Usuario);

                    if (respuesta.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }

                    if (respuesta.Resultado)
                    {
                        _ventanaMenu.ContenedorNotificaciones.Children.Remove(_panelSolicitud);
                        this.Close();
                    }

                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
