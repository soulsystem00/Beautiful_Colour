using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isEnemyUnit;
    [SerializeField] UnitHudElement hud;
    public Unit unit { get; set; }
    public UnitHudElement Hud { get => hud; }

    Vector3 originalPos;
    Color originalColor;
    private void Awake()
    {

    }
    public void Setup(Unit unit, UnitHudElement unitHudElement)
    {
        this.unit = unit;
        hud = unitHudElement;
        isEnemyUnit = unit.Base.IsEnemy;
        hud.SetData(unit);
        hud.SetSprite();


        //PlayerEnterAnimation();
    }



    //public void PlayerEnterAnimation()
    //{
    //    if (IsPlayerUnit)
    //        image.transform.localPosition = new Vector3(-500f, originalPos.y);
    //    else
    //        image.transform.localPosition = new Vector3(500f, originalPos.y);

    //    image.transform.DOLocalMoveX(originalPos.x, 1f);
    //}

    //public void PlayAttackAnimation()
    //{
    //    var sequence = DOTween.Sequence();
    //    if (IsPlayerUnit)
    //        sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
    //    else
    //        sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

    //    sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    //}

    //public void PlayHitAnimation()
    //{
    //    var sequence = DOTween.Sequence();
    //    sequence.Append(image.DOColor(Color.gray, 0.1f));
    //    sequence.Append(image.DOColor(originalColor, 0.1f));
    //}

    //public void PlayFaintAnimation()
    //{
    //    var sequence = DOTween.Sequence();
    //    sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
    //    sequence.Join(image.DOFade(0f, 0.5f));
    //}
}
