using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    public partial class MBQuery
    {
        /*
         - Toevoegen van Enabled flag in de toekomst om disabled components wel/niet te vinden
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
            var genericValues = new List<T>();

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

        public MBQuery OnChildren<T>(Component parentComponent, params Type[] componentType)
            => OnChildren(parentComponent, typeof(T));

        public MBQuery OnChildren(Component parentComponent, params Type[] componentType)
        {
            _queries.Add(new OnOrFromGivenQuery(FindComponentsOnChildren, parentComponent, componentType));
            return this;
        }

        public MBQuery OnParent<T>(Component childComponent) => OnParent(childComponent, typeof(T));
        
        public MBQuery OnParent(Component childComponent, params Type[] componentType)
        {
            _queries.Add(new OnOrFromGivenQuery(FindComponentsOnParent, childComponent, componentType));
            return this;
        }
        
        public MBQuery OnGiven<T>(Component onComponent) => OnGiven(onComponent, typeof(T));

        public MBQuery OnGiven(Component onComponent, params Type[] componentType)
        {
            _queries.Add(new OnOrFromGivenQuery(FindComponentsOnGiven, onComponent, componentType));
            return this;
        }

        public MBQuery OnTag(string tagName) => OnName(tagName, typeof(Component));

        public MBQuery OnTag<T>(string tagName) where T : Component => OnName(tagName, typeof(T));

        public MBQuery OnTag(string tagName, params Type[] componentType)
        {
            _queries.Add(new OnNameOrTagQuery(FindComponentsByTag, tagName, componentType));
            return this;
        }

        public MBQuery OnName(string objectName) => OnName(objectName, typeof(Component));

        public MBQuery OnName<T>(string objectName) where T : Component => OnName(objectName, typeof(T));

        public MBQuery OnName(string objectName, params Type[] componentType)
        {
            _queries.Add(new OnNameOrTagQuery(FindComponentsByName, objectName, componentType));
            return this;
        }

        public MBQuery OnType() => OnType(false, typeof(Component));

        public MBQuery OnType(params Type[] componentType) => OnType(false, componentType);

        public MBQuery OnType<T>(bool includeInactive = false) where T : Component => OnType(includeInactive, typeof(T));

        public MBQuery OnType(bool includeInactive, params Type[] componentType)
        {
            _queries.Add(new OnTypeQuery(FindComponentsByType, includeInactive, componentType));
            return this;
        }
    }
}


