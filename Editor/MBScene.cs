using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace BWolf.MonoBehaviourQuerying.Editor
{
    /// <summary>
    /// Provides a static API to retrieve mono behaviour instances in the scene(s) or prefab stage.
    /// </summary>
    public static class MBScene
    {
        private static readonly MBQuery _query = new MBQuery(true);

        /// <summary>
        /// Returns a list of selected mono behaviours.
        /// </summary>
        public static IList<MonoBehaviour> FromSelected()
        {
            if (Selection.gameObjects.Length == 0)
                return new List<MonoBehaviour>();

            return FromSelected(false, true, typeof(MonoBehaviour));
        }

        /// <summary>
        /// Returns a list of selected mono behaviours of given type(s).
        /// </summary>
        public static IList<MonoBehaviour> FromSelected(params Type[] monoBehaviourType) => FromSelected(false, true, monoBehaviourType);

        /// <summary>
        /// Returns a list of selected mono behaviours of given type(s).
        /// </summary>
        public static IList<MonoBehaviour> FromSelected(bool resetSelection, params Type[] monoBehaviourType)
            => FromSelected(false, resetSelection, monoBehaviourType);

        /// <summary>
        /// Returns a list of selected mono behaviours of given type(s), resetting the selection if needed.
        /// </summary>
        public static IList<MonoBehaviour> FromSelected(bool includeInActive, bool resetSelection, params Type[] monoBehaviourType)
        {
            if (monoBehaviourType == null)
                throw new ArgumentNullException(nameof(monoBehaviourType));

            if (Selection.gameObjects.Length == 0)
                return new List<MonoBehaviour>();

            var selected = Selection.gameObjects
                .SelectMany(gameObject => gameObject.GetComponentsInChildren<MonoBehaviour>(includeInActive))
                .Where(monoBehaviour => monoBehaviourType.Any(type => type == monoBehaviour.GetType()))
                .ToList();

            if (resetSelection)
            {
                Selection.objects = Array.Empty<Object>();
            }

            return selected;
        }

        /// <summary>
        /// Returns a list of selected mono behaviours of type T.
        /// </summary>
        public static IList<T> FromSelected<T>(bool includeInActive = false) where T : MonoBehaviour
        {
            if (Selection.gameObjects.Length == 0)
                return new List<T>();

            return Selection.gameObjects.SelectMany(o => o.GetComponentsInChildren<T>(includeInActive)).ToList();
        }

        /// <summary>
        /// Selects mono behaviours of given type(s).
        /// </summary>
        public static void Select(params Type[] monoBehaviourType)
        {
            if (monoBehaviourType == null)
                throw new ArgumentNullException(nameof(monoBehaviourType));

            Selection.objects = _query
                .ByType(monoBehaviourType)
                .Values()
                .Select(monoBehaviour => monoBehaviour.gameObject)
                .ToArray();
        }

        /// <summary>
        /// Selects mono behaviours of given type(s).
        /// </summary>
        public static void Select(bool includeInActive = false, params Type[] monoBehaviourType)
        {
            if (monoBehaviourType == null)
                throw new ArgumentNullException(nameof(monoBehaviourType));

            Selection.objects = _query
                .ByType(includeInActive, monoBehaviourType)
                .Values()
                .Select(monoBehaviour => monoBehaviour.gameObject)
                .ToArray();
        }

        /// <summary>
        /// Selects mono behaviours corresponding with given mono script.
        /// </summary>
        public static void Select(MonoScript monoScript, bool includeInactive = false)
        {
            if (monoScript == null)
                throw new ArgumentNullException(nameof(monoScript));

            Select(includeInactive, monoScript.GetClass());
        }

        /// <summary>
        /// Returns the path of the mono behaviour towards its greatest grandparent in the scene
        /// in format: grandparent/parent/child.
        /// </summary>
        public static string GetScenePath(this MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null)
                throw new ArgumentNullException(nameof(monoBehaviour));

            StringBuilder path = new StringBuilder(monoBehaviour.gameObject.name);
            Transform parent = monoBehaviour.transform.parent;

            while (parent != null)
            {
                path.Insert(0, '/');
                path.Insert(0, parent.gameObject.name);

                parent = parent.parent;
            }

            return path.ToString();
        }
    }

}