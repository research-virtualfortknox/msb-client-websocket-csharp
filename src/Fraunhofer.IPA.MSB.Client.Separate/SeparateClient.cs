namespace Fraunhofer.IPA.MSB.Client.Separate
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fraunhofer.IPA.MSB.Client.API;
    using Fraunhofer.IPA.MSB.Client.API.Model;

    public class InterfaceInstruction
    {
        public string interfaceIdent;
        public object instruction;
    }

    public class SeparateClient : AbstractMsbClient
    {
        Dictionary<string, Fraunhofer.IPA.MSB.Client.Separate.Common.Interfaces.IBaseInterface> interfaces;

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

                                foreach (var sub in config.Subscriptions)
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
                                Fraunhofer.IPA.MSB.Client.Separate.TCP.TCPConfiguration config;

                                if (v.instruction.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                                {
                                    config = ((Newtonsoft.Json.Linq.JObject)v.instruction).ToObject<Fraunhofer.IPA.MSB.Client.Separate.TCP.TCPConfiguration>();
                                }
                                else
                                {
                                    config = (Fraunhofer.IPA.MSB.Client.Separate.TCP.TCPConfiguration)v.instruction;
                                }

                                this.interfaces.Add(o_.Key, new TCP.TCPInterface(config));

                                foreach (var sub in config.Subscriptions)
                                {
                                    foreach (var intf in sub.Value.IntegrationFlows)
                                    {
                                        intf.Value.FunctionPointer = functionRegister[intf.Value.FunctionId].FunctionPointer;
                                    }
                                }

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
            IncomingData incomingData = new IncomingData(service.Uuid, eventData.Event.Id, eventData.EventPriority, eventData.PublishingDate, eventData.Value, eventData.CorrelationId);
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(incomingData);

            foreach (var i in this.interfaces)
            {
                i.Value.PublishEvent(eventData.Event.Id, data);
            }

            return true;
        }
    }
}
