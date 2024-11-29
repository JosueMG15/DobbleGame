using DobbleGame.Servidor;
using DobbleGame.Utilidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DobbleGame
{
    public partial class App : Application
    {
        private IGestionJugador proxy;
        private CancellationTokenSource tokenDeCancelacion;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

        }

        private void InicializarProxy()
        {
            if (proxy != null)
            {
                var estado = ((ICommunicationObject)proxy).State;

                if (estado == CommunicationState.Opened)
                {
                    return;
                }
                else if (estado == CommunicationState.Closed || estado == CommunicationState.Faulted)
                {
                    ((ICommunicationObject)proxy).Abort();
                }
            }

            var factory = new ChannelFactory<IGestionJugador>("NetTcpBinding_IGestionJugador");
            proxy = factory.CreateChannel();

            try
            {
                ((ICommunicationObject)proxy).Open();
            }
            catch (Exception ex)
            {
                Registro.Error($"Error al abrir el proxy: {ex.Message}");
                proxy = null;
            }
        }

        public void IniciarPing()
        {
            DetenerPing();

            InicializarProxy();

            tokenDeCancelacion = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!tokenDeCancelacion.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), tokenDeCancelacion.Token);
                        VerificarConexion();
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Registro.Error($"Error en el ping: {ex.Message}");
                        DetenerPing();
                        ManejarDesconexion();
                    }
                }
            });
        }

        private void VerificarConexion()
        {
            if (proxy == null || ((ICommunicationObject)proxy).State != CommunicationState.Opened)
            {
                ManejarDesconexion();
                return;
            }

            if (Dominio.CuentaUsuario.CuentaUsuarioActual?.Usuario == null)
            {
                Registro.Error("El usuario actual no está configurado");
                return;
            }

            bool servidorDisponible = proxy.Ping(Dominio.CuentaUsuario.CuentaUsuarioActual.Usuario);
            if (!servidorDisponible)
            {
                DetenerPing();
                ManejarDesconexion();
            }
        }

        private void ManejarDesconexion()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window ventana in Application.Current.Windows)
                {
                    if (ventana.IsActive)
                    {
                        Utilidades.Utilidades.MostrarVentanaErrorConexionServidor(ventana, true);
                        break;
                    }
                }
            });
        }

        public void DetenerPing()
        {
            try
            {
                /*tokenDeCancelacion?.Cancel();
                tokenDeCancelacion?.Dispose();
                tokenDeCancelacion = null;*/

                if (proxy != null && ((ICommunicationObject)proxy).State == CommunicationState.Opened)
                {
                    ((ICommunicationObject)proxy).Close();
                }
            }
            catch (Exception ex)
            {
                Registro.Error($"Error al detener el ping: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DetenerPing();
        }
    }
}
