using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UrSerializer
{
    /// <summary>
    ///     Use this type to register ur serializers under <b>[CustomSerializationHere]</b> attribute.<br/>
    ///     Ur serializers will be gathered automatically on first access.
    /// </summary>
    public static class SerializationContext
    {
        private static readonly Type SerializationAttributeType;
        private static readonly Dictionary<Type, object> Serializers;
        private static bool IsSerializersContainerSealed = false;

        static SerializationContext()
        {
            SerializationAttributeType = typeof(CustomSerializationHereAttribute);
            Serializers = new Dictionary<Type, object>();
            GatherSerializers();
            SealSerializersContainer();
        }

        /// <summary>
        ///     Defines ur serialization logic.<br/>
        ///     Later the serializer may be used by <b>SerializableType&lt;T&gt;</b>.
        /// </summary>
        /// <typeparam name="T">Type that serializer and deserializer made for.</typeparam>
        public static void RegisterSerializer<T>(Func<T, string> serializationMethod, Func<string, T> deserializationMethod)
        {
            if(IsSerializersContainerSealed)
            {
                throw GenerateUnexpectedSerializerRegistrationException();
            }

            Type serializerTargetType = typeof(T);
            Serializer<T> serializer = new(serializationMethod, deserializationMethod);
            bool isAddedSuccesfully = Serializers.TryAdd(serializerTargetType, serializer);

            if(isAddedSuccesfully is false)
            {
                throw GenerateAddingExistingSerializerAttemptException(serializerTargetType);
            }
        }

        internal static T Deserialize<T>(string serializedValue)
        {
            Type serializerTargetType = typeof(T);
            bool isSerializerFound = Serializers.TryGetValue(serializerTargetType, out object serializerFromContainer);

            if(isSerializerFound)
            {
                var serializer = (Serializer<T>)serializerFromContainer;
                T deserializedValue = serializer.Deserialize(serializedValue);

                return deserializedValue;
            }
            else
            {
                throw GenerateMissingSerializerException(serializerTargetType);
            }
        }

        internal static string Serialize<T>(T deserializedValue)
        {
            Type serializerTargetType = typeof(T);
            bool isSerializerFound = Serializers.TryGetValue(serializerTargetType, out object serializerFromContainer);

            if(isSerializerFound)
            {
                var serializer = (Serializer<T>)serializerFromContainer;
                string serializedValue = serializer.Serialize(deserializedValue);

                return serializedValue;
            }
            else
            {
                throw GenerateMissingSerializerException(serializerTargetType);
            }
        }

        private static IEnumerable<MethodInfo> GatherMarkedMethods(Type type)
        {
            const BindingFlags DesiredMethodFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo[] markedMethods = type.GetMethods(DesiredMethodFlags);

            return markedMethods.Where(MethodHasSerializationAttribute);
        }

        // We can't use those attributes because unity does utilize serialization (loads files) before those callbacks.
        // But we must gather all serializers before that! in other words: we must learn how to deserialize before actually doing it.
        //#if UNITY_EDITOR
        //    [UnityEditor.InitializeOnLoadMethod]
        //#elif UNITY_STANDALONE
        //    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        //#endif
        private static void GatherSerializers()
        {
            const object ExploitedInstance = null;
            const object[] MethodArguments = null;
            Assembly[] currentAppDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach(Assembly assembly in currentAppDomainAssemblies)
            {
                IEnumerable<Type> assemblyTypes = assembly.GetTypes();
                IEnumerable<Type> typesWithSerializationAttribute = assemblyTypes.Where(TypeHasSerializationAttribute);

                foreach(Type type in typesWithSerializationAttribute)
                {
                    IEnumerable<MethodInfo> registrationMethods = GatherMarkedMethods(type);

                    foreach(MethodInfo registrationMethod in registrationMethods)
                    {
                        _ = registrationMethod.Invoke(ExploitedInstance, MethodArguments);
                    }
                }
            }
        }

        private static Exception GenerateAddingExistingSerializerAttemptException(Type serializerTargetType)
        {
            string message = $"U tried to add serializer for [{serializerTargetType.Name}] which is already defined.";

            return new NotSupportedException(message);
        }

        private static Exception GenerateMissingSerializerException(Type serializerTargetType)
        {
            string message = $"Desired serializer missing. U probably forgot to register serializer for [{serializerTargetType.Name}] type.";

            return new KeyNotFoundException(message);
        }

        private static Exception GenerateUnexpectedSerializerRegistrationException()
        {
            const string Message = "U cannot register serializers when the container is sealed!" +
                " Please register serializer ONLY under [CustomSerializationHere] attribute.";

            return new InsufficientExecutionStackException(Message);
        }

        private static bool MethodHasSerializationAttribute(MethodInfo method)
        {
            return method.IsDefined(SerializationAttributeType);
        }

        private static void SealSerializersContainer() => IsSerializersContainerSealed = true;

        private static bool TypeHasSerializationAttribute(Type type)
        {
            const bool IsInheritedFromMarkedTypeCounts = false;

            return type.IsDefined(SerializationAttributeType, IsInheritedFromMarkedTypeCounts);
        }
    }
}
