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
        private readonly SelectorPlantillaJugador selectorPlantilla = new SelectorPlantillaJugador();
        public ObservableCollection<CuentaUsuario> UsuariosConectados { get; set; }
        public bool EsAnfitrion {  get; set; }
        public string CodigoSala {  get; set; }
        public bool HayConexionConSala { get; set; }

        public PaginaSala(bool esAnfitrion, string codigoSala)
        {
            InitializeComponent();
            this.DataContext = this;
            UsuariosConectados = new ObservableCollection<CuentaUsuario>();
            //UsuariosConectados.CollectionChanged += UsuariosConectados_CollectionChanged;
            EsAnfitrion = esAnfitrion;
            HayConexionConSala = false;
            CodigoSala = codigoSala;
            IniciarSesionSala();
            InicializarSala();
        }

        public void IniciarSesionSala()
        {
            if (EsAnfitrion)
            {
                CrearSala();
            }
            else
            {
                UnirseASala();
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

                var usuarioActual = Dominio.CuentaUsuario.cuentaUsuarioActual;

                resultado = proxy.CrearNuevaSala(usuarioActual.Usuario, CodigoSala);

                if (resultado)
                {
                    HayConexionConSala = proxy.UnirseASala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_UnionSala, EsAnfitrion);
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
                var usuarioActual = Dominio.CuentaUsuario.cuentaUsuarioActual;
                resultado = proxy.UnirseASala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_UnionSala, EsAnfitrion);

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
                var usuarioActual = Dominio.CuentaUsuario.cuentaUsuarioActual;
                proxy.AbandonarSala(usuarioActual.Usuario, CodigoSala, Properties.Resources.msg_AbandonoSala);
                
                ((ICommunicationObject)proxy).Close();
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            AbandonarSala();
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
                    proxy.EnviarMensajeSala(Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario, CodigoSala, mensaje);
                    tbChat.Text = string.Empty;
                }
                catch (Exception)
                {
                    MessageBox.Show("No se puede enviar el mensaje. Verifique la conexión.");
                }
            }
        }

        private void InicializarSala()
        {
            InicializarProxySiEsNecesario();

            proxy.NotificarUsuarioConectado(CodigoSala);

            if (!EsAnfitrion)
            {
                btnIniciarPartida.IsEnabled = false;
                
            }
        }

        private void TbChat_GotFocus(object sender, RoutedEventArgs e)
        {
            tbContenedor.Visibility = Visibility.Visible;
        }

        private void BtnIniciarPartida_Click(object sender, RoutedEventArgs e)
        {
            /*if (UsuariosConectados.Count < 2)
            {
                MessageBox.Show("Se necesitan al menos 2 jugadores para iniciar la partida");
                return;
            }*/

            VentanaPartida ventanaPartida = new VentanaPartida(CodigoSala, Window.GetWindow(this));

            try
            {
                ventanaPartida.CrearPartida();
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.ManejarExcepciones((ICommunicationObject)proxy, ex, this);
            }
        }

        public void CambiarVentanaAPartida()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                Window ventanaActual = Window.GetWindow(this);
                ventanaActual.Hide();
                var ventanaPartida = new VentanaPartida(CodigoSala, Window.GetWindow(this)); 
                ventanaPartida.Show();
            }));
        }

        private void UsuariosConectados_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            btnIniciarPartida.IsEnabled = UsuariosConectados.Count >= 2;
        }

        public void MostrarMensajeSala(string mensaje)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                tbContenedor.Text += $"{mensaje}{Environment.NewLine}";
                tbContenedor.ScrollToEnd();
            }));
        }

        public void ActualizarUsuariosConectados(CuentaUsuario[] cuentaUsuarios)
        {
            selectorPlantilla.ReiniciarPlantillas();
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                UsuariosConectados.Clear();
                foreach (var usuario in cuentaUsuarios)
                {
                    Console.WriteLine($"Usuario: {usuario.Usuario}, Puntaje: {usuario.Puntaje}, EsAnfitrion: {usuario.EsAnfitrion}");
                    UsuariosConectados.Add(usuario);
                }
            }));
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
    }
}