using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this component exists to sit on the CardsManager and maintain a Dictionary of <string, GameObject> pairs
// this allows us to connect a string (from json or other load method) to a specific prefab (or prefab variant) that represents a specific card


namespace Com.WhiteSwan.OpheliaDigital
{

    [System.Serializable]
    public struct prefabMap
    {
        public string prefabNameKey;
        public GameObject prefab;
    }

    public class CardsPrefabLibrary : MonoBehaviour
    {
        [SerializeField]
        private List<prefabMap> raw_prefabMaps;

        public Dictionary<string, GameObject> prefabMapDict = new Dictionary<string, GameObject>();

        private void Awake()
        {
            foreach(var map in raw_prefabMaps)
            {
                if (!prefabMapDict.ContainsKey(map.prefabNameKey))
                {
                    prefabMapDict.Add(map.prefabNameKey, map.prefab);
                } else
                {
                    Debug.LogErrorFormat("invalid prefab string key '{0}', not loaded", map.prefabNameKey);
                }
            }
        }

    }
}