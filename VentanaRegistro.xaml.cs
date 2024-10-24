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
            this.Owner.Show();
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
            if (!TieneCamposVacios() && ValidarCorreo() && ValidarContraseña() && ValidarComparacionContraseña())
            {
                byte[] foto = CargarFotoDefecto();
                if (foto == null) return;

                string contraseñaHasheada = Utilidades.EncriptadorContraseña.GenerarHashSHA512(pbContraseña.Password);

                Servidor.GestionJugadorClient proxy = new Servidor.GestionJugadorClient();
                Servidor.CuentaUsuario cuentaUsuario = new Servidor.CuentaUsuario
                {
                    Correo = tbCorreo.Text,
                    Usuario = tbNombreUsuario.Text,
                    Contraseña = contraseñaHasheada,
                    Foto = CargarFotoDefecto(),
                };

                if (!proxy.ExisteCorreoAsociado(cuentaUsuario.Correo))
                {
                    if (!proxy.ExisteNombreUsuario(cuentaUsuario.Usuario))
                    {
                        if (proxy.RegistrarUsuario(cuentaUsuario))
                        {
                            MostrarMensaje(Properties.Resources.lb_RegistroExitoso);
                        }
                        else
                        {
                            MostrarMensaje("Error inesperado");
                        }
                    }
                    else
                    {
                        MostrarMensaje(Properties.Resources.lb_UsuarioExistente_);
                    }
                }
                else
                {
                    MostrarMensaje(Properties.Resources.lb_CorreoExistente_);
                }

            }
        }

        private bool TieneCamposVacios()
        {
            bool hayCaposVacios =
                string.IsNullOrWhiteSpace(tbCorreo.Text) ||
                string.IsNullOrWhiteSpace(tbNombreUsuario.Text) ||
                string.IsNullOrWhiteSpace(pbContraseña.Password) ||
                string.IsNullOrWhiteSpace(pbContraseñaConfirmada.Password);

            if (hayCaposVacios)
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return true;
            }

            return false;
        }

        private bool ValidarContraseña()
        {
            if (!Utilidades.Utilidades.ValidarContraseña(pbContraseña))
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaIncorrecta_);
                return false;
            }

            return true;
        }

        private bool ValidarComparacionContraseña()
        {
            bool validado = false;
            String contraseña = pbContraseña.Password;
            if (contraseña.Equals(pbContraseñaConfirmada.Password))
            {
                validado = true;
            }
            else
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide_);
            }
            return validado;
        }

        private bool ValidarCorreo()
        {
            bool validado = false;
            string patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (Regex.Match(tbCorreo.Text, patronCorreo).Success)
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
