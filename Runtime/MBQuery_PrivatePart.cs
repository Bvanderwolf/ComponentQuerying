using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif

namespace BWolf.MonoBehaviourQuerying
{
    public partial class MBQuery
    {
        private Component[] FindComponentsOnComponent(Component target, params Type[] componentType)
        {
            return null;
        }

        private Component[] FindComponentsByTag(string tagName, params Type[] componentType)
        {
            if (tagName == null)
                throw new ArgumentNullException(nameof(tagName));

            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tagName);
            if (gameObjects.Length == 0)
            {
                Debug.LogWarning($"Failed finding a game object in the scene(s) with the tag {tagName} during query.");
                return Array.Empty<Component>();
            }

            List<Component> components = new List<Component>(gameObjects.SelectMany(go => go.GetComponents<Component>()));
            FilterComponentsUsingWhitelist(components, componentType);

            return components.ToArray();
        }

        private Component[] FindComponentsByName(string objectName, params Type[] componentType)
        {
            if (objectName == null)
                throw new ArgumentNullException(nameof(objectName));

            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                Debug.LogWarning($"Failed finding a game object in the scene(s) with name {objectName} during query.");
                return Array.Empty<Component>();
            }

            List<Component> components = new List<Component>(gameObject.GetComponents<Component>());
            FilterComponentsUsingWhitelist(components, componentType);

            return components.ToArray();
        }

        /// <summary>
        /// Returns a list of mono behaviours of given type(s) found in the active scene or prefab stage.
        /// </summary>
        private Component[] FindComponentsByType(bool includeInactive, params Type[] componentType)
        {
            if (componentType == null)
                throw new ArgumentNullException(nameof(componentType));

            if (componentType.Length == 0)
                throw new ArgumentException("No mono behaviour types were given.");

            List<Component> components = new List<Component>();

#if UNITY_EDITOR
            // Look for mono behaviours in the current prefab stage if we are in the editor and it exists.
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                components.AddRange(stage.prefabContentsRoot.GetComponentsInChildren<Component>(includeInactive));
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }
#endif

            // We are in a scene and should look for objects there.
            // *Resources.FindObjectsOfAll is om ook inactive objects in de scene te vinden. Check of
            // hij ook alle field references meepakt, want dan moeten duplicates worden verwijderd.
            components.AddRange(includeInactive ? Resources.FindObjectsOfTypeAll<Component>() : Object.FindObjectsOfType<Component>());
            FilterComponentsUsingWhitelist(components, componentType);

            return components.ToArray();

        }

        private static void FilterComponentsUsingWhitelist(List<Component> components, Type[] whitelist)
        {
            for (int i = components.Count - 1; i >= 0; i--)
                if (!whitelist.Any(type => type.IsInstanceOfType(components[i])))
                    components.RemoveAt(i);
        }
    }
}