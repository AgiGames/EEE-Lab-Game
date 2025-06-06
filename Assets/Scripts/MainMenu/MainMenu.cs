using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using Unity.XR.CoreUtils;

public class MainMenu : MonoBehaviour
{
    private ARSession arSession;
    private Camera arCamera;
    private XROrigin xrOrigin;

    public void PlayGame5()
    {
        // Load the AR Scene asynchronously and ensure ARCamera persists
        SceneManager.LoadScene(1);
    }

    public void OpenMainMenu()
    {
        RestartApp();
    }

    private void RestartApp()
    {
        Debug.Log("Restarting Game...");

#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            string packageName = currentActivity.Call<string>("getPackageName");
            AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);
            intent.Call<AndroidJavaObject>("addFlags", 0x20000000); // FLAG_ACTIVITY_CLEAR_TOP
            currentActivity.Call("startActivity", intent);
            currentActivity.Call("finish");
            System.Diagnostics.Process.GetCurrentProcess().Kill(); // Kill old instance
        }
#else
        Debug.Log("Application.Quit() in Editor");
        Application.Quit();
#endif
    }
}