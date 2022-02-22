using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    /// <summary>
    /// Can be used to retrieve component instances in the scene, reuse those values
    /// and refresh them if necessary.
    /// </summary>
    public partial class SceneQuery
    {
        /// <summary>
        /// Holds contextual information for a scene query that looks for mono behaviours of given types in the scene(s).
        /// </summary>
        private readonly struct OnTypeQuery : ISceneQuery
        {
            /// <summary>
            /// The method to perform the query.
            /// </summary>
            private readonly OnTypeMethod _method;

            /// <summary>
            /// Whether to include inactive game objects in the search.
            /// </summary>
            private readonly bool _includeInactive;

            /// <summary>
            /// The type of component type(s) to look for.
            /// </summary>
            private readonly Type[] _componentTypes;

            /// <summary>
            /// Creates a new query instance.
            /// </summary>
            /// <param name="method">The method to perform the query.</param>
            /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
            /// <param name="componentTypes">The type of component type(s) to look for.</param>
            public OnTypeQuery(OnTypeMethod method, bool includeInactive, Type[] componentTypes)
            {
                _method = method;
                _includeInactive = includeInactive;
                _componentTypes = componentTypes;
            }

            /// <inheritdoc/>
            public Component[] Values() => _method.Invoke(_includeInactive, _componentTypes);

            /// <inheritdoc/>
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

        /// <summary>
        /// Holds contextual information for a scene query that looks for mono behaviours from a given component.
        /// </summary>
        private readonly struct FromGivenQuery : ISceneQuery
        {
            /// <summary>
            /// The method to perform the query.
            /// </summary>
            private readonly FromGivenMethod _method;

            /// <summary>
            /// The given component to perform the query on.
            /// </summary>
            private readonly Component _givenComponent;

            /// <summary>
            /// Whether to include inactive game objects in the search.
            /// </summary>
            private readonly bool _includeInactive;

            /// <summary>
            /// The type of component type(s) to look for.
            /// </summary>
            private readonly Type[] _componentTypes;

            /// <summary>
            /// Creates a new instance of the query.
            /// </summary>
            /// <param name="method">The method to perform the query.</param>
            /// <param name="givenComponent">The given component to perform the query from.</param>
            /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
            /// <param name="componentTypes">The type of component type(s) to look for.</param>
            public FromGivenQuery(FromGivenMethod method, Component givenComponent, bool includeInactive, Type[] componentTypes)
            {
                _method = method;
                _givenComponent = givenComponent;
                _includeInactive = includeInactive;
                _componentTypes = componentTypes;
            }

            /// <inheritdoc/>
            public Component[] Values() => _method.Invoke(_givenComponent, _includeInactive, _componentTypes);

            /// <inheritdoc/>
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

        /// <summary>
        /// Holds contextual information for a scene query that looks for mono behaviours on a given component.
        /// </summary>
        private readonly struct OnGivenQuery : ISceneQuery
        {
            /// <summary>
            /// The method to perform the query.
            /// </summary>
            private readonly OnGivenMethod _method;

            /// <summary>
            /// The given component to perform the query on.
            /// </summary>
            private readonly Component _givenComponent;

            /// <summary>
            /// The type of component type(s) to look for.
            /// </summary>
            private readonly Type[] _componentTypes;

            /// <summary>
            /// Creates a new instance of the query.
            /// </summary>
            /// <param name="method">The method to perform the query.</param>
            /// <param name="givenComponent">The given component to perform the query from.</param>
            /// <param name="componentTypes">The type of component type(s) to look for.</param>
            public OnGivenQuery(OnGivenMethod method, Component givenComponent, Type[] componentTypes)
            {
                _method = method;
                _givenComponent = givenComponent;
                _componentTypes = componentTypes;
            }

            /// <inheritdoc/>
            public Component[] Values() => _method.Invoke(_givenComponent, _componentTypes);

            /// <inheritdoc/>
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

        /// <summary>
        /// Holds contextual information for a scene query that looks for mono behaviours with a given name or tag.
        /// </summary>
        private readonly struct OnNameOrTagQuery : ISceneQuery
        {
            /// <summary>
            /// The method to perform the query.
            /// </summary>
            private readonly OnNameOrTagMethod _method;

            /// <summary>
            /// The objects name or tag.
            /// </summary>
            private readonly string _objectNameOrTag;

            /// <summary>
            /// The type of component type(s) to look for.
            /// </summary>
            private readonly Type[] _componentTypes;

            /// <summary>
            /// Creates a new instance of the query.
            /// </summary>
            /// <param name="method">The method to perform the query.</param>
            /// <param name="objectNameOrTag">The objects name or tag.</param>
            /// <param name="componentTypes">The type of component type(s) to look for.</param>
            public OnNameOrTagQuery(OnNameOrTagMethod method, string objectNameOrTag, Type[] componentTypes)
            {
                _method = method;
                _objectNameOrTag = objectNameOrTag;
                _componentTypes = componentTypes;
            }

            /// <inheritdoc/>
            public Component[] Values() => _method.Invoke(_objectNameOrTag, _componentTypes);

            /// <inheritdoc/>
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

        /// <summary>
        /// The method that looks for mono behaviours with a given name or tag. 
        /// </summary>
        /// <param name="objectNameOrTag">The objects name or tag.</param>
        /// <param name="componentTypes">The type of component type(s) to look for.</param>
        /// <returns>The found component(s).</returns>
        private delegate Component[] OnNameOrTagMethod(string objectNameOrTag, Type[] componentTypes);

        /// <summary>
        /// The method that looks for mono behaviours on a given component.
        /// </summary>
        /// <param name="givenComponent">The given component to perform the query on.</param>
        /// <param name="componentTypes">The type of component type(s) to look for.</param>
        /// <returns>The found component(s).</returns>
        private delegate Component[] OnGivenMethod(Component givenComponent, Type[] componentTypes);

        /// <summary>
        /// The method that looks for mono behaviours from a given component.
        /// </summary>
        /// <param name="givenComponent">The given component to perform the query on.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentTypes">The type of component type(s) to look for.</param>
        /// <returns>The found component(s).</returns>
        private delegate Component[] FromGivenMethod(Component givenComponent, bool includeInactive, Type[] componentTypes);

        /// <summary>
        /// The method that looks for mono behaviours of given types in the scene(s).
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentTypes">The type of component type(s) to look for.</param>
        /// <returns>The found component(s).</returns>
        private delegate Component[] OnTypeMethod(bool includeInactive, Type[] componentTypes);
    }
}
