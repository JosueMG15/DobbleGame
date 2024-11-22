using DobbleGame.Servidor;
using DobbleGame.Utilidades;
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
using System.Windows.Media.Animation;
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
        private readonly SelectorPlantillaJugadorPartida selectorPlantilla;
        private readonly Window _ventanaMenu;
        private readonly Page _paginaSala;
        private readonly int _numeroJugadoresEsperados;
        private bool partidaIniciada = false;
        
        public ObservableCollection<Jugador> JugadoresEnPartida { get; set; }
        public string CodigoSala {  get; set; }
        public bool HayConexionPartida { get; set; }
        public bool EsAnfitrion {  get; set; }

        public VentanaPartida(string codigoSala, bool esAnfitrion, int numeroJugadoresEsperados, Window ventanaMenu,
            Page paginaSala)
        {
            InitializeComponent();
            this.Closing += VentanaMenu_Closing;
            this.DataContext = this;
            JugadoresEnPartida = new ObservableCollection<Jugador>();
            JugadoresEnPartida.CollectionChanged += JugadoresEnPartida_IniciarPartida;
            HayConexionPartida = false;
            CodigoSala = codigoSala;
            EsAnfitrion = esAnfitrion;
            _numeroJugadoresEsperados = numeroJugadoresEsperados;
            _ventanaMenu = ventanaMenu;
            _paginaSala = paginaSala;
            selectorPlantilla = (SelectorPlantillaJugadorPartida)this.Resources["SelectorPlantillaJugadorPartida"];
        }

        private void VentanaMenu_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var proxyUsuario = new Servidor.GestionAmigosClient();
            var proxy = new GestionJugadorClient();
            try
            {
                if (!string.IsNullOrEmpty(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario))
                {
                    proxy.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                    CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                    proxyUsuario.NotificarCambios();
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            InicializarProxySiEsNecesario();

            try
            {
                var usuarioActual = Dominio.CuentaUsuario.CuentaUsuarioActual;
                
                if (proxy.AbandonarPartida(usuarioActual.Usuario, CodigoSala))
                {
                    ((ICommunicationObject)proxy).Close();

                    PaginaMenu paginaMenu = new PaginaMenu();
                    _paginaSala.NavigationService.Navigate(paginaMenu);
                    _ventanaMenu.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }
        }

        public bool IniciarSesionPartida()
        {
            if (EsAnfitrion)
            {
                return CrearPartida();
            }
            else
            {
                return UnirseAPartida();
            }
        }

        private bool CrearPartida()
        {
            InicializarProxySiEsNecesario();

            bool resultado = false;

            try
            {
                resultado = proxy.CrearNuevaPartida(CodigoSala);

                if (resultado)
                {
                    proxy.UnirJugadorAPartida(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, CodigoSala);
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

        private bool UnirseAPartida()
        {
            InicializarProxySiEsNecesario();

            bool resultado = false;

            try
            {
                proxy.UnirJugadorAPartida(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, CodigoSala);
                proxy.NotificarActualizacionDeJugadoresEnPartida(CodigoSala);
                resultado = true;
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }

            return resultado;
        }

       

        private void JugadoresEnPartida_IniciarPartida(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!partidaIniciada && JugadoresEnPartida.Count == _numeroJugadoresEsperados)
            {
                partidaIniciada = true;
                if (EsAnfitrion)
                {
                    proxy.NotificarInicioPartida(CodigoSala);
                }
            }
        }

        public void ActualizarJugadoresEnPartida(Jugador[] jugadoresConectados)
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach (var jugador in jugadoresConectados)
                {
                    if (!JugadoresEnPartida.Any(j => j.Usuario == jugador.Usuario))
                    {
                        JugadoresEnPartida.Add(jugador);
                    }
                }
            });
        }

        public void ActualizarPuntosEnPartida(string nombreUsuario, int puntosEnPartida)
        {
            this.Dispatcher.Invoke(() =>
            {
                var jugadorEnPartida = JugadoresEnPartida.FirstOrDefault(j => j.Usuario == nombreUsuario);

                if (jugadorEnPartida != null)
                {
                    jugadorEnPartida.PuntosEnPartida = puntosEnPartida;
                }
            });
        }

        private void BtnValidarIcono_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.Content is Image imagenIcono)
            {
                if (imagenIcono.Source != null)
                {
                    string rutaIcono = imagenIcono.Source.ToString();
                    proxy.ValidarCarta(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, rutaIcono, CodigoSala);
                }
            }
        }

        public void AsignarCarta(Carta carta)
        {
            Icono1.Source = new BitmapImage(new Uri(carta.Iconos[0].Ruta));
            Icono2.Source = new BitmapImage(new Uri(carta.Iconos[1].Ruta));
            Icono3.Source = new BitmapImage(new Uri(carta.Iconos[2].Ruta));
            Icono4.Source = new BitmapImage(new Uri(carta.Iconos[3].Ruta));
            Icono5.Source = new BitmapImage(new Uri(carta.Iconos[4].Ruta));
            Icono6.Source = new BitmapImage(new Uri(carta.Iconos[5].Ruta));
            Icono7.Source = new BitmapImage(new Uri(carta.Iconos[6].Ruta));
            Icono8.Source = new BitmapImage(new Uri(carta.Iconos[7].Ruta));
        }

        public void AsignarCartaCentral(Carta cartaCentral)
        {
            IconoCentral1.Source = new BitmapImage(new Uri(cartaCentral.Iconos[0].Ruta));
            IconoCentral2.Source = new BitmapImage(new Uri(cartaCentral.Iconos[1].Ruta));
            IconoCentral3.Source = new BitmapImage(new Uri(cartaCentral.Iconos[2].Ruta));
            IconoCentral4.Source = new BitmapImage(new Uri(cartaCentral.Iconos[3].Ruta));
            IconoCentral5.Source = new BitmapImage(new Uri(cartaCentral.Iconos[4].Ruta));
            IconoCentral6.Source = new BitmapImage(new Uri(cartaCentral.Iconos[5].Ruta));
            IconoCentral7.Source = new BitmapImage(new Uri(cartaCentral.Iconos[6].Ruta));
            IconoCentral8.Source = new BitmapImage(new Uri(cartaCentral.Iconos[7].Ruta));
        }

        public void IniciarPartida()
        {
            IniciarContador();
        }

        private void IniciarContador()
        {
            contador = 6;
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
                if (contador != 1)
                {
                    int contadorTexto = contador - 1;
                    tbTexto.Text = contadorTexto.ToString();
                    contador--;
                }
                else
                {
                    tbTexto.FontSize = 300;
                    tbTexto.Text = Properties.Resources.lb_Dobble;
                    contador--;
                }
            }
            else
            {
                cronometro.Stop();
                cartaCentral.Visibility = Visibility.Visible;
                tbTexto.Visibility = Visibility.Collapsed;

                if (EsAnfitrion)
                {
                    proxy.NotificarDistribucionCartas(CodigoSala);
                }

                btnRegresar.IsEnabled = true;
                btnRegresar.Opacity = 1;
            }
        }

        public void FinalizarPartida()
        {
            cartaCentral.Visibility = Visibility.Collapsed;
            tbTexto.FontSize = 200;
            tbTexto.Text = Properties.Resources.lb_FinDelJuego;
            tbTexto.Visibility = Visibility.Visible;
            AnimacionFinalizarPartida();
        }

        private void AnimacionFinalizarPartida()
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(1)
            };
            tbTexto.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            DoubleAnimation fontSizeAnimation = new DoubleAnimation
            {
                From = 50, 
                To = 200,  
                Duration = TimeSpan.FromSeconds(1) 
            };
            tbTexto.BeginAnimation(System.Windows.Controls.TextBlock.FontSizeProperty, fontSizeAnimation);
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
    }
}
