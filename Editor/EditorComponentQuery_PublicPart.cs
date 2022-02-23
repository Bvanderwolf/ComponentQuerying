using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying.Editor
{
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
        /// Selects game objects with components of given type(s).
        /// </summary>
        /// <param name="componentType">The type of component(s) to look for.</param>
        /// <returns>The found components</returns>
        public EditorComponentQuery Select(params Type[] componentType) => Select(false, componentType);

        /// <summary>
        /// Selects game objects with components of mono script class type.
        /// </summary>
        /// <param name="monoScript">The mono script of a mono behaviour class.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <returns>The found components</returns>
        public EditorComponentQuery Select(MonoScript monoScript, bool includeInactive = false) => Select(includeInactive, monoScript.GetClass());

        /// <summary>
        /// Selects game objects with components of given type(s).
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        /// <returns>The selected components</returns>
        public EditorComponentQuery Select(bool includeInactive = false, params Type[] componentType)
        {
            p_queries.Add(new SelectQuery(SelectGameObjectsWithComponents, includeInactive, componentType));
            return this;
        }

        public EditorComponentQuery OnSelected() => OnSelected(false, false, typeof(Component));


        public EditorComponentQuery OnSelected(params Type[] componentTye) => OnSelected(false, false, componentTye);


        public EditorComponentQuery OnSelected(bool resetSelection, params Type[] componentType) => OnSelected(false, resetSelection, componentType);


        public EditorComponentQuery OnSelected<T>(bool includeInActive = false) where T : Component => OnSelected(includeInActive, typeof(T));


        public EditorComponentQuery OnSelected(bool includeInActive, bool resetSelection, params Type[] componentType)
        {
            p_queries.Add(new OnSelectedQuery(FindComponentsOnSelectedGameObjects, includeInActive, resetSelection, componentType));
            return this;
        }
    }
}