using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitParty : MonoBehaviour
{
    [SerializeField] List<Unit> units;

    public event Action OnUpdated;
    public List<Unit> Units { get => units; set { units = value; OnUpdated?.Invoke(); } }

    
    // Start is called before the first frame update
    void Awake()
    {
        foreach(var unit in Units)
        {
            unit.init();
        }
    }
    public static UnitParty GetUnitParty()
    {
        return FindObjectOfType<PlayerActions>().GetComponent<UnitParty>();
    }
}
