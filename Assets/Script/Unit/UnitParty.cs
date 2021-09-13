using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitParty : MonoBehaviour
{
    [SerializeField] List<Unit> units;

    public List<Unit> Units { get => units; }


    // Start is called before the first frame update
    void Start()
    {
        foreach(var unit in Units)
        {
            unit.init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
