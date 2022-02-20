using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    public partial class MBQuery
    {
        private interface IQuery
        {
            MonoBehaviour[] Values();

            T[] Values<T>() where T : MonoBehaviour;
        }

        private readonly struct OfTypeQuery : IQuery
        {
            private readonly OfTypeMethod _method;

            private readonly bool _includeInactive;

            private readonly Type[] _monoBehaviourTypes;

            public OfTypeQuery(OfTypeMethod method, bool includeInactive, Type[] monoBehaviourTypes)
            {
                _method = method;
                _includeInactive = includeInactive;
                _monoBehaviourTypes = monoBehaviourTypes;
            }

            public MonoBehaviour[] Values() => _method.Invoke(_includeInactive, _monoBehaviourTypes);

            public T[] Values<T>() where T : MonoBehaviour
            {
                MonoBehaviour[] values = Values();
                List<T> results = new List<T>();

                for (int i = 0; i < values.Length; i++)
                {
                    if (values is T result)
                        results.Add(result);
                }

                return results.ToArray();
            }
        }

        private readonly struct OfTypeQuery<T> : IQuery where T : MonoBehaviour
        {
            private readonly Func<bool, T[]> _method;

            private readonly bool _includeInactive;

            public OfTypeQuery(Func<bool, T[]> method, bool includeInactive)
            {
                _method = method;
                _includeInactive = includeInactive;
            }

            public MonoBehaviour[] Values() => _method.Invoke(_includeInactive);

            public TValue[] Values<TValue>() where TValue : MonoBehaviour
            {
                if (typeof(TValue) != typeof(T))
                    return Array.Empty<TValue>();

                // TESTEN OF DIT NIET VOOR PROBLEMEN ZORGT!
                return _method.Invoke(_includeInactive).Cast<TValue>().ToArray();
            }
        }

        private readonly struct OnGameObjectQuery : IQuery
        {
            private readonly OnGameObjectMethod _method;

            private readonly string _objectName;

            private readonly Type[] _monoBehaviourTypes;

            // Toevoegen van Enabled flag in de toekomst om disabled components ook te vinden??

            public OnGameObjectQuery(OnGameObjectMethod method, string objectName, Type[] monoBehaviourTypes)
            {
                _method = method;
                _objectName = objectName;
                _monoBehaviourTypes = monoBehaviourTypes;
            }

            public MonoBehaviour[] Values() => _method.Invoke(_objectName, _monoBehaviourTypes);

            public T[] Values<T>() where T : MonoBehaviour
            {
                MonoBehaviour[] values = Values();
                List<T> results = new List<T>();

                for (int i = 0; i < values.Length; i++)
                {
                    if (values is T result)
                        results.Add(result);
                }

                return results.ToArray();
            }
        }

        private readonly struct OnGameObjectQuery<T> : IQuery where T : MonoBehaviour
        {
            private readonly Func<string, T[]> _method;

            private readonly string _objectName;

            public OnGameObjectQuery(Func<string, T[]> method, string objectName)
            {
                _method = method;
                _objectName = objectName;
            }

            public MonoBehaviour[] Values() => _method.Invoke(_objectName);

            public TValue[] Values<TValue>() where TValue : MonoBehaviour
            {
                if (typeof(TValue) != typeof(T))
                    return Array.Empty<TValue>();

                // TESTEN OF DIT NIET VOOR PROBLEMEN ZORGT! (Up en down casting + same type casting).
                return _method.Invoke(_objectName).Cast<TValue>().ToArray();
            }
        }

        private delegate MonoBehaviour[] OnGameObjectMethod(string objectName, Type[] monoBehaviourTypes);

        private delegate MonoBehaviour[] OfTypeMethod(bool includeInactive, Type[] monoBehaviourTypes);
    }
}
