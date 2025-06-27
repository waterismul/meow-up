using UnityEngine;

public class Item : MonoBehaviour
{
    private GameManager gm;
    private void Start()
    {
        gm = GameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            gameObject.transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            gameObject.transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
    
}
