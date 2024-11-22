using DobbleGame.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Proxies;
using Dominio;
using System.Windows.Navigation;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.ServiceModel;

namespace DobbleGame
{
    public partial class VentanaCambioNombre : Window
    {
        private PaginaPerfil _paginaPerfil;
        private VentanaMenu _ventanaMenu;
        public VentanaCambioNombre(PaginaPerfil paginaPerfil, VentanaMenu ventanaMenu)
        {
            InitializeComponent();
            _paginaPerfil = paginaPerfil;
            _ventanaMenu = ventanaMenu;
        }

        private void BtnActualizarUsuario(object sender, RoutedEventArgs e)
        {
            String nuevoNombre = tbNuevoNombre.Text.Trim();
            GuardarCambioNombre(nuevoNombre);

            _paginaPerfil.ActualizarNombreUsuario(CuentaUsuario.CuentaUsuarioActual.Usuario);
            _ventanaMenu.ActualizarNombreUsuario(CuentaUsuario.CuentaUsuarioActual.Usuario);
        }

        private void GuardarCambioNombre(string nuevoNombre)
        {
            if (Utilidades.Utilidades.EsCampoVacio(nuevoNombre))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return;
            }

            using (var proxy = new Servidor.GestionJugadorClient())
            {
                try
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                        throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                    }

                    var respuestaUsuario = proxy.ExisteNombreUsuario(nuevoNombre);
                    if (respuestaUsuario.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaUsuario.Resultado)
                    {
                        MostrarMensaje(Properties.Resources.lb_UsuarioExistente_);
                        return;
                    }

                    var respuestaModificarUsuario = proxy.ModificarNombreUsuario(CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario, nuevoNombre);
                    if (respuestaModificarUsuario.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaModificarUsuario.Resultado)
                    {
                        var proxyUsuario = new Servidor.GestionAmigosClient();

                        CuentaUsuario.CuentaUsuarioActual.Usuario = nuevoNombre;
                        CuentaUsuario.CuentaUsuarioActual = new CuentaUsuario
                        {
                            IdCuentaUsuario = Dominio.CuentaUsuario.CuentaUsuarioActual.IdCuentaUsuario,
                            Usuario = nuevoNombre,
                            Correo = Dominio.CuentaUsuario.CuentaUsuarioActual.Correo,
                            Contraseña = Dominio.CuentaUsuario.CuentaUsuarioActual.Contraseña,
                            Foto = Dominio.CuentaUsuario.CuentaUsuarioActual.Foto,
                            Puntaje = Dominio.CuentaUsuario.CuentaUsuarioActual.Puntaje,
                            Estado = true,
                        };

                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }

            }
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void TbNuevoNombre(object sender, TextChangedEventArgs e)
        {

        }

    }
}
