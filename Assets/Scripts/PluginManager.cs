using UnityEngine;
using UnityEngine.UI;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Ads
{
    public enum AdType { Interstitial, Rewarded, Banner }
}

public abstract class BaseSDK : IRewardedVideoAdListener
{
    public abstract void Initialize();

    #region Appodeal
    public string appKey;
    /// <summary>
    /// Initialize Appodeal.
    /// </summary>
    public void InitializeAppodeal()
    {
#if UNITY_ANDROID
        appKey = "9440206048d7aa7ede65d5e38da3fa6ad1955ba6ce03889a";
#elif UNITY_IOS
        appKey = "d69b153104983f02e46a80ff0d7f32216b85a57eb38a11a4";
#endif
        //Appodeal.disableNetwork("startapp");
        //Appodeal.disableNetwork("mobvista");
        Appodeal.setTesting(true); // use it to show test ad
        Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true); // auto caching rewarded video ads
        Appodeal.setAutoCache(Appodeal.INTERSTITIAL, true); // auto caching interstitial ads
        Appodeal.setAutoCache(Appodeal.BANNER_BOTTOM, true); // auto caching banner bottom ads
        Appodeal.initialize(appKey, Appodeal.BANNER | Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO, true);
        Appodeal.setRewardedVideoCallbacks(this);
        Debug.Log("<color=blue>Appodeal initialized for Android.</color>");
    }

    /// <summary>
    /// Runs Appodeal Ad of type.
    /// </summary>
    /// <param name="type"></param>
    public void RunAppodealAd(Ads.AdType type)
    {
        switch (type)
        {
            case Ads.AdType.Interstitial:
                if (Appodeal.isPrecache(Appodeal.INTERSTITIAL) || Appodeal.isLoaded(Appodeal.INTERSTITIAL))
                {
                    Appodeal.show(Appodeal.INTERSTITIAL);
                    Debug.Log("Running Interstitial ad.");
                }
                break;

            case Ads.AdType.Rewarded:
                if (Appodeal.isPrecache(Appodeal.REWARDED_VIDEO) || Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
                {
                    Appodeal.show(Appodeal.REWARDED_VIDEO);
                    Debug.Log("Running Rewarded Video ad.");
                }
                break;
                
            case Ads.AdType.Banner:
                if (Appodeal.isPrecache(Appodeal.BANNER) || Appodeal.isLoaded(Appodeal.BANNER))
                {
                    Appodeal.show(Appodeal.BANNER_BOTTOM);
                    Debug.Log("Running Banner ad.");
                }
                break;

            default:
                throw new System.NotImplementedException();
        }
    }

    public void DisableAppodealAd()
    {
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
    }

    // Rewarded Video callback handlers
    public void onRewardedVideoLoaded(bool isPrecache) { Debug.Log("Video loaded"); }
    public void onRewardedVideoFailedToLoad() { Debug.Log("Video failed"); }
    public void onRewardedVideoShown() { Debug.Log("Video shown"); }
    public void onRewardedVideoClosed(bool finished) { Debug.Log("Video closed"); }
    public void onRewardedVideoFinished(double amount, string name) { Debug.Log("Reward: " + amount + " " + name); }
    public void onRewardedVideoExpired() { Debug.Log("Video expired"); }
    #endregion

    #region Google Play Games
#if UNITY_ANDROID
    // Initialize Google Play Games for Android
    public void InitializeGooglePlayGames()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
#endif

    public abstract void SignIn();
    #endregion
}

public class AndroidSDK : BaseSDK
{
    public override void Initialize()
    {
        InitializeGooglePlayGames();
        SignIn();
        InitializeAppodeal();

        Debug.Log("<color=blue>SDK Manager Initialized</color>");
    }

    public override void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            if (success)
                Debug.Log("<color=green>Google Play Games: Authenticated.</color>");
            else
                Debug.Log("<color=green>Google Play Games: Failed to authenticate.</color>");
        });
    }
}

public class IOSSDK : BaseSDK
{
    public override void Initialize()
    {
        SignIn();
        InitializeAppodeal();

        Debug.Log("<color=blue>SDK Manager Initialized</color>");
    }

    public override void SignIn()
    {
        Social.localUser.Authenticate(success =>
        {
            if (success)
                Debug.Log("Game Center: Authenticated.");
            else
                Debug.Log("Game Center: Failed to authenticate.");
        });
    }
}

public class PluginManager : MonoBehaviour
{
    BaseSDK sdk;
    public bool enableAppodeal;

    private void Awake()
    {
        InitializeSdks();
    }

    public void InitializeSdks()
    {
#if UNITY_ANDROID
        sdk = new AndroidSDK();
#elif UNITY_IOS
        sdk = new IOSSDK();
#endif
        sdk.Initialize();
    }

    public void RunAppodealAd(int type)
    {
        sdk.RunAppodealAd((Ads.AdType)type);
    }
}
