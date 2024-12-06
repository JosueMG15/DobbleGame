using DobbleGame.Servidor;
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
        private GestionAmigosClient _proxyGestionAmigos = new GestionAmigosClient();
        private VentanaMenu _ventanaMenu;
        private Dominio.Amistad _amistad;
        private Border _panelSolicitud;

        public VentanaEliminarAmigo(VentanaMenu ventanaMenu, Dominio.Amistad amistad, Border panelSolicitud)
        {
            _ventanaMenu = ventanaMenu;
            _amistad = amistad;
            _panelSolicitud = panelSolicitud;
            InitializeComponent();
        }

        private void BtnAceptar(object sender, RoutedEventArgs e)
        {
            try
            {
                var respuestaUsuarioPrincipal = _proxyGestionAmigos.ObtenerUsuario(_amistad.UsuarioPrincipalId);
                if (respuestaUsuarioPrincipal.ErrorBD)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                    return;
                }
                var cuentaPrincipal = respuestaUsuarioPrincipal.Resultado;


                var respuestaUsuarioAmigo = _proxyGestionAmigos.ObtenerUsuario(_amistad.UsuarioAmigoId);
                if (respuestaUsuarioAmigo.ErrorBD)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                    return;
                }
                var cuentaAmigo = respuestaUsuarioAmigo.Resultado;

                var respuesta = _proxyGestionAmigos.EliminarAmistad(_amistad.IdAmistad, cuentaPrincipal.Usuario, cuentaAmigo.Usuario);
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
                Utilidades.Utilidades.ManejarExcepciones(_proxyGestionAmigos, ex, this);
            }
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
