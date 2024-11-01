using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Europa.waterFallTool
{
    [ExecuteInEditMode]
    public class SplineSampler : MonoBehaviour
    {
        [SerializeField] private SplineContainer m_splineContainer;

        [SerializeField] private int m_splineIndex;
        [SerializeField][Range(0f, 1f)] private float m_time;
        [SerializeField][Range(0f, 100f)] private float m_width;

        private float3 position;
        private float3 forward;
        private float3 upVector;

        private float3 p1;
        private float3 p2;

        private void Update()
        {

        }

        public void SampleSplineWidth(float t, out Vector3 p1, out Vector3 p2)
        {
            m_splineContainer.Evaluate(m_splineIndex, t, out position, out forward, out upVector);

            //Tangent is the (Forward) Direction of the Travel Along The Spline To The Next Point;
            //Find The *Right( Direction Based On This.
            float3 right = Vector3.Cross(forward, upVector).normalized;

            p1 = position + (right * m_width);
            p2 = position + (-right * m_width);
        }

        private void OnDrawGizmos()
        {
            
        }
    }
}
