using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BWolf.MonoBehaviourQuerying.Editor
{
    public partial class EditorComponentQuery
    {
        internal class SceneQuery : IOnSceneQuery
        {
            public Component[] FindComponentsByName(string objectName, params Type[] componentType)
            {
                if (objectName == null)
                    throw new ArgumentNullException(nameof(objectName));

                var components = new List<Component>();

                PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    // Look for components in the current prefab stage if it exists.
                    GameObject root = stage.prefabContentsRoot;
                    components.AddRange(root.GetComponentsInChildren<Component>(true).Where(component => component.name == objectName));
                    if (components.Count == 0)
                    {
                        Debug.LogWarning($"Failed finding a game object in the prefab stage with the name {objectName} during query.");
                        return Array.Empty<Component>();
                    }

                    FilterComponentsUsingWhitelist(components, componentType);

                    return components.ToArray();
                }

                GameObject gameObject = GameObject.Find(objectName);
                if (gameObject == null)
                {
                    Debug.LogWarning($"Failed finding a game object in the scene(s) with the name {objectName} during query.");
                    return Array.Empty<Component>();
                }

                components.AddRange(gameObject.GetComponents<Component>());
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }

            public Component[] FindComponentsByTag(string tagName, params Type[] componentType)
            {
                if (tagName == null)
                    throw new ArgumentNullException(nameof(tagName));

                var components = new List<Component>();

                PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    // Look for components in the current prefab stage if it exists.
                    GameObject root = stage.prefabContentsRoot;
                    components.AddRange(root.GetComponentsInChildren<Component>(true).Where(component => component.tag == tagName));
                    if (components.Count == 0)
                    {
                        Debug.LogWarning($"Failed finding a game object in the prefab stage with the tag {tagName} during query.");
                        return Array.Empty<Component>();
                    }

                    FilterComponentsUsingWhitelist(components, componentType);

                    return components.ToArray();
                }

                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tagName);
                if (gameObjects.Length == 0)
                {
                    Debug.LogWarning($"Failed finding a game object in the scene(s) with the tag {tagName} during query.");
                    return Array.Empty<Component>();
                }

                components.AddRange(gameObjects.SelectMany(go => go.GetComponents<Component>()));
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

                // Look for mono behaviours in the current prefab stage if it exists.
                PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    components.AddRange(stage.prefabContentsRoot.GetComponentsInChildren<Component>(includeInactive));
                    FilterComponentsUsingWhitelist(components, componentType);

                    return components.ToArray();
                }

                // We are in a scene and should look for objects there.
                components.AddRange(includeInactive ? Resources.FindObjectsOfTypeAll<Component>() : Object.FindObjectsOfType<Component>());
                FilterComponentsUsingWhitelist(components, componentType);

                return components.ToArray();
            }
        }


        public Component[] FindComponentsOnSelectedGameObjects()
        {
            if (Selection.gameObjects.Length == 0)
                return Array.Empty<Component>();

            return FindComponentsOnSelectedGameObjects(false, false, typeof(Component));
        }


        public Component[] FindComponentsOnSelectedGameObjects(params Type[] componentTye) => FindComponentsOnSelectedGameObjects(false, false, componentTye);


        public Component[] FindComponentsOnSelectedGameObjects(bool resetSelection, params Type[] componentType)
            => FindComponentsOnSelectedGameObjects(false, resetSelection, componentType);


        public Component[] FindComponentsOnSelectedGameObjects(bool includeInactive, bool resetSelection, params Type[] componentType)
        {
            if (componentType == null)
                throw new ArgumentNullException(nameof(componentType));

            if (Selection.gameObjects.Length == 0)
                return Array.Empty<Component>();

            Component[] selected = Selection.gameObjects
                .SelectMany(gameObject => gameObject.GetComponentsInChildren<Component>(includeInactive))
                .Where(component => componentType.Any(type => type == component.GetType()))
                .ToArray();

            if (resetSelection)
                Selection.objects = Array.Empty<Object>();

            return selected;
        }

        /// <summary>
        /// Selects game objects with components of given type(s).
        /// </summary>
        /// <param name="componentType">The type of component(s) to look for.</param>
        /// <returns>The found components</returns>
        public Component[] SelectGameObjectsWithComponents(params Type[] componentType)
            => SelectGameObjectsWithComponents(false, componentType);

        /// <summary>
        /// Selects game objects with components of mono script class type
        /// </summary>
        /// <param name="monoScript">The mono script of a mono behaviour class.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <returns>The found components</returns>
        public Component[] SelectGameObjectsWithComponents(MonoScript monoScript, bool includeInactive = false)
            => SelectGameObjectsWithComponents(includeInactive, monoScript.GetClass());

        /// <summary>
        /// Selects game objects with components of given type(s)/
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to look for.</param>
        /// <returns>The selected components</returns>
        public Component[] SelectGameObjectsWithComponents(bool includeInactive = false, params Type[] componentType)
        {
            if (componentType == null)
                throw new ArgumentNullException(nameof(componentType));

            Component[] components = p_onSceneQuery.FindComponentsByType(includeInactive, componentType);
            Selection.objects = components.Select(monoBehaviour => monoBehaviour.gameObject).Distinct().ToArray();

            return components;
        }
    }
}