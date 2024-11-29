using DobbleGame.Extensiones;
using DobbleGame.Servidor;
using DobbleGame.Utilidades;
using Dominio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DobbleGame
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void IniciarSesion()
        {
            if (!Utilidades.Utilidades.EsCampoVacio(tbUsuario.Text) || !Utilidades.Utilidades.EsCampoVacio(pbContraseña.Password))
            {
                var proxy = new Servidor.GestionJugadorClient();

                try
                {
                    var respuestaInicioSesion = proxy.IniciarSesionJugador(tbUsuario.Text, Utilidades.EncriptadorContraseña.GenerarHashSHA512(pbContraseña.Password));

                    if (respuestaInicioSesion.ErrorBD)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionBD(this);
                        return;
                    }
                    else if (!respuestaInicioSesion.Exitoso)
                    {
                        Utilidades.Utilidades.MostrarMensajeStackPanel(panelMensaje, lbMensaje, Properties.Resources.lb_SesiónActiva);
                    }
                    else if (respuestaInicioSesion.Resultado != null)
                    {
                        var cuentaInicioSesion = respuestaInicioSesion.Resultado;
                        Dominio.CuentaUsuario.CuentaUsuarioActual = new Dominio.CuentaUsuario
                        {
                            IdCuentaUsuario = cuentaInicioSesion.IdCuentaUsuario,
                            Usuario = cuentaInicioSesion.Usuario,
                            Correo = cuentaInicioSesion.Correo,
                            Contraseña = cuentaInicioSesion.Contraseña,
                            Foto = cuentaInicioSesion.Foto,
                            Puntaje = cuentaInicioSesion.Puntaje,
                            Estado = true,
                        };

                        // Conectar a la lista de clientes
                        CallbackManager.Instance.Conectar(cuentaInicioSesion.Usuario);

                        var proxyUsuario = new Servidor.GestionAmigosClient();
                        proxyUsuario.NotificarCambios();
                        //proxyUsuario.NotificarBotonInvitacion(cuentaInicioSesion.Usuario);

                        VentanaMenu ventanaMenu = new VentanaMenu();
                        this.Close();                     
                        ventanaMenu.Show();

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            ((App)Application.Current).IniciarPing();
                        });
                    }
                    else
                    {
                        MostrarMensaje(Properties.Resources.lb_ErrorInicioSesión);
                    }
                }
                catch (Exception ex)
                {
                    Utilidades.Utilidades.ManejarExcepciones(proxy, ex, this);
                }
            }
            else
            {
                MostrarMensaje(Properties.Resources.lb_CamposVacíos);
            }
        }


        private void BtnEntrarMenu_Click(object sender, RoutedEventArgs e)
        {
            IniciarSesion();
        }

        private void ClicCrearCuentaTf(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VentanaRegistro ventanaRegistro = new VentanaRegistro();
            ventanaRegistro.Show();
            this.Close();
        }

        private void ClicRecuperarContraseñaTf(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VentanaRecuperarContraseña ventanaRecuperarContraseña = new VentanaRecuperarContraseña();
            ventanaRecuperarContraseña.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ventanaRecuperarContraseña.ShowDialog();
        }

        private void BtnCambioIdioma_Click(object sender, RoutedEventArgs e)
        {
            string idiomaEspañol = "es-MX";
            string idiomaIngles = "en-US";

            if (Thread.CurrentThread.CurrentUICulture.Name == idiomaEspañol)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(idiomaIngles);
            } 
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(idiomaEspañol);
            }

            ActualizarIdiomaIU();
        }

        private void ActualizarIdiomaIU()
        {
            this.Title = Properties.Resources.global_IniciarSesión;
            lbDobble.Content = Properties.Resources.lb_Dobble_NET;
            lbIniciarSesion.Content = Properties.Resources.global_IniciarSesión;
            TextBoxExtensiones.SetTextoSugerido(tbUsuario, Properties.Resources.global_Usuario);
            PasswordBoxExtensiones.SetTextoSugerido(pbContraseña, Properties.Resources.lb_Contraseña);
            tbCrearCuenta.Text = Properties.Resources.lb_CrearCuenta;
            tbContraseñaOlvidada.Text = Properties.Resources.lb_ContraseñaOlvidada;
            btnEntrar.Content = Properties.Resources.btn_Entrar;
            btnEntrarComoInvitado.Content = Properties.Resources.btn_EntrarComoInvitado;
        }

        private void MostrarMensaje(string mensaje)
        {
            panelMensaje.Visibility = Visibility.Visible;
            lbMensaje.Content = mensaje;
        }

        private void PasswordBox_CambioDeContraseña(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var textoSugerido = ContraseñaHelper.EncontrarHijoVisual<TextBlock>(passwordBox, "TextoSugerido");
            ContraseñaHelper.ActualizarVisibilidadTextoSugerido(passwordBox, textoSugerido);
        }

        private void Window_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource != panelMensaje && panelMensaje.Visibility == Visibility.Visible)
            {
                panelMensaje.Visibility = Visibility.Hidden;
            }
        }
    }
}
