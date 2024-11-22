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
            var codigoSala = tbCodigoSala.Text;

            if (!String.IsNullOrWhiteSpace(codigoSala))
            {
                PaginaSala paginaSala = new PaginaSala(false, codigoSala);

                if (paginaSala.ExisteSala())
                {
                    if (paginaSala.EsSalaDisponible())
                    {
                        if (paginaSala.HayEspacioEnSala())
                        {
                            if (paginaSala.IniciarSesionSala())
                            {
                                Sala = paginaSala;
                                this.DialogResult = true;
                                this.Close();
                            }
                            else
                            {
                                Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_ErrorInesperado);
                            }
                        }
                        else
                        {
                            Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_SalaLlena);
                        }
                    }
                    else
                    {
                        Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_SalaEnPartida);
                    }
                }
                else
                {
                    Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_SalaInexistente);
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

        private void Window_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource != panelMensaje && panelMensaje.Visibility == Visibility.Visible)
            {
                panelMensaje.Visibility = Visibility.Hidden;
            }
        }
    }
}
