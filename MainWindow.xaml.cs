using DobbleGame.Utilidades;
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
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_CambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

        private void BtnEntrarMenu_Click(object sender, RoutedEventArgs e)
        {
            VentanaMenu ventanaMenu = new VentanaMenu();
            this.Close();
            ventanaMenu.Show();
        }

        private void ClicCrearCuentaTf(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VentanaRegistro ventanaRegistro = new VentanaRegistro();
            ventanaRegistro.Owner = this;
            ventanaRegistro.Show();
            this.Hide();
        }

        private void ClicRecuperarContraseñaTf(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VentanaRecuperarContraseña ventanaRecuperarContraseña = new VentanaRecuperarContraseña();
            ventanaRecuperarContraseña.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaRecuperarContraseña.ShowDialog();
        }
    }
}
