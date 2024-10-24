using DobbleGame.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
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
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Proxies;

namespace DobbleGame
{
    public partial class VentanaCambioNombre : Window
    {
        public VentanaCambioNombre()
        {
            InitializeComponent();
        }

        private void BtnActualizarUsuario(object sender, RoutedEventArgs e)
        {
            String nuevoNombre = tbNuevoNombre.Text.Trim();
            Servidor.GestionJugadorClient proxy = new Servidor.GestionJugadorClient();

            if(string.IsNullOrEmpty(nuevoNombre))
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
            }
            else
            {
                if (proxy.ExisteNombreUsuario(nuevoNombre))
                {
                    MostrarMensaje(Properties.Resources.lb_UsuarioExistente_);
                }
                else
                {
                    proxy.ModificarNombreUsuario(1, nuevoNombre);
                    this.Close();
                }
            }           
        }

        private void BtnCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MostrarMensaje(string mensaje)
        {
            advertenciaIcono.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void TbNuevoNombre(object sender, TextChangedEventArgs e)
        {

        }

    }
}
