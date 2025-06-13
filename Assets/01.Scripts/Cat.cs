using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class Cat : MonoBehaviour
{
    [SerializeField] private float posX = 2.3f;
    [SerializeField] private float posY = -4.45f;
    [SerializeField] private int swappingSpeed = 1;

    private Animator animator;
    private Tween swappingTween;
    private bool isJumping;
    private Rigidbody2D rb;
    private GameManager gm;
    private ObjectPoolManager pool;

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
        transform.position = new Vector3(posX, posY, transform.position.z);
    }

    public void Swapping()
    {
        swappingTween = transform.DOMoveX(posX, swappingSpeed)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .From(-posX);
    }
    
    public void Jumping()
    {
        if (isJumping) return;
        isJumping = true;

        swappingTween?.Pause();
        swappingTween?.Kill();
        
        animator.SetTrigger("Jump");

        transform.DOMoveY(0, 1f).OnComplete(() => { rb.bodyType = RigidbodyType2D.Dynamic; });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic) return;
        
        if ((gm.cats.Count == 0 && collision.gameObject.CompareTag("Floor")) ||
            (gm.cats.Count > 0 && collision.gameObject.CompareTag("Cat")))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            animator.SetTrigger("Land");
            gm.cats.Add(this);
            
            gm.catCount += 1;
            
            Debug.Log(gm.cats.Count);
            
            UpdateCatColliders();

            if (gm.cats.Count > 4)
            {
                StartCoroutine(gm.DownCats());
            }
            
            OnNextCatCallback?.Invoke();
            OnNextCatCallback = null;
            
        }
        else
        {
            gm.DecreaseLife();
            pool.ReturnPrefabObj(gameObject, pool.catPrefabObjQueue);
            OnNextCatCallback?.Invoke();
            OnNextCatCallback = null;
        }
    }

    private void UpdateCatColliders()
    {
        if (gm.cats.Count is 0 or 1) return;

        for (int i = 0; i < gm.cats.Count; i++)
        {
            Collider2D col = gm.cats[i].GetComponent<BoxCollider2D>();
            
            if (i == gm.cats.Count - 1) 
                col.enabled = true;
                
            else
                col.enabled = false;
        }
    }


    
}