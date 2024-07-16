using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorpseSeller : MonoBehaviour
{
    [SerializeField] private Transform sellButton;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sellButton.DOScale(1, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sellButton.DOScale(0, 0.5f);
        }
    }
}
