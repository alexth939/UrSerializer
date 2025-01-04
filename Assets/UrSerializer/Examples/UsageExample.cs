using System;
using UnityEngine;
using AnyUnitysTypeOrOther = UnityEngine.MonoBehaviour;

namespace UrSerializer.Examples
{
    internal class UsageExample : AnyUnitysTypeOrOther
    {
        // The serializer was registered. See [YourGlobalSerializer.cs].
        [SerializeField] private SerializableType<Type> _containerForTypeThatUnityNotKnowHowToSerialize;

        private void UseInstance()
        {
            Type someValue = typeof(Tuple);

            CreateAndAssignNull();

            CreateAndAssignSomeValue(someValue);

            CreateAndAssignSomeValueImplicitely();

            AssignSomeValue(someValue);

            Type encapsulatedValue = RetrieveEncapsulatedValue();

            ForceSerializationOrDeserialization();
        }

        private Type RetrieveEncapsulatedValue()
        {
            return _containerForTypeThatUnityNotKnowHowToSerialize?.Value;
        }

        private void AssignSomeValue(Type someValue)
        {
            if(_containerForTypeThatUnityNotKnowHowToSerialize is not null)
                _containerForTypeThatUnityNotKnowHowToSerialize.Value = someValue;
        }

        private void CreateAndAssignSomeValueImplicitely()
        {
            _containerForTypeThatUnityNotKnowHowToSerialize = typeof(MonoBehaviour);
        }

        private void ForceSerializationOrDeserialization()
        {
            throw new NotSupportedException("U don't do it manually. We utilize [UnityEngine.ISerializationCallbackReceiver] under the hood.");
        }

        private void CreateAndAssignSomeValue(Type someValue)
        {
            _containerForTypeThatUnityNotKnowHowToSerialize = new SerializableType<Type>(someValue);
        }

        private void CreateAndAssignNull()
        {
            _containerForTypeThatUnityNotKnowHowToSerialize = new SerializableType<Type>();
        }
    }
}
