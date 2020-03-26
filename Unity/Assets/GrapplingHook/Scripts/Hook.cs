using UnityEngine;
using Vector3 = UnityEngine.Vector3;
public enum GrapplingHookState {EXPANDING, PULLING, RETRACING, INACTIVE}
[RequireComponent(typeof(MeshRenderer))]
public class Hook : MonoBehaviour
{
    #region SerializedFields ============================================================================================
    [Header("Linked objects")]
    [Tooltip("Miner rigid body.")]
    public MinerController player;

    [Header("General Settings")]
    [Tooltip("Hook inking point transform.")][SerializeField]
    private Transform hookPoint;
    [Tooltip("Grappling hook rope origin transform.")][SerializeField]
    public Transform origin;
    
    [Header("Behaviour parameters")]
    
    [Tooltip("Maximum rope deployment length.")][SerializeField]
    public float maxDeployDistance = 30f;
    [Tooltip("Rope deployment speed.")][SerializeField]
    private float deploySpeed;
    [Tooltip("Rope retraction speed.")][SerializeField]
    private float retractSpeed;
    [Tooltip("Pull speed.")][SerializeField]
    private float pullSpeed;
    [Tooltip("Minimum pull speed")][SerializeField]
    private float minPullClampDistance = 10f;
    [Tooltip("Maximum pull speed.")][SerializeField]
    private float maxPullClampDistance = 40f;
    [Tooltip("Rotation angle.")][SerializeField]
    private float rotationAngle = 180f;

    #endregion
    
    private Rope rope;
    [HideInInspector]
    public GrapplingHookState state = GrapplingHookState.INACTIVE;
    [HideInInspector]
    public Vector3 target;
    private Vector3 direction;
    [HideInInspector]
    public MeshRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        rope = GetComponentInChildren<Rope>();
        rope.origin = origin;
    }

    void Start()
    {
        renderer.enabled = false;
        rope.renderer.enabled = false;
        rope.origin = this.origin;
        state = GrapplingHookState.INACTIVE;
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (state == GrapplingHookState.INACTIVE)
            {
                //print("Hook : INACTIVE");
                return;
            }
            if (state == GrapplingHookState.EXPANDING)
            {
                //print("Hook : EXPANDING");
                rope.renderer.enabled = true;
                Vector3 direction = target - transform.position;
                float magnitude = Mathf.Clamp(direction.magnitude,10f,20f);
                Vector3 normDir = direction.normalized;
                RaycastHit hit;
                //Debug.DrawRay(transform.position, normDir * magnitude * deploySpeed * Time.deltaTime);
                if (Physics.Raycast(transform.position, normDir, out hit, magnitude * deploySpeed * Time.deltaTime))
                {
                    //print("Hook : SURFACE TOUCHED");
                    transform.position = hit.point;
                    target = hit.point;
                    transform.forward = hit.normal;
                    transform.position -= hookPoint.position-transform.position;
                    state = GrapplingHookState.PULLING;
                }
                else
                {
                    transform.position += normDir * magnitude * deploySpeed * Time.deltaTime; //
                    transform.forward = normDir;
                }
            }
            if (state == GrapplingHookState.PULLING)
            {
                //print("Hook : PULLING");
                Vector3 direction = target - player.transform.position;
                float speed = Mathf.Clamp(direction.magnitude,minPullClampDistance,maxPullClampDistance);
                Vector3 normDir = direction.normalized;
                Vector3 force = speed * normDir * pullSpeed;
                //print("direction.magnitude : " + direction.magnitude);
                player.velocity += force * Time.deltaTime; //player.AddForce(force);
                if (direction.magnitude < 1f)
                {
                    state = GrapplingHookState.INACTIVE;
                    renderer.enabled = false;
                    rope.renderer.enabled = false;
                }
            }
        }

        if (state == GrapplingHookState.RETRACING)
        {
            //print("Hook : RETRACING");
            Vector3 direction = rope.origin.position - transform.position;
            float magnitude = Mathf.Clamp(direction.magnitude,1f,20f);
            Vector3 normDir = direction.normalized;
            transform.position += normDir * magnitude * retractSpeed * Time.deltaTime;
            transform.forward = normDir;
            RaycastHit hit;
            //print("magnitude : " + direction.magnitude);
            if (direction.magnitude < 0.2)
            {
                //print("Disabling grappling hook.");
                renderer.enabled = false;
                rope.renderer.enabled = false;
                state = GrapplingHookState.INACTIVE;
            }
        }
    }

    public void setHookAnchor(Transform anchor)
    {
        this.rope.origin = anchor;
    }
}
