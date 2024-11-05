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
    public partial class VentanaIngresoCodigoSala : Window
    {
        public PaginaSala Sala { get; set; }
        public VentanaIngresoCodigoSala()
        {
            InitializeComponent();
        }

        private void BtnAceptar(object sender, RoutedEventArgs e)
        {

            if (!String.IsNullOrWhiteSpace(tbCodigoSala.Text))
            {
                PaginaSala paginaSala = new PaginaSala(false, tbCodigoSala.Text);

                if (paginaSala.HayConexionConSala)
                {
                    Sala = paginaSala;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo unir a la sala");
                }
            }
            else
            {
                Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_CamposVacíos);
            }
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
