using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;

    public Color HighlightedColor => highlightedColor;
    public static GlobalSettings i;
    private void Awake()
    {
        i = this;
    }
}
