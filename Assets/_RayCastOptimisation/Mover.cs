using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Europa
{
    public class Mover : MonoBehaviour
    {
        private float speed = 5f;
        private float accuracy = 1f;
        private float turnSpeed = 5f;

        private int currentWayPoint;
        OctreeNode currentNode;
        Vector3 destination;

        public OctreeGenerator octreeGenerator;
        Graph graph;

        private void Start()
        {
            graph = octreeGenerator.waypoints;
            currentNode = GetClosestNode(transform.position);
            GetRandomDestination();
        }

        private void Update()
        {
            if (graph == null) return;

            if (graph.GetPathLength() == 0 || currentWayPoint >= graph.GetPathLength())
            {
                GetRandomDestination();
                return;
            }

            if (Vector3.Distance(graph.GetPathNode(currentWayPoint).bounds.center, transform.position) < accuracy)
            {
                currentWayPoint++;
                Debug.Log($"WayPoint {currentWayPoint} reached");
            }

            if (currentWayPoint < graph.GetPathLength())
            {
                currentNode = graph.GetPathNode(currentWayPoint);
                destination = currentNode.bounds.center;

                Vector3 direction = destination - transform.position;
                direction.Normalize();

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
                transform.Translate(0, 0, speed * Time.deltaTime);
            }
            else
            {
                GetRandomDestination();
            }
        }

        private OctreeNode GetClosestNode(Vector3 position)
        {
            OctreeNode closestNode = null;
            float closestDistanceSqr = Mathf.Infinity;

            foreach (var nodePair in graph.nodes)
            {
                OctreeNode node = nodePair.Key;
                float distanceSqr = (node.bounds.center - position).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestNode = node;
                }

            }
            return closestNode;
        }

        private void GetRandomDestination()
        {
            OctreeNode destinationNode;
            do
            {
                destinationNode = graph.nodes.ElementAt(Random.Range(0, graph.nodes.Count)).Key;
            } while (!graph.Astar(currentNode, destinationNode));
            currentWayPoint = 0;
        }

        private void OnDrawGizmos()
        {
            if (graph == null || graph.GetPathLength() == 0) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(graph.GetPathNode(0).bounds.center, 0.7f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(graph.GetPathNode(graph.GetPathLength() - 1).bounds.center, 0.7f);

            Gizmos.color = Color.green;
            for ( int i = 0; i < graph.GetPathLength(); i++ )
            {
                Gizmos.DrawWireSphere(graph.GetPathNode(i).bounds.center, 0.5f);
                if(i < graph.GetPathLength() - 1)
                {
                    Vector3 start = graph.GetPathNode(i).bounds.center;
                    Vector3 end = graph.GetPathNode(i+1).bounds.center;
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }
}
