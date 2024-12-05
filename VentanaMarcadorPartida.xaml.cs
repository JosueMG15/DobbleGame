using DobbleGame.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para VentanaMarcadorPartida.xaml
    /// </summary>
    public partial class VentanaMarcadorPartida : Window
    {
        public VentanaMarcadorPartida(List<Jugador> resultadoJugadores)
        {
            InitializeComponent();
            MarcadorFinal.ItemsSource = resultadoJugadores;

            if (resultadoJugadores != null )
            {
                lbGanador.Content = resultadoJugadores.FirstOrDefault().Usuario;
            }
            
        }

        private void BtnIrSala(object sender, RoutedEventArgs e)
        {
            var proxyGestionAmigos = new GestionAmigosClient();
            try
            {
                this.DialogResult = true;
                proxyGestionAmigos.NotificarCambios();
                this.Close();
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxyGestionAmigos, ex, this);
            }
        }
    }
}
