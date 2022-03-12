using UnityEngine;

namespace BWolf.ComponentQuerying
{
    /// <summary>
    /// To be implemented by objects that can query for component values and return the result.
    /// </summary>
    public interface IComponentQuery
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
