﻿using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobbleGame.Extensiones
{
    public static class TextBoxExtensiones
    {
        public static readonly DependencyProperty PropiedadTextoSugerido =
            DependencyProperty.RegisterAttached(
                "TextoSugerido",
                typeof(string),
                typeof(TextBoxExtensiones),
                new FrameworkPropertyMetadata(string.Empty)
            );
        public static string GetTextoSugerido(DependencyObject obj)
        {
            return (string)obj.GetValue(PropiedadTextoSugerido);
        }

        public static void SetTextoSugerido(DependencyObject obj, string value)
        { 
            obj.SetValue(PropiedadTextoSugerido, value);
        }
    }
}