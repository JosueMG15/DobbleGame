﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DobbleGame.ServidorDobble {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CuentaUsuario", Namespace="http://schemas.datacontract.org/2004/07/Logica")]
    [System.SerializableAttribute()]
    public partial class CuentaUsuario : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ContraseñaField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CorreoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] FotoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdCuentaUsuarioField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int PuntajeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UsuarioField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Contraseña {
            get {
                return this.ContraseñaField;
            }
            set {
                if ((object.ReferenceEquals(this.ContraseñaField, value) != true)) {
                    this.ContraseñaField = value;
                    this.RaisePropertyChanged("Contraseña");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Correo {
            get {
                return this.CorreoField;
            }
            set {
                if ((object.ReferenceEquals(this.CorreoField, value) != true)) {
                    this.CorreoField = value;
                    this.RaisePropertyChanged("Correo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Foto {
            get {
                return this.FotoField;
            }
            set {
                if ((object.ReferenceEquals(this.FotoField, value) != true)) {
                    this.FotoField = value;
                    this.RaisePropertyChanged("Foto");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IdCuentaUsuario {
            get {
                return this.IdCuentaUsuarioField;
            }
            set {
                if ((this.IdCuentaUsuarioField.Equals(value) != true)) {
                    this.IdCuentaUsuarioField = value;
                    this.RaisePropertyChanged("IdCuentaUsuario");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Puntaje {
            get {
                return this.PuntajeField;
            }
            set {
                if ((this.PuntajeField.Equals(value) != true)) {
                    this.PuntajeField = value;
                    this.RaisePropertyChanged("Puntaje");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Usuario {
            get {
                return this.UsuarioField;
            }
            set {
                if ((object.ReferenceEquals(this.UsuarioField, value) != true)) {
                    this.UsuarioField = value;
                    this.RaisePropertyChanged("Usuario");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDobble.IGestionJugador")]
    public interface IGestionJugador {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGestionJugador/RegistrarUsuario", ReplyAction="http://tempuri.org/IGestionJugador/RegistrarUsuarioResponse")]
        bool RegistrarUsuario(DobbleGame.ServidorDobble.CuentaUsuario cuentaUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGestionJugador/RegistrarUsuario", ReplyAction="http://tempuri.org/IGestionJugador/RegistrarUsuarioResponse")]
        System.Threading.Tasks.Task<bool> RegistrarUsuarioAsync(DobbleGame.ServidorDobble.CuentaUsuario cuentaUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGestionJugador/ExisteNombreUsuario", ReplyAction="http://tempuri.org/IGestionJugador/ExisteNombreUsuarioResponse")]
        bool ExisteNombreUsuario(string nombreUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGestionJugador/ExisteNombreUsuario", ReplyAction="http://tempuri.org/IGestionJugador/ExisteNombreUsuarioResponse")]
        System.Threading.Tasks.Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGestionJugador/ExisteCorreoAsociado", ReplyAction="http://tempuri.org/IGestionJugador/ExisteCorreoAsociadoResponse")]
        bool ExisteCorreoAsociado(string correoUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGestionJugador/ExisteCorreoAsociado", ReplyAction="http://tempuri.org/IGestionJugador/ExisteCorreoAsociadoResponse")]
        System.Threading.Tasks.Task<bool> ExisteCorreoAsociadoAsync(string correoUsuario);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGestionJugadorChannel : DobbleGame.ServidorDobble.IGestionJugador, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GestionJugadorClient : System.ServiceModel.ClientBase<DobbleGame.ServidorDobble.IGestionJugador>, DobbleGame.ServidorDobble.IGestionJugador {
        
        public GestionJugadorClient() {
        }
        
        public GestionJugadorClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GestionJugadorClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GestionJugadorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GestionJugadorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool RegistrarUsuario(DobbleGame.ServidorDobble.CuentaUsuario cuentaUsuario) {
            return base.Channel.RegistrarUsuario(cuentaUsuario);
        }
        
        public System.Threading.Tasks.Task<bool> RegistrarUsuarioAsync(DobbleGame.ServidorDobble.CuentaUsuario cuentaUsuario) {
            return base.Channel.RegistrarUsuarioAsync(cuentaUsuario);
        }
        
        public bool ExisteNombreUsuario(string nombreUsuario) {
            return base.Channel.ExisteNombreUsuario(nombreUsuario);
        }
        
        public System.Threading.Tasks.Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario) {
            return base.Channel.ExisteNombreUsuarioAsync(nombreUsuario);
        }
        
        public bool ExisteCorreoAsociado(string correoUsuario) {
            return base.Channel.ExisteCorreoAsociado(correoUsuario);
        }
        
        public System.Threading.Tasks.Task<bool> ExisteCorreoAsociadoAsync(string correoUsuario) {
            return base.Channel.ExisteCorreoAsociadoAsync(correoUsuario);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServidorDobble.IGestionSala", CallbackContract=typeof(DobbleGame.ServidorDobble.IGestionSalaCallback))]
    public interface IGestionSala {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IGestionSala/EnviarMensajeSala")]
        void EnviarMensajeSala(string mensaje);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IGestionSala/EnviarMensajeSala")]
        System.Threading.Tasks.Task EnviarMensajeSalaAsync(string mensaje);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGestionSalaCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IGestionSala/SalaResponse")]
        void SalaResponse(string respuesta);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGestionSalaChannel : DobbleGame.ServidorDobble.IGestionSala, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GestionSalaClient : System.ServiceModel.DuplexClientBase<DobbleGame.ServidorDobble.IGestionSala>, DobbleGame.ServidorDobble.IGestionSala {
        
        public GestionSalaClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public GestionSalaClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public GestionSalaClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public GestionSalaClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public GestionSalaClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void EnviarMensajeSala(string mensaje) {
            base.Channel.EnviarMensajeSala(mensaje);
        }
        
        public System.Threading.Tasks.Task EnviarMensajeSalaAsync(string mensaje) {
            return base.Channel.EnviarMensajeSalaAsync(mensaje);
        }
    }
}
