using System;
using UnityEngine;

namespace BWolf.ComponentQuerying.Samples
{
    public class ComponentQueryUser : MonoBehaviour
    {
        [SerializeField]
        private ComponentQuery _query = new ComponentQuery();
        
        private void Awake()
        {
            _query.OnGameObject(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                MeshRenderer renderer = _query.Value<MeshRenderer>();
                Debug.Log($"Found renderer on game object {renderer}.");
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _query.Reset();
            }
        }
    }
}

