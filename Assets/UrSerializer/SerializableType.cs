using System;
using UnityEngine;

namespace UrSerializer
{
    /// <summary>
    ///     Is a container for object whoes type u desire to override his serialization method.<br/>
    ///     U must define ur custom serializer for this type via <b>[CustomSerializationHere]</b> attribute.<br/>
    ///     <i>P.S. we utilize [UnityEngine.ISerializationCallbackReceiver] under the hood.</i>
    /// </summary>
    /// <typeparam name="T">Type of ur encapsulated value.</typeparam>
    [Serializable]
    public class SerializableType<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private string _serializedValue;

        public SerializableType(T value)
        {
            Value = value;
        }

        /// <summary>
        ///     Value will be set to default(T).<br/>
        ///     <i>e.g. null for reference type, 0 for integer</i>.
        /// </summary>
        public SerializableType() : this(default(T))
        {
        }

        public T Value { get; set; }

        public static implicit operator SerializableType<T>(T supportedInstance) => new(supportedInstance);

        /// <summary>
        ///     Oh no, u found me! please don't touch me ;)<br/>
        ///     I'm visible due to unity's silly serialization hooks design.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Value = SerializationContext.Deserialize<T>(_serializedValue);
        }

        /// <summary>
        ///     Oh no, u found me! please don't touch me ;)<br/>
        ///     I'm visible due to unity's silly serialization hooks design.
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if(IsItRedundantSerializationInvokation())
                return;
#endif
            _serializedValue = SerializationContext.Serialize(Value);
        }

        private bool IsItRedundantSerializationInvokation()
        {
            // E.g. when u have scriptable object with {[SerializeField] SerializableType<T> ...} field and the instance is selected in project.
            const string MethodNameThatMayInvokeSerializationEveryFrame = "Internal_VerifyModifiedMonoBehaviours";
            const int FrameIndexOfInvokerOfMyClient = 2;
            bool isRedundant;

            System.Diagnostics.StackTrace trace = new();
            System.Diagnostics.StackFrame clientInvokerFrame = trace.GetFrame(FrameIndexOfInvokerOfMyClient);
            isRedundant = clientInvokerFrame?.GetMethod()?.Name == MethodNameThatMayInvokeSerializationEveryFrame;

            return isRedundant;
        }
    }
}
