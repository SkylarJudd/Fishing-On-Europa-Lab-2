// Ignore Spelling: Octree

using UnityEngine;

namespace Europa
{
    public class OctreeObject
    {
        Bounds bounds;

        public OctreeObject(GameObject obj)
        {
            bounds = obj.GetComponent<Collider>().bounds;
        }

        public bool Intersects(Bounds boundsToCheck) => bounds.Intersects(boundsToCheck);
        
    }
}
