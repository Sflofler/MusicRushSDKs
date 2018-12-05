//
//  VdopiaiOSBridge.m
//
//
//  Created by Sachin Patil on 02/08/17.
//
//

#import "ChocolateUnityBridge.h"
#import <ChocolatePlatform_SDK_Core/ChocolatePlatform_SDK_Core.h>

@interface ChocolateListener : NSObject <ChocolatePlatformInterstitialAdDelegate,ChocolatePlatformRewardAdDelegate> {
    NSString *unityListenerName; //class name in unity to send callbacks to
}

-(void)makeUnityCallback:(NSString *)function withMessage:(NSString *)message;
-(NSString *)baseFuncName:(SEL)method;

@end

@implementation ChocolateListener

- (id)initWithUnityListenerName:(NSString *)name {
    if(self = [super init]){
        unityListenerName = name;
        return self;
    }
    return nil;
}

-(void)makeUnityCallback:(NSString *)function withMessage:(NSString *)message {
    UnitySendMessage([unityListenerName cStringUsingEncoding:NSASCIIStringEncoding],
                     [function cStringUsingEncoding:NSASCIIStringEncoding],
                     [message cStringUsingEncoding:NSASCIIStringEncoding]);
}

-(NSString *)baseFuncName:(SEL)method {
    return [NSStringFromSelector(method) componentsSeparatedByString:@":"].firstObject;
}


#pragma mark - Interstitial Ad Delegate

- (void)onInterstitialLoaded:(ChocolatePlatformInterstitialAdDisplay*)interstitialAd {
    [self makeUnityCallback:[self baseFuncName:_cmd] withMessage:@""];
}

- (void)onInterstitialFailed:(ChocolatePlatformInterstitialAdDisplay*)interstitialAd errorCode:(int)errorCode {
//    NSString *methodName = [NSStringFromSelector(_cmd) componentsSeparatedByString:@":"].firstObject;
    [self makeUnityCallback:[self baseFuncName:_cmd] withMessage:[NSString stringWithFormat:@"%d",(int)errorCode]];
}

- (void)onInterstitialShown:(ChocolatePlatformInterstitialAdDisplay*)interstitialAd {
    [self makeUnityCallback:[self baseFuncName:_cmd] withMessage:@""];
}

- (void)onInterstitialClicked:(ChocolatePlatformInterstitialAdDisplay*)interstitialAd {
    [self makeUnityCallback:[self baseFuncName:_cmd] withMessage:@""];
}

- (void)onInterstitialDismissed:(ChocolatePlatformInterstitialAdDisplay*)interstitialAd {
    [self makeUnityCallback:[self baseFuncName:_cmd] withMessage:@""];
}

#pragma mark - Reward Ad Delegate

- (UIViewController*)rewardAdViewControllerForPresentingModalView {
    UIViewController* controller = [UIApplication sharedApplication].keyWindow.rootViewController;
    
    return [self visibleViewController:controller];
}

- (void)rewardedVideoDidLoadAd:(ChocolatePlatformRewardAdDisplay*)rewardedAd {
    [self makeUnityCallback:@"onRewardLoaded" withMessage:@""];
}

- (void)rewardedVideoDidFailToLoadAdWithError:(int)error rewardAdView:(ChocolatePlatformRewardAdDisplay*)rewardedAd {
    [self makeUnityCallback:@"onRewardFailed" withMessage:[NSString stringWithFormat:@"%d",(int)error]];
}

- (void)rewardedVideoDidStartVideo:(ChocolatePlatformRewardAdDisplay*)rewardedAd {
    [self makeUnityCallback:@"onRewardShown" withMessage:@""];
}

- (void)rewardedVideoDidFailToStartVideoWithError:(int)error rewardAdView:(ChocolatePlatformRewardAdDisplay*)rewardedAd {
    [self makeUnityCallback:@"onRewardFailed" withMessage:[NSString stringWithFormat:@"%d",(int)error]];
}

- (void)rewardedVideoWillDismiss:(ChocolatePlatformRewardAdDisplay*)rewardedAd {
    [self makeUnityCallback:@"onRewardDismissed" withMessage:@""];
}

