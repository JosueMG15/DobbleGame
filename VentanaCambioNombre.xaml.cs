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
            guardarCambioNombre(nuevoNombre);

            _paginaPerfil.ActualizarNombreUsuario(CuentaUsuario.cuentaUsuarioActual.Usuario);
            _ventanaMenu.ActualizarNombreUsuario(CuentaUsuario.cuentaUsuarioActual.Usuario);
        }

        private void guardarCambioNombre(string nuevoNombre)
        {
            try
            {
                Servidor.GestionJugadorClient proxy = new Servidor.GestionJugadorClient();

                if (string.IsNullOrEmpty(nuevoNombre))
                {
                    MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                }
                else
                {
                    if (proxy.ExisteNombreUsuario(nuevoNombre))
                    {
                        MostrarMensaje(Properties.Resources.lb_UsuarioExistente_);
                }
                    else
                    {
                        proxy.ModificarNombreUsuario(CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario, nuevoNombre);
                        CuentaUsuario.cuentaUsuarioActual.Usuario = nuevoNombre;
                        this.Close();
                    }
                }
            }
            catch (CommunicationException ex)
            {
                //Error de conexión con el servidor
                var ventanaErrorConexion = new VentanaErrorConexion(
                    Properties.Resources.lb_ErrorConexiónServidor,
                    Properties.Resources.lb_MensajeErrorConexiónServidor
                    )
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                ventanaErrorConexion.ShowDialog();
            }
            catch (SqlException ex)
            {
                //Error de conexión con la base de datos
                var ventanaErrorConexion = new VentanaErrorConexion(
                    Properties.Resources.lb_ErrorConexiónBD,
                    Properties.Resources.lb_MensajeErrorConexiónBD
                    )
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                ventanaErrorConexion.ShowDialog();
            }
            catch (Exception ex)
            {
                //Excepción generica
                MostrarMensaje("Ocurrió un error inesperado: " + ex.Message);
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
