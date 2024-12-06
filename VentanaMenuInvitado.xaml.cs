using System.Windows;

namespace DobbleGame
{
    public partial class VentanaMenuInvitado : Window
    {
        public VentanaMenuInvitado()
        {
            InitializeComponent();
            Inicializar();
        }

        private void Inicializar()
        {
            MarcoPrincipal.NavigationService.Navigate(new PaginaMenu());
            ControlDeUsuarioNotificacion controlNotificacion = new ControlDeUsuarioNotificacion();
            this.gridPrincipal.Children.Add(controlNotificacion);
        }

        private void BtnCerrarSesion(object sender, RoutedEventArgs e)
        {
            VentanaModalDecision ventana = new VentanaModalDecision(Properties.Resources.lb_MensajeCerrarSesion,
                Properties.Resources.btn_CerrarSesión, Properties.Resources.global_Cancelar);
            bool? respuesta = ventana.ShowDialog();

            if (respuesta == true)
            {
                MainWindow mainWindow = new MainWindow();
                this.Close();
                mainWindow.Show();
            }
        }
    }
}
