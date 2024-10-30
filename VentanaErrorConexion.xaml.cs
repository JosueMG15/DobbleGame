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
    /// Lógica de interacción para VentanaErrorConexion.xaml
    /// </summary>
    public partial class VentanaErrorConexion : Window
    {
        public VentanaErrorConexion(string titulo, string mensaje)
        {
            InitializeComponent();
            lbTitulo.Content = titulo;
            tbMensaje.Text = mensaje;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            MainWindow inicioSesion = new MainWindow();

            foreach (Window window in Application.Current.Windows)
            {
                if(window != inicioSesion)
                {
                    window.Close();
                }
            }
            inicioSesion.Show();
        }
    }
}
