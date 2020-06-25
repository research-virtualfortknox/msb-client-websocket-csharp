using System;
using System.Collections.Generic;

namespace Fraunhofer.IPA.MSB.Client.Separate.Common
{
    public class IntegrationFlow
    {
        public Delegate FunctionPointer;
        public Dictionary<String, String> Parametermapping;

        public void Invoke(Dictionary<String, Object> data)
        {
            var parameters = FunctionPointer.Method.GetParameters();
            var parameterArrayForInvoke = new object[parameters.Length];

            foreach (var eintrag in Parametermapping)
            {
                int currentParameterCallIndex = 0;
                for (; currentParameterCallIndex < parameters.Length; currentParameterCallIndex++)
                {
                    if (parameters[currentParameterCallIndex].Name == eintrag.Key)
                    {
                        break;
                    }
                }

                Object deserializedParameter = null;

                if (data.ContainsKey(eintrag.Value)) deserializedParameter = data[eintrag.Value];

                parameterArrayForInvoke[currentParameterCallIndex] = deserializedParameter;
            }

            FunctionPointer.DynamicInvoke(parameterArrayForInvoke);
        }
    }
}
