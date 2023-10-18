using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DisintegrationEffect
{
    public class Disintegratable : MonoBehaviour
    {
        private Mesh sourceMesh;
        private Material sourceMaterial;
        private Renderer sourceRenderer;

        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;

        // the speed at which a particle will move away from the origin point
        [SerializeField] private float particleSpeed;

        // the speed at which a particle will spread out along its normal
        [SerializeField] private float particleSpread;

        // a velocity that is uniform across all particles
        [SerializeField] private Vector3 particleDrift;

        [Header ("Lifetime")] 
        [SerializeField] [Min (0.001f)] private float minLifetime;
        [SerializeField] [Min (0.001f)] private float maxLifetime;

        // the amount a particle is delayed based on its distance from the
        // origin point (in seconds per meter)
        [Header ("Delay")] 
        [SerializeField] private float delayFactor;

        // the minimum amount of delay (additive with the delay factor)
        [SerializeField] [Min (0)] private float delayVarianceMinimum;

        // the maximum amount of delay (additive with the delay factor)
        [SerializeField] [Min (0)] private float delayVarianceMaximum;

        // immutable data
        private List<Mesh> triangleMeshes;                  // meshes
        private List<Vector3> initialPositions;             // initial positions
        private List<Vector3> triangleNormals;              // normals
        private List<float> particleLifetimes;              // lifetime

        private void Start () {
            // if source is skinned mesh renderer, pull info from that
            if(TryGetComponent(out SkinnedMeshRenderer smr))
            {
                sourceMesh = smr.sharedMesh;
                sourceMaterial = smr.sharedMaterial;
                sourceRenderer = smr;
            } 
            // otherwise, the information should be pulled from MeshFilter and MeshRenderer
            // componenets
            else {
                sourceMesh = GetComponent<MeshFilter>().sharedMesh;
                var mr = GetComponent<MeshRenderer>();
                sourceMaterial = mr.sharedMaterial;
                sourceRenderer = mr;
            }

            // decompose the source mesh into individual triangle meshes
            DecomposeMesh ();
        }

        private void DecomposeMesh () {
            // store local references to mesh data 
            var meshTriangles = sourceMesh.triangles;
            var meshVertices = sourceMesh.vertices;
            var meshUV = sourceMesh.uv;
            var meshNormals = sourceMesh.normals;

            // allocate the lists
            var triangleCount = meshTriangles.Length;
            triangleMeshes = new List<Mesh> (triangleCount);
            initialPositions = new List<Vector3> (triangleCount);
            triangleNormals = new List<Vector3> (triangleCount);
            particleLifetimes = new List<float> (triangleCount);

            // process every triangle in the source mesh
            for (int t = 0; t < triangleCount; t += 3) {
                // get the indices for the current triangle
                var t0 = meshTriangles[t];
                var t1 = meshTriangles[t + 1];
                var t2 = meshTriangles[t + 2];

                // get the vertices
                var vertex0 = meshVertices[t0];
                var vertex1 = meshVertices[t1];
                var vertex2 = meshVertices[t2];

                // calculate the triangle position (in object-space coordinates)
                var trianglePosition = (vertex0 + vertex1 + vertex2) / 3;

                // create vertex array for triangle mesh. vertex positions
                // should be in object-space coordinates (for the disintegratable
                // object)
                var vertices = new Vector3[] {
                    vertex0 - trianglePosition,
                    vertex1 - trianglePosition,
                    vertex2 - trianglePosition
                };

                // create uv array for triangle mesh
                var uvs = new Vector2[] {
                    meshUV[t0],
                    meshUV[t1],
                    meshUV[t2]
                };

                // get the normals
                var normals = new Vector3[] {
                    meshNormals[t0],
                    meshNormals[t1],
                    meshNormals[t2]
                };

                // calculate the triangle normal (average of vertex normals)
                var triangleNormal = (normals[0] + normals[1] + normals[2]) / 3;

                // create a new mesh for the current triangle from the source mesh
                var triangleMesh = new Mesh {
                    vertices = vertices,
                    uv = uvs,
                    normals = normals,
                    triangles = new int[] { 0, 1, 2 }
                };

                // calculate the particle lifetime
                var lifetime = Random.Range (minLifetime, maxLifetime);

                // add the triangle/particle data to the lists
                initialPositions.Add (trianglePosition);
                triangleNormals.Add (triangleNormal);
                triangleMeshes.Add (triangleMesh);
                particleLifetimes.Add (lifetime);
            }
        }

        Vector3 CalculateVelocity(Vector3 pos, int i)
        {
            // get particle position and normal, rotated by the object rotation
            var position = transform.rotation * pos;
            var normal = transform.rotation * triangleNormals[i];

            // velocity is the speed moving from the particle away from the origin,
            // and also moving along the normal of the triangle in the source mesh,
            // combined with the uniform drift.
            var velocity = (position - transform.position).normalized * particleSpeed;
            velocity += normal * particleSpread;
            velocity += particleDrift;

            return velocity;
        }
        
        float CalculateDelay(Vector3 pos, int i)
        {
            var delay = Vector3.Distance (transform.rotation * pos, transform.position) * delayFactor;
            delay += Random.Range (delayVarianceMinimum, delayVarianceMaximum);
            return delay;
        }
        
        
        public void Disintegrate(Vector3 originPoint) {
            var count = initialPositions.Count;

            // create lists for generated data
            var particleVelocities = initialPositions.Select(CalculateVelocity).ToList();  
            var particleDelay = initialPositions.Select(CalculateDelay).ToList();
            var particleAge = new float[count].ToList();
            var activeParticles = Enumerable.Range(0, count).ToList();

            // disable the source renderer
            sourceRenderer.enabled = false;

            // Create and Play the effect
            var effect = DisintegrateEffect.Create (transform.position, transform.rotation);
            effect.Play (
                triangleMeshes, sourceMaterial, particleMesh, particleMaterial, 
                initialPositions, particleVelocities, particleLifetimes, particleDelay, 
                particleAge, activeParticles
            );
        }
    }
}