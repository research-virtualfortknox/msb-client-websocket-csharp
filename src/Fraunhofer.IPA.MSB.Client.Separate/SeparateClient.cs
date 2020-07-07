namespace Fraunhofer.IPA.MSB.Client.Separate
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fraunhofer.IPA.MSB.Client.API;
    using Fraunhofer.IPA.MSB.Client.API.EventArgs;
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.API.OpenApi;
    using MQTTnet.Client.Disconnecting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class InterfaceInstruction
    {
        public string interfaceIdent;
        public object instruction;
    }

    public class SeparateClient : AbstractMsbClient
    {
        Dictionary<string, Fraunhofer.IPA.MSB.Client.Separate.Common.Interfaces.IBaseInterface> interfaces;
        Dictionary<string, MSB.Client.API.Model.Function> functions;

        public SeparateClient()
        {
            this.interfaces = new Dictionary<string, Common.Interfaces.IBaseInterface>();
        }

        public override async Task<bool> ConnectAsync()
        {
            foreach (var i in this.interfaces)
            {
                i.Value.Start();
            }

            return true;
        }

        /// <summary>
        /// Disconnect interfaces.
        /// </summary>
        public override void Disconnect()
        {
            foreach (var i in this.interfaces)
            {
                i.Value.Stop();
            }
        }

        public override bool IsConnected()
        {
            return true;
        }

        public override async Task<bool> RegisterAsync(Service serviceToRegister)
        {
            if (serviceToRegister.Configuration.Parameters.ContainsKey("interfaces"))
            {
                Dictionary<string, InterfaceInstruction> o;

                if (serviceToRegister.Configuration.Parameters["interfaces"].Value.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                {
                    o = ((Newtonsoft.Json.Linq.JObject)serviceToRegister.Configuration.Parameters["interfaces"].Value).ToObject<Dictionary<string, InterfaceInstruction>>();
                }
                else
                {
                    o = (Dictionary<string, InterfaceInstruction>)serviceToRegister.Configuration.Parameters["interfaces"].Value;
                }

                Dictionary<string, Function> functionRegister = new Dictionary<string, Function>();
                foreach (var f in serviceToRegister.Functions)
                {
                    functionRegister.Add(f.Id, f);
                }

                foreach (var o_ in o)
                {
                    var v = o_.Value;

                    switch (v.interfaceIdent)
                    {
                        case "MQTT":
                            {
                                Fraunhofer.IPA.MSB.Client.Separate.MQTT.MQTTConfiguration config;

                                if (v.instruction.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                                {
                                    config = ((Newtonsoft.Json.Linq.JObject)v.instruction).ToObject<Fraunhofer.IPA.MSB.Client.Separate.MQTT.MQTTConfiguration>();
                                }
                                else
                                {
                                    config = (Fraunhofer.IPA.MSB.Client.Separate.MQTT.MQTTConfiguration)v.instruction;
                                }

                                this.interfaces.Add(o_.Key, new MQTT.MQTTInterface(config));

                                foreach (var sub in config.subscriptions)
                                {
                                    foreach (var intf in sub.Value.IntegrationFlows)
                                    {
                                        intf.Value.FunctionPointer = functionRegister[intf.Value.FunctionId].FunctionPointer;
                                    }
                                }

                                break;
                            }

                        case "TCP":
                            {
                                break;
                            }

                        default: break;
                    }
                }
            }

            return true;
        }

        public override async Task<bool> PublishAsync(Service service, EventData eventData)
        {
            Common.EventData data = new Common.EventData();
            data.Id = eventData.Event.Id;
            data.Data = new Dictionary<string, object>();

            var p = eventData.Value.GetType().GetProperties();

            foreach (var p_ in p)
            {
                data.Data.Add(p_.Name, p_.GetValue(eventData.Value));
            }

            foreach (var i in this.interfaces)
            {
                i.Value.PublishEvent(data);
            }

            return true;
        }
    }
}
