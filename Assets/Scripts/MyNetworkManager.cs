using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    override public void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("Client connected to server");
    }

    override public void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"Player Connected. \n Player count: {numPlayers}");

        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();

        player.SetDisplayName($"Player {numPlayers}");
        player.SetDisplayColour();
        
        // Move the first player 5 units to the right
        if (numPlayers      == 1) player.transform.position = new Vector3(Random.Range(-5,5), 0, 0);
        else if (numPlayers != 1) player.transform.position = new Vector3(Random.Range(-5,5), 0, 0);
    }
}