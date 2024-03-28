using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerListManager : MonoBehaviourPunCallbacks
{
    public GameObject playerListItemPrefab; // Assign a prefab that represents a player item in the list
    public Transform playerListContent; // Assign the content of the ScrollView where player items will be added
    public Color[] availableColors = new Color[] { Color.blue, Color.red, Color.yellow, Color.green };

    private Dictionary<Player, GameObject> playerListItems = new Dictionary<Player, GameObject>();

    public override void OnJoinedRoom()
    {
        AssignRandomNickname(); // Assign a random nickname to the local player
        UpdatePlayerList();
        AssignRandomColor();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerListItem(otherPlayer);
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!playerListItems.ContainsKey(player))
            {
                GameObject item = Instantiate(playerListItemPrefab, playerListContent);
                item.GetComponentInChildren<Text>().text = player.NickName;
                playerListItems[player] = item;
            }

            // Update the color of the player list item
            if (player.CustomProperties.TryGetValue("color", out object color))
            {
                playerListItems[player].GetComponentInChildren<Image>().color = (Color)color;
            }
        }
    }

    private void RemovePlayerListItem(Player player)
    {
        if (playerListItems.TryGetValue(player, out GameObject item))
        {
            Destroy(item);
            playerListItems.Remove(player);
        }
    }

    private void AssignRandomColor()
    {
        Color randomColor = availableColors[Random.Range(0, availableColors.Length)];
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "color", randomColor } });
    }

    private void AssignRandomNickname()
    {
        // Assuming a method to generate or retrieve a random nickname
        string randomNickname = "Player" + Random.Range(1000, 9999);
        PhotonNetwork.LocalPlayer.NickName = randomNickname;
    }
}
