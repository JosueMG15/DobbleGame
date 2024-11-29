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
        private Servidor.IGestionSala proxy;
        private const int NUMERO_JUGADORES_MINIMOS_INICIO_PARTIDA = 2;
        private readonly SelectorPlantillaJugador selectorPlantilla;
        public ObservableCollection<Jugador> UsuariosConectados { get; set; }
        public bool EsAnfitrion {  get; set; }
        public string CodigoSala {  get; set; }
        public bool HayConexionConSala { get; set; }

        public PaginaSala(bool esAnfitrion, string codigoSala)
        {
            InitializeComponent();
            this.DataContext = this;
            UsuariosConectados = new ObservableCollection<Jugador>();
            UsuariosConectados.CollectionChanged += UsuariosConectados_CollectionChanged;
            EsAnfitrion = esAnfitrion;
            HayConexionConSala = false;
            CodigoSala = codigoSala;
            selectorPlantilla = (SelectorPlantillaJugador)this.Resources["SelectorPlantillaJugador"];
            selectorPlantilla.IniciarlizarPlantillas();
            
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
                CodigoSala = proxy.GenerarCodigoNuevaSala();
                btnCodigoSala.Content = CodigoSala;

                var usuarioActual = Dominio.CuentaUsuario.CuentaUsuarioActual;

                resultado = proxy.CrearNuevaSala(usuarioActual.Usuario, CodigoSala);

                if (resultado)
                {
                    HayConexionConSala = proxy.UnirseASala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_UnionSala, EsAnfitrion);
                    proxy.NotificarUsuarioConectado(CodigoSala);
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
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
                resultado = proxy.UnirseASala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_UnionSala, EsAnfitrion);
                proxy.NotificarUsuarioConectado(CodigoSala);
                HayConexionConSala = resultado;
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }

            return resultado;
        }

        private void AbandonarSala()
        {
            InicializarProxySiEsNecesario();

            try
            {
                var usuarioActual = Dominio.CuentaUsuario.CuentaUsuarioActual;
                proxy.AbandonarSala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_AbandonoSala);
                
                ((ICommunicationObject)proxy).Close();
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }
        }

        public bool HayEspacioEnSala()
        {
            InicializarProxySiEsNecesario();

            try
            {
                return proxy.HayEspacioSala(CodigoSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
                return false;
            }
        }

        public bool ExisteSala()
        {
            InicializarProxySiEsNecesario();

            try
            {
                return proxy.ExisteSala(CodigoSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
                return false;
            }
        }

        public bool EsSalaDisponible()
        {
            InicializarProxySiEsNecesario();

            try
            {
                return proxy.EsSalaDisponible(CodigoSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
                return false;
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            AbandonarSala();
            IrPaginaMenu();
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
            string mensaje = tbChat.Text.Trim();

            InicializarProxySiEsNecesario();

            if (!string.IsNullOrEmpty(mensaje) && ((ICommunicationObject)proxy)?.State == CommunicationState.Opened)
            {
                try
                {
                    proxy.EnviarMensajeSala(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario, CodigoSala, mensaje);
                    tbChat.Text = string.Empty;
                }
                catch (Exception)
                {
                    MessageBox.Show("No se puede enviar el mensaje. Verifique la conexión.");
                }
            }
        }

        private void BtnIniciarPartida_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int numeroJugadoresEsperados = UsuariosConectados.Count;
                VentanaPartida ventanaPartida = new VentanaPartida(CodigoSala, EsAnfitrion, numeroJugadoresEsperados, Window.GetWindow(this), this);
                if (ventanaPartida.IniciarSesionPartida())
                {
                    proxy.CambiarVentanaParaTodos(CodigoSala);
                    ventanaPartida.Show();
                    Window.GetWindow(this).Hide();
                }
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }
        }

        private void BtnExpulsar_Click(object sender, RoutedEventArgs e)
        {
            Button boton = sender as Button;

            var jugadorAExpulsar = boton?.DataContext as Jugador;

            if (jugadorAExpulsar != null)
            {
                VentanaConfirmarExpulsion ventanaConfirmarExpulsion = new VentanaConfirmarExpulsion(jugadorAExpulsar.Usuario);
                bool? respuesta = ventanaConfirmarExpulsion.ShowDialog();

                if (respuesta == true)
                {
                    ExpulsarJugador(jugadorAExpulsar.Usuario);
                }
            }
        }

        private void ExpulsarJugador(string nombreUsuario)
        {
            InicializarProxySiEsNecesario();

            try
            {
                proxy.ExpulsarJugador(nombreUsuario, CodigoSala, Properties.Resources.msg_ExpulsionSala);
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }
        }

        public void NotificarExpulsionAJugador()
        {
            ((ICommunicationObject)proxy).Close();
            IrPaginaMenu();
            MessageBox.Show(Properties.Resources.lb_HasSidoExpulsado);
        }

        private void UsuariosConectados_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (EsAnfitrion && UsuariosConectados.Count >= NUMERO_JUGADORES_MINIMOS_INICIO_PARTIDA)
            {
                btnIniciarPartida.IsEnabled = true;
            }
        }

        public void MostrarMensajeSala(string mensaje)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                tbContenedor.Text += $"{mensaje}{Environment.NewLine}";
                scrollViewer.ScrollToEnd();
            }));
        }

        public void ActualizarUsuariosConectados(Jugador[] cuentaUsuarios)
        {
            UsuariosConectados.Clear();
            selectorPlantilla.ReiniciarPlantillas();
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            { 
                foreach (var usuario in cuentaUsuarios)
                {
                    Console.WriteLine($"Usuario: {usuario.Usuario}, Puntaje: {usuario.Puntaje}, EsAnfitrion: {usuario.EsAnfitrion}");
                    UsuariosConectados.Add(usuario);
                }
            }));
        }

        public void ConvertirEnAnfitrion()
        {
            EsAnfitrion = true;
        }

        public void CambiarVentana()
        {
            int numeroJugadoresEsperados = UsuariosConectados.Count;
            VentanaPartida ventanaPartida = new VentanaPartida(CodigoSala, EsAnfitrion, numeroJugadoresEsperados, Window.GetWindow(this), this);
            if (ventanaPartida.IniciarSesionPartida())
            {
                ventanaPartida.Show();
                Window.GetWindow(this).Hide();
            }
        }

        private void BtnCopiarCodigoSala_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton)
            {
                Clipboard.SetText(boton.Content.ToString());
            }
        }

        private void EnviarMensaje_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnEnviarMensaje.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
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
            var factory = new DuplexChannelFactory<IGestionSala>(contexto, "NetTcpBinding_IGestionSala");
            proxy = factory.CreateChannel();
        }

        private void InicializarProxySiEsNecesario()
        {
            if (proxy == null || ((ICommunicationObject)proxy).State != CommunicationState.Opened)
            {
                InicializarProxy();
            }
        }

        private void TbChat_GotFocus(object sender, RoutedEventArgs e)
        {
            borderContenedor.OpacityMask = null;
        }

        private void TbChat_LostFocus(object sender, RoutedEventArgs e)
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

        private void Pagina_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is TextBox)) 
            { 
                var scope = FocusManager.GetFocusScope(this); 
                FocusManager.SetFocusedElement(scope, this as IInputElement); 
                Keyboard.ClearFocus();
            }
        }

        private void BtnEnviarMensaje_GotFocus(object sender, RoutedEventArgs e)
        {
            borderContenedor.OpacityMask = null;
        }

        private void BtnEnviarMensaje_LostFocus(object sender, RoutedEventArgs e)
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