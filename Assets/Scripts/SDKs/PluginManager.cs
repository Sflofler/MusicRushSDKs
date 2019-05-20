// ADD #IFs LATER
using UnityEngine;
#if USE_APPODEAL
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
#endif
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
#if USE_GAMEANALYTICS
using GameAnalyticsSDK;
using System.Collections;
#endif
#if USE_FIREBASE
using Firebase;
#endif
#if USE_CHOCOLATE
using Vdopia; // Chocolate
#endif

namespace Ads
{
    public enum AdType { Interstitial, Rewarded, Banner }
}

public abstract class BaseSDK //: IRewardedVideoAdListener, IInterstitialAdListener, IBannerAdListener 
{
    public abstract void Initialize();

#region Appodeal
#if USE_APPODEAL
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
        //Appodeal.setRewardedVideoCallbacks(this);
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
#endif
#endregion

#region Google Play Games
#if UNITY_ANDROID
    /// <summary>
    /// Initializes Google Play Games SDK.
    /// </summary>
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
#if USE_GAMEANALYTICS
    /// <summary>
    /// Initializes Game Analytics SDK(use it in Start method).
    /// </summary>
    public void InitializeGameAnalytics()
    {
        GameAnalytics.Initialize();
        Debugger.Log("blue", "Game Analytics", "Initialized.");
    }
#endif
#endregion

#region Firebase
#if USE_FIREBASE
    /// <summary>
    /// Initializes Firebase SDK.
    /// </summary>
    public void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if(dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debugger.Log("red", "Firebase", "Initialized.");
            }
            else
            {
                Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }
#endif
#endregion

#region Chocolate
#if USE_CHOCOLATE
    VdopiaPlugin chocoPlugin;
    /// <summary>
    /// Initialize Chocolate Ads SDK.
    /// </summary>
    public void InitializeChocolate()
    {
        chocoPlugin = VdopiaPlugin.GetInstance();

        if (chocoPlugin != null)
        {
            VdopiaListener.GetInstance().VdopiaAdDelegateEventHandler += onVdopiaEventReceiver;
            chocoPlugin.SetAdRequestUserData("23", "23/11/1990", "m", "single", "Asian", "999", "123123", "321321", "", "");
            chocoPlugin.SetAdRequestAppData("UnityDemo", "Chocolate", "unity-demo.com", "chocolateplatform.com", "", "IAB9");

            // 'XqjhRR' is a test key
            chocoPlugin.ChocolateInit("XqjhRR");
            Debugger.Log("green", "Chocolate", "Initialized.");
        }
        else
        {
            Debugger.Log("green", "Chocolate", "failed to initialize.");
        }
	}

#region Callbacks handlers
    //This is your defined ad event receiver; invoked after you
    //call loadInterstitialAd() or loadRewardAd() which are defined below.
    void onVdopiaEventReceiver(string adType, string eventName)     //Ad Event Receiver
    {
        Debug.Log("Ad Event Received : " + eventName + " : For Ad Type : " + adType);

        if (eventName == "INTERSTITIAL_LOADED")
        {
            showInterstitialAd();
        }
        else if (eventName == "INTERSTITIAL_FAILED") { }
        else if (eventName == "INTERSTITIAL_SHOWN") { }
        else if (eventName == "INTERSTITIAL_DISMISSED")
        {
            //New!  Optional.  Silently pre-fetch the next interstitial ad without making
            //any callbacks.  The pre-fetched ad will remain in cache until you call
            //the next LoadInterstitialAd.
            //feel free to use our test app key 'XqjhRR'
            chocoPlugin.PrefetchInterstitialAd("XqjhRR");
        }
        else if (eventName == "INTERSTITIAL_CLICKED") { }
        else if (eventName == "REWARD_AD_LOADED") { showRewardAd(); }
        else if (eventName == "REWARD_AD_FAILED") { }
        else if (eventName == "REWARD_AD_SHOWN") { }
        else if (eventName == "REWARD_AD_SHOWN_ERROR") { }
        else if (eventName == "REWARD_AD_DISMISSED")
        {
            //Pre-fetch: Silently pre-fetch the next reward ad without making 
            //any callbacks. The pre-fetched ad will remain in cache until you call
            //the next LoadRewardAd. 
            //feel free to use our test app key 'XqjhRR'
            chocoPlugin.PrefetchRewardAd("XqjhRR");
        }
        else if (eventName == "REWARD_AD_COMPLETED")
        {
            //If you setup server-to-server (S2S) rewarded callbacks you can
            //assume your server url will get hit at this time.
            //Or you may choose to reward your user from the client here.
        }
    }
#endregion

#region Interstitial Ad Methods
    public void loadInterstitialAd()     //called when btnLoadInterstitial Clicked
    {
        Debug.Log("Load Interstitial...");
        if (Application.platform == RuntimePlatform.Android && chocoPlugin != null)
        {
            //Param 1: AdUnit Id (This is your SSP App ID you received from your account manager or obtained from the portal)
            chocoPlugin.LoadInterstitialAd("XqjhRR");  //feel free to use our test app key 'XqjhRR'
        }
    }

