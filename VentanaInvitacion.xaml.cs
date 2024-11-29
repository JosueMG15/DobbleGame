using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
    /// Lógica de interacción para VentanaInvitacion.xaml
    /// </summary>
    public partial class VentanaInvitacion : Window
    {
        public VentanaInvitacion(string nombreUsuario)
        {
            InitializeComponent();
            lbTitulo.Content = "¡" + nombreUsuario + " " + Properties.Resources.titulo_TeEstaInvitando_;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
