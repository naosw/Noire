using UnityEngine;

namespace OccaSoftware.CloudShadows.Runtime
{
    [AddComponentMenu("OccaSoftware/Cloud Shadows/Cloud Shadows")]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteAlways]
    /// <summary>
    /// This component enables the dynamic cloud shadows asset.
    /// </summary>
    public class CloudShadows : MonoBehaviour
    {
        [SerializeField]
        private float cloudLayerRelativeHeight = 200;

        [SerializeField]
        private Transform followTarget = null;

        [SerializeField]
        [Range(0, 360)]
        private float windDirection = 0;

        [SerializeField]
        [Min(0)]
        private float windSpeed = 1.0f;

        [SerializeField]
        [Range(0, 1)]
        private float fadeInExtents = 0.2f;

        [SerializeField]
        [Range(0, 1)]
        private float maximumOpacity = 0.8f;

        [SerializeField]
        [Min(0)]
        private float ditherScale = 1.0f;

        [SerializeField]
        [Min(0)]
        private float tilingDomain = 1000f;

        [SerializeField]
        [Range(0, 360)]
        private float orientation = 0;

        [SerializeField]
        [Range(0, 1)]
        private float cloudiness = 0.3f;

        [SerializeField]
        private Texture2D cloudTexture = null;

        [SerializeField]
        private DitherNoiseType ditherNoiseType = DitherNoiseType.InterleavedGradient;

        private Vector4[] offsets = new Vector4[3];
        private Material m;

        private static class ShaderParams
        {
            public static int _CloudShadowOffsets = Shader.PropertyToID("_CloudShadowOffsets");
            public static int _FadeInExtents = Shader.PropertyToID("_FadeInExtents");
            public static int _OneOverTilingDomain = Shader.PropertyToID("_OneOverTilingDomain");
            public static int _Orientation = Shader.PropertyToID("_Orientation");
            public static int _TilingDomain = Shader.PropertyToID("_TilingDomain");
            public static int _Cloudiness = Shader.PropertyToID("_Cloudiness");
            public static int _Texture = Shader.PropertyToID("_Texture");
            public static int _DitherScale = Shader.PropertyToID("_DitherScale");
            public static int _MaximumOpacity = Shader.PropertyToID("_MaximumOpacity");
            public static int _NoiseOption = Shader.PropertyToID("_NoiseOption");
        }

        private void OnEnable()
        {
            m = GetComponent<MeshRenderer>().sharedMaterial;
            offsets[0] = Vector4.zero;
            offsets[1] = Vector4.zero;
            offsets[2] = Vector4.zero;
            UpdateShaderProperties();
        }

        private void Update()
        {
            UpdateShaderProperties();
        }

        private void UpdateShaderProperties()
        {
            if (m == null)
            {
                m = GetComponent<MeshRenderer>().sharedMaterial;
            }
            offsets[0] = SetShadowOffset(windDirection, windSpeed, offsets[0]);
            offsets[1] = SetShadowOffset(windDirection, windSpeed * 2.01f, offsets[1]);
            offsets[2] = SetShadowOffset(windDirection, windSpeed * 4.03f, offsets[2]);
            m.SetVectorArray(ShaderParams._CloudShadowOffsets, offsets);
            m.SetFloat(ShaderParams._FadeInExtents, fadeInExtents);
            m.SetFloat(ShaderParams._Cloudiness, cloudiness);
            m.SetFloat(ShaderParams._TilingDomain, tilingDomain);
            m.SetFloat(ShaderParams._OneOverTilingDomain, 1.0f / tilingDomain);
            m.SetFloat(ShaderParams._Orientation, orientation);
            m.SetTexture(ShaderParams._Texture, cloudTexture);
            m.SetFloat(ShaderParams._DitherScale, ditherScale);
            m.SetFloat(ShaderParams._MaximumOpacity, maximumOpacity);
            m.SetFloat(ShaderParams._NoiseOption, (int)ditherNoiseType);
        }

        private Vector4 SetShadowOffset(float windDirection, float windSpeed, Vector2 offset)
        {
            float radians = windDirection * Mathf.Deg2Rad;
            Vector2 offsetDirection = new Vector2(-Mathf.Sin(radians), -Mathf.Cos(radians));
            Vector2 velocity = offsetDirection * windSpeed;
            offset += velocity * Time.deltaTime;

            return offset;
        }

        private void LateUpdate()
        {
            if (followTarget != null)
            {
                transform.position = followTarget.position + Vector3.up * cloudLayerRelativeHeight;
            }
            UpdateShaderProperties();
        }

        public enum DitherNoiseType
        {
            InterleavedGradient,
            Blue
        }
    }
}
