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
    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }
    public void Interact(Transform initiator)
    {
        player = initiator;
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, default, teleport(), true));
    }
    IEnumerator teleport()
    {
        yield return fader.FadeIn(0.5f);
        var obj = FindObjectsOfType<HwaiColour>().First(x => x != this && x.colourPair == this.colourPair);
        player.GetComponent<PlayerActions>().Character.SetPositionAndSnapToTile(obj.spawnPoint.position);
        yield return fader.FadeOut(0.5f);
    }
}
public enum ColourPair
{
    A, B, C, D, E, F, G, H, I, J, K
}