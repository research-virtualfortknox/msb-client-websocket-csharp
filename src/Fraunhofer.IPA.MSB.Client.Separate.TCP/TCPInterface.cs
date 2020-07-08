namespace Fraunhofer.IPA.MSB.Client.Separate.TCP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Fraunhofer.IPA.MSB.Client.Separate.Common;
    using Fraunhofer.IPA.MSB.Client.Separate.Common.Interfaces;
    using Newtonsoft.Json;

    public class TCPInterface : IBaseInterface
    {
        public static readonly string Description =
        "\"Type\":\"TCP\"," +
        "\"Patterns\":{" +
            "\"Events\":{" +
                "\"EventId\":\"String\"," +
                "\"Ip\":\"String\"," +
                "\"Port\":\"Int\"" +
            "}," +
            "\"Functions\":{" +
                "\"EventId\":\"String\"," +
                "\"Ip\":\"String\"," +
                "\"Port\":\"Int\"" +
            "}" +
        "}";

        private TCPConfiguration Configuration;

        private List<TCPSubscriber> subscriber;
        private List<TCPPublisher> publisher;

        public TCPInterface(TCPConfiguration config)
        {
            this.Configuration = config;

            List<string> events = new List<string>();
            foreach (var p in config.publications)
            {
                events.Add(p.Value.EventId);
            }

            List<KeyValuePair<string, UInt16>> subscriptions = new List<KeyValuePair<string, ushort>>();

            foreach (var s in config.subscriptions)
            {
                if (!subscriptions.Exists(e => e.Key == s.Value.Ip))
                {
                    subscriptions.Add(new KeyValuePair<string, ushort>(s.Value.Ip, s.Value.Port));
                }
            }

            if (subscriptions.Count != 0) {
                this.subscriber = new List<TCPSubscriber>();

                foreach (var s in subscriptions)
                {
                    var sub = new TCPSubscriber(s.Key, s.Value);
                    var subs = config.subscriptions.Where(e => e.Value.Ip == s.Key && e.Value.Port == s.Value);

                    foreach (var s_ in subs)
                    {
                        sub.AddSubscription(s_.Key, s_.Value);
                    }

                    this.subscriber.Add(sub);
                }
            }

            List<KeyValuePair<string, UInt16>> publications = new List<KeyValuePair<string, ushort>>();

            foreach (var s in config.publications)
            {
                if (!publications.Exists(e => e.Key == s.Value.Ip))
                {
                    publications.Add(new KeyValuePair<string, ushort>(s.Value.Ip, s.Value.Port));
                }
            }

            if (publications.Count != 0)
            {
                this.publisher = new List<TCPPublisher>();

                foreach (var p in publications)
                {
                    // var pubs = config.publications.Where(e => e.Value.Ip == p.Key && e.Value.Port == p.Value); //funktioniert nicht?
                    List<string> eventList = new List<string>();

                    // foreach (var p_ in pubs) eventList.Add(p_.Value.EventId);
                    foreach (var e_ in config.publications)
                    {
                        if (e_.Value.Ip == p.Key && e_.Value.Port == p.Value)
                        {
                            eventList.Add(e_.Value.EventId);
                        }
                    }

                    var pub = new TCPPublisher(p.Key, p.Value, eventList);
                    this.publisher.Add(pub);
                }
            }
        }

        public void Start()
        {
            if (this.publisher != null)
            {
                foreach (var p in this.publisher)
                {
                    p.Start();
                }
            }

            if (this.subscriber != null)
            {
                foreach (var s in this.subscriber)
                {
                    s.Connect();
                }
            }
        }

        public void Stop()
        {
            if (this.subscriber != null)
            {
                foreach (var s in this.subscriber)
                {
                    s.Disconnect();
                }
            }

            if (this.publisher != null)
            {
                foreach (var p in this.publisher)
                {
                    p.Stop();
                }
            }
        }

        public void PublishEvent(string eventId, string data)
        {
            foreach (var p in this.publisher)
            {
                p.PublishEvent(eventId, data);
            }
        }
    }

    public class TCPConfiguration
    {
        public class TCPSubscriptionInstruction : SubscriptionInstruction
        {
            public string Ip;
            public UInt16 Port;
        }

        public class TCPPublicationInstruction : PublicationInstruction
        {
            public string Ip;
            public UInt16 Port;
        }

        public Dictionary<string, TCPSubscriptionInstruction> subscriptions;
        public Dictionary<string, TCPPublicationInstruction> publications;
    }

    public class TCPSubscriber : IBaseSubscriber
    {
        private TcpClient tcpClient;

        private readonly string Ip;
        private readonly UInt16 Port;
        private Dictionary<string, SubscriptionInstruction> Subscriptions;

        private byte[] buffer = new byte[1024];

        public TCPSubscriber(string localIp, UInt16 localPort)
        {
            this.Ip = localIp;
            this.Port = localPort;

            this.Subscriptions = new Dictionary<string, SubscriptionInstruction>();
        }

        public bool Connect()
        {
            this.tcpClient = new TcpClient();

            this.tcpClient.BeginConnect(this.Ip, this.Port, this.ConnectCallback, null);

            return true;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                this.tcpClient.EndConnect(ar);

                this.MakeSubscriptions();

                this.Listen();
            }
            catch
            {
            }
        }

        public void Disconnect()
        {
            this.tcpClient.GetStream().Close();

            this.tcpClient.Close();
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
                var d = new SubscriptionInstruction() { EventId = s.Value.EventId };
                var j = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                var b = System.Text.ASCIIEncoding.ASCII.GetBytes(j);

                this.tcpClient.GetStream().Write(b, 0, b.Length);
            }
        }

        public void Listen()
        {
            this.tcpClient.GetStream().BeginRead(this.buffer, 0, this.buffer.Length, this.ListenCallback, this.buffer);
        }

        private void ListenCallback(IAsyncResult result)
        {
            string messageBufferAsUnicode = Encoding.ASCII.GetString(this.buffer);
            messageBufferAsUnicode = messageBufferAsUnicode.Trim('\0');

            try
            {
                this.tcpClient.GetStream().BeginRead(this.buffer, 0, this.buffer.Length, this.ListenCallback, this.buffer);

                var deserializedData = JsonConvert.DeserializeObject<Fraunhofer.IPA.MSB.Client.API.Model.IncomingData>(messageBufferAsUnicode);

                foreach (var s in this.Subscriptions)
                {
                    if (s.Value.EventId == deserializedData.EventId)
                    {
                        s.Value.Invoke(deserializedData.DataObject);
                    }
                }
            }
            catch
            {
            }
        }
    }

    public class TCPPublisher : IBasePublisher
    {
        private TcpListener tcpListener;

        private readonly string Ip;
        private readonly UInt16 Port;

        private List<TcpClient> Subscribers;
        private Dictionary<TcpClient, byte[]> SubscriberBuffer;
        private Dictionary<string, List<TcpClient>> TopicSubscriberlist;

        public TCPPublisher(string localIp, UInt16 localPort, List<string> topics)
        {
            this.Ip = localIp;
            this.Port = localPort;
            this.Subscribers = new List<TcpClient>();
            this.SubscriberBuffer = new Dictionary<TcpClient, byte[]>();
            this.TopicSubscriberlist = new Dictionary<string, List<TcpClient>>();

            foreach (var t in topics)
            {
                this.TopicSubscriberlist.Add(t, new List<TcpClient>());
            }
        }

        public void Start()
        {
            try
            {
                this.tcpListener = new TcpListener(IPAddress.Parse(this.Ip), this.Port);
                this.tcpListener.Start();

                this.tcpListener.BeginAcceptTcpClient(new AsyncCallback(this.DoAcceptTcpClientCallback), this.tcpListener);
            }
            catch
            {
            }
        }

        public void Stop()
        {
            if (this.tcpListener != null)
            {
                this.tcpListener.Stop();
            }

            this.Subscribers.Clear();
            this.SubscriberBuffer.Clear();
            foreach (var t in this.TopicSubscriberlist)
            {
                t.Value.Clear();
            }
        }

        private void DoAcceptTcpClientCallback(IAsyncResult result)
        {
            TcpListener listener = (TcpListener)result.AsyncState;

            var cl = listener.EndAcceptTcpClient(result);

            this.Subscribers.Add(cl);
            this.SubscriberBuffer.Add(cl, new byte[1024]);

            cl.GetStream().BeginRead(this.SubscriberBuffer[cl], 0, this.SubscriberBuffer[cl].Length, this.ServerReadCallback, cl);
        }

        private void ServerReadCallback(IAsyncResult result)
        {
            try
            {
                var cl = (TcpClient)result.AsyncState;
                var b = this.SubscriberBuffer[cl];

                string messageBuffer = Encoding.ASCII.GetString(b);
                messageBuffer = messageBuffer.Trim('\0');

                cl.GetStream().BeginRead(this.SubscriberBuffer[cl], 0, this.SubscriberBuffer[cl].Length, this.ServerReadCallback, this.SubscriberBuffer[cl]);

                try
                {
                    var deserializedData = JsonConvert.DeserializeObject<SubscriptionInstruction>(messageBuffer);

                    if (!this.TopicSubscriberlist.ContainsKey(deserializedData.EventId))
                    {
                        return;
                    }
                    else
                    {
                        this.TopicSubscriberlist[deserializedData.EventId].Add(cl);
                    }
                }
                catch
                {
                }
            }
            catch
            {
            }
        }

        public bool PublishEvent(string eventId, string data)
        {
            if (this.TopicSubscriberlist.ContainsKey(eventId))
            {
                var c_liste = this.TopicSubscriberlist[eventId];
                foreach (var c in c_liste)
                {
                    var b = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
                    c.GetStream().Write(b, 0, b.Length);
                }
            }

            return true;
        }
    }
}
