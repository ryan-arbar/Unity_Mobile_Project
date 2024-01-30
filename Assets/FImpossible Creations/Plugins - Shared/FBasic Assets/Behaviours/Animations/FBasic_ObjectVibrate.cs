using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FM: Components which is changing transform's position in vibration-like style
    /// </summary>
    public class FBasic_ObjectVibrate : MonoBehaviour
    {
        [Tooltip("How fast object should change translation directions")]
        public float VibrationRate = 8f;

        [Tooltip("How far object can go from it's initial local position")]
        public float BaseRange = 0.5f;

        [Tooltip("Smoothing motion for object")]
        [Range(0f,1f)]
        public float SmoothTranslation = 0.5f;

        #region Private variables for calculations

        /// <summary> Array of random floats for randomness in movement </summary>
        private float[] randomOffsets = new float[6];

        /// <summary> Position defined at start for animation correctness, you can put this object into other empty to have mobility </summary>
        public Vector3 initialPosition { get; private set; }

        /// <summary> Target calculated position </summary>
        public Vector3 localPosition { get; private set; }

        /// <summary> Time variable for changing rate of vibration without jittering like using Time.time </summary>
        private float time;

        /// <summary> Animation Variable </summary>
        private float speed;
        /// <summary> Animation variable </summary>
        private float range;
        /// <summary> Animation variable </summary>
        internal float intensity;

        #endregion

        /// <summary> Separate power of vibration on different axes, or turn off some </summary>
        public Vector3 AxesMultiplier = Vector3.one;

        /// <summary> False when you only want use calculated variables like 'position' in component </summary>
        public bool ChangeObjectPosition = true;

        private void Start()
        {
            initialPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            localPosition = new Vector3(0f, 0f, 0f);
            ChooseNewSeed();

            speed = VibrationRate;
            range = BaseRange;
        }

        private void Update()
        {
            if (BaseRange == 0f)
            {
                if (ChangeObjectPosition) transform.localPosition = initialPosition;
                return;
            }

            // Calculating base variables for animating vibration
            intensity = Mathf.Max(intensity * Time.deltaTime, 0f);

            float intensityLog = Mathf.Max(0f, Mathf.Log(intensity) / 5f);
            intensity -= Mathf.Max(0f, intensityLog);

            speed = Mathf.Min(75f, VibrationRate + intensity * 4f - intensityLog * 5f);
            range = (BaseRange + intensity * 0.01f) * 0.01f;

            time += Time.deltaTime * speed;
            Vector3 newPos = new Vector3(0f, 0f, 0f);

            // Calculating different axes values with trigonometric functions
            if (AxesMultiplier.x != 0f)
            {
                newPos.x = Mathf.Sin(time * randomOffsets[0] + randomOffsets[1]) * range * randomOffsets[3];
                newPos.x += Mathf.Pow(Mathf.Cos(time / 1.5f * randomOffsets[2]), 2) * range * randomOffsets[5];
                newPos.x *= AxesMultiplier.x;
            }
            else newPos.x = 0f;

            if (AxesMultiplier.y != 0f)
            {
                newPos.y = Mathf.Cos(time * randomOffsets[1] + randomOffsets[2]) * range * randomOffsets[4];
                newPos.y += Mathf.Sin(time / 2.2f * randomOffsets[4] + randomOffsets[1]) * range * randomOffsets[3];
                newPos.y *= AxesMultiplier.y;
            }
            else newPos.y = 0f;

            if (AxesMultiplier.z != 0f)
            {
                newPos.z = Mathf.Sin(time * randomOffsets[2] + randomOffsets[0]) * range * randomOffsets[5];
                newPos.z += Mathf.Cos(time * 1.24f * randomOffsets[3] + randomOffsets[0]) * range * randomOffsets[4];
                newPos.z *= AxesMultiplier.z;
            }
            else newPos.z = 0f;

            // Assigning new variables
            float smoothDelta = Mathf.Lerp(1f, Time.deltaTime * 0.5f, SmoothTranslation);

            localPosition = Vector3.Lerp(localPosition, newPos, smoothDelta );

            if (ChangeObjectPosition)
            {
                newPos += initialPosition;
                transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, smoothDelta);
            }
        }

        /// <summary>
        /// Setting randomizing variables
        /// </summary>
        public void ChooseNewSeed()
        {
            Random.InitState(Random.Range(0, 999999));
            for (int i = 0; i < 3; i++) randomOffsets[i] = Random.Range(0.8f, 1.0f);
            for (int i = 3; i < 6; i++) randomOffsets[i] = Random.Range(1.0f, 2.5f);
            time = Random.Range(0f, 4f);
        }
    }
}