
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;

public class LobbyManager : MonoBehaviour
{
    /*
    public Text lobbyIdText; // UI element to display the lobby ID
    public Button generateIdButton;
    public Button copyIdButton; // Button to copy the lobby ID
    public Button backButton; // Button to go back
    public Button startButton;

    private bool isCreatingRoom = false;

    void Start()
    {
        generateIdButton.onClick.AddListener(GenerateAndCreateLobby);
        copyIdButton.onClick.AddListener(CopyLobbyIdToClipboard);
        backButton.onClick.AddListener(GoBack);
        startButton.onClick.AddListener(StartGame); // Add this listener for the startButton
        PhotonNetwork.ConnectUsingSettings();

        // Display stored lobby ID if it exists
        if (PlayerPrefs.HasKey("LobbyID"))
        {
            lobbyIdText.text = PlayerPrefs.GetString("LobbyID");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        generateIdButton.interactable = true;
    }

    private void GenerateAndCreateLobby()
    {
        if (isCreatingRoom)
        {
            Debug.Log("Already attempting to create a room.");
            return;
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            isCreatingRoom = true;
            generateIdButton.interactable = false; // Disable the button to prevent multiple clicks
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                CreateLobby();
            }
        }
        else
        {
            Debug.LogError("Cannot create room, not connected to master server or not ready.");
        }
    }

    public override void OnLeftRoom()
    {
        isCreatingRoom = false;
        generateIdButton.interactable = true; // Re-enable the button
        CreateLobby();
    }

    private void CreateLobby()
    {
        string lobbyId = GenerateLobbyId();
        PhotonNetwork.CreateRoom(lobbyId, new RoomOptions { MaxPlayers = 4 });
    }

    private string GenerateLobbyId()
    {
        return Guid.NewGuid().ToString();
    }

    public override void OnCreatedRoom()
    {
        lobbyIdText.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Room created with ID: " + PhotonNetwork.CurrentRoom.Name);
        isCreatingRoom = false; // Reset the flag
        PlayerPrefs.SetString("LobbyID", PhotonNetwork.CurrentRoom.Name); // Store the lobby ID
    }

    private void CopyLobbyIdToClipboard()
    {
        TextEditor textEditor = new TextEditor
        {
            text = lobbyIdText.text
        };
        textEditor.SelectAll();
        textEditor.Copy();
    }

    private void GoBack()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("Back button clicked");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
        isCreatingRoom = false;
        generateIdButton.interactable = true; // Allow retry
    }

    // Start the game and load the map scene for all players in the lobby
    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient) // Only the Master Client can start the game
        {
            PhotonNetwork.LoadLevel("SampleScene"); // Replace with your map scene name
        }
    }
    */
}
