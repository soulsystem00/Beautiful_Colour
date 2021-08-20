using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Colour : MonoBehaviour
{
    public int id;
    public bool isNpc;

    public int x, y;

    public TilemapCollider2D tc;
    public Tilemap tile;
    void Start()
    {
        //tc.GetComponent<TilemapCollider2D>();
        //tile.GetComponent<Tilemap>();
    }
    public void teleport(GameObject gameObject)
    {
        gameObject.transform.position = new Vector3(x, y, 0);
    }

    public void function()
    {
        tc.enabled = false;
        tile.color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 128 / 255f);
    }
}

