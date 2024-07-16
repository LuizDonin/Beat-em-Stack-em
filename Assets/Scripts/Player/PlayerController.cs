using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Animator animator;

    [SerializeField] private SkinnedMeshRenderer currentClothe;
    [SerializeField] private List<Material> clothesMaterials;

    [SerializeField] private Transform pileTransform;
    [SerializeField] private Transform targetPile;
    [SerializeField] private Transform coinTransform;
    [SerializeField] private int coinAmount;
    
    public int maxPileSize;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float punchRange = 1.5f;

    private int upgradeCount = -1;

    public LayerMask Enemy;

    public float collectionHeightOffset = 0.1f;
    public List<GameObject> corpses = new List<GameObject>();

    public int corpseCount;

    private void FixedUpdate()
    {
        rigidBody.velocity = new Vector3(joystick.Horizontal * moveSpeed, rigidBody.velocity.y, joystick.Vertical * moveSpeed);
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            RotationControl();
            if (corpses.Count > 0)
            {
                CorpsePhysics();
            }
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
            if(corpses.Count > 0)
            {
                ReturnCorpse();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Punch();
        }
    }

    //Method to update the corpses physics on the back of the player
    private void CorpsePhysics()
    {
        for (int i = 0; i < corpses.Count; i++)
        {
            corpses[i].transform.DOLocalMoveZ(targetPile.transform.localPosition.z - i * 0.3f, 1f);
        }
    }

    //Method to return de corpses back to original position
    private void ReturnCorpse()
    {
        for (int i = 0; i < corpses.Count; i++)
        {
            corpses[i].transform.DOLocalMoveZ(0, 1f);
        }
    }

    //Method to call Punch Animation
    public void Punch()
    {
        animator.SetTrigger("Punching");
    }

    //Controls the player rotation, not letting it fall facing the ground
    private void RotationControl()
    {
        Vector3 direction = rigidBody.velocity;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    //Method called on UpgradeButton in UI
    public void UpgradePileSize()
    {
        if (coinAmount >= 2) 
        {
            ChangeClothesColor();
            maxPileSize += coinAmount;
            uiManager.pileAmountText.text = maxPileSize.ToString();
            coinAmount = 0;
            uiManager.coinAmountText.text = coinAmount.ToString();
            coinTransform.DOPunchScale(new Vector3(1, 1, 1), 0.5f, 1);
        }
        else
        {
            uiManager.NotEnoughCoins();
        }
    }

    //Method to change the clothes material of the player
    private void ChangeClothesColor()
    {
        upgradeCount++;
        if(upgradeCount <= 1)
        {
            currentClothe.material = clothesMaterials[upgradeCount];
        }
    }


    //Method called on an event, in the middle of the punch animation
    public void OnPunchImpact()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, punchRange, Enemy);

        foreach (Collider enemy in hitEnemies)
        {
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
                    Invoke("StackCorpses", 2f);
                }
                else
                {
                    uiManager.NotEnoughStackSlot();
                }              
            }
        }
    }

    //Method called to put the corpse on the back of the player, stacked
    private void StackCorpses()
    {
        Transform corpseTransform = corpses[corpseCount].transform;
        Animator corpseAnimator = corpses[corpseCount].GetComponent<Animator>();
        corpses[corpseCount].transform.DOScale(0, 0.1f).OnComplete(() =>
        {
            corpseAnimator.enabled = true;
            corpseAnimator.SetBool("isCorpse", true);
            corpseTransform.parent = pileTransform;
            corpseTransform.position = pileTransform.position + new Vector3(0, collectionHeightOffset + corpseCount * 0.5f, 0);
            corpseTransform.DOScale(1, 1f);
            Quaternion planeRotation = pileTransform.rotation;
            Quaternion laidDownRotation = planeRotation * Quaternion.Euler(90, 0, 90);
            corpseTransform.rotation = laidDownRotation;
        });
        
    }

    //Method called on the "sell" button on UI - Exchange corpses for coins
    public void SellCorpses()
    {
        Sequence sequence = DOTween.Sequence();

        float scaleDuration = 0.5f;
        if(corpses.Count > 0)
        {
            for (int i = corpses.Count - 1; i >= 0; i--)
            {
                GameObject corpse = corpses[i];
                sequence.Append(corpse.transform.DOScale(0, scaleDuration));
                sequence.AppendCallback(() =>
                {
                    coinTransform.DOPunchScale(new Vector3(1, 1, 1), 0.5f, 1);
                    coinAmount++;
                    uiManager.coinAmountText.text = coinAmount.ToString();
                });
            }
        }
        else
        {
            uiManager.NotEnoughCorpses();
        }
        sequence.Play();
        corpses.Clear();
        corpseCount = -1;
    }
}