    public void showInterstitialAd()     //called when btnShowInterstitial Clicked
    {
        Debug.Log("Show Interstitial...");
        if (Application.platform == RuntimePlatform.Android && chocoPlugin != null)
        {
            //Make sure Interstitial Ad is loaded before call this method
            chocoPlugin.ShowInterstitialAd();
        }
    }
#endregion

#region Rewarded Video Ad Methods
    public void requestRewardAd()       //called when btnRequestReward Clicked
    {
        Debug.Log("Request Reward...");
        if (Application.platform == RuntimePlatform.Android && chocoPlugin != null)
        {
            //Param 1: AdUnit Id (This is your SSP App ID you received from your account manager or obtained from the portal)
            chocoPlugin.RequestRewardAd("XqjhRR");  //feel free to use our test app key 'XqjhRR'
        }
    }

    public void showRewardAd()           //called when btnShowReward Clicked
    {
        Debug.Log("Show Reward...");
        //Make sure Ad is loaded before call this method
        if (Application.platform == RuntimePlatform.Android && chocoPlugin != null)
        {
            //Parma 1: Secret Key (Get it from Vdopia Portal: Required if server-to-server callback enabled)
            //Parma 2: User name – this is the user ID of your user account system
            //Param 3: Reward Currency Name or Measure
            //Param 4: Reward Amount
            chocoPlugin.ShowRewardAd("qj5ebyZ0F0vzW6yg", "Chocolate1", "coin", "30");
        }
    }

    public void checkRewardAdAvailable()
    {
        Debug.Log("Check Reward Ad Available...");
        if (Application.platform == RuntimePlatform.Android && chocoPlugin != null)
        {
            bool isReady = chocoPlugin.IsRewardAdAvailableToShow();
            Debug.Log("Check Reward Ad Available..." + isReady);
        }
    }
#endregion
#endif
#endregion

#region DevToDev
#if USE_DEVTODEV
    /// <summary>
    /// Initialized DevToDev SDK.
    /// </summary>
    public void InitializeDevToDev()
    {
#if UNITY_ANDROID
        DevToDev.Analytics.Initialize("", ""); // edit later
        Debugger.Log("blue", "DevToDev", "Initialized.");
#endif
    }
#endif
#endregion
}

public class AndroidSDK : BaseSDK
{
    /// <summary>
    /// Initializes the SDK Manager.
    /// </summary>
    public override void Initialize()
    {
#if UNITY_ANDROID
        InitializeGooglePlayGames();
        SignIn();
#if USE_APPODEAL
        InitializeAppodeal();
#endif
#if USE_GAMEANALYTICS
        InitializeGameAnalytics();
#endif
#if USE_FIREBASE
        InitializeFirebase();
#endif
#if USE_CHOCOLATE
        InitializeChocolate();
#endif
#if USE_USE_DEVTODEV
        InitializeDevToDev();
#endif

        Debug.Log("<color=blue>SDK Manager Initialized</color>");
#endif
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
#if UNITY_IOS
        SignIn();
#if USE_APPODEAL
        InitializeAppodeal();
#endif
#if USE_GAMEANALYTICS
        InitializeGameAnalytics();
#endif
#if USE_FIREBASE
        InitializeFirebase();
#endif
#if USE_CHOCOLATE
        InitializeChocolate();
#endif
#if USE_DEVTODEV
        InitializeDevToDev();
#endif

        Debug.Log("<color=blue>SDK Manager Initialized</color>");
#endif
    }

    /// <summary>
    /// Sign In to Game Center.
    /// </summary>
    public override void SignIn()
    {
#if UNITY_IOS
        Social.localUser.Authenticate(success =>
        {
            if (success)
                Debug.Log("Game Center: Authenticated.");
            else
                Debug.Log("Game Center: Failed to authenticate.");
        });
#endif
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
        StartCoroutine(StartFirebaseCallBacks(3f));
        //sdk.InitializeGameAnalytics();
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

    #region Firebase Cloud Message CallBacks
    /// <summary>
    /// Start Firebase Cloud Messaging Callbacks.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator StartFirebaseCallBacks(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
    #endregion

    public void RunAppodealAd(int type)
    {
#if USE_APPODEAL
        sdk.RunAppodealAd((Ads.AdType)type);
#endif
    }
}
