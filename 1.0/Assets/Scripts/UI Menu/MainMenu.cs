using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public Button exitButton;
    public Button settingsButton;
    public Button joinLobbyButton;
    public Button hostLobbyButton;
    public TMP_InputField lobbyIdInputField;

    private void Start()
    {
        exitButton.onClick.AddListener(ExitGame);
        settingsButton.onClick.AddListener(OpenSettings);
        joinLobbyButton.onClick.AddListener(StartJoinLobby);
        hostLobbyButton.onClick.AddListener(StartHostLobby);
    }

    private void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game");
    }

    private void OpenSettings()
    {
        Debug.Log("Open Settings");
    }

    private void StartJoinLobby()
    {
        string lobbyId = lobbyIdInputField.text;
        if (!string.IsNullOrEmpty(lobbyId))
        {
            // Check if we are connected to the master server
            if (PhotonNetwork.IsConnectedAndReady)
            {
                // Attempt to join the lobby
                PhotonNetwork.JoinRoom(lobbyId);
            }
            else
            {
                // Connect to the master server first
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.LogError("Lobby ID is empty or invalid.");
        }
    }

    public override void OnConnectedToMaster()
    {
        // Now that we're connected to the master server, we can enable the join lobby button
        joinLobbyButton.interactable = true;
    }

    // This callback is used to handle the scenario where connection to the master is lost
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        // Disable the join lobby button because we are not connected to the master server
        joinLobbyButton.interactable = false;
        Debug.LogError("Disconnected from master server: " + cause.ToString());
    }


    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("HostLobbyScene");
        Debug.Log("Joined room with ID: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Join room failed: " + message);
    }
    private void StartHostLobby()
    {
        // Load the lobby scene
        SceneManager.LoadScene("HostLobbyScene");
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
    }
}
