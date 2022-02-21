using System;
using System.Collections.Generic;
using UnityEngine;


namespace BWolf.MonoBehaviourQuerying
{
    public partial class MBQuery
    {
        /*
         - GetComponentInParent
         - GetComponentInChildren
         - GetComponent
         - GetComponents
         */

        private readonly List<IQuery> _queries = new List<IQuery>();

        private readonly List<Component> _values = new List<Component>();

        private bool _isRefreshed;

        public bool autoRefresh;

        public MBQuery(bool autoRefresh = false) => this.autoRefresh = autoRefresh;

        public Component[] Values()
        {
            // If values require auto refreshing or the query hasn't refreshed yet.
            if (autoRefresh || !_isRefreshed)
            {
                // Refresh the values.
                _values.Clear();

                for (int i = 0; i < _queries.Count; i++)
                    _values.AddRange(_queries[i].Values());

                _isRefreshed = true;
            }

            return _values.ToArray();
        }

        public Component Value()
        {
            Component[] values = Values();
            if (values.Length == 0)
                return null;

            return values[values.Length - 1];
        }

        public T[] Values<T>() where T : Component
        {
            List<T> genericValues = new List<T>();

            // If values require auto refreshing or the query hasn't refreshed yet.
            if (autoRefresh || !_isRefreshed)
            {
                // Refresh the values.
                _values.Clear();

                for (int i = 0; i < _queries.Count; i++)
                    _values.AddRange(_queries[i].Values<T>());

                _isRefreshed = true;
            }
            else
            {
                // Only the generic values are returned.
                for (int i = 0; i < _values.Count; i++)
                    if (_values[i] is T value)
                        genericValues.Add(value);
            }

            return genericValues.ToArray();
        }

        public T Value<T>() where T : Component
        {
            T[] values = Values<T>();
            if (values.Length == 0)
                return null;

            return values[values.Length - 1];
        }

        public MBQuery Dirty()
        {
            _isRefreshed = false;
            return this;
        }

        public MBQuery Clear()
        {
            _values.Clear();
            return this;
        }

        public MBQuery OnComponent(Component fromComponent, params Type[] componentType)
        {
            return this;
        }

        public MBQuery OnComponent<T>(Component fromComponent)
        {
            return this;
        }

        public MBQuery ByTag(string tagName) => ByName(tagName, typeof(Component));

        public MBQuery ByTag<T>(string tagName) where T : Component => ByName(tagName, typeof(T));

        public MBQuery ByTag(string tagName, params Type[] componentType)
        {
            _queries.Add(new OnGameObjectQuery(FindComponentsByTag, tagName, componentType));
            return this;
        }

        public MBQuery ByName(string objectName) => ByName(objectName, typeof(Component));

        public MBQuery ByName<T>(string objectName) where T : Component => ByName(objectName, typeof(T));

        public MBQuery ByName(string objectName, params Type[] componentType)
        {
            _queries.Add(new OnGameObjectQuery(FindComponentsByName, objectName, componentType));
            return this;
        }

        public MBQuery ByType() => ByType(false, typeof(Component));

        public MBQuery ByType(params Type[] componentType) => ByType(false, componentType);

        public MBQuery ByType<T>(bool includeInactive = false) where T : Component => ByType(includeInactive, typeof(T));

        public MBQuery ByType(bool includeInactive, params Type[] componentType)
        {
            _queries.Add(new OfTypeQuery(FindComponentsByType, includeInactive, componentType));
            return this;
        }
    }
}


