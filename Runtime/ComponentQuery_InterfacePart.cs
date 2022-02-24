using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BWolf.MonoBehaviourQuerying
{
    /// <summary>
    /// Can be used to retrieve component instances in the scene, reuse those values
    /// and refresh them if necessary.
    /// </summary>
    public partial class ComponentQuery
    {
        /// <summary>
        /// The default query interface used for retrieving components on game objects, from components
        /// and/or in scene(s).
        /// </summary>
        internal class DefaultQueryInterface : IOnGameObjectQuery, IFromComponentQuery, IOnSceneQuery
        {
            /// <summary>
            /// Looks for components in children from a given parent component.
            /// </summary>
            /// <param name="parentComponent">The parent component to search from.</param>
            /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
            /// <param name="componentType">The type of component(s) to look for.</param>
            public Component[] FindComponentsOnChildren(Component parentComponent, bool includeInactive, params Type[] componentType)
            {
                if (parentComponent == null)
                    throw new ArgumentNullException(nameof(parentComponent));

                var components = new List<Component>(parentComponent.GetComponentsInChildren(typeof(Component), includeInactive));
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

            /// <summary>
            /// Looks for components in the parent from a given child component.
            /// </summary>
            /// <param name="childComponent">The child component to search from.</param>
            /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
            /// <param name="componentType">The type of component(s) to look for.</param>
            public Component[] FindComponentsOnParent(Component childComponent, bool includeInactive, params Type[] componentType)
            {
                if (childComponent == null)
                    throw new ArgumentNullException(nameof(childComponent));

                var components = new List<Component>(childComponent.GetComponentsInParent(typeof(Component), includeInactive));
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

            /// <summary>
            /// Looks for components on the game object a given component is on.
            /// </summary>
            /// <param name="gameObject">The game object to search from.</param>
            /// <param name="componentType">The type of component(s) to look for.</param>
            public Component[] FindComponentsOnGameObject(GameObject gameObject, params Type[] componentType)
            {
                if (gameObject == null)
                    throw new ArgumentNullException(nameof(gameObject));

                var components = new List<Component>(gameObject.GetComponents(typeof(Component)));
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

            /// <summary>
            /// Looks for components in the scene(s) attached to a game object with a given tag name.
            /// </summary>
            /// <param name="tagName">The name of the tag to use.</param>
            /// <param name="componentType">The type of component(s) to look for.</param>
            public Component[] FindComponentsByTag(string tagName, params Type[] componentType)
            {
                if (tagName == null)
                    throw new ArgumentNullException(nameof(tagName));

                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tagName);
                if (gameObjects.Length == 0)
                {
                    Debug.LogWarning($"Failed finding a game object in the scene(s) with the tag {tagName} during query.");
                    return Array.Empty<Component>();
                }

                var components = new List<Component>(gameObjects.SelectMany(go => go.GetComponents<Component>()));
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

            /// <summary>
            /// Looks for components in the scene(s) attached to a game object with a given name.
            /// </summary>
            /// <param name="objectName">The name of the object to use.</param>
            /// <param name="componentType">The type of component(s) to look for.</param>
            public Component[] FindComponentsByName(string objectName, params Type[] componentType)
            {
                if (objectName == null)
                    throw new ArgumentNullException(nameof(objectName));

                GameObject gameObject = GameObject.Find(objectName);
                if (gameObject == null)
                {
                    Debug.LogWarning($"Failed finding a game object in the scene(s) with name {objectName} during query.");
                    return Array.Empty<Component>();
                }

                var components = new List<Component>(gameObject.GetComponents<Component>());
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

            /// <summary>
            /// Looks for components in the scene(s) with a given type.
            /// </summary>
            /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
            /// <param name="componentType">The type of component(s) to look for.</param>
            public Component[] FindComponentsByType(bool includeInactive, params Type[] componentType)
            {
                if (componentType == null)
                    throw new ArgumentNullException(nameof(componentType));

                if (componentType.Length == 0)
                    throw new ArgumentException("No mono behaviour types were given.");

                var components = new List<Component>();

                // We are in a scene and should look for objects there.
                components.AddRange(includeInactive ? Resources.FindObjectsOfTypeAll<Component>() : Object.FindObjectsOfType<Component>());
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }
        }

        /// <summary>
        /// Filters a given list of components on a white list of types that should be implemented.
        /// </summary>
        /// <param name="components">The components to filter.</param>
        /// <param name="whitelist">The whitelist of components to filter with.</param>
        protected static void FilterComponentsUsingWhitelist(List<Component> components, Type[] whitelist)
        {
            for (int i = components.Count - 1; i >= 0; i--)
                if (!whitelist.Any(type => type.IsInstanceOfType(components[i]))) // Weghalen van LinQ hier
                    components.RemoveAt(i);
        }
    }
}
