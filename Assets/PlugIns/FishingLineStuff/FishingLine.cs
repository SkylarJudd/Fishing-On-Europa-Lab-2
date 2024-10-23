using Obvious.Soap;
using System.Collections;
using UnityEngine;

public class BezierLine : MonoBehaviour
{
    
    [SerializeField] private Vector3Reference lineStartPoint;
    [SerializeField] private Vector3Reference lineEndPoint;

    public Transform controlPoint;
    public int resolution = 10; // Number of points on the line
    public GameObject averagePointObject; // GameObject representing the average point
    public float springStiffness = 100.0f;
    public float springDamping = 5.0f;
    public float springDistanceMax = 5.0f;
    public float controlPointMass = 2.0f; // Mass of the control point
    public float controlPointDrag = 2.0f; // Drag of the control point

    [Header("Tension")]
    public float tensionStrenght = 0;
    public float minDistance = 0;
    public float maxDistance = 20;
    public float tensionFloor = 10;

    private LineRenderer lineRenderer;
    private Transform averagePoint;

    private Vector3 lastStartPointPos;
    private Vector3 lastEndPointPos;

    private SpringJoint springJoint; // Spring Joint to attach the control point to the average point

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component not found!");
            return;
        }

        if (averagePointObject == null)
        {
            Debug.LogError("Average Point GameObject not assigned!");
            return;
        }

        averagePoint = averagePointObject.transform;

        // Set LineRenderer settings
        lineRenderer.positionCount = resolution + 1;
        lineRenderer.useWorldSpace = true;

        // Initialize last positions
        lastStartPointPos = lineStartPoint.Value;
        lastEndPointPos = lineEndPoint.Value;

        // Add Spring Joint component to control point
        springJoint = controlPoint.gameObject.AddComponent<SpringJoint>();
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedBody = averagePointObject.GetComponent<Rigidbody>();
        springJoint.connectedAnchor = Vector3.zero;
        springJoint.spring = springStiffness;
        springJoint.damper = springDamping;
        springJoint.maxDistance = springDistanceMax;

        // Set control point mass and drag
        Rigidbody controlRigidbody = controlPoint.GetComponent<Rigidbody>();
        if (controlRigidbody != null)
        {
            controlRigidbody.mass = controlPointMass;
            controlRigidbody.linearDamping = controlPointDrag;
        }
    }

    private void LateUpdate()
    {
        // Update the line in LateUpdate for smooth rendering
        StartCoroutine(GenerateAndRenderCurve());
    }

    private void FixedUpdate()
    {
        // Update the average point in FixedUpdate
        averagePoint.position = (lineStartPoint.Value + lineEndPoint.Value) / 2;

        float currentDistance = Vector3.Distance(lineStartPoint.Value, lineEndPoint.Value);

        tensionStrenght = Mathf.InverseLerp(minDistance, maxDistance, currentDistance);

        ApplyTension();
    }

    private IEnumerator GenerateAndRenderCurve()
    {
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = CalculateQuadraticBezierPoint(t, lineStartPoint.Value, controlPoint.position, lineEndPoint.Value);
            lineRenderer.SetPosition(i, point);
            yield return null;
        }
    }

    //Calculate a point on the quadratic Bezier curve
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttu = tt * u;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += tt * t * lineEndPoint.Value;

        return p;
    }

    void ApplyTension()
    {
        Vector3 midPoint = (lineStartPoint.Value + lineEndPoint.Value) / 2;

        float slack = Mathf.Lerp(tensionFloor, 0, tensionStrenght);
        midPoint += Vector3.up * slack;

        float minY = Mathf.Min(lineStartPoint.Value.y, lineEndPoint.Value.y);
        midPoint.y = Mathf.Max(midPoint.y, minY);

        controlPoint.position = midPoint;
    }
}