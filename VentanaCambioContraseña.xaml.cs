using DobbleGame.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
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

            ServidorDobble.GestionJugadorClient proxy = new ServidorDobble.GestionJugadorClient();

            if(string.IsNullOrEmpty(contraseñaActual) || string.IsNullOrEmpty(nuevaContraseña) ||
                        string.IsNullOrEmpty(confirmarNuevaContraseña))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
            }
            else
            {
                if (!proxy.ValidarContraseña(1, contraseñaActual))
                {
                    MostrarMensaje(Properties.Resources.lb_ContraseñaIncorrecta_);
                }
                else
                {
                    if (nuevaContraseña != confirmarNuevaContraseña)
                    {
                        MostrarMensaje(Properties.Resources.lb_ContraseñaNoCoincide_);
                    }
                    else
                    {
                        if(ValidarContraseña(contraseñaActual) == true && ValidarContraseña(nuevaContraseña) == true 
                            && ValidarContraseña(confirmarNuevaContraseña) == true)
                        {
                            proxy.ModificarContraseñaUsuario(1, nuevaContraseña);
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

        private bool ValidarContraseña(String contraseña)
        {
            if (contraseña.Contains(" "))
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaIncorrecta_);
                return false;
            }

            if (contraseña.Length < 8)
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaIncorrecta_);
                return false;
            }

            if (!contraseña.Any(Char.IsUpper))
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaIncorrecta_);
                return false;
            }

            if (contraseña.Count(Char.IsDigit) < 2)
            {
                MostrarMensaje(Properties.Resources.lb_ContraseñaIncorrecta_);
                return false;
            }
            return true;
        }

    }
}
