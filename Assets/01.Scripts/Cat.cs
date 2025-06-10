using System;
using DG.Tweening;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] private float posX = 2.3f;
    [SerializeField] private float posY = -4.45f;
    [SerializeField] private int swappingSpeed = 2;

    private Animator animator;
    private Tween swappingTween;
    private bool isJumpping;
    private Rigidbody2D rb;
    private ObjectPoolManager pool;
    private GameManager gm;

    private event Action OnNextCatCallback;

    private void Start()
    {
        pool = ObjectPoolManager.Instance;
        gm = GameManager.Instance;
        gm.cats.Add(this);
        Debug.Log($"list 개수 : {gm.cats.Count}");
    }

    private void Update()
    {
        if (transform.position.y < -6f)
        {
            ReturnPoolSpawnCat();
        }
    }

    public void Init(Action onNextCatCallback)
    {
        OnNextCatCallback = onNextCatCallback;
        transform.position = new Vector2(-posX, posY);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Swapping()
    {
        swappingTween = transform.DOMoveX(posX, swappingSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void Jumpping()
    {
        if (isJumpping) return;
        isJumpping = true;
        
        swappingTween.Pause();
        animator.SetTrigger("Jump");
        
        transform.DOMoveY(0, 1f).OnComplete(() =>
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic) return;
        if ((gm.cats.Count==1 && collision.gameObject.CompareTag("Floor"))||(gm.cats.Count>1 && collision.gameObject.CompareTag("Cat")))
        {
            isJumpping = false;
            animator.SetTrigger("Land");
            OnNextCatCallback?.Invoke();
            OnNextCatCallback = null;
            
            rb.bodyType = RigidbodyType2D.Kinematic;
           
            UpdateCatColliders();
            gm.DownCats();
        }
        else
        {
            ReturnPoolSpawnCat();
            gm.cats.Remove(this);
        }
            
    }
    
    private void UpdateCatColliders()
    {
        if (gm.cats.Count is 1 or 2) return;
        
        for (int i = 0; i < gm.cats.Count; i++)
        {
            Collider2D col = gm.cats[i].GetComponent<Collider2D>();
            if (i > gm.cats.Count - 2)
                col.enabled = true;
            else
                col.enabled = false;
        }
    }

    private void ReturnPoolSpawnCat()
    {
        OnNextCatCallback?.Invoke();
        OnNextCatCallback = null;
        pool.ReturnPrefabObj(gameObject, pool.catPrefabObjQueue);
        gm.cats.Remove(this);
    }

}