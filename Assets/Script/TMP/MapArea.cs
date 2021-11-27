using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Unit> wildUnits;

    List<Unit> wildUnitsCopy;
    public List<Unit> GetWildUnit()
    {
        wildUnitsCopy = new List<Unit>();
        foreach (var i in wildUnits)
        {
            var wildUnitCopy = new Unit(i.Base, i.Level);
            wildUnitCopy.init();
            wildUnitsCopy.Add(wildUnitCopy);
        }
        return wildUnitsCopy;
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
