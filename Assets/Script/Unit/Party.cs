using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    [SerializeField] List<Unit> units;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var unit in units)
        {
            unit.init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