- (void)rewardedVideoDidFinish:(NSUInteger)rewardAmount name:(NSString *)rewardName {
    NSString *rewardInfo = [NSString stringWithFormat:@"%d",(int)rewardAmount];
    rewardInfo = [rewardInfo stringByAppendingString:@","];
    rewardInfo = [rewardInfo stringByAppendingString:rewardName];
    
    [self makeUnityCallback:@"onRewardFinished" withMessage:rewardInfo];
}

# pragma mark - helpers

- (UIViewController*) visibleViewController:(UIViewController *)rootViewController
{
    if (rootViewController.presentedViewController == nil)
    {
        return rootViewController;
    }
    if ([rootViewController.presentedViewController isKindOfClass:[UINavigationController class]])
    {
        UINavigationController *navigationController = (UINavigationController *)rootViewController.presentedViewController;
        UIViewController *lastViewController = [[navigationController viewControllers] lastObject];
        
        return  [self visibleViewController:lastViewController];
    }
    if ([rootViewController.presentedViewController isKindOfClass:[UITabBarController class]])
    {
        UITabBarController *tabBarController = (UITabBarController *)rootViewController.presentedViewController;
        UIViewController *selectedViewController = tabBarController.selectedViewController;
        
        return [self visibleViewController:selectedViewController ];
    }
    
    UIViewController *presentedViewController = (UIViewController *)rootViewController.presentedViewController;
    
    return [self visibleViewController:presentedViewController];
}
    
@end


static ChocolateListener *listener;
static ChocolatePlatformInterstitialAdDisplay *interstitial;
static ChocolatePlatformRewardAdDisplay *reward;

void _initWithAPIKey(char *apiKey) {
    [ChocolatePlatform initWithAdUnitID:[NSString stringWithCString:apiKey encoding:NSASCIIStringEncoding]];
}

void _setupWithListener(char *listenerName) {
    listener = [[ChocolateListener alloc] initWithUnityListenerName:[NSString stringWithCString:listenerName encoding:NSASCIIStringEncoding]];
}

void _loadInterstitialAd(char *adUnitID) {
    interstitial = [[ChocolatePlatformInterstitialAdDisplay alloc]
                    initWithAdUnitID:[NSString stringWithCString:adUnitID encoding:NSASCIIStringEncoding]
                    delegate:listener
                    viewControllerForPresentingModalView:[listener rewardAdViewControllerForPresentingModalView]];
    [interstitial load];
}

void _showInterstitialAd(void) {
    [interstitial show];
}

void _loadRewardAd(char *adUnitID) {
    reward = [[ChocolatePlatformRewardAdDisplay alloc]
              initWithAdUnitID:[NSString stringWithCString:adUnitID encoding:NSASCIIStringEncoding]
              delegate:listener];
    [reward load];
}

void _showRewardAd(int rewardAmount,char* rewardName, char* userId, char* secretKey) {
    ChocolatePlatformRewardAdSettings *set = [ChocolatePlatformRewardAdSettings blankSettings];
    set.rewardName = [NSString stringWithCString:rewardName encoding:NSASCIIStringEncoding];
    set.rewardAmount = rewardAmount;
    set.userID = [NSString stringWithCString:userId encoding:NSASCIIStringEncoding];
    set.secretKey = [NSString stringWithCString:secretKey encoding:NSASCIIStringEncoding];
    [reward show:set];
}

void _setCoppaStatus(_Bool coppa) {
    //[ChocolatePlatform setCoppaStatus:coppa];
    [[ChocolatePlatform privacySettings] setCoppaEnabled:coppa];
}

