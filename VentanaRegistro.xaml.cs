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
            if (!TieneCamposVacios() && ValidarCorreo() && ValidarComparacionContraseña())
            {
                byte[] foto = CargarFotoDefecto();
                if (foto == null) return;

                ServidorDobble.GestionJugadorClient proxy = new ServidorDobble.GestionJugadorClient();
                ServidorDobble.CuentaUsuario cuentaUsuario = new ServidorDobble.CuentaUsuario
                {
                    Correo = tbCorreo.Text.Trim(),
                    Usuario = tbNombreUsuario.Text.Trim(),
                    Contraseña = tbContraseña.Password.Trim(),
                    Foto = CargarFotoDefecto(),
                };
                
                if (!proxy.ExisteCorreoAsociado(cuentaUsuario.Correo))
                {
                    if (!proxy.ExisteNombreUsuario(cuentaUsuario.Usuario))
                    {
                        if (proxy.RegistrarUsuario(cuentaUsuario))
                        {
                            MostrarMensaje("Cuenta creada con éxito");
                        }
                        else
                        {
                            MostrarMensaje("Error inesperado");
                        }
                    }
                    else
                    {
                        MostrarMensaje("El nombre de usuario ya existe");
                    }
                }
                else
                {
                    MostrarMensaje("El correo ya se encuentra \nasociado a una cuenta");
                }
                
            }
        }

        private bool TieneCamposVacios()
        {
            bool hayCaposVacios =
                string.IsNullOrWhiteSpace(tbCorreo.Text) ||
                string.IsNullOrWhiteSpace(tbNombreUsuario.Text) ||
                string.IsNullOrWhiteSpace(tbContraseña.Password) ||
                string.IsNullOrWhiteSpace(tbContraseñaConfirmada.Password);

            if (hayCaposVacios)
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
                return true;
            }

            return false;
        }

        private bool ValidarComparacionContraseña()
        {
            bool validado = false;
            String contraseña = tbContraseña.Password.Trim();
            if (contraseña.Equals(tbContraseñaConfirmada.Password.Trim()))
            {
                validado = true;
            }
            else
            {
                MostrarMensaje("Las contraseñas no coinciden");
            }
            return validado;
        }

        private bool ValidarCorreo()
        {
            bool validado = false;
            string patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (Regex.Match(tbCorreo.Text.Trim(), patronCorreo).Success)
            {
                validado = true;
            }
            else
            {
                MostrarMensaje("Correo no válido");
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
                string rutaFotoDefecto = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Imagenes", "PerfilPorDefecto.png");

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
    }
}
