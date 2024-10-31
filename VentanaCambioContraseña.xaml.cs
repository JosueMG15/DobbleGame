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
            if (Utilidades.Utilidades.EsCampoVacio(contraseñaActual) || Utilidades.Utilidades.EsCampoVacio(nuevaContraseña) 
                || Utilidades.Utilidades.EsCampoVacio(confirmarNuevaContraseña))
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

                    var respuestaUsuario = proxy.ValidarContraseña(Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario,
                        Utilidades.EncriptadorContraseña.GenerarHashSHA512(contraseñaActual));

                    if (respuestaUsuario.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (!respuestaUsuario.Resultado)
                    {
                        MostrarMensaje(Properties.Resources.lb_ContraseñaActualInvalida);
                        return;
                    }

                    if(nuevaContraseña != confirmarNuevaContraseña)
                    {
                        MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide_);
                        return;
                    }

                    if (Utilidades.Utilidades.ValidarContraseña(contraseñaActual) && Utilidades.Utilidades.ValidarContraseña(nuevaContraseña) 
                        && Utilidades.Utilidades.ValidarContraseña(confirmarNuevaContraseña))
                    {
                        string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(pbNuevaContraseña.Password);
                        var respuestaModificarContraseña = proxy.ModificarContraseñaUsuario(Dominio.CuentaUsuario.cuentaUsuarioActual.IdCuentaUsuario, contraseñaHasheada);

                        if (respuestaModificarContraseña.ErrorBD)
                        {
                            Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                            return;
                        }
                        if (respuestaModificarContraseña.Resultado)
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        MostrarMensaje(Properties.Resources.lb_DatosInválidos);
                        return;
                    }                
                }
                catch (CommunicationObjectFaultedException faultEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error en el objeto de comunicación: {faultEx.Message}");
                }
                catch (CommunicationException commEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error de comunicación: {commEx.Message}");
                }
                catch (TimeoutException timeoutEx)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error de tiempo de espera: {timeoutEx.Message}");
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }
                finally
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {
                        proxy.Abort();
                    }
                    else
                    {
                        proxy.Close();
                    }
                }

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
