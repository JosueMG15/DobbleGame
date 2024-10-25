using Microsoft.Win32;
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
    public partial class PaginaPerfil : Page
    {
        public PaginaPerfil()
        {
            InitializeComponent();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
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

        private void BtnCambiarUsuario(object sender, RoutedEventArgs e)
        {
            VentanaCambioNombre ventanaCambioNombre = new VentanaCambioNombre();
            ventanaCambioNombre.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaCambioNombre.ShowDialog();
        }

        private void BtnCambiarContraseña(object sender, RoutedEventArgs e)
        {
            VentanaCambioContraseña ventanaCambioContraseña = new VentanaCambioContraseña();
            ventanaCambioContraseña.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaCambioContraseña.ShowDialog();

        }

        private void BtnActualizarFoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            openFileDialog.Title = "Selecciona una imagen";
            openFileDialog.ShowDialog();
            string selectedFilePath = openFileDialog.FileName;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(selectedFilePath);
            bitmap.EndInit();

            ImagenPerfil.Source = bitmap;
        }
    }
}
