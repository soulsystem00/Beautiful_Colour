using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Unit> wildUnits;

    public List<Unit> GetWildUnit()
    {
        foreach (var i in wildUnits)
            i.init();
        return wildUnits;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
