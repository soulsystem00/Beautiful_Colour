using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colour : MonoBehaviour
{
    public int id;
    public bool isNpc;

    public int x, y;
    public void teleport(GameObject gameObject)
    {
        gameObject.transform.position = new Vector3(x, y, 0);
    }
}

