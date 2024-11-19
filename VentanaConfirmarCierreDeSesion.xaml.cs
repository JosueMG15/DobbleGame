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
    /// Lógica de interacción para VentanaConfirmarCierreDeSesion.xaml
    /// </summary>
    public partial class VentanaConfirmarCierreDeSesion : Window
    {
        public VentanaConfirmarCierreDeSesion()
        {
            InitializeComponent();
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            var proxyUsuario = new Servidor.GestionAmigosClient();
            try
            {
                proxyUsuario.QuitarUsuario(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxyUsuario, ex, this);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult= false;
        }
    }
}
