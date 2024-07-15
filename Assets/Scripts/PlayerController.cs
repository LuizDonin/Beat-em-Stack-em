using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NUnit.Framework.Constraints;
using TMPro;

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;
    [SerializeField] private PileController pileController;

    [SerializeField] private SkinnedMeshRenderer currentClothe;
    [SerializeField] private List<Material> clothesMaterials;

    [SerializeField] private Transform planeTransform;
    [SerializeField] private Transform targetPile;
    [SerializeField] private Transform coinTransform;
    [SerializeField] private TextMeshProUGUI coinAmountText;
    [SerializeField] private TextMeshProUGUI pileSizeText;
    [SerializeField] private CanvasGroup alertCanvasGroup;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private int coinAmount;
    public int maxPileSize;

    private const string text = "sdhjsdkj";

    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform sellCorpseTransform;

    [SerializeField] private float _punchRange = 1.5f;

    public LayerMask Enemy;

    public float collectionHeightOffset = 0.1f; // Offset for each collected object
    public List<Transform> collectedObjects = new List<Transform>();
    public List<GameObject> corpses = new List<GameObject>();

    public bool fly;

    public int corpseCount;

    private void FixedUpdate()
    {
        _rigidBody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, _rigidBody.velocity.y, _joystick.Vertical * _moveSpeed);
        if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
        {
            if(corpses.Count > 0)
            {
                CorpsePhysics();
            }
            Vector3 direction = _rigidBody.velocity;
            direction.y = 0; // Mantenha a rotação no plano XZ
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false);
            if(corpses.Count > 0)
            {
                ReturnCorpse();
            }

        }



    }

    private IEnumerator ShowAlertCoroutine(string text, int duration)
    {
        alertText.text = text;
        alertCanvasGroup.DOFade(1, 0.5f);
        yield return new WaitForSeconds(duration);
        alertCanvasGroup.DOFade(0, 0.5f);
    }

    public void UpgradePileSize()
    {
        if(coinAmount >= 2) 
        {
            maxPileSize += coinAmount;
            pileSizeText.text = maxPileSize.ToString();
            coinAmount = 0;
            coinAmountText.text = coinAmount.ToString();
            coinTransform.DOPunchScale(new Vector3(1, 1, 1), 0.5f, 1);
        }
        else
        {
            StartCoroutine(ShowAlertCoroutine("Moedas insuficientes", 3));
        }

    }

    private void CorpsePhysics()
    {
        for(int i = 0; i < corpses.Count; i++)
        {
            corpses[i].transform.DOLocalMoveZ(targetPile.transform.localPosition.z - i * 0.5f, 1f);
        }
    }

    private void ReturnCorpse()
    {
        for (int i = 0; i < corpses.Count; i++)
        {
            corpses[i].transform.DOLocalMoveZ(0, 1f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Punch();
        }
    }

    public void Punch()
    {
        _animator.SetTrigger("Punching");
        this.transform.DOScale(1.1f, 0.4f).OnComplete(() => { this.transform.DOScale(1, 0.2f); });
    }

    public void OnPunchImpact()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _punchRange, Enemy);

        foreach (Collider enemy in hitEnemies)
        {
            BoxCollider[] hipsCollider = enemy.GetComponentsInChildren<BoxCollider>();
            CapsuleCollider capsuleCollider = enemy.GetComponent<CapsuleCollider>();
            Animator animator = enemy.GetComponent<Animator>();
            if (animator.enabled)
            {
                animator.SetBool("isCorpse", true);
                animator.enabled = false;
                Destroy(capsuleCollider);
                if(corpses.Count <= maxPileSize-1)
                {
                    corpseCount++;
                    corpses.Add(enemy.gameObject);
                    Invoke("DeactivateAnimation", 2f);
                }
                else
                {
                    StartCoroutine(ShowAlertCoroutine("Você não pode empilhar mais corpos!", 3));
                }
                
                

            }
        }
    }

    private void DeactivateAnimation()
    {
        corpses[corpseCount].transform.DOScale(0, 0.1f).OnComplete(() =>
        {
            corpses[corpseCount].GetComponent<Animator>().enabled = true;
            corpses[corpseCount].GetComponent<Animator>().SetBool("isCorpse", true);

            corpses[corpseCount].transform.parent = planeTransform;
            corpses[corpseCount].transform.position = planeTransform.position + new Vector3(0, collectionHeightOffset + corpseCount * 0.5f, 0);           
            corpses[corpseCount].transform.DOScale(1, 1f);
            Quaternion planeRotation = planeTransform.rotation;

            Quaternion laidDownRotation = planeRotation * Quaternion.Euler(90, 0, 90);

            corpses[corpseCount].transform.rotation = laidDownRotation;
        });
        
    }

    public void SellCorpses()
    {
        Sequence sequence = DOTween.Sequence();

        float delay = 0.2f;
        float scaleDuration = 0.5f;
        if(corpses.Count > 0)
        {
            for (int i = corpses.Count - 1; i >= 0; i--)
            {
                GameObject corpse = corpses[i];
                sequence.Append(corpse.transform.DOScale(0, scaleDuration).SetDelay(delay * (corpses.Count - 1 - i)));
                sequence.AppendCallback(() =>
                {
                    coinTransform.DOPunchScale(new Vector3(1, 1, 1), 0.5f, 1);
                    coinAmount++;
                    coinAmountText.text = coinAmount.ToString();
                });
            }
        }
        else
        {
            StartCoroutine(ShowAlertCoroutine("Você não possui corpos para vender!", 3));
        }


        sequence.Play();
        corpses.Clear();
        corpseCount = -1;
    }
}
