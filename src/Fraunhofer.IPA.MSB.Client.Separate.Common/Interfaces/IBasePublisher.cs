namespace Fraunhofer.IPA.MSB.Client.Separate.Common.Interfaces
{
    public interface IBasePublisher
    {
        bool PublishEvent(string eventId, string data);
    }
}
