using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    }
}
