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
using System.Windows.Threading;

namespace DobbleGame
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class VentanaPartida : Window, Servidor.IGestionPartidaCallback
    {
        private Servidor.IGestionPartida proxy;
        private DispatcherTimer cronometro;
        private int contador;
        public ObservableCollection<CuentaUsuario> UsuariosEnPartida { get; set; }
        private readonly Window _ventanaMenu;
        public string CodigoSala {  get; set; }
        public bool HayConexionPartida { get; set; }
        public string Contador {  get; set; }

        public VentanaPartida(string codigoSala, Window ventanaMenu)
        {
            InicializarProxySiEsNecesario();
            InitializeComponent();
            this.DataContext = this;
            UsuariosEnPartida = new ObservableCollection<CuentaUsuario>();
            HayConexionPartida = false;
            CodigoSala = codigoSala;
            _ventanaMenu = ventanaMenu;
            IniciarContador();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            InicializarProxySiEsNecesario();

            try
            {
                var usuarioActual = Dominio.CuentaUsuario.CuentaUsuarioActual;
                proxy.AbandonarPartida(usuarioActual.Usuario, CodigoSala);

                ((ICommunicationObject)proxy).Close();
                _ventanaMenu.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }
        }

        public bool CrearPartida()
        {
            InicializarProxySiEsNecesario();

            bool resultado = false;

            try
            {
                resultado = proxy.CrearNuevaPartida(CodigoSala);

                if (resultado)
                {
                    proxy.UnirJugadoresAPartida(CodigoSala);
                    proxy.NotificarActualizacionDeJugadoresEnPartida(CodigoSala);
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

        private void InicializarProxySiEsNecesario()
        {
            if (proxy == null || ((ICommunicationObject)proxy).State != CommunicationState.Opened)
            {
                InicializarProxy();
            }
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

            //IniciarContador();
        }

        private void IniciarContador()
        {
            contador = 10;
            cronometro = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            cronometro.Tick += Cronometro_Tick;
            cronometro.Start();
        }

        private void Cronometro_Tick(object sender, EventArgs e)
        {
            if (contador > 0)
            {
                tbContador.Text = contador.ToString();
                contador--;
                
            }
            else
            {
                cronometro.Stop();
                tbContador.Visibility = Visibility.Collapsed;
            }
        }

        //
        // String casa = "[DobblGame/Iconos/Casa]";
        // *presionar boton*
        // if(boton.

        // FICHA array[8 iconos]
        // PARTIDA array [58 fichas]

        // for each (array [fichas] 
        // {
        //   [1].[1] = 
    }
}
