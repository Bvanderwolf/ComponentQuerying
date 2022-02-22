using System;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    /// <summary>
    /// To be implemented by objects that can query for component values in a scene or prefab stage context and return the result.
    /// </summary>
    public interface IOnSceneQuery
    {
        Component[] FindComponentsByTag(string tagName, params Type[] componentType);

        Component[] FindComponentsByName(string objectName, params Type[] componentType);

        Component[] FindComponentsByType(bool includeInactive, params Type[] componentType);
    }
}

