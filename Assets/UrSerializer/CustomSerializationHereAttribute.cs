using System;

namespace UrSerializer
{
    /// <summary>
    ///     Mark ur <b>types</b> and ur <b>static methods</b> that contains serializers registration with this attribute.<br/>
    ///     Any marked methods in not marked type will be ignored.<br/>
    ///     Use <b>SerializationContext</b> to register serializers.<br/>
    ///     Use <b>SerializableType&lt;T&gt;</b> to utilize custom serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CustomSerializationHereAttribute : Attribute
    {
    }
}
