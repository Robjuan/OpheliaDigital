using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [Tooltip("UI element to indicate player status")]
    public GameObject playerUIPrefab;
    private GameObject playerDisplay;

    void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);

        if (playerUIPrefab != null)
        {
        }


    }

    private void Start()
    {
        // spawn UI to show that we're connected

        // if local player, spawn on bottom

        // otherwise, up top
    }

    void Update()
    {
        
    }
}
