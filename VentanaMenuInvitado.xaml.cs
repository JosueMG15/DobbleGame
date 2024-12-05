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
    /// Lógica de interacción para VentanaMenuInvitado.xaml
    /// </summary>
    public partial class VentanaMenuInvitado : Window
    {
        public VentanaMenuInvitado()
        {
            InitializeComponent();
            MarcoPrincipal.NavigationService.Navigate(new PaginaMenu());
        }

        private void BtnCerrarSesion(object sender, RoutedEventArgs e)
        {
            VentanaModalDecision ventana = new VentanaModalDecision(Properties.Resources.lb_MensajeCerrarSesion,
                Properties.Resources.btn_CerrarSesión, Properties.Resources.global_Cancelar);
            bool? respuesta = ventana.ShowDialog();

            if (respuesta == true)
            {
                MainWindow mainWindow = new MainWindow();
                this.Close();
                mainWindow.Show();
            }
        }
    }
}
