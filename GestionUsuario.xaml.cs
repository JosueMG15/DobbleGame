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
    /// Lógica de interacción para GestionUsuario.xaml
    /// </summary>
    public partial class GestionUsuario : UserControl
    {
        public GestionUsuario()
        {
            InitializeComponent();
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Window ventanaActual = Window.GetWindow(this);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            if (ventanaActual != null)
            {
                ventanaActual.Close();
            }
        }

        private void BtnIrPerfil_Click(object sender, RoutedEventArgs e)
        {
            Window ventanaActual = Window.GetWindow(this);
            VentanaPerfil ventanaPerfil = new VentanaPerfil();
            ventanaPerfil.Show();
            if (ventanaActual != null) 
            {
                ventanaActual.Close();
            }
        }
    }
}
