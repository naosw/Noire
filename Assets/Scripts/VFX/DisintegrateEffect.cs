using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DisintegrationEffect
{
    public class DisintegrateEffect : MonoBehaviour
    {
        private Material sourceMaterial;                    // material from the source object
        private Mesh particleMesh;                          // particle mesh
        private Material particleMaterial;                  // particle material

        // read-only data lists
        private List<Mesh> triangleMeshes;                  // triangle meshes (form source object)
        private List<Vector3> initialPositions;             // initial positions
        private List<Vector3> particleVelocities;           // particle velocities
        private List<float> particleDelays;                 // particle delays
        private List<float> particleLifetimes;              // particle lifetimes

        // read-write data lists
        private List<float> particleAges;                   // particle ages
        private List<int> activeParticles;                  // the indicies of the particles that are currently active

        private UnityAction onComplete;                     // customizable method to call on completion of the effect

        private float elapsedTime;                          // time time elapsed for the effect

        private int timeDirection = 1;

        // list of TRS matrices for particles being rendered (updated each frame)
        private readonly List<Matrix4x4> renderMatrices = new();

        public static DisintegrateEffect Create (Vector3 position, Quaternion rotation) {
            // get the GameObject
            var go = new GameObject ();
            go.transform.SetPositionAndRotation (position, rotation);

            // attach a DisintegrationEffect component
            var effect = go.AddComponent<DisintegrateEffect> ();
            return effect;
        }

        private void Update () {
            // if there are no active particles, complete the effect
            if (activeParticles.Count == 0) {
                Complete ();
            }

            RenderEffect();
        }

        public void Play (
                List<Mesh> triangleMeshes, Material sourceMaterial, Mesh particleMesh, Material particleMaterial, 
                List<Vector3> initialPositions, List<Vector3> particleVelocities, List<float> particleLifetimes, 
                List<float> particleDelays, List<float> particleAges, List<int> activeParticles, 
                UnityAction onComplete = null
            ) {

            this.triangleMeshes = triangleMeshes;
            this.sourceMaterial = sourceMaterial;
            this.particleMesh = particleMesh;
            this.particleMaterial = particleMaterial;
            this.initialPositions = initialPositions;
            this.particleVelocities = particleVelocities;
            this.particleLifetimes = particleLifetimes;
            this.particleDelays = particleDelays;
            this.particleAges = particleAges;
            this.activeParticles = activeParticles;
            this.onComplete = onComplete;

            elapsedTime = 0f;
            timeDirection = 1;

            // render the effect (1st pass)
            // this is necessary, otherwise you'll get a flicker as the source renderer is turned off the frame
            // before the first update is executed on the RenderEffect (depending on object execution order)
            RenderEffect ();
        }

        private void RenderEffect () {
            // get the camera rotation (particles need to be facing the camera)
            var cameraRotation = Camera.main.transform.rotation;

            // locally store object position and rotation
            var objectPosition = transform.position;
            var objectRotation = transform.rotation;

            // loop over the active particles to render and update their data. loop backward because this
            // list will be deleted from while looping.
            for (int i = activeParticles.Count - 1; i >= 0; i--) {
                // get the particle index for the current active particle
                var particleIndex = activeParticles[i];

                // is the particle still within the delay window?
                var isDelayOver = particleAges[particleIndex] > particleDelays[particleIndex];

                // determine the scale based on the current age (only scale once the delay is over)
                var scale = 0f;
                if (isDelayOver) {
                    scale = Mathf.Min (1f, (particleAges[particleIndex] - particleDelays[particleIndex]) / particleLifetimes[particleIndex]);
                }

                // determine the speed step based on whether it is still within the delayed window (if so, the speed is zero)
                var speedStep = isDelayOver ? 1 : 0;

                // calculate the position based on the initial position, velocity, age, and object rotation
                var position = initialPositions[particleIndex] + ((particleAges[particleIndex] - particleDelays[particleIndex]) * speedStep * particleVelocities[particleIndex]);
                position = (objectRotation * position) + objectPosition;

                // when within delay, render the triangle. after the delay, render a particle.
                if (isDelayOver) {
                    var matrix = Matrix4x4.TRS (position, cameraRotation, Vector3.one * (1 - scale));
                    renderMatrices.Add (matrix);
                }
                else {
                    var matrix = Matrix4x4.TRS (position, objectRotation, Vector3.one);
                    Graphics.DrawMesh (triangleMeshes[particleIndex], matrix, sourceMaterial, 0);
                }

                // update the age
                particleAges[particleIndex] += (Time.deltaTime * timeDirection);

                // remove particle once the age (minus delay) has exceeded lifetime
                if ((particleAges[particleIndex] - particleDelays[particleIndex]) >= particleLifetimes[particleIndex]) {
                    activeParticles.RemoveAt(i);
                }
            }

            // increase the elapsed time and reverse time if loopTime exceeded
            elapsedTime += Time.deltaTime;

            // draw the particles
            if (renderMatrices.Count > 0) {
                DrawParticles ();
            }
        }

        private void DrawParticles () {
            // for each TRS matrix, render a particle
            for (int i = 0; i < renderMatrices.Count - 1;) {
                // get the data to draw (DrawMeshInstanced only support drawing 1023 meshes at a time)
                var count = Mathf.Min (1023, renderMatrices.Count - i);
                var matrixRange = renderMatrices.GetRange (i, count);

                // draw instanced
                Graphics.DrawMeshInstanced (particleMesh, 0, particleMaterial, matrixRange);
                i += count;
            }

            // clear the render matrices for the next frame
            renderMatrices.Clear();
        }

        public void Complete() {
            onComplete?.Invoke();

            Destroy (gameObject);
        }
    }
}