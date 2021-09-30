using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    [SerializeField] Text descriptionText;
    [SerializeField] Text spText;
    [SerializeField] GameObject spBar;
    [SerializeField] Scrollbar scroll;
    [SerializeField] List<Text> texts;

    public Text DescriptionText { get => descriptionText; }
    public Text SpText { get => spText; }
    public GameObject SpBar { get => spBar; }
    public Scrollbar Scroll { get => scroll; }
    public List<Text> Texts { get => texts; }
    public void UpdateScrollSmooth(float value)
    {
        if (value < Scroll.value)
            StartCoroutine(UpdateDownScrollSmooth(value));
        else
            StartCoroutine(UpdateUpScrollSmooth(value));
            
    }
    public IEnumerator UpdateDownScrollSmooth(float value)
    {
        float curVal = Scroll.value;
        float Diff = curVal - value;
        while (curVal - value > Mathf.Epsilon)
        {
            curVal -= Diff * Time.deltaTime * 2;
            Scroll.value = curVal;
            yield return null;
        }
        Scroll.value = value;

    }
    public IEnumerator UpdateUpScrollSmooth(float value)
    {
        float curVal = Scroll.value;
        float Diff = curVal - value;
        while (value - curVal > Mathf.Epsilon)
        {
            curVal -= Diff * Time.deltaTime * 2;
            Scroll.value = curVal;
            yield return null;
        }
        Scroll.value = value;
    }
}
