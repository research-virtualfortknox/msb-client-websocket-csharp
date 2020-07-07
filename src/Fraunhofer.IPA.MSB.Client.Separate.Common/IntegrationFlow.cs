using System;
using System.Collections.Generic;

namespace Fraunhofer.IPA.MSB.Client.Separate.Common
{
    public class IntegrationFlow
    {
        public string FunctionId;
        public Delegate FunctionPointer;
        public Dictionary<string, string> Parametermapping;

        public void Invoke(Dictionary<string, object> data)
        {
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

                object deserializedParameter = null;

                if (data.ContainsKey(eintrag.Value)) deserializedParameter = data[eintrag.Value];

                parameterArrayForInvoke[currentParameterCallIndex] = deserializedParameter;
            }

            this.FunctionPointer.DynamicInvoke(parameterArrayForInvoke);
        }
    }
}
