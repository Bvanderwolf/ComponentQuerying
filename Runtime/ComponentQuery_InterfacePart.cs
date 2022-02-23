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
        internal class DefaultQueryInterface : IOnGameObjectQuery, IFromComponentQuery, IOnSceneQuery
        {
            public Component[] FindComponentsOnChildren(Component parentComponent, bool includeInactive, params Type[] componentType)
            {
                if (parentComponent == null)
                    throw new ArgumentNullException(nameof(parentComponent));

                var components = new List<Component>(parentComponent.GetComponentsInChildren(typeof(Component), includeInactive));
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

            public Component[] FindComponentsOnParent(Component childComponent, bool includeInactive, params Type[] componentType)
            {
                if (childComponent == null)
                    throw new ArgumentNullException(nameof(childComponent));

                var components = new List<Component>(childComponent.GetComponentsInParent(typeof(Component), includeInactive));
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

            public Component[] FindComponentsOnGameObject(GameObject gameObject, params Type[] componentType)
            {
                if (gameObject == null)
                    throw new ArgumentNullException(nameof(gameObject));

                var components = new List<Component>(gameObject.GetComponents(typeof(Component)));
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

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

        protected static void FilterComponentsUsingWhitelist(List<Component> components, Type[] whitelist)
        {
            for (int i = components.Count - 1; i >= 0; i--)
                if (!whitelist.Any(type => type.IsInstanceOfType(components[i]))) // Weghalen van LinQ hier
                    components.RemoveAt(i);
        }
    }
}
