using UnityEngine;

namespace BWolf.MonoBehaviourQuerying
{
    public interface ISceneQuery
    {
        Component[] Values();

        T[] Values<T>() where T : Component;
    }
}

