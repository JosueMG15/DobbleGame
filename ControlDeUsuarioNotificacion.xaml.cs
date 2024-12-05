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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para ControlDeUsuarioNotificacion.xaml
    /// </summary>
    public partial class ControlDeUsuarioNotificacion : UserControl
    {
        private static ControlDeUsuarioNotificacion instancia;

        public static ControlDeUsuarioNotificacion Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ControlDeUsuarioNotificacion();
                }

                return instancia;
            }
        }

        public ControlDeUsuarioNotificacion()
        {
            InitializeComponent();
        }

        public void MostrarNotificacion(string mensaje)
        {
            tbMensaje.Text = mensaje;
            Notificacion.IsOpen = true;

            Task.Delay(5000).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() => Notificacion.IsOpen = false);
            });
        }
    }
}
