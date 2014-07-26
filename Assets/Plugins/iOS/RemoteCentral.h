//
//  RemoteCentral.h
//  RemoteCentral
//
//  Created by John Murray on 11/2/13.
//  Copyright (c) 2013 Seebright, Inc. All rights reserved.
//

#import <opencv2/opencv.hpp>

#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>
#include <opencv2/highgui/cap_ios.h>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
//#include <time.h>


@interface RemoteCentral : NSObject <CBCentralManagerDelegate, CBPeripheralDelegate, CvVideoCameraDelegate>
{
    // OpenCV Video Camera
    CvVideoCamera* videoCamera;
    
    short _pos_x;//_pos_x is the x position of circle found
    short _pos_y;//_pos_y is the y position of circle found
    short _radius;//_radius is the radius of circle found
    int distance,//distance from sphere away from camera after calculations
    //These are used for Hough Circles
    minRadius,//minimum radius of circles to be found
    maxRadius,//maximum radius of circles to be found
    cannyup,//canny edge upper threshold value... lower 1/2 of this
    accum;//accumulator value... larger finds less circles, smaller finds more
    
    float perp_distance,//Raw distance of sphere from camera without calculations
    y_intercept,//Y-intercept of the equation for converting radius into perpendicular distance
    slope,//Slope of the equation for converting radius into perpendicular distance
    angle,//Angle of sphere away from horizontal axis of phone
    pixel_degrees,//Conversion of pixels into degrees for calculation
    deg_radians,//Conversion from degrees to radians
    PI;
    
    //for fps checker
    short counter;
    time_t start, end;
    double fps, sec;
}

-(void)initOccurred;

-(void)StartTracking;
-(void)StopTracking;

-(void)launchApplication:(NSString*) appID;

// Bluetooth Properties
@property (nonatomic, strong) CBPeripheral  *discoveredPeripheral;
@property (nonatomic, strong) NSMutableData *data;
@property (nonatomic, strong) CBCentralManager *manager;

// Remote Joystick Properties
@property ( atomic, readwrite ) short joy_x;
@property ( atomic, readwrite ) short joy_y;

// Remote Button Properties
@property ( atomic, readwrite ) bool bSelect;
@property ( atomic, readwrite ) bool bBack;
@property ( atomic, readwrite ) bool bOption;
@property ( atomic, readwrite ) bool bUp;
@property ( atomic, readwrite ) bool bDown;
@property ( atomic, readwrite ) bool bTrigger;
@property ( atomic, readwrite ) bool bNav;

// Remote Quaternian Properties
@property ( atomic, readwrite ) short quat_x;
@property ( atomic, readwrite ) short quat_y;
@property ( atomic, readwrite ) short quat_z;
@property ( atomic, readwrite ) short quat_w;

// Remote Position
@property ( atomic, readwrite ) short pos_x;
@property ( atomic, readwrite ) short pos_y;
@property ( atomic, readwrite ) short radius;

// Remote data output
@property ( atomic, readwrite ) char* dataChar;

// OpenCV Camera input
@property (nonatomic, retain) CvVideoCamera* videoCamera;

@end
