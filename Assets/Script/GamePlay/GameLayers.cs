using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask colour;
    [SerializeField] LayerMask colourDisable;
    [SerializeField] LayerMask portalLayer;
    public static GameLayers i { get; set; }
    public LayerMask SolidObjectsLayer { get => solidObjectsLayer; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask GrassLayer { get => grassLayer; }
    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask Colour { get => colour; }
    public LayerMask ColourDiable { get => colourDisable; }
    public LayerMask PortalLayer { get => portalLayer; }
    public LayerMask TriggerableLayers { get => grassLayer | portalLayer; }

    private void Awake()
    {
        i = this;
    }
}
