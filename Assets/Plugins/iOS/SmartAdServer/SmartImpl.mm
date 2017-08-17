/////////////////////////////////////////////////////////////////////////////////
//
// SMART ADSERVER iOS SDK — UNITY NATIVE WRAPPER
//
// This wrapper is used to handle the creation of the AdView and the
// communication with the managed wrapper since Unity can't call Objective-C
// code directly.
//
/////////////////////////////////////////////////////////////////////////////////

#import "SASBannerView.h"
#import "SASInterstitialView.h"
#import "SASAdView.h"
#import "SASReward.h"
#import "SASRewardedVideo.h"
#import "SASRewardedVideoDelegate.h"
#import "SASRewardedVideoPlacement.h"

// Maximum number of ad views that can be handled by the native wrapper.
#define MAX_AD_VIEW       100

// Constants for the banner position.
#define POSITION_TOP      0
#define POSITION_BOTTOM   1


/////////////////////////////////////////////////////////////////////////////////
// ObjC class used as delegate.
/////////////////////////////////////////////////////////////////////////////////

@interface SASAdViewDelegateHolder : NSObject <SASAdViewDelegate>

@property (nonatomic, readonly) int adId;

@property (nonatomic) BOOL isLoaded;
@property (nonatomic) BOOL isFailed;

@property (nonatomic) BOOL hasReward;
@property (nonatomic) double rewardAmount;
@property (nonatomic) NSString *rewardCurrency;

@end


@implementation SASAdViewDelegateHolder

- (instancetype)initWithAdId:(int)adId {
  self = [super init];
  if (self) {
    _adId = adId;
    _isLoaded = NO;
    _isFailed = NO;
    _hasReward = NO;
  }
  return self;
}

- (void)adViewDidLoad:(SASAdView *)adView {
  NSLog(@"AdView %d did load", self.adId);
  self.isLoaded = YES;

  // Interstitial are displayed automatically when ready
  if ([adView isKindOfClass:[SASInterstitialView class]]) {
      UIView *currentView = [[[[UIApplication sharedApplication] keyWindow] subviews] lastObject];
      [adView setFrame:CGRectMake(0, 0, CGRectGetWidth(currentView.frame), CGRectGetHeight(currentView.frame))];
      [currentView addSubview:adView];
  }
}

- (void)adView:(SASAdView *)adView didFailToLoadWithError:(NSError *)error {
  NSLog(@"AdView %d did fail to load with error %@", self.adId, error);
  self.isFailed = YES;
}

- (void)adView:(SASAdView *)adView didCollectReward:(SASReward *)reward {
  NSLog(@"AdView %d did collect reward %@", self.adId, reward);
  _hasReward = YES;
  _rewardAmount = [reward.amount doubleValue];
  _rewardCurrency = reward.currency;
}

@end


@interface SASRewardedVideoDelegateStatus : NSObject
@property (nonatomic, assign) BOOL rewardedVideoDidLoadCalled;
@property (nonatomic, assign) BOOL rewardedVideoDidFailToLoad;
@property (nonatomic, assign) BOOL rewardedVideoDidFailToShow;
@property (nonatomic, assign) BOOL rewardedVideoDidCollectReward;
@property (nonatomic, strong) SASReward *reward;
@end

@implementation SASRewardedVideoDelegateStatus
@end

@interface SASRewardedVideoDelegateHolder : NSObject <SASRewardedVideoDelegate>
@property (nonatomic, strong) NSMutableDictionary <NSString *, SASRewardedVideoDelegateStatus *> *status;
- (void)reset;
- (void)resetForPlacement:(SASRewardedVideoPlacement *)placement;
@end

