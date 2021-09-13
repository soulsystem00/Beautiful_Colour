using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] Image face;
    [SerializeField] Text description;
    [SerializeField] Text name;
    [SerializeField] Text hP;
    [SerializeField] Image hpBar;

    public Image Face { get => face; set => face = value; }
    public Text Description { get => description; set => description = value; }
    public Text Name { get => name; set => name = value; }
    public Text HP { get => hP; set => hP = value; }
    public Image HpBar { get => hpBar; set => hpBar = value; }
}
