using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using DobbleGame.Servidor;
using System.Windows;
using System.Windows.Threading;

namespace DobbleGame.Utilidades
{
    public class CallbackManager : IGestionNotificacionesAmigosCallback
    {
        private static CallbackManager _instance;
        private GestionNotificacionesAmigosClient _proxyNotificaciones;

        // Eventos para las notificaciones de amistad
        public event Action NotificarSolicitudAmistadEvent;
        public event Action NotificarCambioEvent;

        // Propiedad para acceder a la única instancia de la clase
        public static CallbackManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CallbackManager();
                }
                return _instance;
            }
        }

        private CallbackManager()
        {
            // Configuración del contexto de instancia
            var context = new InstanceContext(this);
            _proxyNotificaciones = new GestionNotificacionesAmigosClient(context);
        }

        public void Conectar(string usuario)
        {
            try
            {
                _proxyNotificaciones.ConectarCliente(usuario);
            }
            catch (Exception ex)
            {
                // Manejar errores de conexión
                MessageBox.Show("Error al conectar para recibir notificaciones: " + ex.Message);
            }
        }

        public void Desconectar(String usuario)
        {
            if (_proxyNotificaciones != null && _proxyNotificaciones.State == CommunicationState.Opened)
            {
                _proxyNotificaciones.DesconectarCliente(usuario);
            }
        }

        // Método de callback de solicitud de amistad
        public void NotificarSolicitudAmistad()
        {
            NotificarSolicitudAmistadEvent?.Invoke();
        }

        // Método de callback para notificar un cambio
        public void NotificarCambio()
        {
            NotificarCambioEvent?.Invoke();
        }

        public GestionNotificacionesAmigosClient ObtenerProxy()
        {
            return _proxyNotificaciones;
        }
    }
}

