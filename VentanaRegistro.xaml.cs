using DobbleGame.Utilidades;
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
    /// <summary>
    /// Lógica de interacción para VentanaRegistro.xaml
    /// </summary>
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
            try
            {
                IniciarRegistro();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void IniciarRegistro()
        {
            if (TieneCamposVacios())
            {
                return;
            }

            if (!ValidarPatronCorreo())
            {
                return;
            }

            if (!ValidarContraseña())
            {
                return;
            }

            if (!ValidarComparacionContraseña())
            {
                return;
            }

            byte[] foto = CargarFotoDefecto();
            if (foto == null)
            {
                return;
            }

            string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(pbContraseña.Password);

            using (var proxy = new Servidor.GestionJugadorClient())
            {
                Servidor.CuentaUsuario cuentaUsuario = new Servidor.CuentaUsuario
                {
                    Correo = tbCorreo.Text,
                    Usuario = tbNombreUsuario.Text,
                    Contraseña = contraseñaHasheada,
                    Foto = foto
                };

                try
                {
                    if (proxy.ExisteCorreoAsociado(cuentaUsuario.Correo))
                    {
                        MostrarMensaje(Properties.Resources.lb_CorreoExistente_);
                        return;
                    }

                    if (proxy.ExisteNombreUsuario(cuentaUsuario.Usuario))
                    {
                        MostrarMensaje(Properties.Resources.lb_UsuarioExistente_);
                        return;
                    }

                    if (proxy.RegistrarUsuario(cuentaUsuario))
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
                        MostrarMensaje("Error inesperado durante el registro.");
                    }
                }
                catch (CommunicationException)
                {
                    var ventanaErrorConexion = new VentanaErrorConexion(
                             Properties.Resources.lb_ErrorConexiónServidor,
                             Properties.Resources.lb_MensajeErrorConexiónServidor
                         )
                    {
                        Owner = this,
                        WindowStartupLocation= WindowStartupLocation.CenterOwner
                    };
                    ventanaErrorConexion.ShowDialog();
                }
            }
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
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return true;
            }

            return false;
        }

        private bool ValidarContraseña()
        {
            if (!Utilidades.Utilidades.ValidarContraseña(pbContraseña.Password))
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaIncorrecta_);
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
                MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide_);
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
                MostrarMensaje(Properties.Resources.lb_CorreoInválido);
            }
            
            return validado;
        }

        private void PasswordBox_CambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

        private void MostrarMensaje(string mensaje)
        {
            panelMensaje.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private byte[] CargarFotoDefecto()
        {
            try
            {
                string rutaProyecto = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string rutaFotoDefecto = System.IO.Path.Combine(rutaProyecto, "Imagenes", "PerfilPorDefecto.png");

                if (!File.Exists(rutaFotoDefecto))
                {
                    MostrarMensaje("No se encontró la imagen por defecto");
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
