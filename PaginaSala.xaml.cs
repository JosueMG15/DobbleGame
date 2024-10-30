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
        }

        public bool CrearSala()
        {
            bool resultado = false;

            try
            {
                proxy = new Servidor.GestionSalaClient(new InstanceContext(this));
                if (EsNuevaSala)
                {
                    CodigoSala = proxy.GenerarCodigoNuevaSala();
                    btnCodigoSala.Content = CodigoSala;
                    proxy.CrearNuevaSala(Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario, CodigoSala);
                }
                btnCodigoSala.Content = CodigoSala;
                proxy.UnirseASala(Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario, CodigoSala, " Se ha unido a la sala");
                resultado = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return resultado;
        }

        private void AbandonarSala()
        {
            proxy.AbandonarSala(Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario, CodigoSala, " Ha abandonado la sala");
            proxy.Abort();
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

        private void BtnEnviar_Mnesaje(object sender, RoutedEventArgs e)
        {
            string mensaje = tbChat.Text.Trim();

            if (!string.IsNullOrEmpty(mensaje))
            {
                proxy.EnviarMensajeSala(Dominio.CuentaUsuario.cuentaUsuarioActual.Usuario, CodigoSala, mensaje);
                tbChat.Text = string.Empty;
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
