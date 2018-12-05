using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public interface ChocolateInterstitialCallbackReceiver {
	void onInterstitialLoaded(string msg);
	void onInterstitialFailed(string msg);
	void onInterstitialShown(string msg);
 	void onInterstitialClicked(string msg);
 	void onInterstitialDismissed(string msg);
}

public interface ChocolateRewardCallbackReceiver {
 	void onRewardLoaded(string msg);
 	void onRewardFailed(string msg);
 	void onRewardShown(string msg);
 	void onRewardFinished(string msg);
	void onRewardDismissed(string msg);
}

public class ChocolateUnityBridge : MonoBehaviour {
	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _initWithAPIKey(string apiKey);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _setupWithListener(string listenerName);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _loadInterstitialAd(string adUnitID);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _showInterstitialAd();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _loadRewardAd(string adUnitID);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _showRewardAd(int rewardAmount, string rewardName, string userId, string secretKey);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _setCoppaStatus(bool coppa);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _setDemograpics(int age, string birthDate, string gender, string maritalStatus,
	                     string ethnicity);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _setLocation(string dmaCode, string postal, string curPostal, string latitude, string longitude);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _setAppInfo(string appName, string pubName,
	                 string appDomain, string pubDomain,
	                 string storeUrl, string iabCategory);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	extern static public void _setPrivacySettings(bool gdprApplies, string gdprConsentString);

	private static bool iOSEnvironment() {
		return (Application.platform == RuntimePlatform.OSXPlayer
            || Application.platform == RuntimePlatform.IPhonePlayer);
	}

	//invokes the Objective-C fuctions only on iOS, not in Unity
	public static void initWithAPIKey(string apiKey) {
		if(iOSEnvironment()) {
			_initWithAPIKey(apiKey);
		}
	}

	public static void setupWithListener(string listenerName) {
		if(iOSEnvironment()) {
			_setupWithListener(listenerName);
		}
	}

	public static void loadInterstitialAd(string adUnitID){
		if(iOSEnvironment()) {
			_loadInterstitialAd(adUnitID);
		}
	}

	public static void showInterstitialAd() {
		if(iOSEnvironment()) {
			_showInterstitialAd();
		}
	}

	public static void loadRewardAd(string adUnitID){
		if(iOSEnvironment()) {
			_loadRewardAd(adUnitID);
		}
	}

	public static void showRewardAd(int rewardAmount,string rewardName, string userId, string secretKey) {
		if(iOSEnvironment()) {
			_showRewardAd(rewardAmount, rewardName, userId, secretKey);
		}
	}

	public static void setCoppaStatus(bool coppa) {
		if(iOSEnvironment()) {
			_setCoppaStatus(coppa);
		}
	}

	public static void setDemograpics(int age, string birthDate, string gender, string maritalStatus,
	                     string ethnicity) {
		if(iOSEnvironment()) {
			 _setDemograpics(age,birthDate,gender,maritalStatus,ethnicity);
		}
	}

	public static void setLocation(string dmaCode, string postal, string curPostal, string latitude, string longitude) {
		if(iOSEnvironment()) {
			_setLocation(dmaCode,postal,curPostal,latitude,longitude);
		}
	}

	public static void setAppInfo(string appName, string pubName,
	                 string appDomain, string pubDomain,
	                 string storeUrl, string iabCategory) {
	  if(iOSEnvironment()) {
			_setAppInfo(appName,pubName,appDomain,pubDomain,storeUrl,iabCategory);
	  }
  }

	public static void setPrivacySettings(bool gdprApplies, string gdprConsentString) {
		if(iOSEnvironment()) {
			_setPrivacySettings(gdprApplies,gdprConsentString);
		}
	}
}
