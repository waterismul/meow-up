using System;
using DG.Tweening;
using UnityEngine;


public class Cat : MonoBehaviour
{
    [SerializeField] private float jumpSpeed = 0.5f;
    
    private Animator animator;
    private Tween swappingTween;
    private bool isJumping;
    private Rigidbody2D rb;
    private GameManager gm;
    private ObjectPoolManager pool;
    private SpriteRenderer sr;
    private AudioManager am;

    private Vector3 leftX;
    private Vector3 rightX;
    
    public bool IsJumping
    {
        get { return isJumping; }
        set { isJumping = value; }
    }

    private Action OnNextCatCallback;

    private void Start()
    {
        pool = ObjectPoolManager.Instance;
        gm = GameManager.Instance;
        
        am = AudioManager.Instance;
    }

    private void Update()
    {
        if (transform.position.y < -6f && rb.bodyType == RigidbodyType2D.Kinematic)
        {
            pool.ReturnPrefabObj(gameObject, pool.catPrefabObjQueue);
            gm.cats.Remove(this);
        }
    }
    
    public void Init(Action onNextCatCallback)
    {
        OnNextCatCallback = onNextCatCallback;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        

    }
    
    private void OnEnable()
    {
        float camZ = Mathf.Abs(UnityEngine.Camera.main.transform.position.z);
        transform.position = UnityEngine.Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.15f, camZ));
        rightX = UnityEngine.Camera.main.ViewportToWorldPoint(new Vector3(0.85f, 0.15f, camZ));
        leftX = UnityEngine.Camera.main.ViewportToWorldPoint(new Vector3(0.15f, 0.15f, camZ));
    }


    public void Swapping(float dur)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = 8;
        
        swappingTween = transform.DOMoveX(rightX.x, dur/2)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 시작 전 위치 조절 → 부드럽게 시작
                transform.position = rightX;
                
                swappingTween = transform.DOMoveX(leftX.x, dur)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Yoyo);
            });

  
    }

    public void Jumping()
    {
        if (isJumping) return;
        isJumping = true;

        swappingTween?.Pause();
        swappingTween?.Kill();

        sr.sortingOrder = 10;

        animator.SetTrigger("Jump");

        transform.DOMoveY(0, jumpSpeed).SetEase(Ease.InOutQuad)
            .OnComplete(() => { rb.bodyType = RigidbodyType2D.Dynamic; });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic) return;
        if ((gm.cats.Count == 0 && collision.gameObject.CompareTag("Floor")) ||
            (gm.cats.Count > 0 && collision.gameObject.CompareTag("Cat") 
                               && collision.contacts[0].normal.y >= 1f))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            animator.SetTrigger("Land");
            sr.sortingOrder = 5;
            
            am.OnSfxPlay(0);
            
            gm.CountCat(gameObject);
            
            gm.UpdateCountUI();
            
            gm.ComboInit(gameObject);
            
            UpdateCatColliders();

            if (gm.cats.Count > 3)
            {
                StartCoroutine(gm.DownCtrl());
            }

            OnNextCatCallback?.Invoke();
            OnNextCatCallback = null;
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            gm.DecreaseLife();
            pool.ReturnPrefabObj(gameObject, pool.catPrefabObjQueue);
            OnNextCatCallback?.Invoke();
            OnNextCatCallback = null;
            
            am.OnSfxPlay(1);
            
            gm.ComboReset();
        }
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("ItemTime") && animator.GetCurrentAnimatorStateInfo(0).IsName("Land"))
        {
            gm._im.GetItemTime();
        }
        //
        // if (other.CompareTag("ItemPoint") && animator.GetCurrentAnimatorStateInfo(0).IsName("Land"))
        // {
        //     gm._im.GetItemPoint();
        // }
        
        if (other.CompareTag("ItemMinus") && animator.GetCurrentAnimatorStateInfo(0).IsName("Land"))
        {
            gm._im.GetItemMinus();
        }
            
    }

    private void UpdateCatColliders()
    {
        if (gm.cats.Count is 0 or 1) return;

        for (int i = 0; i < gm.cats.Count; i++)
        {
            Collider2D boxColl = gm.cats[i].GetComponent<BoxCollider2D>();

            if (i == gm.cats.Count - 1)
            {
                boxColl.enabled = true;
            }
            else
            {
                boxColl.enabled = false;
            }
        }
    }
}