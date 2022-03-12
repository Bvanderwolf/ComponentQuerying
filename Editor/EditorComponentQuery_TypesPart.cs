using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.ComponentQuerying.Editor
{
    public partial class EditorComponentQuery
    {
        private readonly struct OnSelectedQuery : IComponentQuery
        {
            private readonly FromSelectedMethod _method;

            private readonly bool _includeInactive;

            private readonly bool _resetSelection;

            private readonly Type[] _componentTypes;

            public OnSelectedQuery(FromSelectedMethod method, bool includeInActive, bool resetSelection, Type[] componentTypes)
            {
                _method = method;
                _includeInactive = includeInActive;
                _resetSelection = resetSelection;
                _componentTypes = componentTypes;
            }

            public Component[] Values() => _method.Invoke(_includeInactive, _resetSelection, _componentTypes);

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

        private readonly struct SelectQuery : IComponentQuery
        {
            private readonly SelectMethod _method;

            private readonly bool _includeInactive;

            private readonly Type[] _componentTypes;

            public SelectQuery(SelectMethod method, bool includeInActive, Type[] componentTypes)
            {
                _method = method;
                _includeInactive = includeInActive;
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

        private delegate Component[] FromSelectedMethod(bool includeInActive, bool resetSelection, Type[] componentTypes);

        private delegate Component[] SelectMethod(bool includeInactive, Type[] componentTypes);
    }
}
