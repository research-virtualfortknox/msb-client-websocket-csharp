namespace Fraunhofer.IPA.MSB.Client.Separate.Common
{
    using System;
    using System.Collections.Generic;

    public class SubscriptionInstruction
    {
        public string EventId;
        public Dictionary<string, IntegrationFlow> IntegrationFlows;

        public void Invoke(object data)
        {
            foreach (var integrationFlow in this.IntegrationFlows)
            {
                integrationFlow.Value.Invoke(data);
            }
        }
    }
}
