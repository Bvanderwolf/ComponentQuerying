using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    public partial class SceneQuery
    {
        private readonly struct OnTypeQuery : ISceneQuery
        {
            private readonly OnTypeMethod _method;

            private readonly bool _includeInactive;

            private readonly Type[] _componentTypes;

            public OnTypeQuery(OnTypeMethod method, bool includeInactive, Type[] componentTypes)
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
                    if (values[i] is T result)
                        results.Add(result);
                }

                return results.ToArray();
            }
        }

        private readonly struct OnOrFromGivenQuery : ISceneQuery
        {
            private readonly OnGivenMethod _method;

            private readonly Component _target;

            private readonly Type[] _componentTypes;

            public OnOrFromGivenQuery(OnGivenMethod method, Component target, Type[] componentTypes)
            {
                _method = method;
                _target = target;
                _componentTypes = componentTypes;
            }

            public Component[] Values() => _method.Invoke(_target, _componentTypes);

            public T[] Values<T>() where T : Component
            {
                Component[] values = Values();
                List<T> results = new List<T>();

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] is T result)
                        results.Add(result);
                }

                return results.ToArray();
            }
        }
        
        private readonly struct OnNameOrTagQuery : ISceneQuery
        {
            private readonly OnNameOrTagMethod _method;

            private readonly string _objectNameOrTag;

            private readonly Type[] _componentTypes;

            public OnNameOrTagQuery(OnNameOrTagMethod method, string objectNameOrTag, Type[] componentTypes)
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
                    if (values[i] is T result)
                        results.Add(result);
                }

                return results.ToArray();
            }
        }

        private delegate Component[] OnNameOrTagMethod(string objectNameOrTag, Type[] componentTypes);

        private delegate Component[] OnGivenMethod(Component givenComponent, Type[] componentType);

        private delegate Component[] OnTypeMethod(bool includeInactive, Type[] componentTypes);
    }
}
