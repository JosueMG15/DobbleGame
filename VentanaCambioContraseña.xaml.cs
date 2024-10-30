using DobbleGame.Servidor;
using DobbleGame.Utilidades;
using Dominio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
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

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para VentanaCambioContraseña.xaml
    /// </summary>
    public partial class VentanaCambioContraseña : Window
    {
        public VentanaCambioContraseña()
        {
            InitializeComponent();
        }


        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnActualizarContraseña(object sender, RoutedEventArgs e)
        {
            String contraseñaActual = pbContraseñaActual.Password.Trim();
            String nuevaContraseña = pbNuevaContraseña.Password.Trim();
            String confirmarNuevaContraseña = pbConfirmarNuevaContraseña.Password.Trim();

            ActualizarContraseña(contraseñaActual, nuevaContraseña, confirmarNuevaContraseña);
        }

        private void ActualizarContraseña(String contraseñaActual, String nuevaContraseña, String confirmarNuevaContraseña)
        {
            try
            {
                Servidor.GestionJugadorClient proxy = new Servidor.GestionJugadorClient();

                if (string.IsNullOrEmpty(contraseñaActual) || string.IsNullOrEmpty(nuevaContraseña) ||
                            string.IsNullOrEmpty(confirmarNuevaContraseña))
                {
                    MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                }
                else
                {
                    if (!proxy.ValidarContraseña(Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario, Utilidades.EncriptadorContraseña.GenerarHashSHA512(contraseñaActual)))
                    {
                        MostrarMensaje(Properties.Resources.lb_ContraseñaActualInvalida);
                    }
                    else
                    {
                        if (nuevaContraseña != confirmarNuevaContraseña)
                        {
                            MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide_);
                        }
                        else
                        {
                            if (Utilidades.Utilidades.ValidarContraseña(contraseñaActual) == true && Utilidades.Utilidades.ValidarContraseña(nuevaContraseña) == true
                                && Utilidades.Utilidades.ValidarContraseña(confirmarNuevaContraseña) == true)
                            {
                                string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(pbNuevaContraseña.Password);
                                proxy.ModificarContraseñaUsuario(Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario, contraseñaHasheada);
                                this.Close();
                            }
                            else
                            {
                                MostrarMensaje(Properties.Resources.lb_DatosInválidos);
                            }
                        }
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

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void PasswordBox_CambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

    }
}
