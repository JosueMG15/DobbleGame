using DobbleGame.Utilidades;
using System;
using System.Collections.Generic;
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
                ServidorDobble.GestionJugadorClient proxy = new ServidorDobble.GestionJugadorClient();
                ServidorDobble.CuentaUsuario cuentaUsuario = new ServidorDobble.CuentaUsuario
                {
                    Correo = tbCorreo.Text.Trim(),
                    Usuario = tbNombreUsuario.Text.Trim(),
                    Contraseña = tbContraseña.Password.Trim(),
                };
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
        }

        private bool TieneCamposVacios()
        {
            bool validado = true;
            String correo = tbCorreo.Text.Trim();
            String nombreUsuario = tbNombreUsuario.Text.Trim();
            String contraseña = tbContraseña.Password.Trim();
            String contraseñaConfirmada = tbContraseñaConfirmada.Password.Trim();

            if (!String.IsNullOrEmpty(correo) && !String.IsNullOrEmpty(nombreUsuario) && !String.IsNullOrEmpty(contraseña) 
                && !String.IsNullOrEmpty(contraseñaConfirmada))
            {
                validado = false;
            }
            else
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
            }

            return validado;
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
                MostrarMensaje("Las contraseña no coinciden");
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
    }
}
