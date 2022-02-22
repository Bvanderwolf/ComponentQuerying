using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    /// <summary>
    /// Can be used to retrieve component instances in the scene, reuse those values
    /// and refresh them if necessary.
    /// </summary>
    public partial class SceneQuery : ISceneQuery
    {
        /*
         - Toevoegen van includeInActive flag voor OnChildren en OnParent

         - Toevoegen van Extension method genaamd Query() voor een Component instance.
          - doel is om Component.Query() te kunnen gebruiken zodat queries makkelijk te doen zijn vanaf components. 
          - voeg SceneQuery.OnThis, SceneQuery.OnParent en SceneQuery.OnChildren toe zodat deze queries makkelijk vanaf components te doen zijn.
           - Intern worden deze OnGiven, OnParent en OnChildren methodes gebruikt.
         */

        /// <summary>
        /// Holds scene queries to be executed when values are retrieved.
        /// </summary>
        private readonly List<ISceneQuery> _queries = new List<ISceneQuery>();

        /// <summary>
        /// Holds values retrieved by the query's result.
        /// </summary>
        private readonly List<Component> _values = new List<Component>();

        /// <summary>
        /// Whether this query is refreshed. If this query is not refreshed,
        /// query's will be executed when retrieving values.
        /// </summary>
        private bool _isRefreshed;

        /// <summary>
        /// Whether the query should execute the query's each time before
        /// the values are retrieved.
        /// </summary>
        public bool autoRefresh;

        /// <summary>
        /// Creates a new instance of the scene query.
        /// </summary>
        /// <param name="autoRefresh">Whether the query should execute the query's each time before
        /// the values are retrieved.</param>
        public SceneQuery(bool autoRefresh = false) => this.autoRefresh = autoRefresh;

        /// <summary>
        /// Creates a new instance of the scene query.
        /// </summary>
        /// <param name="query">The query to initialize with.</param>
        /// <param name="autoRefresh">Whether the query should execute the query's each time before
        /// the values are retrieved.</param>
        public SceneQuery(ISceneQuery query, bool autoRefresh = false)
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
        public SceneQuery(IEnumerable<ISceneQuery> queries, bool autoRefresh = false)
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

                for (int i = 0; i < _queries.Count; i++)
                    _values.AddRange(_queries[i].Values());

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
        public SceneQuery Use(ISceneQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            _queries.Add(query);
            return this;
        }

        /// <summary>
        /// Uses custom scene queries to retrieve components.
        /// </summary>
        /// <param name="query">The custom queries.</param>
        public SceneQuery Use(IEnumerable<ISceneQuery> queries)
        {
            if (queries == null)
                throw new ArgumentNullException(nameof(queries));

            _queries.AddRange(queries);
            return this;
        }

        /// <summary>
        /// Dirties the query, ensuring it will be refreshed when retrieving the values.
        /// </summary>
        public SceneQuery Dirty()
        {
            _isRefreshed = false;
            return this;
        }

        /// <summary>
        /// Clears the query, removing all of its currently stored scene queries.
        /// Will also dirty the query, ensuring it will be refreshed when retrieving the new values.
        /// </summary>
        public SceneQuery Clear()
        {
            _queries.Clear();

            Dirty();

            return this;
        }

        /// <summary>
        /// Adds a query that looks for child components from a given parent component.
        /// Internally this query uses Component.GetComponentsInChildren to get its result.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public SceneQuery OnChildren<T>(Component parentComponent, bool includeInactive = false)
            => OnChildren(parentComponent, includeInactive, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in children from a given parent component.
        /// Internally this query uses Component.GetComponentsInChildren to get its result.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        public SceneQuery OnChildren(Component parentComponent, bool includeInactive = false)
            => OnChildren(parentComponent, includeInactive, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in children from a given parent component.
        /// Internally this query uses Component.GetComponentsInChildren to get its result.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public SceneQuery OnChildren(Component parentComponent, params Type[] componentType) => OnChildren(parentComponent, false, componentType);

        /// <summary>
        /// Adds a query that looks for components in children from a given parent component.
        /// Internally this query uses Component.GetComponentsInChildren to get its result.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public SceneQuery OnChildren(Component parentComponent, bool includeInactive, params Type[] componentType)
        {
            _queries.Add(new FromGivenQuery(FindComponentsOnChildren, parentComponent, includeInactive, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components in the parent from a given child component.
        /// Internally this query uses Component.GetComponentsInParent to get its result.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public SceneQuery OnParent<T>(Component childComponent, bool includeInactive = false) => OnParent(childComponent, includeInactive, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in the parent from a given child component.
        /// Internally this query uses Component.GetComponentsInParent to get its result.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        public SceneQuery OnParent(Component childComponent, bool includeInactive = false)
            => OnParent(childComponent, includeInactive, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in the parent from a given child component.
        /// Internally this query uses Component.GetComponentsInParent to get its result.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public SceneQuery OnParent(Component childComponent, params Type[] componentType) => OnParent(childComponent, false, componentType);

        /// <summary>
        /// Adds a query that looks for components in the parent from a given child component.
        /// Internally this query uses Component.GetComponentsInParent to get its result.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public SceneQuery OnParent(Component childComponent, bool includeInactive, params Type[] componentType)
        {
            _queries.Add(new FromGivenQuery(FindComponentsOnParent, childComponent, includeInactive, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components on a given component
        /// Internally this query uses Component.GetComponents to get its result.
        /// </summary>
        /// <param name="onComponent">The component to search on.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public SceneQuery OnGiven<T>(Component onComponent) => OnGiven(onComponent, typeof(T));

        /// <summary>
        /// Adds a query that looks for components on a given component
        /// Internally this query uses Component.GetComponents to get its result.
        /// </summary>
        /// <param name="onComponent">The component to search on.</param>
        public SceneQuery OnGiven(Component onComponent) => OnGiven(onComponent, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components on a given component
        /// Internally this query uses Component.GetComponents to get its result.
        /// </summary>
        /// <param name="onComponent">The component to search on.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public SceneQuery OnGiven(Component onComponent, params Type[] componentType)
        {
            _queries.Add(new OnGivenQuery(FindComponentsOnGiven, onComponent, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given tag name.
        /// Internally this query uses GameObject.FindGameObjectsWithTag to get its result.
        /// </summary>
        /// <param name="tagName">The name of the tag to use.</param>
        public SceneQuery OnTag(string tagName) => OnTag(tagName, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given tag name.
        /// Internally this query uses GameObject.FindGameObjectsWithTag to get its result.
        /// </summary>
        /// <param name="tagName">The name of the tag to use.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public SceneQuery OnTag<T>(string tagName) where T : Component => OnTag(tagName, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) attached to a game object with a given tag name.
        /// Internally this query uses GameObject.FindGameObjectsWithTag to get its result.
        /// </summary>
        /// <param name="tagName">The name of the tag to use.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public SceneQuery OnTag(string tagName, params Type[] componentType)
        {
            _queries.Add(new OnNameOrTagQuery(FindComponentsByTag, tagName, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components in the scene(s) attached to a game object with a given name.
        /// Internally this query uses GameObject.Find to get its result.
        /// </summary>
        /// <param name="objectName">The name of the object to use.</param>
        public SceneQuery OnName(string objectName) => OnName(objectName, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) attached to a game object with a given name.
        /// Internally this query uses GameObject.Find to get its result.
        /// </summary>
        /// <param name="objectName">The name of the object to use.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public SceneQuery OnName<T>(string objectName) where T : Component => OnName(objectName, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) attached to a game object with a given name.
        /// Internally this query uses GameObject.Find to get its result.
        /// </summary>
        /// <param name="objectName">The name of the object to use.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public SceneQuery OnName(string objectName, params Type[] componentType)
        {
            _queries.Add(new OnNameOrTagQuery(FindComponentsByName, objectName, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given type.
        /// Internally this query uses Object.FindObjectsOfType or Resources.FindObjectsOfTypeAll to get its result.
        /// </summary>
        public SceneQuery OnType() => OnType(false, typeof(Component));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given type.
        /// Internally this query uses Object.FindObjectsOfType or Resources.FindObjectsOfTypeAll to get its result.
        /// </summary>
        /// <param name="componentType">The component type(s) to look for.</param>
        public SceneQuery OnType(params Type[] componentType) => OnType(false, componentType);

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given type.
        /// Internally this query uses Object.FindObjectsOfType or Resources.FindObjectsOfTypeAll to get its result.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public SceneQuery OnType<T>(bool includeInactive = false) where T : Component => OnType(includeInactive, typeof(T));

        /// <summary>
        /// Adds a query that looks for components in the scene(s) with a given type.
        /// Internally this query uses Object.FindObjectsOfType or Resources.FindObjectsOfTypeAll to get its result.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public SceneQuery OnType(bool includeInactive, params Type[] componentType)
        {
            _queries.Add(new OnTypeQuery(FindComponentsByType, includeInactive, componentType));
            return this;
        }
    }
}


