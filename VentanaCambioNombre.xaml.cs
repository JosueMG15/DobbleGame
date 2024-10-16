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
    /// Lógica de interacción para VentanaCambioNombre.xaml
    /// </summary>
    public partial class VentanaCambioNombre : Window
    {
        public VentanaCambioNombre()
        {
            InitializeComponent();
        }

        private void BtnActualizarUsuario(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
