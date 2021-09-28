using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    void Interactable.Interact(Transform initiator)
    {
        Debug.Log("NPC Interact");

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
