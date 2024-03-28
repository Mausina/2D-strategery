using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Text lobbyIdText; // UI element to display the lobby ID
    public Button generateIdButton;
    public Button copyIdButton; // Button to copy the lobby ID
    public Button backButton; // Button to go back
    public Button oneVsOneButton;
    public Button twoVsTwoButton;
    private bool isCreatingRoom = false;

    void Start()
    {
        generateIdButton.onClick.AddListener(GenerateAndCreateLobby);
        copyIdButton.onClick.AddListener(CopyLobbyIdToClipboard);
        backButton.onClick.AddListener(GoBack);
        PhotonNetwork.ConnectUsingSettings();
                oneVsOneButton.onClick.AddListener(() => UpdateMaxPlayers(2)); // 1vs1, so 2 players
        twoVsTwoButton.onClick.AddListener(() => UpdateMaxPlayers(4)); // 2vs2, so 4 players

    }
    private void UpdateMaxPlayers(byte maxPlayers)
    {
        if (PhotonNetwork.IsMasterClient) // Only the master client can update this
        {
            // Update the custom property for max players, which will automatically update for all clients
            PhotonNetwork.CurrentRoom.MaxPlayers = maxPlayers;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "maxPlayers", maxPlayers } });
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
        generateIdButton.interactable = true; // Re-enable the button in case we need to create another room
        CreateLobby();
    }

    private void CreateLobby()
    {
        string lobbyId = GenerateLobbyId();
        PhotonNetwork.CreateRoom(lobbyId, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    private string GenerateLobbyId()
    {
        return Guid.NewGuid().ToString();
    }

    public override void OnCreatedRoom()
    {
        lobbyIdText.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Room created with ID: " + PhotonNetwork.CurrentRoom.Name);
        isCreatingRoom = false; // Room is created, reset the flag
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
        Debug.Log("Back button clicked");
        SceneManager.LoadScene("Menu");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
        isCreatingRoom = false;
        generateIdButton.interactable = true; // Allow the user to try creating a room again
    }

    // Add a method to synchronize settings changes.
    public void UpdateLobbySettings(string key, object value)
    {
        // Only the master client can update the lobby settings
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable newSettings = new ExitGames.Client.Photon.Hashtable();
            newSettings[key] = value;
            PhotonNetwork.CurrentRoom.SetCustomProperties(newSettings);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // Here you would handle the changes and update the UI accordingly.
        // This method is called on all clients when the room's custom properties are updated.
        foreach (DictionaryEntry entry in propertiesThatChanged)
        {
            Debug.Log("Lobby setting updated: " + entry.Key + " = " + entry.Value);
            // Example: if (entry.Key.Equals("gameMode")) { UpdateGameModeUI(entry.Value.ToString()); }
        }
    }
}
