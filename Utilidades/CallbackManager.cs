﻿using System;
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
        public event Action<string> NotificarSalidaEvent;
        public event Action<string> NotificarInvitacionCambioEvent;
        public event Action<string, string> NotificarVentanaInvitacionEvent;

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
            var context = new InstanceContext(this);
            _proxyNotificaciones = new GestionNotificacionesAmigosClient(context);
        }

        public void Conectar(string usuario)
        {
            var callbackInstance = new InstanceContext(this); 
            _proxyNotificaciones = new GestionNotificacionesAmigosClient(callbackInstance);

            try
            {
                _proxyNotificaciones.ConectarCliente(usuario);
            }
            catch (Exception ex)
            {
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

        public void NotificarSolicitudAmistad()
        {
            NotificarSolicitudAmistadEvent?.Invoke();
        }

        public void NotificarCambio()
        {
            NotificarCambioEvent?.Invoke();
        }

        public void NotificarSalida(string nombreUsuario)
        {
            NotificarSalidaEvent?.Invoke(nombreUsuario);
        }

        public void NotificarInvitacionCambio(string nombreUsuario)
        {
            NotificarInvitacionCambioEvent?.Invoke(nombreUsuario);
        }

        public void NotificarVentanaInvitacion(string nombreUsuarioInvitacion, string codigoSala)
        {
            NotificarVentanaInvitacionEvent?.Invoke(nombreUsuarioInvitacion, codigoSala);
        }
    }
}

