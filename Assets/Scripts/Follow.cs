using UnityEngine;
using UnityEngine.AI;

public class Follow : MonoBehaviour
{
    public enum Mode { Track, Spy };
    Transform target;
    Mode mode;

    [SerializeField] Vector3 offset = new Vector3(-1f, 12f, -4f);

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        FollowTarget(GameObject.FindGameObjectWithTag("Player").transform);

    }

    private void Update()
    {
        //Debug.Log("Target.position=" + target.position);
        if (mode ==  Mode.Spy)
        {
            transform.position = target.position + offset;
            transform.LookAt(target.TransformPoint(Vector3.zero));
        }
        else
        {
            transform.position = target.position;
            transform.forward = Vector3.down;
        }
    }

    public void FollowTarget( Transform target, Mode mode = Mode.Track)
    {
        this.target = target;
        this.mode = mode;
    }
}