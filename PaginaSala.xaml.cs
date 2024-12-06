using DobbleGame.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class PaginaSala : Page, Servidor.IGestionSalaCallback
    {
        private Servidor.IGestionSala proxySala;
        private const int NUMERO_JUGADORES_MINIMOS_INICIO_PARTIDA = 2;
        private readonly SelectorPlantillaJugador _selectorPlantilla;
        private bool _estaListo = false;
        public ObservableCollection<Jugador> UsuariosConectados { get; set; }
        public bool EsAnfitrion { get; set; }
        public string CodigoSala { get; set; }
        public bool HayConexionConSala { get; set; }
        private readonly ControlDeUsuarioNotificacion _controlNotificacion = new ControlDeUsuarioNotificacion();

        public PaginaSala(bool esAnfitrion, string codigoSala)
        {
            InitializeComponent();
            this.DataContext = this;
            UsuariosConectados = new ObservableCollection<Jugador>();
            EsAnfitrion = esAnfitrion;
            HayConexionConSala = false;
            CodigoSala = codigoSala;
            _selectorPlantilla = (SelectorPlantillaJugador)this.Resources["SelectorPlantillaJugador"];
            _selectorPlantilla.IniciarlizarPlantillas();
            btnIniciarPartida.DataContext = UsuariosConectados;
            this.gridPrincipal.Children.Add(_controlNotificacion);
        }


        public bool IniciarSesionSala()
        {
            if (EsAnfitrion && CodigoSala == null)
            {
                return CrearSala();
            }
            else
            {
                return UnirseASala();
            }
        }

        private bool CrearSala()
        {
            InicializarProxySiEsNecesario();

            bool resultado = false;

            try
            {
                CodigoSala = proxySala.GenerarCodigoNuevaSala();
                btnCodigoSala.Content = CodigoSala;

                var usuarioActual = Dominio.CuentaUsuario.CuentaUsuarioActual;

                resultado = proxySala.CrearNuevaSala(usuarioActual.Usuario, CodigoSala);

                if (resultado)
                {
                    HayConexionConSala = proxySala.UnirseASala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_UnionSala, EsAnfitrion);
                    proxySala.NotificarUsuarioConectado(CodigoSala);
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
            }

            return resultado;
        }

        private bool UnirseASala()
        {
            InicializarProxySiEsNecesario();

            bool resultado = false;

            try
            {
                btnCodigoSala.Content = CodigoSala;
                var usuarioActual = Dominio.CuentaUsuario.CuentaUsuarioActual;
                resultado = proxySala.UnirseASala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_UnionSala, EsAnfitrion);
                proxySala.NotificarUsuarioConectado(CodigoSala);
                HayConexionConSala = resultado;
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
            }

            return resultado;
        }

        public void AbandonarSala()
        {
            InicializarProxySiEsNecesario();

            try
            {
                var usuarioActual = Dominio.CuentaUsuario.CuentaUsuarioActual;
                proxySala.AbandonarSala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_AbandonoSala);

                ((ICommunicationObject)proxySala).Close();
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
            }
        }

        public bool HayEspacioEnSala()
        {
            InicializarProxySiEsNecesario();

            try
            {
                return proxySala.HayEspacioSala(CodigoSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
                return false;
            }
        }

        public bool ExisteSala()
        {
            InicializarProxySiEsNecesario();

            try
            {
                return proxySala.ExisteSala(CodigoSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
                return false;
            }
        }

        public bool EsSalaDisponible()
        {
            InicializarProxySiEsNecesario();

            try
            {
                return proxySala.EsSalaDisponible(CodigoSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
                return false;
            }
        }

        private void BtnRegresar(object sender, RoutedEventArgs e)
        {
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Application.Current.MainWindow);

            AbandonarSala();
            IrPaginaMenu();

            var proxyGestionAmigos = new GestionAmigosClient();
            proxyGestionAmigos.NotificarBotonInvitacion(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
        }

        private void IrPaginaMenu()
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            fadeOutAnimation.Completed += (s, a) =>
            {
                PaginaMenu paginaMenu = new PaginaMenu();
                this.NavigationService.Navigate(paginaMenu);

                AnimateElementsInPaginaMenu(paginaMenu);
            };
            this.BeginAnimation(Frame.OpacityProperty, fadeOutAnimation);
        }

        private void AnimateElementsInPaginaMenu(PaginaMenu paginaMenu)
        {
            if (paginaMenu.Content is Panel panel)
            {
                foreach (UIElement element in panel.Children)
                {
                    element.Opacity = 0;

                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)))
                    {
                        BeginTime = TimeSpan.FromMilliseconds(200)
                    };

                    element.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                }
            }
        }

        private void BtnEnviarMensaje(object sender, RoutedEventArgs e)
        {
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Application.Current.MainWindow);

            string mensaje = tbChat.Text.Trim();

            InicializarProxySiEsNecesario();

            try
            {
                proxySala.EnviarMensajeSala(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, CodigoSala, mensaje);
                tbChat.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
            }
        }

        private void EnviarMensajeEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    btnEnviarMensaje.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
                }
            }
        }

        private void BtnIniciarPartida(object sender, RoutedEventArgs e)
        {
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Application.Current.MainWindow);

            try
            {
                if (!EsAnfitrion)
                {
                    _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_DebesSerAnfitrion);
                    return;
                }

                if (UsuariosConectados.Count >= NUMERO_JUGADORES_MINIMOS_INICIO_PARTIDA)
                {
                    if (proxySala.TodosLosJugadoresEstanListos(CodigoSala))
                    {
                        IniciarPartida();
                    }
                    else
                    {
                        _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_JugadoresNoListos);
                    }
                }
                else
                {
                    _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_MinimoJugadores);
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
            }
        }

        private void IniciarPartida()
        {
            try
            {
                int numeroJugadoresEsperados = UsuariosConectados.Count;
                VentanaPartida ventanaPartida = new VentanaPartida(CodigoSala, EsAnfitrion, numeroJugadoresEsperados, Window.GetWindow(this));
                if (ventanaPartida.IniciarSesionPartida())
                {
                    proxySala.CambiarVentanaParaTodos(CodigoSala);
                    ventanaPartida.Show();
                    Window.GetWindow(this).Hide();
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
            }
        }

        private void BtnExpulsar(object sender, RoutedEventArgs e)
        {
            Utilidades.Utilidades.EstaConectado(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, Application.Current.MainWindow);

            Button boton = sender as Button;

            var jugadorAExpulsar = boton?.DataContext as Jugador;

            if (jugadorAExpulsar != null)
            {
                string mensaje = String.Format(Properties.Resources.lb_MensajeExpulsarJugador, jugadorAExpulsar.Usuario);
                VentanaModalDecision ventanaModalDecision = new VentanaModalDecision(mensaje,
                    Properties.Resources.btn_Expulsar, Properties.Resources.global_Cancelar);
                bool? respuesta = ventanaModalDecision.ShowDialog();

                if (respuesta == true)
                {
                    try
                    {
                        ExpulsarJugador(jugadorAExpulsar.Usuario);
                    }
                    catch (Exception ex)
                    {
                        Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
                    }
                }
            }
        }

        private void ExpulsarJugador(string nombreUsuario)
        {
            InicializarProxySiEsNecesario();

            try
            {
                proxySala.ExpulsarJugador(nombreUsuario, CodigoSala, Properties.Resources.msg_ExpulsionSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
            }
        }

        public void NotificarExpulsionAJugador()
        {
            ((ICommunicationObject)proxySala).Close();
            IrPaginaMenu();
            _controlNotificacion.MostrarNotificacion(Properties.Resources.lb_HasSidoExpulsado);
        }

        public void MostrarMensajeSala(string mensaje)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                tbContenedor.Text += $"{mensaje}{Environment.NewLine}";
                scrollViewer.ScrollToEnd();
            }));
        }

        public void ActualizarUsuariosConectados(Jugador[] usuariosConectados)
        {
            UsuariosConectados.Clear();
            _selectorPlantilla.ReiniciarPlantillas();
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                foreach (var usuario in usuariosConectados)
                {
                    Console.WriteLine($"Usuario: {usuario.Usuario}, Puntaje: {usuario.Puntaje}, EsAnfitrion: {usuario.EsAnfitrion}");
                    UsuariosConectados.Add(usuario);
                }
            }));
        }

        public void ConvertirEnAnfitrion(string nombreUsuario)
        {
            EsAnfitrion = true;

            this.Dispatcher.Invoke(() =>
            {
                var usuario = UsuariosConectados.FirstOrDefault(u => u.Usuario == nombreUsuario);

                if (usuario != null)
                {
                    usuario.EsAnfitrion = true;
                }
            });

            _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_EresAnfitrion);
        }

        public void CambiarVentana()
        {
            int numeroJugadoresEsperados = UsuariosConectados.Count;
            VentanaPartida ventanaPartida = new VentanaPartida(CodigoSala, EsAnfitrion, numeroJugadoresEsperados, Window.GetWindow(this));
            if (ventanaPartida.IniciarSesionPartida())
            {
                ventanaPartida.Show();
                Window.GetWindow(this).Hide();
            }
        }

        private void BtnJugadorListo(object sender, RoutedEventArgs e)
        {
            if (_estaListo)
            {
                _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_YaEstasListo);
                return;
            }

            try
            {
                proxySala.NotificarJugadorListo(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, CodigoSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxySala, ex, this);
            }
        }

        public void MostrarJugadorListo(string nombreUsuario, bool estaListo)
        {
            this.Dispatcher.Invoke(() =>
            {
                var usuario = UsuariosConectados.FirstOrDefault(u => u.Usuario == nombreUsuario);

                if (usuario != null)
                {
                    if (usuario.Usuario == Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario)
                    {
                        this._estaListo = estaListo;
                    }

                    usuario.EstaListo = estaListo;
                }
            });
        }

        private void BtnCopiarCodigoSala(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton)
            {
                Clipboard.SetText(boton.Content.ToString());
                _controlNotificacion.MostrarNotificacion(Properties.Resources.msg_CodigoCopiado);
            }
        }

        private void InicializarProxy()
        {
            if (proxySala != null && ((ICommunicationObject)proxySala).State == CommunicationState.Opened)
            {
                return;
            }

            if (proxySala != null && ((ICommunicationObject)proxySala).State == CommunicationState.Faulted)
            {
                ((ICommunicationObject)proxySala).Abort();
            }

            var contexto = new InstanceContext(this);
            var factory = new DuplexChannelFactory<IGestionSala>(contexto, "NetTcpBinding_IGestionSala");
            proxySala = factory.CreateChannel();
        }

        private void InicializarProxySiEsNecesario()
        {
            if (proxySala == null || ((ICommunicationObject)proxySala).State != CommunicationState.Opened)
            {
                InicializarProxy();
            }
        }

        private void TbChatTieneElFoco(object sender, RoutedEventArgs e)
        {
            borderContenedor.OpacityMask = null;
        }

        private void TbChatPierdeElFoco(object sender, RoutedEventArgs e)
        {
            borderContenedor.OpacityMask = new LinearGradientBrush
            {
                StartPoint = new Point(0, 4),
                EndPoint = new Point(0, 0),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.Black, 0.5),
                    new GradientStop(Colors.Transparent, 1)
                }
            };
        }

        private void OcultarDialogo(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is TextBox))
            {
                var scope = FocusManager.GetFocusScope(this);
                FocusManager.SetFocusedElement(scope, this as IInputElement);
                Keyboard.ClearFocus();
            }
        }

        private void BtnEnviarMensajeTieneElFoco(object sender, RoutedEventArgs e)
        {
            borderContenedor.OpacityMask = null;
        }

        private void BtnEnviarMensajePierdeElFoco(object sender, RoutedEventArgs e)
        {
            borderContenedor.OpacityMask = new LinearGradientBrush
            {
                StartPoint = new Point(0, 4),
                EndPoint = new Point(0, 0),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.Black, 0.5),
                    new GradientStop(Colors.Transparent, 1)
                }
            };
        }
    }
}