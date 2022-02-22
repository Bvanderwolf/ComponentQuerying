using System;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    public interface IFromComponentQuery
    {
        Component[] FindComponentsOnChildren(Component parentComponent, bool includeInactive, params Type[] componentType);

        Component[] FindComponentsOnParent(Component childComponent, bool includeInactive, params Type[] componentType);
    }
}