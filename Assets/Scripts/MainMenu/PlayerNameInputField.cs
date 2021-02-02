using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;

namespace Com.WhiteSwan.OpheliaDigital
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        const string playerNamePrefKey = "PlayerName";

        void Start()
        {
            string name = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    name = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = name;
                }
            }

            PhotonNetwork.NickName = name;

        }

        public void SetPlayerName(string newName)
        {
            if(string.IsNullOrEmpty(newName))
            {
                Debug.Log("new name is empty or null");
                return;
            }
            PhotonNetwork.NickName = newName;
            PlayerPrefs.SetString(playerNamePrefKey, newName);
        }
    }

}

public class PlayerName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {

    }
}
