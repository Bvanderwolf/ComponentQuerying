using System;
using System.Linq;
using UnityEngine;

using Object = UnityEngine.Object;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif

namespace BWolf.MonoBehaviourQuerying
{
    public partial class MBQuery
    {
        /*
         - GetComponent
         - GetComponents
         - Find(name)
         - Find(tag)
         - Refresh()
         - Values()
         - auto-refresh flag in constructor
         */

        private readonly List<IQuery> _queries = new List<IQuery>();

        private readonly List<MonoBehaviour> _values = new List<MonoBehaviour>();

        private bool _isRefreshed;

        public bool autoRefresh;

        public MBQuery(bool autoRefresh = false) => this.autoRefresh = autoRefresh;

        public MonoBehaviour[] Values()
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

        public T[] Values<T>() where T : MonoBehaviour
        {
            List<T> genericValues = new List<T>();

            // If values require auto refreshing or the query hasn't refreshed yet.
            if (autoRefresh || !_isRefreshed)
            {
                // Refresh the values.
                _values.Clear();

                for (int i = 0; i < _queries.Count; i++)
                    _values.AddRange(_queries[i].Values<T>());

                _isRefreshed = true;
            }

            // Only the generic values are returned.
            for (int i = 0; i < _values.Count; i++)
                if (_values[i] is T value)
                    genericValues.Add(value);

            return genericValues.ToArray();
        }

        public MBQuery Dirty()
        {
            _isRefreshed = false;
            return this;
        }

        public MBQuery ByName(string objectName) => ByName(objectName, typeof(MonoBehaviour));

        public MBQuery ByName(string objectName, params Type[] monoBehaviourType)
        {
            _queries.Add(new OnGameObjectQuery(FindMonoBehavioursOnGameObject, objectName, monoBehaviourType));
            return this;
        }

        public MBQuery ByName<T>(string objectName) where T : MonoBehaviour
        {
            _queries.Add(new OnGameObjectQuery<T>(FindMonoBehavioursOnGameObject<T>, objectName));
            return this;
        }

        public MBQuery ByType() => ByType(false, typeof(MonoBehaviour));

        public MBQuery ByType(params Type[] monoBehaviourType) => ByType(false, monoBehaviourType);

        public MBQuery ByType(bool includeInactive, params Type[] monoBehaviourType)
        {
            _queries.Add(new OfTypeQuery(FindMonoBehavioursOfType, includeInactive, monoBehaviourType));
            return this;
        }

        public MBQuery ByType<T>(bool includeInactive = false) where T : MonoBehaviour
        {
            _queries.Add(new OfTypeQuery<T>(FindMonoBehavioursOfType<T>, includeInactive));
            return this;
        }

        private MonoBehaviour[] FindMonoBehavioursOnGameObject(string objectName, params Type[] monoBehaviourType)
        {
            if (objectName == null)
                throw new ArgumentNullException(nameof(objectName));

            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                Debug.LogWarning($"Failed finding a game object in the scene(s) with name {objectName} during query.");
                return Array.Empty<MonoBehaviour>();
            }

            return gameObject.GetComponents<MonoBehaviour>()
                .Where(monoBehaviour => monoBehaviourType.Any(type => type.IsInstanceOfType(monoBehaviour)))
                .ToArray();
        }

        private T[] FindMonoBehavioursOnGameObject<T>(string objectName) where T : MonoBehaviour
        {
            if (objectName == null)
                throw new ArgumentNullException(nameof(objectName));

            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                Debug.LogWarning($"Failed finding a game object in the scene(s) with name {objectName} during query.");
                return Array.Empty<T>();
            }

            return gameObject.GetComponents<T>();
        }

        /// <summary>
        /// Returns a list of mono behaviours of given type(s) found in the active scene or prefab stage.
        /// </summary>
        private MonoBehaviour[] FindMonoBehavioursOfType(bool includeInactive, params Type[] monoBehaviourType)
        {
            if (monoBehaviourType == null)
                throw new ArgumentNullException(nameof(monoBehaviourType));

            if (monoBehaviourType.Length == 0)
                throw new ArgumentException("No mono behaviour types were given.");

#if UNITY_EDITOR
            // Look for mono behaviours in the current prefab stage if we are in the editor and it exists.
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                // *TESTEN WAT ALS ER GEEN GEVONDEN WORDT
                return stage.prefabContentsRoot.GetComponentsInChildren<MonoBehaviour>(includeInactive)
                    .Where(monoBehaviour => monoBehaviourType.Any(type => type.IsInstanceOfType(monoBehaviour)))
                    .ToArray();
            }
#endif

            // We are in a scene and should look for objects there.
            return (includeInactive ? Resources.FindObjectsOfTypeAll<MonoBehaviour>() : Object.FindObjectsOfType<MonoBehaviour>())
                .Where(monoBehaviour => monoBehaviourType.Any(type => type.IsInstanceOfType(monoBehaviour)))
                .ToArray();
        }

        /// <summary>
        /// Returns a list of mono behaviours of a given type T found in the active scene or prefab stage.
        /// </summary>
        private T[] FindMonoBehavioursOfType<T>(bool includeInactive = false) where T : MonoBehaviour
        {
#if UNITY_EDITOR
            // Look for mono behaviours in the current prefab stage if we are in the editor and it exists.
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                // *TESTEN WAT ALS ER GEEN GEVONDEN WORDT
                return stage.prefabContentsRoot.GetComponentsInChildren<T>(includeInactive);
            }
#endif

            // We are in a scene 
            // *Resources.FindObjectsOfAll is om ook inactive objects in de scene te vinden. Check of
            // hij ook alle field references meepakt, want dan moeten duplicates worden verwijderd.
            return (includeInactive ? Resources.FindObjectsOfTypeAll<T>() : Object.FindObjectsOfType<T>());
        }
    }
}


