using UnityEngine;
using UnityEngine.AI;

public class Follow : MonoBehaviour
{

    public Transform target;
    [SerializeField] float height = 10f;
    [SerializeField] Vector3 offset = new Vector3(-1f, 12f, -4f);

    private void Update()
    {
        Vector3 dest = target.position + offset;
        dest.x++;
        dest.y = height;
        dest.z++;
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}