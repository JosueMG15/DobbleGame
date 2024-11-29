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
        private bool permitirCierreInesperado = true;

        public ObservableCollection<Jugador> JugadoresEnPartida { get; set; }
        public string CodigoSala { get; set; }
        public bool EsAnfitrion { get; set; }

        public VentanaPartida(string codigoSala, bool esAnfitrion, int numeroJugadoresEsperados, Window ventanaMenu,
            Page paginaSala)
        {
            InitializeComponent();
            this.Closing += VentanaMenu_Closing;
            this.DataContext = this;
            JugadoresEnPartida = new ObservableCollection<Jugador>();
            JugadoresEnPartida.CollectionChanged += JugadoresEnPartida_IniciarPartida;
            CodigoSala = codigoSala;
            EsAnfitrion = esAnfitrion;
            _numeroJugadoresEsperados = numeroJugadoresEsperados;
            _ventanaMenu = ventanaMenu;
            _paginaSala = paginaSala;
            selectorPlantilla = (SelectorPlantillaJugadorPartida)this.Resources["SelectorPlantillaJugadorPartida"];
        }

        private void VentanaMenu_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (permitirCierreInesperado)
            {
                var proxyUsuario = new Servidor.GestionAmigosClient();
                var proxyJugador = new GestionJugadorClient();
                try
                {
                    if (!string.IsNullOrEmpty(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario))
                    {
                        proxy.AbandonarPartida(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, CodigoSala);
                        proxyJugador.CerrarSesionJugador(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Properties.Resources.msg_AbandonoSala);
                        CallbackManager.Instance.Desconectar(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
                        proxyUsuario.NotificarCambios();
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxyJugador, ex, this);
                }
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            InicializarProxySiEsNecesario();
            permitirCierreInesperado = false;
            VentanaModalDecision ventanaModalDecision = new VentanaModalDecision(Properties.Resources.lb_MensajeAbandonarPartida);
            bool? respuesta = ventanaModalDecision.ShowDialog();

            if (respuesta == true)
            {
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
                    try
                    {
                        proxy.NotificarInicioPartida(CodigoSala);
                    }
                    catch (Exception ex)
                    {
                        Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
                    }
                }
            }
        }

        public void ActualizarJugadoresEnPartida(Jugador[] jugadoresConectados)
        {
            JugadoresEnPartida.Clear();
            selectorPlantilla.ReiniciarPlantillasPartida();
            this.Dispatcher.Invoke(() =>
            {
                foreach (var jugador in jugadoresConectados)
                {
                    JugadoresEnPartida.Add(jugador);
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

        public void BloquearCarta()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Button[] botones = { boton1, boton2, boton3, boton4, boton5, boton6, boton7, boton8 };

                foreach (var boton in botones)
                {
                    var triangulo = (Polygon)boton.Template.FindName("Triangulo", boton);
                    triangulo.Fill = new SolidColorBrush(Colors.IndianRed);
                }

                cartaJugador.IsEnabled = false;
            });
        }

        public void DesbloquearCarta()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Button[] botones = { boton1, boton2, boton3, boton4, boton5, boton6, boton7, boton8 };

                foreach (var boton in botones)
                {
                    var tringulo = (Polygon)boton.Template.FindName("Triangulo", boton);
                    tringulo.Fill = new SolidColorBrush(Colors.White);
                }

                cartaJugador.IsEnabled = true;
            });
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
                    try
                    {
                        proxy.NotificarDistribucionCartas(CodigoSala);
                    }
                    catch (Exception ex)
                    {
                        Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
                    }
                }

                btnRegresar.IsEnabled = true;
                btnRegresar.Opacity = 1;
            }
        }

        public void FinalizarPartida()
        {
            RegistrarPuntosDelJugador();
            cartaCentral.Visibility = Visibility.Collapsed;
            tbTexto.FontSize = 200;
            tbTexto.Text = Properties.Resources.lb_FinDelJuego;
            tbTexto.Visibility = Visibility.Visible;
            cartaJugador.Visibility = Visibility.Collapsed;
            controlJugadores.Visibility = Visibility.Collapsed;
            AnimacionFinalizarPartida();
        }

        private async void RegistrarPuntosDelJugador()
        {
            Jugador jugador = JugadoresEnPartida.FirstOrDefault(j => j.Usuario == Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);

            if (jugador != null)
            {
                try
                {
                    var respuestaRegistroPuntos = await Task.Run(() => proxy.GuardarPuntosJugador(jugador.Usuario, jugador.PuntosEnPartida));
                    if (respuestaRegistroPuntos.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
                }
            }
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

            fontSizeAnimation.Completed += (s, e) =>
            {
                var contador = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(2)
                };

                contador.Tick += (sender, args) =>
                {
                    contador.Stop();
                    tbTexto.Visibility = Visibility.Collapsed;
                    RegresarASala();
                };
                contador.Start();
            };

            tbTexto.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            tbTexto.BeginAnimation(System.Windows.Controls.TextBlock.FontSizeProperty, fontSizeAnimation);
        }

        private void RegresarASala()
        {
            List<Jugador> jugadoresOrdenadosPorPuntos = JugadoresEnPartida.OrderByDescending(j => j.PuntosEnPartida).ToList();

            VentanaMarcadorPartida ventanaMarcadorPartida = new VentanaMarcadorPartida(jugadoresOrdenadosPorPuntos);
            bool? resultado = ventanaMarcadorPartida.ShowDialog();

            if (resultado == true)
            {
                permitirCierreInesperado = false;
                this.Close();
                PaginaSala paginaSala = new PaginaSala(EsAnfitrion, CodigoSala);
                if (paginaSala.IniciarSesionSala())
                {
                    _paginaSala.NavigationService.Navigate(paginaSala);
                    _ventanaMenu.Show();
                }
            }
        }

        public void ConvertirEnAnfitrionDesdePartida()
        {
            EsAnfitrion = true;
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
