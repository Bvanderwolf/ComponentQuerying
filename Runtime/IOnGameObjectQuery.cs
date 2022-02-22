using System;
using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    public interface IOnGameObjectQuery
    {
        Component[] FindComponentsOnGameObject(Component givenComponent, params Type[] componentType);
    }
}
