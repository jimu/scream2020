using UnityEngine;

public class FearIcon : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public int fears;
    [SerializeField] Vector3 offset;


    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void MoveTo(Vector3 pos)
    {
        transform.position = pos + offset;
    }

    public void SetFear(int n)
    {
        fears = n;
        spriteRenderer.size = new Vector2(n * 1.1f / 3, 1);
    }
}
