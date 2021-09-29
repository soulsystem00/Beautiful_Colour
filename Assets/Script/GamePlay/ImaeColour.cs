using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ImaeColour : MonoBehaviour, Interactable 
{
    [SerializeField] Dialog dialog;
    public void Interact(Transform initiator)
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, DisableObj, true));
    }
    public void DisableObj()
    {
        Debug.Log("imea start");
        StartCoroutine(Fade(GetComponent<Tilemap>()));
    }
    IEnumerator Fade(Tilemap tilemap)
    {
        if(gameObject.layer == 10)
        {
            gameObject.layer = 11;
            float curAlpha = 1f;
            float changeAmt = 0.5f;
            while (curAlpha - 0.5f > Mathf.Epsilon)
            {
                curAlpha -= changeAmt * Time.deltaTime;

                tilemap.color = new Color(1f, 1f, 1f, curAlpha);
                yield return null;
            }
            tilemap.color = new Color(1f, 1f, 1f, 0.5f);
            
        }
        else
        {
            gameObject.layer = 10;
            float curAlpha = 0.5f;
            float changeAmt = 1f;
            while (1f - curAlpha > Mathf.Epsilon)
            {
                curAlpha += changeAmt * Time.deltaTime;

                tilemap.color = new Color(1f, 1f, 1f, curAlpha);
                yield return null;
            }
            tilemap.color = new Color(1f, 1f, 1f, 1f);
            
        }

    }
}