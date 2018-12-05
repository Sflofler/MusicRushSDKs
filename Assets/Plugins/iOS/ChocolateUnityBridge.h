//
//  VdopiaiOSBridge.h
//
//
//  Created by Sachin Patil on 02/08/17.
//
//

#ifdef __cplusplus
extern "C" {
#endif
    
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
    
#ifdef __cplusplus
}
#endif

void _initWithAPIKey(char *apiKey);
void _setupWithListener(char *listenerName);
void _loadInterstitialAd(char *adUnitID);
void _showInterstitialAd(void);
void _loadRewardAd(char *adUnitID);
void _showRewardAd(int rewardAmount,char* rewardName, char* userId, char* secretKey);

void _setCoppaStatus(_Bool coppa);
void _setDemograpics(int age, char* birthDate, char* gender, char* maritalStatus,
                     char* ethnicity);
void _setLocation(char* dmaCode, char* postal, char* curPostal, char* latitude, char* longitude);
void _setAppInfo(char* appName, char* pubName,
                 char* appDomain, char* pubDomain,
                 char* storeUrl, char* iabCategory);
void _setPrivacySettings(_Bool gdprApplies, char* gdprConsentString);

