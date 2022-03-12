using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BWolf.ComponentQuerying.Editor
{
    /// <summary>
    /// Can be used to retrieve component instances in the scene in edit mode, reuse those values
    /// and refresh them if necessary.
    /// </summary>
    public partial class EditorComponentQuery : ComponentQuery
    {
        /// <summary>
        /// Creates a new instance of the scene query.
        /// </summary>
        /// <param name="autoRefresh">Whether the query should execute the query's each time before
        /// the values are retrieved.</param>
        public EditorComponentQuery(bool autoRefresh = false)
            : base(autoRefresh) => p_onSceneQuery = new SceneQuery();

        /// <summary>
        /// Creates a new instance of the scene query.
        /// </summary>
        /// <param name="query">The query to initialize with.</param>
        /// <param name="autoRefresh">Whether the query should execute the query's each time before
        /// the values are retrieved.</param>
        public EditorComponentQuery(IComponentQuery query, bool autoRefresh = false)
            : base(query, autoRefresh) => p_onSceneQuery = new SceneQuery();

        /// <summary>
        /// Creates a new instance of the scene query.
        /// </summary>
        /// <param name="queries">The queries to initialize with.</param>
        /// <param name="autoRefresh">Whether the query should execute the query's each time before
        /// the values are retrieved.</param>
        public EditorComponentQuery(IEnumerable<IComponentQuery> queries, bool autoRefresh = false)
            : base(queries, autoRefresh) => p_onSceneQuery = new SceneQuery();

        /// <inheritdoc/>
        public override ComponentQuery Reset()
        {
            p_onGameObjectQuery = null;
            p_fromComponentQuery = null;

            // The 'OnScene' query should not be reset as it has its own editor implementation.

            Clear();

            return this;
        }

        /// <summary>
        /// Adds a query that selects game objects with components of given type(s).
        /// </summary>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public EditorComponentQuery Select(params Type[] componentType) => Select(false, componentType);

        /// <summary>
        /// Adds a query that selects game objects with components of mono script class type.
        /// </summary>
        /// <param name="monoScript">The mono script of a mono behaviour class.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        public EditorComponentQuery Select(MonoScript monoScript, bool includeInactive = false) => Select(includeInactive, monoScript.GetClass());

        /// <summary>
        /// Adds a query that selects game objects with components of given type(s).
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        public EditorComponentQuery Select(bool includeInactive = false, params Type[] componentType)
        {
            p_queries.Add(new SelectQuery(SelectGameObjectsWithComponents, includeInactive, componentType));
            return this;
        }

        /// <summary>
        /// Adds a query that finds components on game objects selected in the scene.
        /// </summary>
        public EditorComponentQuery OnSelected() => OnSelected(false, false, typeof(Component));
        
        /// <summary>
        /// Adds a query that finds components on game objects selected in the scene.
        /// </summary>
        /// <param name="componentType">The type(s) of components to look for.</param>
        public EditorComponentQuery OnSelected(params Type[] componentType) => OnSelected(false, false, componentType);

        /// <summary>
        /// Adds a query that finds components on game objects selected in the scene.
        /// </summary>
        /// <param name="resetSelection">Whether to reset the current selection of game objects.</param>
        /// <param name="componentType">The type(s) of components to look for.</param>
        public EditorComponentQuery OnSelected(bool resetSelection, params Type[] componentType) => OnSelected(false, resetSelection, componentType);

        /// <summary>
        /// Adds a query that finds components on game objects selected in the scene.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        public EditorComponentQuery OnSelected<T>(bool includeInactive = false) where T : Component => OnSelected(includeInactive, typeof(T));

        /// <summary>
        /// Adds a query that finds components on game objects selected in the scene.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="resetSelection">Whether to reset the current selection of game objects.</param>
        /// <param name="componentType">The type(s) of components to look for.</param>
        public EditorComponentQuery OnSelected(bool includeInactive, bool resetSelection, params Type[] componentType)
        {
            p_queries.Add(new OnSelectedQuery(FindComponentsOnSelectedGameObjects, includeInactive, resetSelection, componentType));
            return this;
        }
    }
}