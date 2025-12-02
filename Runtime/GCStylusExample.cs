using UnityEngine;
using AbsurdityAI.VisionOS.Input;

namespace AbsurdityAI.VisionOS.Examples
{
    /// <summary>
    /// Example script demonstrating how to use the GCStylus wrapper
    /// </summary>
    public class GCStylusExample : MonoBehaviour
    {
        [Header("References")]
        public Transform stylus3DIndicator;
        public LineRenderer strokeRenderer;

        [Header("Settings")]
        public float positionScale = 0.01f;
        public float forceScale = 2.0f;

        private GCStylusManager stylusManager;
        private Vector3 lastPosition;

        private void Start()
        {
            // Get or add the stylus manager
            stylusManager = FindObjectOfType<GCStylusManager>();
            if (stylusManager == null)
            {
                GameObject go = new GameObject("GCStylusManager");
                stylusManager = go.AddComponent<GCStylusManager>();
            }

            // Subscribe to events
            stylusManager.OnTouchBegan += OnStylusTouchBegan;
            stylusManager.OnTouchEnded += OnStylusTouchEnded;
            stylusManager.OnStylusEntered += OnStylusEntered;
            stylusManager.OnStylusExited += OnStylusExited;
        }

        private void Update()
        {
            if (!GCStylus.IsAvailable())
            {
                return;
            }

            // Get current stylus data
            StylusData data = stylusManager.CurrentData;

            // Update 3D indicator if available
            if (stylus3DIndicator != null && (data.touching || data.isInRange))
            {
                Vector3 position = new Vector3(
                    data.touchX * positionScale,
                    data.touchY * positionScale,
                    0
                );
                stylus3DIndicator.position = position;

                // Scale based on force
                float scale = 1.0f + (data.force * forceScale);
                stylus3DIndicator.localScale = Vector3.one * scale;

                // Rotate based on azimuth and altitude
                Quaternion rotation = Quaternion.Euler(
                    Mathf.Rad2Deg * data.altitudeAngle,
                    0,
                    Mathf.Rad2Deg * data.azimuthAngle
                );
                stylus3DIndicator.rotation = rotation;

                lastPosition = position;

                // Add to stroke if touching
                if (data.touching && strokeRenderer != null)
                {
                    int currentCount = strokeRenderer.positionCount;
                    strokeRenderer.positionCount = currentCount + 1;
                    strokeRenderer.SetPosition(currentCount, position);
                }
            }

            // Log data for debugging
            if (Input.GetKeyDown(KeyCode.D))
            {
                LogStylusData(data);
            }
        }

        private void OnStylusTouchBegan()
        {
            Debug.Log("Stylus touch began!");
            
            // Reset stroke renderer for a new stroke
            if (strokeRenderer != null)
            {
                strokeRenderer.positionCount = 1;
                strokeRenderer.SetPosition(0, lastPosition);
            }
        }

        private void OnStylusTouchEnded()
        {
            Debug.Log("Stylus touch ended!");
        }

        private void OnStylusEntered()
        {
            Debug.Log("Stylus entered range!");
        }

        private void OnStylusExited()
        {
            Debug.Log("Stylus exited range!");
        }

        private void LogStylusData(StylusData data)
        {
            Debug.Log($"=== Stylus Data ===\n" +
                     $"Touch Position: ({data.touchX}, {data.touchY})\n" +
                     $"Azimuth Angle: {Mathf.Rad2Deg * data.azimuthAngle}°\n" +
                     $"Altitude Angle: {Mathf.Rad2Deg * data.altitudeAngle}°\n" +
                     $"Force: {data.force}\n" +
                     $"In Range: {data.isInRange}\n" +
                     $"Touching: {data.touching}");
        }

        private void OnDestroy()
        {
            if (stylusManager != null)
            {
                stylusManager.OnTouchBegan -= OnStylusTouchBegan;
                stylusManager.OnTouchEnded -= OnStylusTouchEnded;
                stylusManager.OnStylusEntered -= OnStylusEntered;
                stylusManager.OnStylusExited -= OnStylusExited;
            }
        }
    }
}
