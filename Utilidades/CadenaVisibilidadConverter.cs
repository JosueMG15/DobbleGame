using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobbleGame.Utilidades
{
    public class CadenaVisibilidadConverter : IValueConverter
    {
        public object Convert(object valor, Type tipoObjetivo, object parametro, CultureInfo cultura) 
        {
            if (valor is string texto && string.IsNullOrEmpty(texto))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object valor, Type tipoObjetivo, object parametro, CultureInfo cultura)
        {
            throw new NotImplementedException();
        }
    }
}
