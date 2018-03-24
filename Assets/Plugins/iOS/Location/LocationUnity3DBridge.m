//
//  LocationUnity3DBridge.m
//  IphoneMapSdkDemo
//
//  Created by houkai on 2018/1/18.
//  Copyright © 2018年 Baidu. All rights reserved.
//

#import "LocationUnity3DBridge.h"


#if defined (__cplusplus)
extern "C" {
#endif
    
    void __LocationUnity3DBridge_init (void *gameObject, void*callback) {
        NSString *nsGO = [NSString stringWithCString:gameObject encoding:NSUTF8StringEncoding];
        NSString *nsCB = [NSString stringWithCString:callback encoding:NSUTF8StringEncoding];
        NSLog(@"gameObject:%@  callback:%@", nsGO, nsCB);
        [[LocationUnity3DBridge instance] init:nsGO callback:nsCB];
        
    }
    
    void __LocationUnity3DBridge_setAppKey (void *appKey) {
        NSString *nsAppID = [NSString stringWithCString:appKey encoding:NSUTF8StringEncoding];
        NSLog(@"LocationAppkey :%@", nsAppID);
        [LocationUnity3DBridge instance].appKey = nsAppID;
    }
    
    void __LocationUnity3DBridge_reqLocation () {
        [[LocationUnity3DBridge instance] reqLocation];
    }
    
    
#if defined (__cplusplus)
}
#endif

@interface LocationUnity3DBridge(){
@private
    NSString* _appKey;
    NSString* _gameObject;
    NSString* _callback;
}

@property(nonatomic, strong) BMKLocationManager *locationManager;
@property(nonatomic, copy) BMKLocatingCompletionBlock completionBlock;
@end


@implementation LocationUnity3DBridge

@synthesize appKey = _appKey;

- (void)setAppKey:(NSString *)appKey{
    _appKey = appKey;
    [[BMKLocationAuth sharedInstance] checkPermisionWithKey:_appKey authDelegate:self];
}

+(instancetype) instance {
    static dispatch_once_t once;
    static LocationUnity3DBridge *instance;
    dispatch_once(&once, ^{
        instance = [[self.class alloc] init];
        [instance initLocation];
        [instance initBlock];
    });
    return instance;
}

-(void) init:(NSString *)gameobject callback:(NSString *)callback {
    self->_gameObject = gameobject;
    self->_callback = callback;
}

-(void) reqLocation {
    [_locationManager requestLocationWithReGeocode:YES withNetworkState:YES completionBlock:self.completionBlock];
}

- (void)dealloc {
    _locationManager = nil;
    _completionBlock = nil;
}

-(void) initLocation
{
    _locationManager = [[BMKLocationManager alloc] init];
    _locationManager.delegate = self;
    _locationManager.coordinateType = BMKLocationCoordinateTypeGCJ02;
    _locationManager.distanceFilter = kCLDistanceFilterNone;
    _locationManager.desiredAccuracy = kCLLocationAccuracyBest;
    _locationManager.activityType = CLActivityTypeAutomotiveNavigation;
    _locationManager.pausesLocationUpdatesAutomatically = NO;
    _locationManager.allowsBackgroundLocationUpdates = YES;
    _locationManager.locationTimeout = 10;
    _locationManager.reGeocodeTimeout = 10;
}

-(void) initBlock
{
    __weak LocationUnity3DBridge *weakSelf = self;
    self.completionBlock = ^(BMKLocation *location, BMKLocationNetworkState state, NSError *error)
    {
        if (error)
        {
            NSLog(@"locError:{%ld - %@};", (long)error.code, error.localizedDescription);
        }

        NSString * coorType = nil;
        LocationUnity3DBridge *strongSelf = weakSelf;
        BMKLocationCoordinateType type = strongSelf.locationManager.coordinateType;
        switch (type) {
            case BMKLocationCoordinateTypeBMK09LL:
                coorType = @"bd09ll";
                break;
            case BMKLocationCoordinateTypeBMK09MC:
                coorType = @"bd09";
                break;
            case BMKLocationCoordinateTypeWGS84:
                coorType = @"wgs84";
                break;
            case BMKLocationCoordinateTypeGCJ02:
                coorType = @"gcj02";
                break;
        }

        NSMutableDictionary *dict = [NSMutableDictionary dictionaryWithCapacity:3];
        [dict setValue:coorType forKey:@"coorType"];
        [dict setValue:@(location.location.coordinate.latitude) forKey:@"latitude"];
        [dict setValue:@(location.location.coordinate.longitude) forKey:@"longitude"];
        [dict setValue:@(location.location.altitude) forKey:@"altitude"];
        [dict setValue:@(location.location.horizontalAccuracy) forKey:@"radius"];
        if (location.rgcData != nil) {
            [dict setValue:location.rgcData.countryCode forKey:@"countryCode"];
            [dict setValue:location.rgcData.country forKey:@"country"];
            [dict setValue:location.rgcData.province forKey:@"province"];
            [dict setValue:location.rgcData.cityCode forKey:@"cityCode"];
            [dict setValue:location.rgcData.city forKey:@"city"];
            [dict setValue:location.rgcData.adCode forKey:@"adCode"];
            [dict setValue:location.rgcData.district forKey:@"district"];
            [dict setValue:location.rgcData.street forKey:@"street"];
            [dict setValue:location.rgcData.streetNumber forKey:@"streetNumber"];
            [dict setValue:location.rgcData.locationDescribe forKey:@"locationDescribe"];
        }

        BOOL isInchina = [BMKLocationManager BMKLocationDataAvailableForCoordinate:location.location.coordinate withCoorType:type];
        [dict setValue:@(isInchina) forKey:@"isInChina"];

        NSError *jsonError = nil;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:NSJSONWritingPrettyPrinted error:&jsonError];
        if (jsonError != nil || jsonData == nil) {
            NSLog(@"Location json parse error!");
            return;
        }
        NSString *jsonStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        NSLog(@"netstate = %@",jsonStr);
        NSLog(@"netstate = %d",state);
        UnitySendMessage([strongSelf->_gameObject UTF8String], [strongSelf->_callback UTF8String], [jsonStr UTF8String]);
    };
}

- (void)onCheckPermissionState:(BMKLocationAuthErrorCode)iError
{
    NSLog(@"location auth onGetPermissionState %ld",(long)iError);
}

@end
