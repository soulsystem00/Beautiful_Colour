using System.Collections;
using System.Collections.Generic;
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
}
