using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour {

    [SerializeField]
    private float speed;
    [SerializeField]
    private float dampling;

    [SerializeField]
    private float stealingDamage;

    [SerializeField]
    private bool canPatrol;
    [SerializeField]
    public List<PatrolPoint> patrolRoute = new List<PatrolPoint>();
    private int index = 0;
    private bool atPoint = false;
    private float _waitTimer = 0f;
    private bool canMove = false;
    [SerializeField]
    private float _rotateTimeout;
    private float _rotateTimer = 0;
    private Quaternion lookRotation;
    private bool rotateFlag = false;
    private Quaternion oldRotation;

    [SerializeField]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public float meshResolution;
    public MeshFilter meshFilter;
    private Mesh mesh;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;

    private PlayerController _player;
    private new Renderer renderer;
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        mesh = new Mesh();
        mesh.name = "Mesh";
        meshFilter.mesh = mesh;

        _player = GameObject.Find("MainCharacter").GetComponent<PlayerController>();
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.red;
        rb = GetComponent<Rigidbody>();

        if (patrolRoute.Count > 0 && canPatrol)
        {
            foreach (PatrolPoint p in patrolRoute)
            {
                p.waitTimeOut = Random.Range(1f, 3f);
            }
        }       
        lookRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180f, transform.rotation.z);
    }

    // Update is called once per frame
    // https://docs.unity3d.com/Manual/nav-MoveToClickPoint.html
    void FixedUpdate () {

        FindPlayer();

        if (canPatrol)
        {            
            if (atPoint)
            {
                _waitTimer += Time.deltaTime;

                if (_waitTimer >= patrolRoute[index].waitTimeOut)
                {
                    canMove = true;
                    _waitTimer = 0;
                }

                if (canMove)
                {
                    canMove = false;
                    atPoint = false;

                    if (index != patrolRoute.Count - 1)
                        index++;
                    else
                    {
                        patrolRoute.Reverse();
                        index = 1;
                    }

                    Move(patrolRoute[index]);
                }
            }
            else
                Move(patrolRoute[index]);
        }
        //Needs to be better
        else
        {
            rb.velocity = Vector3.zero;
            if (transform.rotation != oldRotation)
            {
                oldRotation = transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * dampling);
            }
            else
            {
                _rotateTimer += Time.deltaTime;

                if (_rotateTimer >= _rotateTimeout)
                {
                    _rotateTimer = 0;
                    oldRotation = Quaternion.Euler(Vector3.zero);
                    rotateFlag = !rotateFlag;
                    if (rotateFlag)
                        lookRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 270f, transform.rotation.z);
                    else
                        lookRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180f, transform.rotation.z);
                }
                
            }
        }      
    }

    void LateUpdate()
    {
        DrawGuardView();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Point"))
        {
            atPoint = true;
        }
    }

    private void Move(PatrolPoint point)
    {
        transform.position = Vector3.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
        Quaternion rotation = Quaternion.LookRotation(point.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampling);
    }

    void FindPlayer()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if (targets.Length > 0)
        {
            Vector3 dirToPlayer = (targets[0].transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float targetDistance = Vector3.Distance(transform.position, targets[0].transform.position);

                if (!Physics.Raycast(transform.position, dirToPlayer, targetDistance, obstacleMask))
                {
                    if (_player.isStealing)
                    {
                        _player.health -= stealingDamage;
                        _player.isStealing = false;
                        _player.stealTimer = 0;
                    }
                }
            }
        }
    }

    void DrawGuardView()
    {
        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float rayAngleSize = viewAngle / rayCount;

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + rayAngleSize * i;
            ViewCastInfo viewCastInfo = ViewCast(angle);

            if(i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - viewCastInfo.distance) > edgeDistanceThreshold;
                if(oldViewCast.hit != viewCastInfo.hit || (oldViewCast.hit && viewCastInfo.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, viewCastInfo);

                    if (edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);

                    if (edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(viewCastInfo.point);
            oldViewCast = viewCastInfo;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo viewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - viewCast.distance) > edgeDistanceThreshold;
            if (viewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = viewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = viewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
