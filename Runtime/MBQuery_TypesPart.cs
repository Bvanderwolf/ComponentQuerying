using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    public partial class MBQuery
    {
        private interface IQuery
        {
            Component[] Values();

            T[] Values<T>() where T : Component;
        }

        private readonly struct OfTypeQuery : IQuery
        {
            private readonly OfTypeMethod _method;

            private readonly bool _includeInactive;

            private readonly Type[] _componentTypes;

            public OfTypeQuery(OfTypeMethod method, bool includeInactive, Type[] componentTypes)
            {
                _method = method;
                _includeInactive = includeInactive;
                _componentTypes = componentTypes;
            }

            public Component[] Values() => _method.Invoke(_includeInactive, _componentTypes);

            public T[] Values<T>() where T : Component
            {
                Component[] values = Values();
                List<T> results = new List<T>();

                for (int i = 0; i < values.Length; i++)
                {
                    if (values is T result)
                        results.Add(result);
                }

                return results.ToArray();
            }
        }

        private readonly struct OnGameObjectQuery : IQuery
        {
            private readonly OnGameObjectMethod _method;

            private readonly string _objectNameOrTag;

            private readonly Type[] _componentTypes;

            // Toevoegen van Enabled flag in de toekomst om disabled components ook te vinden??

            public OnGameObjectQuery(OnGameObjectMethod method, string objectNameOrTag, Type[] componentTypes)
            {
                _method = method;
                _objectNameOrTag = objectNameOrTag;
                _componentTypes = componentTypes;
            }

            public Component[] Values() => _method.Invoke(_objectNameOrTag, _componentTypes);

            public T[] Values<T>() where T : Component
            {
                Component[] values = Values();
                List<T> results = new List<T>();

                for (int i = 0; i < values.Length; i++)
                {
                    if (values is T result)
                        results.Add(result);
                }

                return results.ToArray();
            }
        }

        private delegate Component[] OnGameObjectMethod(string objectName, Type[] componentTypes);

        private delegate Component[] OfTypeMethod(bool includeInactive, Type[] componentTypes);
    }
}
