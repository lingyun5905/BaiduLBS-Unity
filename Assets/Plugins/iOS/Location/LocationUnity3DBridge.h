//
//  LocationUnity3DBridge.h
//  IphoneMapSdkDemo
//
//  Created by houkai on 2018/1/18.
//  Copyright © 2018年 Baidu. All rights reserved.
//

#ifndef LocationUnity3DBridge_h
#define LocationUnity3DBridge_h

#import <Foundation/Foundation.h>
#import <BMKLocationkit/BMKLocationComponent.h>
#import <BMKLocationkit/BMKLocationAuth.h>

#if defined (__cplusplus)
extern "C" {
#endif
    
    /**
     *	@brief	初始化
     *
     *	@param 	gameObject      U3D游戏对象名
     *  @param  callback        回调函数名
     */
    extern void __LocationUnity3DBridge_init (void *gameObject, void*callback);
    
    /**
     *	@brief	设置AppKey
     *
     *	@param 	appKey          appKey
     */
    extern void __LocationUnity3DBridge_setAppKey (void *appKey);
    
    /**
     *	@brief	请求定位
     */
    extern void __LocationUnity3DBridge_reqLocation();
    
#if defined (__cplusplus)
}
#endif



@interface LocationUnity3DBridge : NSObject<BMKLocationManagerDelegate, BMKLocationAuthDelegate> {
}
@property(nonatomic, copy) NSString* appKey;

-(void) init:(NSString *)gameobject callback:(NSString *)callback;
-(void) reqLocation;

+(instancetype) instance;
@end

#endif /* LocationUnity3DBridge_h */
