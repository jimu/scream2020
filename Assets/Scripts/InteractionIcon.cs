using UnityEngine;

public class InteractionIcon : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite spriteTree;
    [SerializeField] Sprite spriteLoot;
    [SerializeField] Sprite spriteKill;

    [Tooltip("Recommeneded offset: 0,1,0.85")]
    [SerializeField] Vector3 offsetDefault;
    [SerializeField] Vector3 offsetKill;
    [SerializeField] Vector3 offsetTree;
    [SerializeField] GameObject treeUIIcon;
    Vector3 offset;
    Transform follow;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (follow == null)
            gameObject.SetActive(false);  // TODO: hack
        else
            transform.position = follow.position + offset;
    }


    public void Follow(Transform who, Sprite sprite)
    {
        Debug.Log("Follow: " + sprite.name + "  sr=");
        //spriteRenderer.size = new Vector2(n * 1.1f / 3, 1);
        if (follow != who)
        {
            follow = who;
            spriteRenderer.sprite =
                who.tag == "Enemy" ? spriteKill :
                who.tag == "Loot" ? spriteLoot :
                who.tag == "Corpse" ? spriteLoot :
                who.tag == "Log" ? spriteTree :
                spriteTree;
            //Debug.Log("Setting Sprite");
            offset =
                who.tag == "Enemy" ? offsetKill :
                who.tag == "Loot" ? offsetDefault :
                who.tag == "Corpse" ? offsetKill :
                who.tag == "Log" ? offsetTree :
                offsetTree;

            treeUIIcon.SetActive(who.tag == "Log");
            //Debug.Break();
        }
    }
}
