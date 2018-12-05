// ADD #IFs LATER
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GameAnalyticsSDK;

namespace Ads
{
    public enum AdType { Interstitial, Rewarded, Banner }
}

public abstract class BaseSDK : IRewardedVideoAdListener, IInterstitialAdListener, IBannerAdListener
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
        Appodeal.cache(Appodeal.BANNER_BOTTOM);
        Appodeal.cache(Appodeal.INTERSTITIAL);
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

    /// <summary>
    /// Disables Appodeal Ad of type.
    /// </summary>
    /// <param name="type"></param>
    public void DisableAppodealAd(Ads.AdType type)
    {
        switch (type)
        {
            case Ads.AdType.Interstitial:
                    Appodeal.hide(Appodeal.INTERSTITIAL);
                break;

            case Ads.AdType.Rewarded:
                    Appodeal.hide(Appodeal.REWARDED_VIDEO);
                break;

            case Ads.AdType.Banner:
                    Appodeal.hide(Appodeal.BANNER_BOTTOM);
                break;

            default:
                throw new System.NotImplementedException();
        }
    }

    #region  Rewarded Video callback handlers
    public void onRewardedVideoLoaded(bool isPrecache) { Debug.Log("Video loaded"); }
    public void onRewardedVideoFailedToLoad() { Debug.Log("Video failed"); }
    public void onRewardedVideoShown() { Debug.Log("Video shown"); }
    public void onRewardedVideoClosed(bool finished) { Debug.Log("Video closed"); }
    public void onRewardedVideoFinished(double amount, string name) { Debug.Log("Reward: " + amount + " " + name); }
    public void onRewardedVideoExpired() { Debug.Log("Video expired"); }
    #endregion

    #region Interstitial callback handlers
    public void onInterstitialLoaded(bool isPrecache) { Debug.Log("Interstitial loaded"); }
    public void onInterstitialFailedToLoad() { Debug.Log("Interstitial failed"); }
    public void onInterstitialShown() { Debug.Log("Interstitial opened"); }
    public void onInterstitialClosed() { Debug.Log("Interstitial closed"); }
    public void onInterstitialClicked() { Debug.Log("Interstitial clicked"); }
    public void onInterstitialExpired() { Debug.Log("Interstitial expired"); }
    #endregion

    #region Banner callback handlers
    public void onBannerLoaded(bool precache) { Debug.Log("banner loaded"); }
    public void onBannerFailedToLoad() { Debug.Log("banner failed"); }
    public void onBannerShown() { Debug.Log("banner opened"); }
    public void onBannerClicked() { Debug.Log("banner clicked"); }
    public void onBannerExpired() { Debug.Log("banner expired"); }
    #endregion
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

    // Sign In to social.
    public abstract void SignIn();
    #endregion

    #region Game Analytics
    /// <summary>
    /// Initializes Game Analytics SDK(use it in Start method).
    /// </summary>
    public void InitializeGameAnalytics()
    {
        GameAnalytics.Initialize();
    }
    #endregion
}

public class AndroidSDK : BaseSDK
{
    /// <summary>
    /// Initializes the SDK Manager.
    /// </summary>
    public override void Initialize()
    {
        InitializeGooglePlayGames();
        SignIn();
        InitializeAppodeal();

        Debug.Log("<color=blue>SDK Manager Initialized</color>");
    }

    /// <summary>
    /// Sign In to Google Play Games.
    /// </summary>
    public override void SignIn()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            if (success)
                Debug.Log("<color=green>Google Play Games: Authenticated.</color>");
            else
                Debug.Log("<color=green>Google Play Games: Failed to authenticate.</color>");
        });
#endif
    }
}

public class IOSSDK : BaseSDK
{
    /// <summary>
    /// Initializes the SDK Manager.
    /// </summary>
    public override void Initialize()
    {
        SignIn();
        InitializeAppodeal();

        Debug.Log("<color=blue>SDK Manager Initialized</color>");
    }

    /// <summary>
    /// Sign In to Game Center.
    /// </summary>
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

    private void Start()
    {
        sdk.InitializeGameAnalytics();
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