@implementation SASRewardedVideoDelegateHolder
- (instancetype)init {
  if (self = [super init]) {
    self.status = [NSMutableDictionary new];
  }
  return self;
}
- (void)reset {
  self.status = [NSMutableDictionary new];
}
- (void)resetForPlacement:(SASRewardedVideoPlacement *)placement {
  [self.status setObject:[SASRewardedVideoDelegateStatus new] forKey:[self placementToString:placement]];
}
- (SASRewardedVideoDelegateStatus *)statusForPlacement:(SASRewardedVideoPlacement *)placement {
  return [self.status objectForKey:[self placementToString:placement]];
}
- (NSString *)placementToString:(SASRewardedVideoPlacement *)placement {
  return [NSString stringWithFormat:@"%ld-%@-%ld-%@", placement.pageId, placement.pageName, placement.formatId, placement.target];
}
- (void)rewardedVideoDidLoadAd:(SASAd *)ad forPlacement:(SASRewardedVideoPlacement *)placement {
  NSLog(@"RewardedVideo for placement %@ did load", placement);
  [self.status objectForKey:[self placementToString:placement]].rewardedVideoDidLoadCalled = YES;
}
- (void)rewardedVideoDidFailToLoadForPlacement:(SASRewardedVideoPlacement *)placement error:(nullable NSError *)error {
  NSLog(@"RewardedVideo for placement %@ did fail to load with error: %@", placement, error);
  [self.status objectForKey:[self placementToString:placement]].rewardedVideoDidFailToLoad = YES;
}
- (void)rewardedVideoDidFailToShowForPlacement:(SASRewardedVideoPlacement *)placement error:(nullable NSError *)error {
  NSLog(@"RewardedVideo for placement %@ did fail to show with error: %@", placement, error);
  [self.status objectForKey:[self placementToString:placement]].rewardedVideoDidFailToShow = YES;
}
- (void)rewardedVideoForPlacement:(SASRewardedVideoPlacement *)placement didCollectReward:(SASReward *)reward {
  NSLog(@"RewardedVideo for placement %@ did collect reward:\n%@", placement, reward);
  [self.status objectForKey:[self placementToString:placement]].rewardedVideoDidCollectReward = YES;
  [self.status objectForKey:[self placementToString:placement]].reward = reward;
}
@end

/////////////////////////////////////////////////////////////////////////////////
// C functions used to handle ad requests and ad display.
/////////////////////////////////////////////////////////////////////////////////

