using DobbleGame.Utilidades;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
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

    public partial class VentanaRegistro : Window 
    {
        public VentanaRegistro()
        {
            InitializeComponent();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            IrMainWindow();
        }

        private void IrMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void BtnRegistrarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (!CamposValidos()) return;

            byte[] foto = CargarFotoDefecto();
            if (foto == null) return;

            string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(pbContraseña.Password);
            string correo = tbCorreo.Text;
            string usuario = tbNombreUsuario.Text;

            RegistrarUsuario(correo, usuario, contraseñaHasheada, foto);
        }

        public void RegistrarUsuario(string correo, string nombreUsuario, string contraseñaHasheada, byte[] foto)
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

                    Servidor.CuentaUsuario cuentaUsuario = new Servidor.CuentaUsuario
                    {
                        Correo = correo,
                        Usuario = nombreUsuario,
                        Contraseña = contraseñaHasheada,
                        Foto = foto
                    };

                    var respuestaCorreo = proxy.ExisteCorreoAsociado(cuentaUsuario.Correo);
                    if (respuestaCorreo.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaCorreo.Resultado)
                    {
                        Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_CorreoExistente_);
                        return;
                    }

                    var respuestaUsuario = proxy.ExisteNombreUsuario(cuentaUsuario.Usuario);
                    if (respuestaUsuario.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaUsuario.Resultado)
                    {
                        Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_UsuarioExistente_);
                        return;
                    }

                    var respuestaRegistro = proxy.RegistrarUsuario(cuentaUsuario);
                    if (respuestaRegistro.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    if (respuestaRegistro.Resultado)
                    {
                        VentanaRegistroExitoso ventanaRegistroExitoso = new VentanaRegistroExitoso
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        ventanaRegistroExitoso.ShowDialog();
                        IrMainWindow();
                    }
                    else
                    {
                        Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_ErrorInesperado);
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

        private bool CamposValidos()
        {
            if (TieneCamposVacios())
            {
                return false;
            }

            if (!ValidarPatronCorreo())
            {
                return false;
            }

            if (!ValidarContraseña())
            {
                return false;
            }

            if (!ValidarComparacionContraseña())
            {
                return false;
            }

            return true;
        }

        private bool TieneCamposVacios()
        {
            bool hayCaposVacios =
                Utilidades.Utilidades.EsCampoVacio(tbCorreo.Text) ||
                Utilidades.Utilidades.EsCampoVacio(tbNombreUsuario.Text) ||
                Utilidades.Utilidades.EsCampoVacio(pbContraseña.Password) ||
                Utilidades.Utilidades.EsCampoVacio(pbContraseñaConfirmada.Password);

            if (hayCaposVacios)
            {
                Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_CamposVacíos);
                return true;
            }

            return false;
        }

        private bool ValidarContraseña()
        {
            if (!Utilidades.Utilidades.ValidarContraseña(pbContraseña.Password))
            {
                Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_ContraseñaIncorrecta_);
                return false;
            }

            return true;
        }

        private bool ValidarComparacionContraseña()
        {
            bool validado = false;
            if (Utilidades.Utilidades.EsMismaContraseña(pbContraseña.Password, pbContraseñaConfirmada.Password))
            {
                validado = true;
            }
            else
            {
                Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_ContraseñaNoCoincide_);
            }
            return validado;
        }

        private bool ValidarPatronCorreo()
        {
            bool validado = false;
            
            if (Utilidades.Utilidades.ValidarPatronCorreo(tbCorreo.Text))
            {
                validado = true;
            }
            else
            {
                Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_CorreoInválido);
            }
            
            return validado;
        }

        private void PasswordBox_CambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

        private byte[] CargarFotoDefecto()
        {
            try
            {
                string rutaProyecto = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string rutaFotoDefecto = System.IO.Path.Combine(rutaProyecto, "Imagenes", "PerfilPorDefecto.png");

                if (!File.Exists(rutaFotoDefecto))
                {
                    Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, "No se encontró la imagen por defecto");
                    return null;
                }

                return File.ReadAllBytes(rutaFotoDefecto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource != panelMensaje && panelMensaje.Visibility == Visibility.Visible)
            {
                panelMensaje.Visibility = Visibility.Hidden;
            }
        }
    }
}
