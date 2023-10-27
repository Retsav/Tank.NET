using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColourDisplay : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TankPlayer player;
    [SerializeField] private SpriteRenderer[] playerSpritesToChange;
    [SerializeField] private TeamColourLookup teamColourLookupSO;

    private void Start()
    {
        HandlePlayerColorChange(-1, player.TeamIndex.Value);
        player.TeamIndex.OnValueChanged += HandlePlayerColorChange;
    }

    private void HandlePlayerColorChange(int previousvalue, int newvalue)
    {
        Color teamColor = teamColourLookupSO.GetTeamColour(newvalue);
        foreach (SpriteRenderer sprite in playerSpritesToChange)
        {
            sprite.color = teamColor;
        }
    }

    private void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= HandlePlayerColorChange;
    }
}
