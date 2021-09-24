using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UnitHudElement : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] HPBar hPBar;
    [SerializeField] HPBar spBar;
    [SerializeField] Image sprite;
    [SerializeField] bool isEnemy;
    Unit _unit;

    public Image Sprite { get => sprite; }
    public Unit Unit { get => _unit; }

    Vector3 originalPos;
    Color originalColor;

    public void SetData(Unit unit)
    {
        _unit = unit;
        isEnemy = unit.Base.IsEnemy;
        if(isEnemy)
        {
            hPBar.SetHP((float)Unit.HP / Unit.MaxHp);
        }
        else
        {
            nameText.text = Unit.Base.Name;
            hPBar.SetHP((float)Unit.HP / Unit.MaxHp);
            spBar.SetHP((float)Unit.energy / Unit.MaxEnergy);
        }

        SetSprite();
    }

    public void SetSprite()
    {
        if (isEnemy)
        {
            Sprite.sprite = Unit.Base.FrontSprite;
        }
        else
        {
            Sprite.sprite = Unit.Base.PortraitSprite;
        }
        originalPos = Sprite.transform.localPosition;
        originalColor = Sprite.color;

        PlayerEnterAnimation();
    }

    public IEnumerator UpdateHP()
    {
        if(isEnemy)
            yield return hPBar.SetHPSmooth((double)Unit.HP / Unit.MaxHp);
        else
        {
            yield return hPBar.SetHPSmooth((double)Unit.HP / Unit.MaxHp);
            yield return spBar.SetHPSmooth((double)Unit.energy / Unit.MaxEnergy);
        }
    }

    public void PlayerEnterAnimation()
    {
        if (isEnemy)
        {
            Sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
            Sprite.DOFade(1f, 1f);
        }
    }

    public void PlayHitAnimaion()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(Sprite.DOColor(Color.gray, 0.1f));
        sequence.Append(Sprite.DOColor(originalColor, 0.1f));
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(sprite.transform.DOLocalMoveY(originalPos.y - 50f, 0.25f));
        sequence.Append(sprite.transform.DOLocalMoveY(originalPos.y, 0.25f));
    }
    public void PlayFaintedAnimation()
    {
        var seqeunce = DOTween.Sequence();
        seqeunce.Append(gameObject.transform.DOLocalMoveY(gameObject.transform.localPosition.y - 70f, 0.25f));
        seqeunce.Join(sprite.DOFade(0f,0.25f));
        //this.gameObject.SetActive(false);
        StartCoroutine(DIsableObject());
    }
    IEnumerator DIsableObject()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
