using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCollider : MonoBehaviour
{
    [SerializeField] private Transform upgradeButton;
  
    void OnTriggerStay(Collider other)
    {
        // Verifica se o objeto com o qual colidimos possui a tag desejada
        if (other.CompareTag("Player"))
        {
            upgradeButton.DOScale(1, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            upgradeButton.DOScale(0, 0.5f);
        }
    }
}
