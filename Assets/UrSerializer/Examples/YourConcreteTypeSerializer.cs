using System;

namespace UrSerializer.Examples
{
    //[CustomSerializationHere]
    internal sealed class YourConcreteTypeSerializer
    {
        private int _x;

        [CustomSerializationHere]
        public static void LeverageSerialization()
        {
            SerializationContext.RegisterSerializer(instance => instance._x.ToString(), default(Func<string, YourConcreteTypeSerializer>));
        }
    }
}
