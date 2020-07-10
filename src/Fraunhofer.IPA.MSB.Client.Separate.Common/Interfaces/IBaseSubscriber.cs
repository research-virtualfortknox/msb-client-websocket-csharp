namespace Fraunhofer.IPA.MSB.Client.Separate.Common.Interfaces
{
    public interface IBaseSubscriber
    {
        bool Connect();

        void Disconnect();

        bool AddSubscription(string id, Configuration.SubscriptionInstruction instr);

        void MakeSubscriptions();
    }
}