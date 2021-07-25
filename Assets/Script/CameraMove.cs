using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject Char;

    Transform char_transform;

    Transform cam_transform;

    Vector3 pos;
    private void Awake()
    {
        Char = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        char_transform = Char.GetComponent<Transform>();
        cam_transform = GetComponent<Transform>();
        pos = new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        pos.x = char_transform.position.x;
        pos.y = char_transform.position.y;
        pos.z = cam_transform.position.z;
        cam_transform.position = pos;
    }
}
