using System;
using DG.Tweening;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] private float posX = 2.3f;
    [SerializeField] private float posY = -4.45f;
    [SerializeField] private int swappingSpeed = 2;
    [SerializeField] private int forcePower = 10;

    private Animator animator;
    private Tween swappingTween;
    private bool isJumpping;
    private Rigidbody2D rb;

    private event Action OnNextCatCallback;

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
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(Vector2.up * forcePower, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            animator.SetTrigger("Land");
            isJumpping = false;
            OnNextCatCallback?.Invoke();
            OnNextCatCallback = null;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}