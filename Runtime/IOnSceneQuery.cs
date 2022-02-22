using System;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    /// <summary>
    /// To be implemented by objects that can query for component values in a scene or
    /// prefab stage context and return the result.
    /// </summary>
    public interface IOnSceneQuery
    {
        /// <summary>
        /// Finds components of given types in the scene based on a game object tag.
        /// </summary>
        /// <param name="tagName">The game object tag to use for the search.</param>
        /// <param name="componentType">The component type(s) to search for.</param>
        /// <returns>The found components.</returns>
        Component[] FindComponentsByTag(string tagName, params Type[] componentType);

        /// <summary>
        /// Finds components of given types in the scene based on a game object name.
        /// </summary>
        /// <param name="objectName">The object name to use for the search.</param>
        /// <param name="componentType">The component type(s) to search for.</param>
        /// <returns>The found components.</returns>
        Component[] FindComponentsByName(string objectName, params Type[] componentType);

        /// <summary>
        /// Finds components of given types in the scene.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to search for.</param>
        /// <returns>The found components.</returns>
        Component[] FindComponentsByType(bool includeInactive, params Type[] componentType);
    }
}

