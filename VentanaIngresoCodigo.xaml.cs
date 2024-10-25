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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para PaginaIngresoCodigo.xaml
    /// </summary>
    public partial class VentanaIngresoCodigo : Window
    {
        public PaginaSala Sala { get; private set; }
        public VentanaIngresoCodigo()
        {
            InitializeComponent();
        }

        private void BtnAceptar(object sender, RoutedEventArgs e)
        {

            if (!String.IsNullOrWhiteSpace(tbCodigoSala.Text))
            {
                PaginaSala paginaSala = new PaginaSala()
                {
                    EsNuevaSala = false,
                    CodigoSala = tbCodigoSala.Text,
                };

                if (paginaSala.CrearSala())
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
                Console.WriteLine("Campos vacios");
            }

        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnReintentar(object sender, RoutedEventArgs e)
        {

        }


    }
}
