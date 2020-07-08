using System;
using System.Collections.Generic;

namespace Fraunhofer.IPA.MSB.Client.Separate.Common
{
    public class IntegrationFlow
    {
        public string FunctionId;
        public Delegate FunctionPointer;
        public Dictionary<string, string> Parametermapping;

        public void Invoke(object data)
        {
            var obj = new Newtonsoft.Json.Linq.JObject();
            obj.Add("dataObject", Newtonsoft.Json.Linq.JToken.FromObject(data));

            var parameters = this.FunctionPointer.Method.GetParameters();
            var parameterArrayForInvoke = new object[parameters.Length];

            foreach (var eintrag in this.Parametermapping)
            {
                int currentParameterCallIndex = 0;
                for (; currentParameterCallIndex < parameters.Length; currentParameterCallIndex++)
                {
                    if (parameters[currentParameterCallIndex].Name == eintrag.Key)
                    {
                        break;
                    }
                }

                if (currentParameterCallIndex == parameters.Length)
                {
                    continue;
                }

                object deserializedParameter = null;

                if (obj.SelectToken(eintrag.Value) != null)
                {
                    deserializedParameter = obj.SelectToken(eintrag.Value).ToObject(parameters[currentParameterCallIndex].ParameterType);
                }

                parameterArrayForInvoke[currentParameterCallIndex] = deserializedParameter;
            }

            this.FunctionPointer.DynamicInvoke(parameterArrayForInvoke);
        }
    }
}
