using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Polliakov.ExtendedValidation.Json
{
    public static class JsonTypeInfoResolvers
    {
        public static DefaultJsonTypeInfoResolver DissalowMissingFields()
        {
            return new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    static typeInfo =>
                    {
                        if (typeInfo.Kind != JsonTypeInfoKind.Object)
                            return;

                        foreach (JsonPropertyInfo propertyInfo in typeInfo.Properties)
                        {
                            if (Nullable.GetUnderlyingType(propertyInfo.GetType()) is null)
                                propertyInfo.IsRequired = true;
                        }
                    }
                }
            };
        }
    }
}
