using System;
using UnityEngine;

namespace BWolf.ComponentQuerying
{
    /// <summary>
    /// To be implemented by an object that can query for components in parents or children.
    /// </summary>
    public interface IFromComponentQuery
    {
        /// <summary>
        /// Finds components of given types on child game objects given a parent component.
        /// </summary>
        /// <param name="parentComponent">The parent component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to search for.</param>
        /// <returns>The found components.</returns>
        Component[] FindComponentsOnChildren(Component parentComponent, bool includeInactive, params Type[] componentType);

        /// <summary>
        /// Finds components of given types on parent game objects given a child component.
        /// </summary>
        /// <param name="childComponent">The child component to search from.</param>
        /// <param name="includeInactive">Whether to include inactive game objects in the search.</param>
        /// <param name="componentType">The type of component(s) to search for.</param>
        /// <returns>The found components.</returns>
        Component[] FindComponentsOnParent(Component childComponent, bool includeInactive, params Type[] componentType);
    }
}