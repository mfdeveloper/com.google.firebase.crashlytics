using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Crashlytics;

namespace Firebase.Crashlytics.Samples
{
    /// <summary>
    /// A manager class that initialize <see cref="FirebaseApp"/> and
    /// logs crashes and custom errors on Crashlytics cloud dashboard
    /// </summary>
    /// <remarks>
    /// TODO: <b>[Refactor]</b> Move this manager to a "subpackage" as a monorepo that could be reused
    /// by others Unity projects.
    /// </remarks>
    public partial class FirebaseCrashlyticsManager : MonoBehaviour
    {
        protected FirebaseApp app;

        public bool IsNetworkConnected
        {
            get
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    string errorMessage = "No internet connection: Firebase Console services requires internet";
                    Debug.LogError(errorMessage);
                    return false;
                }

                return true;
            }
        }

        async void Start()
        {
            if (!IsNetworkConnected)
            {
                return;
            }

            await InitializeFirebase();
        }

        public virtual void CrashException() 
        {
            if (!IsNetworkConnected)
            {
                return;
            }

            throw new Exception("Test exception; please ignore");
        }

        public virtual void Log(string message)
        {
            Crashlytics.Log(message);
            CrashException();
        }

        public virtual void Log(string message, params CrashlyticsParameter[] parameters)
        {
            SetParameters(parameters: parameters);
            Crashlytics.Log(message);
        }

        public virtual void Log(Exception exception, params CrashlyticsParameter[] parameters) 
        {
            SetParameters(parameters: parameters);
            Crashlytics.LogException(exception);
        }

        private bool SetParameters(params CrashlyticsParameter[] parameters)
        {
            bool addedParameters = false;
            if (parameters.Length == 0)
            {
                return false;
            }

            foreach (var param in parameters)
            {
                if (string.IsNullOrEmpty(param.Key))
                {
                    Debug.LogError($"The Crashlytics parameter key \"{param.Key}\" should be a valid string!");
                    continue;
                }

                Crashlytics.SetCustomKey(param.Key, param.Value);
                addedParameters = true;
            }

            return addedParameters;
        }

        private async Task InitializeFirebase(Action<Task> onInit = null) 
        {
            await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(previousTask => 
                {
                    var dependencyStatus = previousTask.Result;
                    if (dependencyStatus == DependencyStatus.Available) {
                        // Create and hold a reference to your FirebaseApp,
                        app = FirebaseApp.DefaultInstance;
                        
                        // Set the recommended Crashlytics uncaught exception behavior.
                        Crashlytics.ReportUncaughtExceptionsAsFatal = true;

                        onInit?.Invoke(previousTask);
                    } else {
                        Debug.LogError(
                        $"Could not resolve all Firebase dependencies: \"{dependencyStatus}\"\n" +
                        "Firebase Unity SDK is not safe to use here");
                    }
                }
            );
        }
    }
}
