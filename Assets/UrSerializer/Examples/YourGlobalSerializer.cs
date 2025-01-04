using System;

namespace UrSerializer.Examples
{
    //[CustomSerializationHere]
    internal static class YourGlobalSerializer
    {
        [CustomSerializationHere]
        public static void LeverageSerialization()
        {
            SerializationContext.RegisterSerializer(SerializeType, DeserializeType);
        }

        private static Type DeserializeType(string serializedValue)
        {
            return Type.GetType(serializedValue);
        }

        private static string SerializeType(Type deserializedValue)
        {
            return deserializedValue?.AssemblyQualifiedName ?? string.Empty;
        }
    }
}
