using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI alertText;
    public TextMeshProUGUI coinAmountText;
    public TextMeshProUGUI pileAmountText;
    public CanvasGroup alertCanvasGroup;

    private const string alertText01 = "Moedas insuficientes";
    private const string alertText02 = "Você não pode empilhar mais corpos!";
    private const string alertText03 = "Você não possui corpos para vender!";

    private int alertDuration = 3;

    private IEnumerator ShowAlertCoroutine(string text, int duration)
    {
        alertText.text = text;
        alertCanvasGroup.DOFade(1, 0.5f);
        yield return new WaitForSeconds(duration);
        alertCanvasGroup.DOFade(0, 0.5f);
    }

    public void NotEnoughCoins()
    {
        StartCoroutine(ShowAlertCoroutine(alertText01, alertDuration));
    }

    public void NotEnoughStackSlot()
    {
        StartCoroutine(ShowAlertCoroutine(alertText02, alertDuration));
    }

    public void NotEnoughCorpses()
    {
        StartCoroutine(ShowAlertCoroutine(alertText03, alertDuration));
    }
}

