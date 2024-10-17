using MaterialDesignThemes.Wpf;
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
using System.Windows.Navigation;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para VentanaRecuperarContraseña.xaml
    /// </summary>
    public partial class VentanaRecuperarContraseña : Window
    {
        public VentanaRecuperarContraseña()
        {
            InitializeComponent();
        }


        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEnviarCodigo(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new PaginaIngresoCodigo());
        }
    }
}
