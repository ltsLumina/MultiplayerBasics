using System;
using System.Collections;
using System.Collections.Generic;
using kcp2k;
using UnityEngine;
using Mirror;
using TMPro;
using System.Linq;
using static UnityEngine.Debug;
using Random = UnityEngine.Random;
using static UsefulShortcuts;

public class MyNetworkPlayer : NetworkBehaviour
{
    #region Serialized Fields
    [SerializeField, SyncVar(hook = nameof(HandleDisplayNameUpdated))] string playerName = "Missing Name";
    [SerializeField, SyncVar(hook = nameof(HandleDisplayColourUpdated))] Color playerColor = Color.white;
    [SerializeField] TMP_Text displayNameText;
    [SerializeField] Renderer displayColourRenderer;

    [Space(25)] [Header("Type Filter")]
    [SerializeField] List<string> blacklistedWords = new List<string>();
    [SerializeField] TextAsset blacklistedWordsFile;
    bool blacklistedWord;
    [SerializeField] bool isBadWord;
    #endregion

    #region Client/Server Connection
    #region Start/Stop Server
    override public void OnStartServer()
    {
        base.OnStartServer();
        playerName = $"Player {netId}";

        blacklistedWords = new List<string>();
        string[] lines = blacklistedWordsFile.text.Split('\n');
        foreach (string line in lines) { blacklistedWords.Add(line); }

        Log($"Blacklisted words loaded: {blacklistedWords.Count}");
    }

    override public void OnStopServer()
    {
        base.OnStopServer();
        Log($"Player {playerName} has left the game");

        blacklistedWords.Clear();
    }
    #endregion

    #region Start/Stop Client
    override public void OnStartClient()
    {
        base.OnStartClient();
        Log($"Player {playerName} has joined the game");
    }

    override public void OnStopClient()
    {
        base.OnStopClient();
        Log($"Player {playerName} has left the game");
    }
    #endregion
    #endregion

    #region Server Configuration

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        playerName = newDisplayName;
    }

    [Server]
    public void SetDisplayColour()
    {
        playerColor = Random.ColorHSV();
        displayColourRenderer.material.color = playerColor;
    }
    #endregion

    #region Client Configuration
    [ContextMenu("Set Username")]
    void SetMyName() //TODO: Remove later. For debugging only.
    {
        CmdSetDisplayName("hiddenboob");
    }

    [Command]
    void CmdSetDisplayName(string newDisplayName)
    {
        //if (newDisplayName.Length < 2 || newDisplayName.Length > 20;

        foreach (string blacklisted in blacklistedWords)
        {
            //TODO: try this in a new NON-MULTIPLAYER scenario.
            //isBadWord = newDisplayName.Contains(blacklisted, StringComparison.InvariantCultureIgnoreCase);
            isBadWord = CheckIfWordContains(newDisplayName, blacklisted);

            if (isBadWord) break;
        }

        // string blacklistedWord = "bad";
        // isBadWord = CheckIfWordContains(newDisplayName, blacklistedWord);

        Log($"{isBadWord}");
        LogWarning(isBadWord ? "The name contains a bad word." : "The name is OK.");

        //ClearConsole(); // Custom method that clears the console.

        SetDisplayName(newDisplayName);
        RpcLogNewName(newDisplayName);
    }

    //TODO: Check for special characters and convert to NORMAL charcters
    bool CheckIfWordContains(string word, string wordToCheck)
    {
        char[] wordChars        = word.ToCharArray();
        char[] wordToCheckChars = wordToCheck.ToCharArray();

        for (int i = 0; i <= wordChars.Length - wordToCheckChars.Length; i++)
        {
            if (wordChars[i] == wordToCheckChars[0])
            {
                bool foundWord = true;

                for (int j = 0; j < wordToCheckChars.Length; j++)
                {
                    if (wordChars[i + j] != wordToCheckChars[j])
                    {
                        foundWord = false;
                        break;
                    }
                }

                if (foundWord) { return true; }
            }
        }

        return false;
    }

    [ClientRpc]
    void RpcLogNewName(string newDisplayName)
    {
        Log($"Player {newDisplayName} has joined the game");
    }

    void HandleDisplayNameUpdated(string oldName, string newName)
    {
        displayNameText.text = newName;
    }

    void HandleDisplayColourUpdated(Color oldColour, Color newColour)
    {
        displayColourRenderer.material.color = newColour;
    }
    #endregion
}