using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Lógica de interacción para VentanaConfirmarExpulsion.xaml
    /// </summary>
    public partial class VentanaConfirmarExpulsion : Window
    {
        public VentanaConfirmarExpulsion(string nombreUsuario)
        {
            InitializeComponent();
            tbMensaje.Text = String.Format(Properties.Resources.lb_MensajeExpulsarJugador, nombreUsuario);
        }

        private void BtnExpulsar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult= false;
            this.Close();
        }

    }
}
