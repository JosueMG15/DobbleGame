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
    /// <summary>
    /// Lógica de interacción para PaginaMenu.xaml
    /// </summary>
    public partial class PaginaMenu : Page
    {
        public PaginaMenu()
        {
            InitializeComponent();
        }

        private void BtnCrearSala_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PaginaSala());
        }
    }
}
