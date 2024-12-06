using DobbleGame.Servidor;
using MaterialDesignThemes.Wpf;
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
    public partial class PaginaMenu : Page
    {
        public PaginaMenu()
        {
            InitializeComponent();
        }

        private void BtnCrearSala(object sender, RoutedEventArgs e)
        {
            PaginaSala paginaSala = new PaginaSala(true, null);

            if (paginaSala.IniciarSesionSala())
            {
                IrPaginaSalaAnimacion(paginaSala);
            }
        }

        private void BtnUnirseASala(object sender, RoutedEventArgs e)
        {
            VentanaIngresoCodigoSala paginaIngresoCodigoSala = new VentanaIngresoCodigoSala();
            bool? resultado = paginaIngresoCodigoSala.ShowDialog();

            if (resultado == true)
            {
                IrPaginaSalaAnimacion(paginaIngresoCodigoSala.Sala);
            }
        }

        public void IrPaginaSalaAnimacion(PaginaSala paginaSala)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            fadeOutAnimation.Completed += (s, a) =>
            {
                this.NavigationService.Navigate(paginaSala);

                //AnimateElementsInPaginaSala(paginaSala);                
            };
            this.BeginAnimation(Frame.OpacityProperty, fadeOutAnimation);
        }
        
        private void AnimateElementsInPaginaSala(PaginaSala paginaSala)
        {
            if (paginaSala.Content is Panel panel)
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
    }
}
