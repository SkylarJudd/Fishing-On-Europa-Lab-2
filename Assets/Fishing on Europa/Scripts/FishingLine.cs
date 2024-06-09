using System.Collections;
using UnityEngine;

public class BezierLine : MonoBehaviour
{
    public Transform startPoint;
    public Transform controlPoint;
    public Transform endPoint;
    public int resolution = 10; // Number of points on the line
    public GameObject averagePointObject; // GameObject representing the average point
    public float springStiffness = 100.0f;
    public float springDamping = 5.0f;
    public float springDistanceMax = 5.0f;
    public float controlPointMass = 2.0f; // Mass of the control point
    public float controlPointDrag = 2.0f; // Drag of the control point

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
        lastStartPointPos = startPoint.position;
        lastEndPointPos = endPoint.position;

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
            controlRigidbody.drag = controlPointDrag;
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
        averagePoint.position = (startPoint.position + endPoint.position) / 2;
    }

    private IEnumerator GenerateAndRenderCurve()
    {
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = CalculateQuadraticBezierPoint(t, startPoint.position, controlPoint.position, endPoint.position);
            lineRenderer.SetPosition(i, point);
            yield return null;
        }
    }

    // Calculate a point on the quadratic Bezier curve
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
        p += tt * t * endPoint.position;

        return p;
    }
}