void _setDemograpics(int age, char* birthDate, char* gender, char* maritalStatus,
                     char* ethnicity) {
    ChocolatePlatformDemographics *dem = [ChocolatePlatform demographics];
    dem.age = age;
    
    NSArray<NSString*> *comps = [[NSString stringWithCString:birthDate encoding:NSASCIIStringEncoding] componentsSeparatedByString:@"-"];
    if(comps.count >= 3){
        NSInteger year = [comps[0] integerValue];
        NSInteger month = [comps[1] integerValue];
        NSInteger day = [comps[2] integerValue];
        if(year != 0 && month != 0 && day != 0){
            [dem setBirthdayYear:year month:month day:day];
        }
    }
    
    NSString *msNS = [NSString stringWithCString:maritalStatus encoding:NSASCIIStringEncoding];
    if([msNS rangeOfString:@"single" options:NSCaseInsensitiveSearch].location != NSNotFound){
        dem.maritalStatus = ChocolatePlatformMaritalStatusSingle;
    }else if([msNS rangeOfString:@"married" options:NSCaseInsensitiveSearch].location != NSNotFound){
        dem.maritalStatus = ChocolatePlatformMaritalStatusMarried;
    }else if([msNS rangeOfString:@"divorced" options:NSCaseInsensitiveSearch].location != NSNotFound){
        dem.maritalStatus = ChocolatePlatformMaritalStatusDivorced;
    }else if([msNS rangeOfString:@"widowed" options:NSCaseInsensitiveSearch].location != NSNotFound){
        dem.maritalStatus = ChocolatePlatformMaritalStatusWidowed;
    }else if([msNS rangeOfString:@"separated" options:NSCaseInsensitiveSearch].location != NSNotFound){
        dem.maritalStatus = ChocolatePlatformMaritalStatusSeparated;
    }else if([msNS rangeOfString:@"other" options:NSCaseInsensitiveSearch].location != NSNotFound){
        dem.maritalStatus = ChocolatePlatformMaritalStatusOther;
    }
    
    NSString *genderString = [NSString stringWithCString:gender encoding:NSASCIIStringEncoding];
    
    NSRange maleRange = [genderString rangeOfString:@"Male" options:NSCaseInsensitiveSearch];
    NSRange feMaleRange = [genderString rangeOfString:@"Female" options:NSCaseInsensitiveSearch];
    
    if (maleRange.location != NSNotFound){
        NSLog(@"Male");
        dem.gender = ChocolatePlatformGenderMale;
    }else if (feMaleRange.location != NSNotFound){
        NSLog(@"Female");
        dem.gender = ChocolatePlatformGenderFemale;
    }else if (feMaleRange.location != NSNotFound){
        NSLog(@"other");
        dem.gender = ChocolatePlatformGenderOther;
    }

    dem.ethnicity = [NSString stringWithCString:ethnicity encoding:NSASCIIStringEncoding];
}

void _setLocation(char* dmaCode, char* postal, char* curPostal, char* latitude, char* longitude) {
    ChocolatePlatformLocation *loc = [ChocolatePlatform location];
    
    if(dmaCode){
        loc.dmacode = [NSString stringWithCString:dmaCode encoding:NSASCIIStringEncoding];
    }
    if(postal){
        loc.postalcode = [NSString stringWithCString:postal encoding:NSASCIIStringEncoding];
    }
    if(curPostal){
        loc.currpostal = [NSString stringWithCString:curPostal encoding:NSASCIIStringEncoding];
    }
    
    if(latitude && longitude){
        NSString *latString = [NSString stringWithCString:latitude encoding:NSASCIIStringEncoding];
        NSString *lonString = [NSString stringWithCString:longitude encoding:NSASCIIStringEncoding];
        CLLocation *LocationAtual = [[CLLocation alloc] initWithLatitude:[latString floatValue] longitude:[lonString floatValue]];
    
    
        loc.location = LocationAtual;
    }
    
}

void _setAppInfo(char* appName, char* pubName,
                 char* appDomain, char* pubDomain,
                 char* storeUrl, char* iabCategory) {
    ChocolatePlatformAppInfo *ai = [ChocolatePlatform appInfo];
    
    if(appName){
        ai.appName = [NSString stringWithCString:appName encoding:NSASCIIStringEncoding];
    }
    if(pubName){
        ai.requester = [NSString stringWithCString:pubName encoding:NSASCIIStringEncoding];
    }
    if(appDomain){
        ai.appDomain = [NSString stringWithCString:appDomain encoding:NSASCIIStringEncoding];
    }
    if(pubDomain){
        ai.publisherdomain = [NSString stringWithCString:pubDomain encoding:NSASCIIStringEncoding];
    }
    if(storeUrl){
        ai.appStoreUrl = [NSString stringWithCString:storeUrl encoding:NSASCIIStringEncoding];
    }
    if(iabCategory){
        ai.Category = [NSString stringWithCString:iabCategory encoding:NSASCIIStringEncoding];
    }
}

void _setPrivacySettings(_Bool gdprApplies, char* gdprConsentString) {
    NSString *consentString = nil;
    if(gdprConsentString){
        consentString = [NSString stringWithCString:gdprConsentString encoding:NSASCIIStringEncoding];
    }
    
    [[ChocolatePlatform privacySettings]
     subjectToGDPR:gdprApplies
     withConsent:consentString];
}
