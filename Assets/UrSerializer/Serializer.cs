using System;

namespace UrSerializer
{
    internal sealed class Serializer<T>
    {
        private readonly Func<T, string> _serializationMethod;
        private readonly Func<string, T> _deserializationMethod;

        public Serializer(Func<T, string> serializationMethod, Func<string, T> deserializationMethod)
        {
            _serializationMethod = serializationMethod;
            _deserializationMethod = deserializationMethod;
        }

        public string Serialize(T deserializedValue) => _serializationMethod.Invoke(deserializedValue);

        internal T Deserialize(string serializedValue) => _deserializationMethod.Invoke(serializedValue);
    }
}
