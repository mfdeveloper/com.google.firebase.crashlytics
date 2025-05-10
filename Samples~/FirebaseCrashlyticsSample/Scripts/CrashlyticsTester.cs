using UnityEngine;

namespace Firebase.Crashlytics.Samples
{
    public class CrashlyticsTester : MonoBehaviour
    {
        [SerializeField]
        private bool crashOnUpdate = false;

        [SerializeField]
        private FirebaseCrashlyticsManager crashlyticsManager;

        // Update is called once per frame
        private void Update()
        {
            // Tests your Crashlytics implementation by
            // throwing an exception every 60 frames.
            // You should see reports in the Firebase console
            // a few minutes after running your app with this method.
            if (crashOnUpdate && Time.frameCount > 0 && (Time.frameCount%60) == 0)
            {
                if (crashlyticsManager != null)
                {
                    crashlyticsManager.CrashException();
                }
            }
        }
    }
}
