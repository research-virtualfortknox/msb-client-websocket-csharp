namespace Fraunhofer.IPA.MSB.Client.Separate.Common.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IBaseSubscriber
    {
        bool Connect();
        void Disconnect();
        bool AddSubscription(string id, SubscriptionInstruction instr);
        void MakeSubscriptions();
    }
}