using UnityEngine;

namespace AbsurdityAI.VisionOS.Input
{
    /// <summary>
    /// MonoBehaviour component that provides easy access to GCStylus input.
    /// Add this to a GameObject in your scene to automatically initialize and update stylus input.
    /// </summary>
    public class GCStylusManager : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Initialize stylus on Awake")]
        public bool initializeOnAwake = true;

        [Tooltip("Update stylus data every frame")]
        public bool updateEveryFrame = true;

        [Header("Debug")]
        [Tooltip("Show debug information in console")]
        public bool showDebugInfo = false;

        [Tooltip("Draw stylus gizmos in scene view")]
        public bool drawGizmos = true;

        private StylusData currentData;

        /// <summary>
        /// Get the current stylus data
        /// </summary>
        public StylusData CurrentData => currentData;

        /// <summary>
        /// Check if stylus is available
        /// </summary>
        public bool IsAvailable => GCStylus.IsAvailable();

        /// <summary>
        /// Event triggered when stylus starts touching
        /// </summary>
        public event System.Action OnTouchBegan;

        /// <summary>
        /// Event triggered when stylus stops touching
        /// </summary>
        public event System.Action OnTouchEnded;

        /// <summary>
        /// Event triggered when stylus enters range
        /// </summary>
        public event System.Action OnStylusEntered;

        /// <summary>
        /// Event triggered when stylus exits range
        /// </summary>
        public event System.Action OnStylusExited;

        private bool wasTouching = false;
        private bool wasInRange = false;

        private void Awake()
        {
            if (initializeOnAwake)
            {
                GCStylus.Initialize();
            }
        }

        private void Update()
        {
            if (updateEveryFrame)
            {
                GCStylus.Update();
                currentData = GCStylus.GetData();

                // Trigger events
                CheckEvents();

                if (showDebugInfo && GCStylus.IsAvailable())
                {
                    LogDebugInfo();
                }
            }
        }

        private void CheckEvents()
        {
            // Check touch events
            if (currentData.touching && !wasTouching)
            {
                OnTouchBegan?.Invoke();
            }
            else if (!currentData.touching && wasTouching)
            {
                OnTouchEnded?.Invoke();
            }

            // Check range events
            if (currentData.isInRange && !wasInRange)
            {
                OnStylusEntered?.Invoke();
            }
            else if (!currentData.isInRange && wasInRange)
            {
                OnStylusExited?.Invoke();
            }

            wasTouching = currentData.touching;
            wasInRange = currentData.isInRange;
        }

        private void LogDebugInfo()
        {
            Debug.Log($"Stylus - Touch: {currentData.touching}, InRange: {currentData.isInRange}, " +
                     $"Position: ({currentData.touchX:F2}, {currentData.touchY:F2}), " +
                     $"Azimuth: {currentData.azimuthAngle:F2}, Altitude: {currentData.altitudeAngle:F2}, " +
                     $"Force: {currentData.force:F2}");
        }

        private void OnDestroy()
        {
            GCStylus.Shutdown();
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            if (!Application.isPlaying)
                return;

            if (currentData.touching || currentData.isInRange)
            {
                // Draw stylus position
                Vector3 position = new Vector3(currentData.touchX, currentData.touchY, 0);
                
                Gizmos.color = currentData.touching ? Color.green : Color.yellow;
                Gizmos.DrawWireSphere(position, 0.1f);

                // Draw force indicator
                if (currentData.force > 0)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(position, 0.1f + currentData.force * 0.2f);
                }
            }
        }
    }
}
