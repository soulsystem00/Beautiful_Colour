using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f, 1f);
        
    }

    public IEnumerator SetHPSmooth(double newHp)
    {
        double curHp = health.transform.localScale.x;
        double changeAmt = curHp - newHp;
        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            
            health.transform.localScale = new Vector3((float)curHp, 1f, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3((float)newHp, 1f, 1f);
        
    }

}
