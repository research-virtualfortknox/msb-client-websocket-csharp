namespace Fraunhofer.IPA.MSB.Client.Separate.MQTT
{
    using System;
    using System.Collections.Generic;
    using Fraunhofer.IPA.MSB.Client.Separate.Common.Configuration;
    using Fraunhofer.IPA.MSB.Client.Separate.Common.Interfaces;
    using Newtonsoft.Json;

    public class MQTTInterface : IBaseInterface
    {
        public static readonly string Description =
        "\"Type\":\"MQTT\"," +
        "\"Patterns\":{" +
            "\"Events\":{" +
                "\"EventId\":\"string\"," +
                "\"Ip\":\"string\"," +
                "\"Port\":\"Int\"" +
            "}," +
            "\"Functions\":{" +
                "\"EventId\":\"string\"," +
                "\"Ip\":\"string\"," +
                "\"Port\":\"Int\"" +
            "}" +
        "}";

        private MQTTConfiguration configuration;

        private List<MQTTPubSub> subInterfaces;

        private Dictionary<string, List<MQTTPubSub>> relevantClientsForPublishing;

        public MQTTInterface(MQTTConfiguration config)
        {
            this.configuration = config;

            List<string> events = new List<string>();
            foreach (var p in config.Publications)
            {
                events.Add(p.Value.EventId);
            }

            List<KeyValuePair<string, UInt16>> connections = new List<KeyValuePair<string, ushort>>();

            foreach (var s in config.Subscriptions)
            {
                if (!connections.Exists(e => e.Key == s.Value.Ip))
                {
                    connections.Add(new KeyValuePair<string, ushort>(s.Value.Ip, s.Value.Port));
                }
            }

            this.relevantClientsForPublishing = new Dictionary<string, List<MQTTPubSub>>();

            foreach (var s in config.Publications)
            {
                if (!this.relevantClientsForPublishing.ContainsKey(s.Value.EventId))
                {
                    this.relevantClientsForPublishing.Add(s.Value.EventId, new List<MQTTPubSub>());
                }

                if (!connections.Exists(e => e.Key == s.Value.Ip))
                {
                    connections.Add(new KeyValuePair<string, ushort>(s.Value.Ip, s.Value.Port));
                }
            }

            if (connections.Count != 0)
            {
                this.subInterfaces = new List<MQTTPubSub>();

                foreach (var s in connections)
                {
                    // var pubs = config.publications.Where(e => e.Value.Ip == p.Key && e.Value.Port == p.Value); //funktioniert nicht?
                    Dictionary<string, SubscriptionInstruction> eventList = new Dictionary<string, SubscriptionInstruction>();

                    // foreach (var p_ in pubs) eventList.Add(p_.Value.EventId);
                    foreach (var e_ in config.Subscriptions)
                    {
                        if (e_.Value.Ip == s.Key && e_.Value.Port == s.Value)
                        {
                            eventList.Add(e_.Key, e_.Value);
                        }
                    }

                    var sub = new MQTTPubSub(s.Key, s.Value, eventList);

                    foreach (var e_ in config.Publications)
                    {
                        if (e_.Value.Ip == s.Key && e_.Value.Port == s.Value)
                        {
                            this.relevantClientsForPublishing[e_.Value.EventId].Add(sub);
                        }
                    }

                    this.subInterfaces.Add(sub);
                }
            }
        }

        public void Start()
        {
            foreach (var s in this.subInterfaces)
            {
                s.Connect();
            }
        }

        public void Stop()
        {
            foreach (var s in this.subInterfaces)
            {
                s.Disconnect();
            }
        }

        public void PublishEvent(string eventId, string data)
        {
            foreach (var p in this.relevantClientsForPublishing[eventId])
            {
                p.PublishEvent(eventId, data);
            }
        }
    }

    public class MQTTConfiguration
    {
        public Dictionary<string, MQTTSubscriptionInstruction> Subscriptions;
        public Dictionary<string, MQTTPublicationInstruction> Publications;

        public class MQTTSubscriptionInstruction : SubscriptionInstruction
        {
            public string Ip;
            public UInt16 Port;
        }

        public class MQTTPublicationInstruction : PublicationInstruction
        {
            public string Ip;
            public UInt16 Port;
        }
    }

    public class MQTTPubSub : IBaseSubscriber, IBasePublisher
    {
        private readonly string ip;
        private readonly UInt16 port;

        private MQTTnet.Client.IMqttClient mqttClient;

        private Dictionary<string, SubscriptionInstruction> Subscriptions;

        public MQTTPubSub(string brokerIp, UInt16 brokerPort, Dictionary<string, SubscriptionInstruction> subs = null)
        {
            this.ip = brokerIp;
            this.port = brokerPort;

            if (subs != null)
            {
                this.Subscriptions = subs;
            }
        }

        public bool Connect()
        {
            try
            {
                var factory = new MQTTnet.MqttFactory();
                this.mqttClient = factory.CreateMqttClient();

                var options = new MQTTnet.Client.Options.MqttClientOptionsBuilder()
                    .WithTcpServer(this.ip, this.port)
                    .Build();

                this.mqttClient.ConnectAsync(options, System.Threading.CancellationToken.None);

                this.mqttClient.ConnectedHandler = new MQTTnet.Client.Connecting.MqttClientConnectedHandlerDelegate(e =>
                {
                    if (this.Subscriptions != null)
                    {
                        this.MakeSubscriptions();
                    }
                });

                this.mqttClient.ApplicationMessageReceivedHandler = new MQTTnet.Client.Receiving.MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    if (e.ApplicationMessage.Payload == null)
                    {
                        return;
                    }

                    string msg = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                    var deserializedData = JsonConvert.DeserializeObject<Fraunhofer.IPA.MSB.Client.API.Model.IncomingData>(msg);
                    deserializedData.EventId = e.ApplicationMessage.Topic;

                    foreach (var s in this.Subscriptions)
                    {
                        if (s.Value.EventId == deserializedData.EventId)
                        {
                            s.Value.Invoke(deserializedData.DataObject);
                        }
                    }
                });
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void Disconnect()
        {
            this.mqttClient.DisconnectAsync(new MQTTnet.Client.Disconnecting.MqttClientDisconnectOptions(), System.Threading.CancellationToken.None);
        }

        public bool AddSubscription(string id, SubscriptionInstruction instr)
        {
            if (this.Subscriptions.ContainsKey(id))
            {
                return false;
            }

            this.Subscriptions.Add(id, instr);

            return true;
        }

        public void MakeSubscriptions()
        {
            foreach (var s in this.Subscriptions)
            {
                this.mqttClient.SubscribeAsync(new MQTTnet.Client.Subscribing.MqttClientSubscribeOptionsBuilder().WithTopicFilter(s.Value.EventId).Build(), System.Threading.CancellationToken.None);
            }
        }

        public bool PublishEvent(string eventId, string data)
        {
            if (!this.mqttClient.IsConnected)
            {
                return false;
            }

            var applicationMessage = new MQTTnet.MqttApplicationMessageBuilder()
                        .WithTopic(eventId)
                        .WithPayload(data)
                        .Build();

            this.mqttClient.PublishAsync(applicationMessage, System.Threading.CancellationToken.None);

            return true;
        }
    }
}