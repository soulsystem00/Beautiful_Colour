using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HwaiColour : MonoBehaviour, Interactable 
{
    [SerializeField] Dialog dialog;
    public ColourPair colourPair;
    public Transform spawnPoint;
    private Transform player;
    public void Interact(Transform initiator)
    {
        player = initiator;
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, teleport, true));
    }
    public void teleport()
    {
        var obj = FindObjectsOfType<HwaiColour>().First(x => x != this && x.colourPair == this.colourPair);
        player.GetComponent<PlayerActions>().Character.SetPositionAndSnapToTile(obj.spawnPoint.position);
    }
}
public enum ColourPair
{
    A, B, C, D, E, F, G, H, I, J, K
}