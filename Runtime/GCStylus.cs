using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AbsurdityAI.VisionOS.Input
{
    /// <summary>
    /// Structure representing stylus input data from Apple's GCStylus
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StylusData
    {
        public float touchX;
        public float touchY;
        public float azimuthAngle;
        public float altitudeAngle;
        public float force;
        public bool isInRange;
        public bool touching;
    }

    /// <summary>
    /// Unity wrapper for Apple's GameController GCStylus API
    /// Provides access to stylus input (Apple Pencil) on iOS and visionOS devices
    /// </summary>
    public static class GCStylus
    {
#if UNITY_IOS || UNITY_VISIONOS
        [DllImport("__Internal")]
        private static extern void GCStylus_Initialize();

        [DllImport("__Internal")]
        private static extern void GCStylus_Update();

        [DllImport("__Internal")]
        private static extern StylusData GCStylus_GetData();

        [DllImport("__Internal")]
        private static extern bool GCStylus_IsAvailable();

        [DllImport("__Internal")]
        private static extern float GCStylus_GetTouchX();

        [DllImport("__Internal")]
        private static extern float GCStylus_GetTouchY();

        [DllImport("__Internal")]
        private static extern float GCStylus_GetAzimuthAngle();

        [DllImport("__Internal")]
        private static extern float GCStylus_GetAltitudeAngle();

        [DllImport("__Internal")]
        private static extern float GCStylus_GetForce();

        [DllImport("__Internal")]
        private static extern bool GCStylus_IsInRange();

        [DllImport("__Internal")]
        private static extern bool GCStylus_IsTouching();

        [DllImport("__Internal")]
        private static extern void GCStylus_SetData(float touchX, float touchY, float azimuth, float altitude, float force, bool inRange, bool touching);

        [DllImport("__Internal")]
        private static extern void GCStylus_Shutdown();
#endif

        private static bool isInitialized = false;

        /// <summary>
        /// Initialize the GCStylus wrapper. Call this once at startup.
        /// </summary>
        public static void Initialize()
        {
#if UNITY_IOS || UNITY_VISIONOS
            if (!isInitialized)
            {
                GCStylus_Initialize();
                isInitialized = true;
            }
#else
            Debug.LogWarning("GCStylus is only available on iOS and visionOS platforms.");
#endif
        }

        /// <summary>
        /// Update stylus data. Call this each frame to get the latest stylus input.
        /// </summary>
        public static void Update()
        {
#if UNITY_IOS || UNITY_VISIONOS
            if (isInitialized)
            {
                GCStylus_Update();
            }
#endif
        }

        /// <summary>
        /// Get all stylus data in a single structure
        /// </summary>
        /// <returns>StylusData structure containing all stylus information</returns>
        public static StylusData GetData()
        {
#if UNITY_IOS || UNITY_VISIONOS
            if (isInitialized)
            {
                return GCStylus_GetData();
            }
#endif
            return default;
        }

        /// <summary>
        /// Check if a stylus is currently available
        /// </summary>
        /// <returns>True if stylus is available</returns>
        public static bool IsAvailable()
        {
#if UNITY_IOS || UNITY_VISIONOS
            if (isInitialized)
            {
                return GCStylus_IsAvailable();
            }
#endif
            return false;
        }

        /// <summary>
        /// Get the X coordinate of the touch position
        /// </summary>
        public static float TouchX
        {
            get
            {
#if UNITY_IOS || UNITY_VISIONOS
                return isInitialized ? GCStylus_GetTouchX() : 0f;
#else
                return 0f;
#endif
            }
        }

        /// <summary>
        /// Get the Y coordinate of the touch position
        /// </summary>
        public static float TouchY
        {
            get
            {
#if UNITY_IOS || UNITY_VISIONOS
                return isInitialized ? GCStylus_GetTouchY() : 0f;
#else
                return 0f;
#endif
            }
        }

        /// <summary>
        /// Get the touch position as a Vector2
        /// </summary>
        public static Vector2 TouchPosition
        {
            get
            {
#if UNITY_IOS || UNITY_VISIONOS
                if (isInitialized)
                {
                    return new Vector2(GCStylus_GetTouchX(), GCStylus_GetTouchY());
                }
#endif
                return Vector2.zero;
            }
        }

        /// <summary>
        /// Get the azimuth angle (rotation around z-axis) in radians
        /// </summary>
        public static float AzimuthAngle
        {
            get
            {
#if UNITY_IOS || UNITY_VISIONOS
                return isInitialized ? GCStylus_GetAzimuthAngle() : 0f;
#else
                return 0f;
#endif
            }
        }

        /// <summary>
        /// Get the altitude angle (tilt from surface) in radians
        /// </summary>
        public static float AltitudeAngle
        {
            get
            {
#if UNITY_IOS || UNITY_VISIONOS
                return isInitialized ? GCStylus_GetAltitudeAngle() : 0f;
#else
                return 0f;
#endif
            }
        }

        /// <summary>
        /// Get the force/pressure applied by the stylus (0.0 to 1.0)
        /// </summary>
        public static float Force
        {
            get
            {
#if UNITY_IOS || UNITY_VISIONOS
                return isInitialized ? GCStylus_GetForce() : 0f;
#else
                return 0f;
#endif
            }
        }

        /// <summary>
        /// Check if the stylus is in range of the screen
        /// </summary>
        public static bool IsInRange
        {
            get
            {
#if UNITY_IOS || UNITY_VISIONOS
                return isInitialized && GCStylus_IsInRange();
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Check if the stylus is currently touching the screen
        /// </summary>
        public static bool IsTouching
        {
            get
            {
#if UNITY_IOS || UNITY_VISIONOS
                return isInitialized && GCStylus_IsTouching();
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Manually set stylus data (for integration with Unity's touch system)
        /// </summary>
        public static void SetData(float touchX, float touchY, float azimuth, float altitude, float force, bool inRange, bool touching)
        {
#if UNITY_IOS || UNITY_VISIONOS
            if (isInitialized)
            {
                GCStylus_SetData(touchX, touchY, azimuth, altitude, force, inRange, touching);
            }
#endif
        }

        /// <summary>
        /// Shutdown and cleanup the GCStylus wrapper
        /// </summary>
        public static void Shutdown()
        {
#if UNITY_IOS || UNITY_VISIONOS
            if (isInitialized)
            {
                GCStylus_Shutdown();
                isInitialized = false;
            }
#endif
        }
    }
}
