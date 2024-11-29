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
    /// Lógica de interacción para VentanaConfirmarIrPerfil.xaml
    /// </summary>
    public partial class VentanaModalDecision : Window
    {
        public VentanaModalDecision(string mensaje)
        {
            InitializeComponent();
            tbMensaje.Text = mensaje;
            btnAceptar.Content = Properties.Resources.global_Aceptar;
            btnCancelar.Content = Properties.Resources.global_Cancelar;
        }

        public VentanaModalDecision(string mensaje, string nombreBotonAceptar, string nombreBotonCancelar)
        {
            InitializeComponent();
            tbMensaje.Text = mensaje;
            btnAceptar.Content = nombreBotonAceptar;
            btnCancelar.Content = nombreBotonCancelar;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