extern "C" {

  // Utils function to copy string on the heap before returning them to Unity
  char *MakeStringCopy (const char *string)
  {
      if (string == NULL) {
        return NULL;
      }

      char *res = (char *)malloc(strlen(string) + 1);
      strcpy(res, string);
      return res;
  }

  // Tables to hold the ad view instances
  SASAdView *adViews[MAX_AD_VIEW];
  SASAdViewDelegateHolder *delegates[MAX_AD_VIEW];

  int _InitAdView(int type) {

    // Look for an empty slot in the ad view table
    for (int i = 0; i < MAX_AD_VIEW; i++) {
      if (adViews[i] == nil) {
        // Instantiate a new ad view
        if (type == 0) { // type 0 => banner
          adViews[i] = [[SASBannerView alloc] initWithFrame:CGRectMake(0, 0, 0, 0)];
        } else {
          adViews[i] = [[SASInterstitialView alloc] initWithFrame:CGRectMake(0, 0, 0, 0) loader:SASLoaderActivityIndicatorStyleBlack];
        }
        return i;
      }
    }

    return -1;
  }

  void _LoadAd(int adId, char *baseUrl, int siteId, char *pageId, int formatId, int master, char *target) {
    // Converting parameters back to ObjC types
    NSString *baseUrlString = [[NSString alloc] initWithUTF8String:baseUrl];
    NSString *pageIdString = [[NSString alloc] initWithUTF8String:pageId];
    BOOL masterBOOL = (master == 1) ? YES : NO;
    NSString *targetString = [[NSString alloc] initWithUTF8String:target];

    // Configure & load the ad view
    [SASAdView setSiteID:siteId baseURL:baseUrlString];
	  [SASAdView setLoggingEnabled:YES];

    [adViews[adId] loadFormatId:formatId pageId:pageIdString master:masterBOOL target:targetString];

    // Add a delegate
    SASAdViewDelegateHolder *delegate = [[SASAdViewDelegateHolder alloc] initWithAdId:adId];
    delegates[adId] = delegate;
    [adViews[adId] setDelegate:delegate];
    [adViews[adId] setModalParentViewController:[[UIApplication sharedApplication] keyWindow].rootViewController];
  }

  int _CheckForLoadedDelegate(int adId) {
    return delegates[adId].isLoaded ? 1 : 0;
  }

  int _CheckForFailedDelegate(int adId) {
    return delegates[adId].isFailed ? 1 : 0;
  }

  int _CheckForRewardDelegate(int adId) {
    return delegates[adId].hasReward ? 1 : 0;
  }

  const char *_RetrieveRewardCurrency(int adId) {
    return MakeStringCopy([delegates[adId].rewardCurrency UTF8String]);
  }

  double _RetrieveRewardAmount(int adId) {
    return delegates[adId].rewardAmount;
  }

  void _DisplayBanner(int adId, int position) {
    UIView *currentView = [[[[UIApplication sharedApplication] keyWindow] subviews] lastObject];
    CGFloat offset = position == POSITION_TOP ? 0.0 : CGRectGetHeight(currentView.frame) - 50.0;
    [adViews[adId] setFrame:CGRectMake(0, offset, CGRectGetWidth(currentView.frame), 50)];
    [currentView addSubview:adViews[adId]];
  }

  void _ShowAdView(int adId, int show) {
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.1 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
      adViews[adId].hidden = (show == 0) ? YES : NO;
    });
  }

  void _ReleaseAdView(int adId) {
    [adViews[adId] removeFromSuperview];
    [adViews[adId] setDelegate:nil];
    delegates[adId] = nil;
    adViews[adId] = nil;
  }

  SASRewardedVideoPlacement *placementFromID(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    NSString *baseUrlString = [[NSString alloc] initWithUTF8String:baseUrl];
    NSString *pageIdString = [[NSString alloc] initWithUTF8String:pageId];
    NSString *targetString = [[NSString alloc] initWithUTF8String:target];
    [SASAdView setSiteID:siteId baseURL:baseUrlString];
    return [SASRewardedVideoPlacement rewardedVideoPlacementWithPageId:[pageIdString intValue]  formatId:formatId target:targetString];
  }

  static SASRewardedVideoDelegateHolder *rewardedVideoDelegate = nil;

  void _LoadRewardedVideo(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
      rewardedVideoDelegate = [[SASRewardedVideoDelegateHolder alloc] init];
    });
    [SASRewardedVideo setDelegate:rewardedVideoDelegate];

    [rewardedVideoDelegate resetForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)];
    [SASRewardedVideo loadAdForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)];
  }

  void _ShowRewardedVideo(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    [SASRewardedVideo showAdForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target) fromViewController:[[UIApplication sharedApplication] keyWindow].rootViewController];
  }

  int _HasRewardedVideo(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    return [SASRewardedVideo isAdReadyForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)] ? 1 : 0;
  }

  int _CheckRewardedVideoDidLoad(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    SASRewardedVideoDelegateStatus *status = [rewardedVideoDelegate statusForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)];
    return status.rewardedVideoDidLoadCalled ? 1 : 0;
  }

  int _CheckRewardedVideoDidFailToLoad(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    SASRewardedVideoDelegateStatus *status = [rewardedVideoDelegate statusForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)];
    return status.rewardedVideoDidFailToLoad ? 1 : 0;
  }

  int _CheckRewardedVideoDidFailToShow(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    SASRewardedVideoDelegateStatus *status = [rewardedVideoDelegate statusForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)];
    return status.rewardedVideoDidFailToShow ? 1 : 0;
  }

  int _CheckRewardedVideoDidCollectReward(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    SASRewardedVideoDelegateStatus *status = [rewardedVideoDelegate statusForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)];
    return status.rewardedVideoDidCollectReward ? 1 : 0;
  }

  const char *_RetrieveRewardedVideoCurrency(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    SASRewardedVideoDelegateStatus *status = [rewardedVideoDelegate statusForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)];
    return MakeStringCopy([status.reward.currency UTF8String]);
  }

  double _RetrieveRewardedVideoAmount(char *baseUrl, int siteId, char *pageId, int formatId, char *target) {
    SASRewardedVideoDelegateStatus *status = [rewardedVideoDelegate statusForPlacement:placementFromID(baseUrl, siteId, pageId, formatId, target)];
    return [status.reward.amount doubleValue];
  }

}
