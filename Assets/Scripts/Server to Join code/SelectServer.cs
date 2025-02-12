using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectServer : MonoBehaviourPunCallbacks
{
    public TMP_Dropdown serverDropdown; 
    public Button joinButton;           
    public string selectedRegion = "";

    private string GetRegionFromIndex(int index)
    {
        switch (index) //All the available servers we gonna use. if we want to add more servers it will take atleast 10 min of wait time
        {
            case 1: return "asia";
            case 2: return "au";
            case 3: return "eu";
            case 4: return "hk";
            case 5: return "in";
            case 6: return "jp";
            case 7: return "us";
            default: return "";
        }//Warning: Names liek "eu" are manditory names dont change
    }

    public void OnServerSelected(int index)//use index from the list to pick out the string
    {
        selectedRegion = GetRegionFromIndex(index);//Index has string value eg: GetRegionFromIndex("eu");
    }
    public void JoinToRegionBtn()
    {
        if (string.IsNullOrEmpty(selectedRegion)) //now selectedRegion has the value of Index ie('eu'etc)
        {
            // If "Best Server" ie. string is empty, connect to the best region automatically
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.ConnectToRegion(selectedRegion);
        }

        Debug.Log("Server BTN Pressed");
    }


    //After connecting to server region
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to: " + PhotonNetwork.CloudRegion);
        PhotonNetwork.LoadLevel(1); //Switch so Main Menu after the server connects to a region
    }//WARNING: Dont use Scenemanager to load level IDk if it will break anything or not.


    //Just Debug Ignore
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Failed Connection. Reason: " + cause);
    }
}
