using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    public partial class SceneQuery
    {
        /*
         - Toevoegen van Enabled flag in de toekomst om disabled components wel/niet te vinden
         */

        private readonly List<ISceneQuery> _queries = new List<ISceneQuery>();

        private readonly List<Component> _values = new List<Component>();

        private bool _isRefreshed;

        public bool autoRefresh;

        public SceneQuery(bool autoRefresh = false) => this.autoRefresh = autoRefresh;

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

        public SceneQuery Dirty()
        {
            _isRefreshed = false;
            return this;
        }

        public SceneQuery Clear()
        {
            _values.Clear();
            return this;
        }

        public SceneQuery OnChildren<T>(Component parentComponent, params Type[] componentType)
            => OnChildren(parentComponent, typeof(T));

        public SceneQuery OnChildren(Component parentComponent, params Type[] componentType)
        {
            _queries.Add(new OnOrFromGivenQuery(FindComponentsOnChildren, parentComponent, componentType));
            return this;
        }

        public SceneQuery OnParent<T>(Component childComponent) => OnParent(childComponent, typeof(T));
        
        public SceneQuery OnParent(Component childComponent, params Type[] componentType)
        {
            _queries.Add(new OnOrFromGivenQuery(FindComponentsOnParent, childComponent, componentType));
            return this;
        }
        
        public SceneQuery OnGiven<T>(Component onComponent) => OnGiven(onComponent, typeof(T));

        public SceneQuery OnGiven(Component onComponent, params Type[] componentType)
        {
            _queries.Add(new OnOrFromGivenQuery(FindComponentsOnGiven, onComponent, componentType));
            return this;
        }

        public SceneQuery OnTag(string tagName) => OnName(tagName, typeof(Component));

        public SceneQuery OnTag<T>(string tagName) where T : Component => OnName(tagName, typeof(T));

        public SceneQuery OnTag(string tagName, params Type[] componentType)
        {
            _queries.Add(new OnNameOrTagQuery(FindComponentsByTag, tagName, componentType));
            return this;
        }

        public SceneQuery OnName(string objectName) => OnName(objectName, typeof(Component));

        public SceneQuery OnName<T>(string objectName) where T : Component => OnName(objectName, typeof(T));

        public SceneQuery OnName(string objectName, params Type[] componentType)
        {
            _queries.Add(new OnNameOrTagQuery(FindComponentsByName, objectName, componentType));
            return this;
        }

        public SceneQuery OnType() => OnType(false, typeof(Component));

        public SceneQuery OnType(params Type[] componentType) => OnType(false, componentType);

        public SceneQuery OnType<T>(bool includeInactive = false) where T : Component => OnType(includeInactive, typeof(T));

        public SceneQuery OnType(bool includeInactive, params Type[] componentType)
        {
            _queries.Add(new OnTypeQuery(FindComponentsByType, includeInactive, componentType));
            return this;
        }
    }
}


