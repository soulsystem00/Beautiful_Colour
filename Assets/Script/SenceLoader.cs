using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SenceLoader : MonoBehaviour
{
    public SceneAsset scene;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (string.Compare(collision.name, "Ludo 0") == 0)
            SceneManager.LoadScene(scene.name);
    }
}
