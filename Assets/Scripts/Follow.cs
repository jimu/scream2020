using UnityEngine;
using UnityEngine.AI;

public class Follow : MonoBehaviour
{
    public Transform target;
    public int mode;

    [SerializeField] Vector3 offset = new Vector3(-1f, 12f, -4f);

    private void Update()
    {
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}