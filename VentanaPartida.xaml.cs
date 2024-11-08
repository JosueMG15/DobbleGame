using DobbleGame.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class VentanaPartida : Window, Servidor.IGestionPartidaCallback
    {
        private Servidor.IGestionPartida proxy;
        public ObservableCollection<CuentaUsuario> UsuariosEnPartida { get; set; }
        private Window VentanaMenu;
        public string CodigoSala {  get; set; }
        public bool HayConexionPartida { get; set; }

        public VentanaPartida(string codigoSala, Window ventanaMenu)
        {
            InicializarProxy();
            InitializeComponent();
            this.DataContext = this;
            UsuariosEnPartida = new ObservableCollection<CuentaUsuario>();
            HayConexionPartida = false;
            CodigoSala = codigoSala;
            VentanaMenu = ventanaMenu;
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            InicializarProxy();

            try
            {
                var usuarioActual = Dominio.CuentaUsuario.cuentaUsuarioActual;
                proxy.AbandonarPartida(usuarioActual.Usuario, CodigoSala);

                ((ICommunicationObject)proxy).Close();
                VentanaMenu.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }
        }

        public bool CrearPartida()
        {
            InicializarProxy();

            bool resultado = false;

            try
            {
                resultado = proxy.CrearNuevaPartida(CodigoSala);

                if (resultado)
                {
                    proxy.UnirJugadoresAPartida(CodigoSala);
                    HayConexionPartida = true;
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }

            return resultado;
        }

        private void Icon_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void InicializarProxy()
        {
            if (proxy != null && ((ICommunicationObject)proxy).State == CommunicationState.Opened)
            {
                return; 
            }

            if (proxy != null && ((ICommunicationObject)proxy).State == CommunicationState.Faulted)
            {
                ((ICommunicationObject)proxy).Abort();
            }

            var contexto = new InstanceContext(this);
            var factory = new DuplexChannelFactory<IGestionPartida>(contexto, "NetTcpBinding_IGestionPartida");
            proxy = factory.CreateChannel();
        }

        public void ActualizarJugadoresEnPartida(CuentaUsuario[] jugadoresConectados)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UsuariosEnPartida.Clear();
                foreach (var jugador in jugadoresConectados)
                {
                    UsuariosEnPartida.Add(jugador);
                }
            });
        }
    }
}
