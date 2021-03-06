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
    [SerializeField] GameObject expBar;
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
            SetExp();
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
    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1f, 1f);
    }
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;

        if(reset)
            expBar.transform.localScale = new Vector3(0f, 1f, 1f);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }
    float GetNormalizedExp()
    {
        int curLevelExp = Unit.Base.GetExpForLevel(Unit.Level);
        int nextLevelExp = Unit.Base.GetExpForLevel(Unit.Level + 1);

        float normalizedExp = (float)(Unit.Exp - curLevelExp) / (nextLevelExp - curLevelExp);

        return Mathf.Clamp01(normalizedExp);
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
