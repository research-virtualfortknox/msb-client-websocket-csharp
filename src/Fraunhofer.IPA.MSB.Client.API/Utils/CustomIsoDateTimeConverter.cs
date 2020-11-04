namespace Fraunhofer.IPA.MSB.Client.API.Utils
{
    using Newtonsoft.Json.Converters;

    public class CustomIsoDateTimeConverter : IsoDateTimeConverter
    {
        public CustomIsoDateTimeConverter()
        {
            this.DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'";
        }

    }
}
