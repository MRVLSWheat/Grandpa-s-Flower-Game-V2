using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }

    // Automatically found Player reference (read-only)
    public Transform Player { get; private set; }

    // List of all active NPCs
    private readonly List<NPCController> npcList = new List<NPCController>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Automatically find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Player = playerObj.transform;
        }
        else
        {
            Debug.LogError("[NPCManager] No GameObject tagged 'Player' found in the scene!");
        }
    }

    /// <summary>
    /// Register an NPC at startup.
    /// </summary>
    public void RegisterNPC(NPCController npc)
    {
        if (!npcList.Contains(npc))
            npcList.Add(npc);
    }

    /// <summary>
    /// Reset all NPCs to idle/roam state.
    /// </summary>
    public void ResetEngagement()
    {
        foreach (var npc in npcList)
        {
            npc.ResetEngagement();
        }
    }
}
