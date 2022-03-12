using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.ComponentQuerying
{
    /// <summary>
    /// Can be used to retrieve component instances in the scene(s), reuse those values
    /// and refresh them if necessary.
    /// </summary>
    public partial class ComponentQuery : IComponentQuery
    {
        /// <summary>
        /// Whether the query should execute the query's each time before
        /// the values are retrieved.
        /// </summary>
        public bool autoRefresh;

        /// <summary>
        /// A custom query to be implemented by an object that can query for components in parents or children.
        /// </summary>
        protected IFromComponentQuery p_fromComponentQuery;

        /// <summary>
        /// A custom query to be implemented by an object that can query for components on a game object.
        /// </summary>
        protected IOnGameObjectQuery p_onGameObjectQuery;

        /// <summary>
        /// To be implemented by objects that can query for component values
        /// in a scene or prefab stage context and return the result.
        /// </summary>
        protected IOnSceneQuery p_onSceneQuery;

        /// <summary>
        /// Holds scene queries to be executed when values are retrieved.
        /// </summary>
        protected readonly List<IComponentQuery> p_queries = new List<IComponentQuery>();

        /// <summary>
        /// Holds values retrieved by the query's result.
        /// </summary>
        private readonly List<Component> _values = new List<Component>();

        /// <summary>
        /// The default interface used for querying for component values.
        /// </summary>
        private readonly DefaultQueryInterface _defaultQuery = new DefaultQueryInterface();

        /// <summary>
        /// Whether this query is refreshed. If this query is not refreshed,
        /// query's will be executed when retrieving values.
        /// </summary>
        private bool _isRefreshed;

        /// <summary>
        /// Creates a new instance of the scene query.
        /// </summary>
        /// <param name="autoRefresh">Whether the query should execute the query's each time before
        /// the values are retrieved.</param>
        public ComponentQuery(bool autoRefresh = false) => this.autoRefresh = autoRefresh;

        /// <summary>
        /// Creates a new instance of the scene query.
        /// </summary>
        /// <param name="query">The query to initialize with.</param>
        /// <param name="autoRefresh">Whether the query should execute the query's each time before
        /// the values are retrieved.</param>
        public ComponentQuery(IComponentQuery query, bool autoRefresh = false)
        {
            Use(query);
            this.autoRefresh = autoRefresh;
        }

        /// <summary>
        /// Creates a new instance of the scene query.
        /// </summary>
        /// <param name="queries">The queries to initialize with.</param>
        /// <param name="autoRefresh">Whether the query should execute the query's each time before
        /// the values are retrieved.</param>
        public ComponentQuery(IEnumerable<IComponentQuery> queries, bool autoRefresh = false)
        {
            Use(queries);
            this.autoRefresh = autoRefresh;
        }

        /// <summary>
        /// Returns the component values resulting of the queries added. If auto refresh
        /// is on, this will execute the queries each time.
        /// </summary>
        /// <returns>The component values resulting of the queries added.</returns>
        public Component[] Values()
        {
            // If values require auto refreshing or the query hasn't refreshed yet.
            if (autoRefresh || !_isRefreshed)
            {
                // Refresh the values.
                _values.Clear();

                for (int i = 0; i < p_queries.Count; i++)
                    _values.AddRange(p_queries[i].Values());

                _isRefreshed = true;
            }

            return _values.ToArray();
        }

        /// <summary>
        /// Returns the component value resulting of the queries added. If auto refresh
        /// is on, this will execute the queries each time. Value() will always return
        /// the last added value in the query result if there are multiple.
        /// </summary>
        /// <returns>The component value resulting of the queries added.</returns>
        public Component Value()
        {
            Component[] values = Values();
            if (values.Length == 0)
                return null;

            return values[values.Length - 1];
        }

        /// <summary>
        /// Returns the component values resulting of the queries added. If auto refresh
        /// is on, this will execute the queries each time.
        /// </summary>
        /// <returns>The component values resulting of the queries added.</returns>
        public T[] Values<T>() where T : Component
        {
            var genericValues = new List<T>();

            // If values require auto refreshing or the query hasn't refreshed yet.
            if (autoRefresh || !_isRefreshed)
            {
                // Refresh the values.
                _values.Clear();

                for (int i = 0; i < p_queries.Count; i++)
                    _values.AddRange(p_queries[i].Values<T>());

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

        /// <summary>
        /// Returns the component value resulting of the queries added. If auto refresh
        /// is on, this will execute the queries each time. Value() will always return
        /// the last added value in the query result if there are multiple.
        /// </summary>
        /// <returns>The component value resulting of the queries added.</returns>
        public T Value<T>() where T : Component
        {
            T[] values = Values<T>();
            if (values.Length == 0)
                return null;

            return values[values.Length - 1];
        }

        /// <summary>
        /// Uses a custom scene query to retrieve components.
        /// </summary>
        /// <param name="query">The custom query.</param>
        public ComponentQuery Use(IComponentQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            p_queries.Add(query);
            return this;
        }

        /// <summary>
        /// use a custom query to be implemented by an object that can query for components in parents or children.
        /// </summary>
        /// <param name="query">The custom query.</param>
        public ComponentQuery Use(IOnGameObjectQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            p_onGameObjectQuery = query;
            return this;
        }

        /// <summary>
        /// Use a custom query to be implemented by an object that can query for components on a game object.
        /// </summary>
        /// <param name="query">The custom query.</param>
        public ComponentQuery Use(IFromComponentQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            p_fromComponentQuery = query;
            return this;
        }

        /// <summary>
        /// To be implemented by objects that can query for component values
        /// in a scene or prefab stage context and return the result.
        /// </summary>
        /// <param name="query">The custom query.</param>
        public ComponentQuery Use(IOnSceneQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            p_onSceneQuery = query;
            return this;
        }

        /// <summary>
        /// Uses custom scene queries to retrieve components.
        /// </summary>
        /// <param name="queries">The custom queries.</param>
        public ComponentQuery Use(IEnumerable<IComponentQuery> queries)
        {
            if (queries == null)
                throw new ArgumentNullException(nameof(queries));

            foreach (IComponentQuery query in queries)
                Use(query);

            return this;
        }

        /// <summary>
        /// Dirties the query, ensuring it will be refreshed when retrieving the values.
        /// </summary>
        public ComponentQuery Dirty()
        {
            _isRefreshed = false;
            return this;
        }

        /// <summary>
        /// Clears the query, removing all of its currently stored scene queries.
        /// Will also dirty the query, ensuring it will be refreshed when retrieving the new values.
        /// </summary>
        public ComponentQuery Clear()
        {
            p_queries.Clear();

            Dirty();

            return this;
        }

        /// <summary>
        /// Resets the query, setting the query interface back to the default.
        /// Will also clear the query, removing all of its currently stored scene queries
        /// and ensuring it will be refreshed when retrieving the new values.
        /// </summary>
        public virtual ComponentQuery Reset()
        {
            p_fromComponentQuery = null;
            p_onGameObjectQuery = null;
            p_onSceneQuery = null;

            Clear();

            return this;
        }

        /// <summary>
        /// Adds a query that looks for child components from a given parent component.
        /// The default query uses Component.GetComponentsInChildren to get its result.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public ComponentQuery OnChildren<T>(Component parentComponent, bool includeInactive = false)
            => OnChildren(parentComponent, includeInactive, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in children from a given parent component.
        /// The default query uses Component.GetComponentsInChildren to get its result.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        public ComponentQuery OnChildren(Component parentComponent, bool includeInactive = false)
            => OnChildren(parentComponent, includeInactive, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in children from a given parent component.
        /// The default query uses Component.GetComponentsInChildren to get its result.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public ComponentQuery OnChildren(Component parentComponent, params Type[] componentType) => OnChildren(parentComponent, false, componentType);

        /// <summary>
        /// Adds a query that looks for components in children from a given parent component.
        /// The default query uses Component.GetComponentsInChildren to get its result.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public ComponentQuery OnChildren(Component parentComponent, bool includeInactive, params Type[] componentType)
        {
            FromGivenMethod method = (p_fromComponentQuery ?? _defaultQuery).FindComponentsOnChildren;
            p_queries.Add(new FromGivenQuery(method, parentComponent, includeInactive, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components in the parent from a given child component.
        /// The default query uses Component.GetComponentsInParent to get its result.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public ComponentQuery OnParent<T>(Component childComponent, bool includeInactive = false) => OnParent(childComponent, includeInactive, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in the parent from a given child component.
        /// The default query uses Component.GetComponentsInParent to get its result.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        public ComponentQuery OnParent(Component childComponent, bool includeInactive = false)
            => OnParent(childComponent, includeInactive, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in the parent from a given child component.
        /// The default query uses Component.GetComponentsInParent to get its result.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public ComponentQuery OnParent(Component childComponent, params Type[] componentType) => OnParent(childComponent, false, componentType);

        /// <summary>
        /// Adds a query that looks for components in the parent from a given child component.
        /// The default query uses Component.GetComponentsInParent to get its result.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public ComponentQuery OnParent(Component childComponent, bool includeInactive, params Type[] componentType)
        {
            FromGivenMethod method = (p_fromComponentQuery ?? _defaultQuery).FindComponentsOnParent;
            p_queries.Add(new FromGivenQuery(method, childComponent, includeInactive, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components on the game object a given component is on.
        /// The default query uses Component.GetComponents to get its result.
        /// </summary>
        /// <param name="gameObject">The game object to search from.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public ComponentQuery OnGameObject<T>(GameObject gameObject) => OnGameObject(gameObject, typeof(T));

        /// <summary>
        /// Adds a query that looks for components on the game object a given component is on.
        /// The default query uses Component.GetComponents to get its result.
        /// </summary>
        /// <param name="gameObject">The game object to search from.</param>
        public ComponentQuery OnGameObject(GameObject gameObject) => OnGameObject(gameObject, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components on the game object a given component is on.
        /// The default query uses Component.GetComponents to get its result.
        /// </summary>
        /// <param name="gameObject">The game object to search from.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public ComponentQuery OnGameObject(GameObject gameObject, params Type[] componentType)
        {
            OnGameObjectMethod method = (p_onGameObjectQuery ?? _defaultQuery).FindComponentsOnGameObject;
            p_queries.Add(new OnGameObjectQuery(method, gameObject, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given tag name.
        /// The default query uses GameObject.FindGameObjectsWithTag to get its result.
        /// </summary>
        /// <param name="tagName">The name of the tag to use.</param>
        public ComponentQuery OnTag(string tagName) => OnTag(tagName, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given tag name.
        /// The default query uses GameObject.FindGameObjectsWithTag to get its result.
        /// </summary>
        /// <param name="tagName">The name of the tag to use.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public ComponentQuery OnTag<T>(string tagName) where T : Component => OnTag(tagName, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) attached to a game object with a given tag name.
        /// The default query uses GameObject.FindGameObjectsWithTag to get its result.
        /// </summary>
        /// <param name="tagName">The name of the tag to use.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public ComponentQuery OnTag(string tagName, params Type[] componentType)
        {
            OnNameOrTagMethod method = (p_onSceneQuery ?? _defaultQuery).FindComponentsByTag;
            p_queries.Add(new OnNameOrTagQuery(method, tagName, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components in the scene(s) attached to a game object with a given name.
        /// The default query uses GameObject.Find to get its result.
        /// </summary>
        /// <param name="objectName">The name of the object to use.</param>
        public ComponentQuery OnName(string objectName) => OnName(objectName, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) attached to a game object with a given name.
        /// The default query uses GameObject.Find to get its result.
        /// </summary>
        /// <param name="objectName">The name of the object to use.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public ComponentQuery OnName<T>(string objectName) where T : Component => OnName(objectName, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) attached to a game object with a given name.
        /// The default query uses GameObject.Find to get its result.
        /// </summary>
        /// <param name="objectName">The name of the object to use.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public ComponentQuery OnName(string objectName, params Type[] componentType)
        {
            OnNameOrTagMethod method = (p_onSceneQuery ?? _defaultQuery).FindComponentsByName;
            p_queries.Add(new OnNameOrTagQuery(method, objectName, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given type.
        /// The default query uses Object.FindObjectsOfType or Resources.FindObjectsOfTypeAll to get its result.
        /// </summary>
        public ComponentQuery OnType() => OnType(false, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given type.
        /// The default query uses Object.FindObjectsOfType or Resources.FindObjectsOfTypeAll to get its result.
        /// </summary>
        /// <param name="componentType">The component type(s) to look for.</param>
        public ComponentQuery OnType(params Type[] componentType) => OnType(false, componentType);

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given type.
        /// The default query uses Object.FindObjectsOfType or Resources.FindObjectsOfTypeAll to get its result.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public ComponentQuery OnType<T>(bool includeInactive = false) where T : Component => OnType(includeInactive, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given type.
        /// The default query uses Object.FindObjectsOfType or Resources.FindObjectsOfTypeAll to get its result.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public ComponentQuery OnType(bool includeInactive, params Type[] componentType)
        {
            OnTypeMethod method = (p_onSceneQuery ?? _defaultQuery).FindComponentsByType;
            p_queries.Add(new OnTypeQuery(method, includeInactive, componentType));
            return this;
        }
    }
}


