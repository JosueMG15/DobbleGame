using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DobbleGame.Utilidades
{
    public static class Utilidades
    {
        public static bool ValidarContraseña(string contraseña)
        {

            if (contraseña.Contains(" "))
            {
                return false;
            }

            if (contraseña.Length < 8)
            {
                return false;
            }

            if (!contraseña.Any(Char.IsUpper))
            {
                return false;
            }

            if (contraseña.Count(Char.IsDigit) < 2)
            {
                return false;
            }

            return true;
        }

        public static bool EsCampoVacio(string contenido)
        {
            return string.IsNullOrWhiteSpace(contenido);
        }

        public static bool EsMismaContraseña(string contraseña, string contraseñaConfirmacion)
        {
            if (contraseña.Equals(contraseñaConfirmacion))
            {
                return true;
            }
            return false;
        }

        public static bool ValidarPatronCorreo(string correo)
        {
            string patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, patronCorreo);
        }
        public static System.Windows.Media.Brush StringABrush(string colorString)
        {
            var convertidorBrush = new BrushConverter();
            return (System.Windows.Media.Brush)convertidorBrush.ConvertFromString(colorString);
        }
        public static void MostrarVentanaErrorConexionBD(Window contenedor)
        {
            var ventanaErrorConexion = new VentanaErrorConexion(
                             Properties.Resources.lb_ErrorConexiónBD,
                             Properties.Resources.lb_MensajeErrorConexiónBD
                         )
            {
                Owner = contenedor,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            ventanaErrorConexion.ShowDialog();
        }
        public static void MostrarVentanaErrorConexionServidor(FrameworkElement contenedor)
        {
            var ventanaErrorConexion = new VentanaErrorConexion(
                 Properties.Resources.lb_ErrorConexiónServidor,
                 Properties.Resources.lb_MensajeErrorConexiónServidor
             )
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            }; 

            if (contenedor is Window ventana)
            {
                ventanaErrorConexion.Owner = ventana;
            }
            else if (contenedor is Page pagina && pagina.Parent is Window ventanaPrincipal)
            {
                ventanaErrorConexion.Owner = ventanaPrincipal;
            }

            ventanaErrorConexion.ShowDialog();
        }

        public static void MostrarMensajeStackPanel(StackPanel contenedor, Label lbMensaje, string mensaje)
        {
            contenedor.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        public static void ManejarExcepciones(ICommunicationObject proxy, Exception ex, FrameworkElement contenedor)
        {
            if (proxy != null)
            {
                try
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
                catch
                {
                    proxy.Abort();
                }
            }

            if (ex is CommunicationObjectFaultedException)
            {
                MostrarVentanaErrorConexionServidor(contenedor);
            }
            else if (ex is CommunicationException)
            {
                MostrarVentanaErrorConexionServidor(contenedor);
            }
            else if (ex is TimeoutException)
            {
                MostrarVentanaErrorConexionServidor(contenedor);
            }
            else
            {
                MostrarVentanaErrorConexionServidor(contenedor);
            }
        }
    }
}
