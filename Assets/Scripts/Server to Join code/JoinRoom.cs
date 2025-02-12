using Photon.Pun;
using TMPro;
using UnityEngine;

public class JoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject LoadingScene;
    [SerializeField] private TMP_InputField Name;
    private string setName = string.Empty;
 
    public void UpadteText()
    {
        setName = Name.text;
        PhotonNetwork.LocalPlayer.NickName = setName;
    }

    public void EnterRoom()
    {
        UpadteText();

        if (!string.IsNullOrWhiteSpace(setName))
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRandomRoom();
                LoadingScene.SetActive(true);
            }
            else
            {
                Debug.LogError("Not connected to server");
            }
        }
        else
        {
            Debug.LogWarning("Player name is empty");
        }

        Debug.Log("C/J BTN Pressed");
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(3);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string randomRoomName = "Room_" + Random.Range(1000, 9999);
        PhotonNetwork.CreateRoom(randomRoomName);
    }
}
