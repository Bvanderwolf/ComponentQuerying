using System;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    /// <summary>
    /// To be implemented by an object that can query for components on a game object.
    /// </summary>
    public interface IOnGameObjectQuery
    {
        /// <summary>
        /// Finds components of given types on a game object given a sibling component.
        /// </summary>
        /// <param name="gameObject">The game object to search from.</param>
        /// <param name="componentType">The type of component(s) to search for.</param>
        /// <returns>The found components.</returns>
        Component[] FindComponentsOnGameObject(GameObject gameObject, params Type[] componentType);
    }
}
