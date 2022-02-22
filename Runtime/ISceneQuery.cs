using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    /// <summary>
    /// To be implemented by objects that can query for component values in a scene or prefab stage context.
    /// </summary>
    public interface ISceneQuery
    {
        /// <summary>
        /// Returns component value(s) found by the query.
        /// </summary>
        /// <returns>The found component(s).</returns>
        Component[] Values();

        /// <summary>
        /// Returns component value(s) of a given type T found by the query.
        /// </summary>
        /// <typeparam name="T">The type of component(s) to select.</typeparam>
        /// <returns>The found component(s).</returns>
        T[] Values<T>() where T : Component;
    }
}

