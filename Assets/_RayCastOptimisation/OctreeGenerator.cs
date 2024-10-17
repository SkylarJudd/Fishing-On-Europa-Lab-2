// Ignore Spelling: waypoints Octree

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class OctreeGenerator : MonoBehaviour
    {
        public GameObject[] objects;
        public float minNodeSize = 1;
        Octree ot;

        public readonly Graph waypoints = new();

        private void Awake()
        {
            ot = new Octree(objects, minNodeSize, waypoints);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            //Gizmos.color = Color.green;
            //Gizmos.DrawWireCube(ot.bounds.center, ot.bounds.size);

            //ot.root.DrawNode();
            //ot.graph.DrawGraph();
        }
    }
}
