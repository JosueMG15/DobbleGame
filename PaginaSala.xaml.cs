using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class PaginaSala : Page, Servidor.IGestionSalaCallback
    {
        private Servidor.GestionSalaClient proxy;
        private Servidor.CuentaUsuario[] cuentaUsuarios;
        public bool EsNuevaSala { get; set; }
        public string CodigoSala { get; set; }

        public PaginaSala()
        {
            InitializeComponent();
            this.DataContext = this;
            proxy = new Servidor.GestionSalaClient(new InstanceContext(this));
        }

        public bool CrearSala()
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
                proxy = new Servidor.GestionSalaClient(new InstanceContext(this));

            bool resultado = false;

            try
            {
                if (proxy.State == CommunicationState.Faulted)
                {
                    proxy.Abort();
                    throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                }

                CodigoSala = proxy.GenerarCodigoNuevaSala();
                btnCodigoSala.Content = CodigoSala;
                var usuarioActual = Dominio.CuentaUsuario.cuentaUsuarioActual;
                proxy.CrearNuevaSala(usuarioActual.Usuario, CodigoSala);
                proxy.UnirseASala(usuarioActual.Usuario, usuarioActual.Puntaje, usuarioActual.Foto, CodigoSala, "Se ha unido a la sala");
                resultado = true;
            }
            catch (CommunicationObjectFaultedException faultEx)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                Console.WriteLine($"Error en el objeto de comunicación: {faultEx.Message}");
            }
            catch (CommunicationException commEx)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                Console.WriteLine($"Error de comunicación: {commEx.Message}");
            }
            catch (TimeoutException timeoutEx)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                Console.WriteLine($"Error de tiempo de espera: {timeoutEx.Message}");
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }

            return resultado;
        }

        public bool UnirseASala()
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
                proxy = new Servidor.GestionSalaClient(new InstanceContext(this));

            bool resultado = false;

            try
            {
                if (proxy.State == CommunicationState.Faulted)
                {
                    proxy.Abort();
                    throw new InvalidOperationException("El canal de comunicación está en estado Faulted.");
                }

                btnCodigoSala.Content = CodigoSala;
                var usuarioActual = Dominio.CuentaUsuario.cuentaUsuarioActual;
                resultado = proxy.UnirseASala(usuarioActual.Usuario, usuarioActual.Puntaje, usuarioActual.Foto, CodigoSala, " Se ha unido a la sala");
            }
            catch (CommunicationObjectFaultedException faultEx)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                Console.WriteLine($"Error en el objeto de comunicación: {faultEx.Message}");
            }
            catch (CommunicationException commEx)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                Console.WriteLine($"Error de comunicación: {commEx.Message}");
            }
            catch (TimeoutException timeoutEx)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                Console.WriteLine($"Error de tiempo de espera: {timeoutEx.Message}");
            }
            catch (Exception ex)
            {
                Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(this);
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }

            return resultado;
        }

        private void AbandonarSala()
        {
            try
            {
                proxy.AbandonarSala(Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario, CodigoSala, " Ha abandonado la sala");
                proxy.Close();
            }
            catch (CommunicationException)
            {
                proxy.Abort();
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

        private void BtnEnviar_Mensaje(object sender, RoutedEventArgs e)
        {
            string mensaje = tbChat.Text.Trim();

            if (!string.IsNullOrEmpty(mensaje) && proxy?.State == CommunicationState.Opened)
            {
                proxy.EnviarMensajeSala(Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario, CodigoSala, mensaje);
                tbChat.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("No se puede enviar el mensaje. Verifique la conexión.");
            }
        }

        private void TbChat_GotFocus(object sender, RoutedEventArgs e)
        {
            tbContenedor.Visibility = Visibility.Visible;
        }

        private void BtnIniciarPartida_Click(object sender, RoutedEventArgs e)
        {

        }

        public void MostrarMensajeSala(string mensaje)
        {
            tbContenedor.Text += $"{mensaje}{Environment.NewLine}";
            tbContenedor.ScrollToEnd();
        }

        private void BtnCopiarCodigoSala_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton)
            {
                Clipboard.SetText(boton.Content.ToString());
            }
        }

    }
}