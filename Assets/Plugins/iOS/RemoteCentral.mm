//
//  RemoteCentral.m
//  RemoteCentral
//
//  Created by John Murray on 11/2/13.
//  Copyright (c) 2013 Seebright, Inc. All rights reserved.
//

#import "RemoteCentral.h"
#import "RemoteService.h"

using namespace cv;


@interface RemoteCentral ()

@property (nonatomic, strong) NSMutableArray *array;


@end

static bool dataAvail = false;
static bool remoteSelected = false;
static bool isCapturingCamera = false;
static short distanceShort;

@implementation RemoteCentral

@synthesize videoCamera;

-(void)initOccurred {
    
    // Start OpenCV Capture session
    // Initialize camera feed
    //Debug enable camera feed
    //self.videoCamera = [[CvVideoCamera alloc] initWithParentView:imageView];
    
    
    // Initialize camera feed - use if you do not want cam feed
    self.videoCamera = [[CvVideoCamera alloc] init];
    self.videoCamera.delegate = self;
    
    // Set camera feed to rear camera, camera resoultion to 640x480, and the Video Orientation to Landscape
    self.videoCamera.defaultAVCaptureDevicePosition = AVCaptureDevicePositionBack;
    self.videoCamera.defaultAVCaptureSessionPreset = AVCaptureSessionPresetiFrame960x540;
    self.videoCamera.defaultAVCaptureVideoOrientation = AVCaptureVideoOrientationLandscapeLeft;
    
    
    //Set FPS of camera feed
    self.videoCamera.defaultFPS = 120;
    
    //Lock autofocus of camera
    [self.videoCamera lockFocus];
    [self.videoCamera lockBalance];
    
    y_intercept =236.42;
    slope = -.9055;
    deg_radians = PI/180;
    pixel_degrees = 13.5;
    PI = 3.14159265359;
    minRadius = 20;
    maxRadius = 200;
    cannyup = 80;
    accum= 65;
    
    //For fps checker
//    counter = 0;
//    time(&start);
    
    NSLog(@"initOccurred");
    _manager =
    [[CBCentralManager alloc] initWithDelegate:self queue:nil];
    // And somewhere to store the incoming data
    _data = [[NSMutableData alloc] initWithLength:30];
    _dataChar = (char*)malloc(sizeof(char)*30);
    
    self.array = [[NSMutableArray alloc] init]; // adding it to a strong reference.
}

-(void)centralManagerDidUpdateState:(CBCentralManager*)central {
    NSLog(@"updateState");
    if (central.state != CBCentralManagerStatePoweredOn) {
        // In a real app, you'd deal with all the states correctly
        return;
    }
    
    // The state must be CBCentralManagerStatePoweredOn...
    
    // ... so start scanning
    [self scan];
}

- (void) scan
{
    [_manager scanForPeripheralsWithServices:@[[CBUUID UUIDWithString:REMOTE_SERVICE_UUID]] options:@{ CBCentralManagerScanOptionAllowDuplicatesKey : @YES }];
    NSLog(@"Scanning started");
}

-(void)centralManager:(CBCentralManager*)central
didDiscoverPeripheral:(CBPeripheral*)peripheral
    advertisementData:(NSDictionary*)advertisementData
                 RSSI:(NSNumber*)RSSI {
    // Reject any where the value is above reasonable range
    if (RSSI.integerValue > -15) {
        return;
    }
    // Ok, it's in range - have we already seen it?
    if(![self.array containsObject:peripheral]) {
        NSLog(@"Discovered %@ at %@", peripheral.name, RSSI);
        if (_discoveredPeripheral != peripheral) {
            
            // Save a local copy of the peripheral, so CoreBluetooth doesn't get rid of it
            _discoveredPeripheral = peripheral;
            
            [self.array addObject:_discoveredPeripheral];
            // And connect
            NSLog(@"Connecting to peripheral %@", peripheral);
            [central connectPeripheral:peripheral options:nil];
        }
    }
    //[central stopScan];
    //NSLog(@"Scanning stopped");
    
}

/** If the connection fails for whatever reason, we need to deal with it.
 */
- (void)centralManager:(CBCentralManager *)central didFailToConnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    NSLog(@"Failed to connect to %@. (%@)", peripheral, [error localizedDescription]);
    [self cleanup];
}

/** We've connected to the peripheral, now we need to discover the services and characteristics to find the 'transfer' characteristic.
 */
- (void)centralManager:(CBCentralManager *)central didConnectPeripheral:(CBPeripheral *)peripheral
{
    NSLog(@"Peripheral Connected");
    
    // Clear the data that we may already have
    [_data setLength:0];
    
    // Make sure we get the discovery callbacks
    peripheral.delegate = self;
    // Search only for services that match our UUID
    [peripheral discoverServices:@[[CBUUID UUIDWithString:REMOTE_SERVICE_UUID]]];
}

- (void)centralManager:(CBCentralManager *)central didDisconnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error{
    NSLog(@"Disconnected!!! Error");
    if(peripheral != _discoveredPeripheral) {
        [self cleanupOthers];
    } else
    {
        [central connectPeripheral:_discoveredPeripheral options:nil];
    }
}

/** The Transfer Service was discovered
 */
- (void)peripheral:(CBPeripheral *)peripheral didDiscoverServices:(NSError *)error
{
    NSLog(@"Input Service Discovered");
    if (error) {
        NSLog(@"Error discovering services: %@", [error localizedDescription]);
        [self cleanup];
        return;
    }
    
    // Discover the characteristic we want...
    
    // Loop through the newly filled peripheral.services array, just in case there's more than one.
    for (CBService *service in peripheral.services) {
        
        [peripheral discoverCharacteristics:@[[CBUUID UUIDWithString:INPUT_CHARACTERISTIC_UUID]] forService:service];
    }
}

/** The Transfer characteristic was discovered.
 *  Once this has been found, we want to subscribe to it, which lets the peripheral know we want the data it contains
 */
- (void)peripheral:(CBPeripheral *)peripheral didDiscoverCharacteristicsForService:(CBService *)service error:(NSError *)error
{
    // Deal with errors (if any)
    if (error) {
        NSLog(@"Error discovering characteristics: %@", [error localizedDescription]);
        [self cleanup];
        return;
    }
    
    // Again, we loop through the array, just in case.
    for (CBCharacteristic *characteristic in service.characteristics) {
        
        // And check if it's the right one
        if ([characteristic.UUID isEqual:[CBUUID UUIDWithString:INPUT_CHARACTERISTIC_UUID]]) {
            NSLog(@"Subscribing to characteristic...");
            
            // If it is, subscribe to it
            [peripheral setNotifyValue:YES forCharacteristic:characteristic];
        }
    }
    
    // Once this is complete, we just need to wait for the data to come in.
}

/** This callback lets us know more data has arrived via notification on the characteristic
 */
- (void)peripheral:(CBPeripheral *)peripheral didUpdateValueForCharacteristic:(CBCharacteristic *)characteristic error:(NSError *)error
{
    if (error) {
        NSLog(@"Error discovering characteristics: %@", [error localizedDescription]);
        return;
    }
    dataAvail=true;
    // Otherwise, just add the data on to what we already have
    [_data setData:characteristic.value];
    /*
     NSRange joy_x_rng = {0,sizeof(short)};
     NSRange joy_y_rng = {2,sizeof(short)};
     
     NSRange quat_x_rng = {4,sizeof(short)};
     NSRange quat_y_rng = {6,sizeof(short)};
     NSRange quat_z_rng = {8,sizeof(short)};
     NSRange quat_w_rng = {10,sizeof(short)};
     NSRange button_select_rng = {12,sizeof(unsigned char)};
     NSRange button_back_rng = {13,sizeof(unsigned char)};
     NSRange button_option_rng = {14,sizeof(unsigned char)};
     NSRange button_up_rng = {15,sizeof(unsigned char)};
     NSRange button_down_rng = {16,sizeof(unsigned char)};
     NSRange button_trigger_rng = {17,sizeof(unsigned char)};
     NSRange button_nav_rng = {18,sizeof(unsigned char)};
     */
    NSRange joy_b1_rng = {0,sizeof(char)};
    NSRange joy_b2_rng = {1,sizeof(char)};
    NSRange joy_b3_rng = {2,sizeof(char)};
    NSRange quat_x_rng = {3,sizeof(float)};
    NSRange quat_y_rng = {7,sizeof(float)};
    NSRange quat_z_rng = {11,sizeof(float)};
    NSRange quat_w_rng = {15,sizeof(float)};
    NSRange buttons_rng = {19,sizeof(unsigned char)};
    char _joy_b1;
    char _joy_b2;
    char _joy_b3;
    char joy_b1_byte[9];
    char joy_b2_byte[9];
    char joy_b3_byte[9];
    char _buttons;
    [_data getBytes:&_joy_b1 range:joy_b1_rng];
    [_data getBytes:&_joy_b2 range:joy_b2_rng];
    [_data getBytes:&_joy_b3 range:joy_b3_rng];
    int _quat_x_int;
    int _quat_y_int;
    int _quat_z_int;
    int _quat_w_int;
    [_data getBytes:&_quat_x_int range:quat_x_rng];
    _quat_x=(float)(_quat_x_int/(2^30));
    [_data getBytes:&_quat_y_int range:quat_y_rng];
    _quat_y=(float)(_quat_y_int/(2^30));
    [_data getBytes:&_quat_z_int range:quat_z_rng];
    _quat_y=(float)(_quat_z_int/(2^30));
    [_data getBytes:&_quat_w_int range:quat_w_rng];
    _quat_w=(float)(_quat_w_int/(2^30));
    [_data getBytes:&_buttons range:buttons_rng];
    _joy_x = 0;
    _joy_y = 0;
    short out_joy_x = 0;
    short out_joy_y = 0;
    out_joy_x = (((short)_joy_b1)&0x00ff)|(((short) (_joy_b2&0xf0)<<4)&0x0f00);
    if(_joy_b2&0x80)
        out_joy_x|=0xf000;
    else
        out_joy_x&=0x0fff;
    out_joy_x*=4;
    out_joy_y = (((short)_joy_b3)&0x00ff)|(((((short)_joy_b2)&0x0f)<<8)&0x0f00);
    if(_joy_b2&0x08)
        out_joy_y|=0xf000;
    else
        out_joy_y&=0x0fff;
    out_joy_y*=4;
    _joy_x = out_joy_x*16;
    _joy_y = out_joy_y*16;
    _bSelect=(_buttons&1);
    _bBack=(_buttons&1<<2)>>2;
    _bOption=(_buttons&1<<3)>>3;
    _bTrigger=(_buttons&1<<4)>>4;
    //_bNav=(_buttons&1<<5)>>5;
    _bDown=(_buttons&1<<6)>>6;
    _bUp=(_buttons&1<<7)>>7;
    char endChar = '\0';
    [_data appendBytes:&_joy_x length:sizeof(short)];
    [_data appendBytes:&_joy_y length:sizeof(short)];
    [_data appendBytes:&_pos_x length:sizeof(short)];
    [_data appendBytes:&_pos_y length:sizeof(short)];
    [_data appendBytes:&distanceShort length:sizeof(short)];
    [_data appendBytes:&endChar length:sizeof(char)];
    [_data getBytes:_dataChar length:30];
    if(_bSelect && _bTrigger)
    {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:@"launch://com.seebright.navigator"]];
    }
    //NSLog(@"%c",*_dataChar);
    // Log it
    //NSLog(@"Received: %i, %i, %i, %i, %i, %i, %@, %@, %@, %@, %@, %@, %@", _joy_x, _joy_y, _quat_x, _quat_y, _quat_z, _quat_w, _bSelect ? @"YES" : @"NO", _bBack ? @"YES" : @"NO", _bOption ? @"YES" : @"NO", _bUp ? @"YES" : @"NO", _bDown ? @"YES" : @"NO", _bTrigger ? @"YES" : @"NO", _bNav ? @"YES" : @"NO");
    if(!remoteSelected && (_bSelect || _bOption || _bBack || _bTrigger)) {
        _discoveredPeripheral=peripheral;
        for(int i = 0; i < [self.array count]; i++) {
            if(self.array[i] != peripheral) {
                [_manager cancelPeripheralConnection:self.array[i]];
                NSLog(@"Not selecting peripheral %@", ((CBPeripheral*)self.array[i]).name);
            }
        }
        NSLog(@"Connected to peripheral! %@", peripheral.name);
        remoteSelected=true;
        [_manager stopScan];
    }
}

/** Call this when things either go wrong, or you're done with the connection.
 *  This cancels any subscriptions if there are any, or straight disconnects if not.
 *  (didUpdateNotificationStateForCharacteristic will cancel the connection if a subscription is involved)
 */
- (void)cleanupOthers
{
    // Don't do anything if we're not connected
    if (!_discoveredPeripheral.state == CBPeripheralStateConnected) {
        return;
    }
    for(int i = 0; i < [self.array count]; i++) {
        CBPeripheral* curPeriph = self.array[i];
        if(curPeriph!=_discoveredPeripheral) {
            // See if we are subscribed to a characteristic on the peripheral
            if (curPeriph.services != nil) {
                for (CBService *service in curPeriph.services) {
                    if (service.characteristics != nil) {
                        for (CBCharacteristic *characteristic in service.characteristics) {
                            if ([characteristic.UUID isEqual:[CBUUID UUIDWithString:INPUT_CHARACTERISTIC_UUID]]) {
                                if (characteristic.isNotifying) {
                                    // It is notifying, so unsubscribe
                                    [curPeriph setNotifyValue:NO forCharacteristic:characteristic];
                                    
                                    // And we're done.
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            
            // If we've got this far, we're connected, but we're not subscribed, so we just disconnect
            //                [_manager cancelPeripheralConnection:curPeriph];
        }
    }
    
    
    //        _discoveredPeripheral = nil;
}


/** Call this when things either go wrong, or you're done with the connection.
 *  This cancels any subscriptions if there are any, or straight disconnects if not.
 *  (didUpdateNotificationStateForCharacteristic will cancel the connection if a subscription is involved)
 */
- (void)cleanup
{
    // Don't do anything if we're not connected
    if (!_discoveredPeripheral.state == CBPeripheralStateConnected) {
        return;
    }
    
    // See if we are subscribed to a characteristic on the peripheral
    if (_discoveredPeripheral.services != nil) {
        for (CBService *service in _discoveredPeripheral.services) {
            if (service.characteristics != nil) {
                for (CBCharacteristic *characteristic in service.characteristics) {
                    if ([characteristic.UUID isEqual:[CBUUID UUIDWithString:INPUT_CHARACTERISTIC_UUID]]) {
                        if (characteristic.isNotifying) {
                            // It is notifying, so unsubscribe
                            [_discoveredPeripheral setNotifyValue:NO forCharacteristic:characteristic];
                            
                            // And we're done.
                            return;
                        }
                    }
                }
            }
        }
    }
    
    // If we've got this far, we're connected, but we're not subscribed, so we just disconnect
    [_manager cancelPeripheralConnection:_discoveredPeripheral];
    
    _discoveredPeripheral = nil;
}

- (void)StartTracking
{
    if (!isCapturingCamera) {
        // Start OpenCV camera feed
        [videoCamera start];
        isCapturingCamera = true;
    }
}

- (void)StopTracking
{
    if (isCapturingCamera) {
        // Stop Capturing camera feed
        [videoCamera stop];
        isCapturingCamera = false;
    }
}


 //CvVideoCameraDelegate Image Process
- (void)processImage:(cv::Mat&)image
{
    Mat bimage,cimage;
    
    //Use bimage for image coprocessing and comparing
    bimage = cimage = image;
    cvtColor(image, image, CV_BGR2GRAY);
    vector<Vec3f> circles;
    
    //Blur/Distort image to allow hough to pick up on the noise of the ball
    blur(image,image,cv::Size(3,9));
    dilate(bimage,bimage,10);
    //Isolate bright colored things
    threshold(image,bimage,245,255,THRESH_TOZERO_INV);
    threshold(bimage,bimage,10,255,THRESH_BINARY);
    bitwise_xor(bimage,image,bimage);
    threshold(bimage,bimage,140,255,THRESH_TOZERO_INV);
    blur(bimage,bimage,cv::Size(5,5));
    
    //Hough circles and highlight circles.
    HoughCircles(bimage, circles, CV_HOUGH_GRADIENT, 1, image.rows/10, cannyup, accum, minRadius, maxRadius);
    for(size_t i = 0; i < circles.size(); i++)
    {
        Point2i center(int(cvRound(circles[i][0])), int(cvRound(circles[i][1])));
        _pos_x = center.x;
        _pos_y = center.y;
        _radius = cvRound(circles[i][2]);
        
        //Compute perpendicular distance of ball, distance if right in front of camera
        //This formula was made using the point slope formula and measuring the perpendicular distance
        //of two points away from the camera and then make an equation to figure out the perpendicular
        //distance of the sphere from its radius.
        perp_distance = (_radius - y_intercept)/(slope);
        
        //Find angle and convert to radians of sphere from the perpendicular/horizontal axis of the camera
        angle = (fabs(270 - center.y)/ pixel_degrees)*deg_radians;
        
        //Distance is equalled to the perp_distance * cosine of the angle found
        distance = perp_distance*cosf(angle);
        
        distanceShort = (short)distance;
        
        //Print distance
        printf("%dmm\n",distance);
        //To print out circle/radius on video
        //circle(cimage, center, 3, Scalar(0,255,0), -1, 8, 0);
        //circle(cimage, center, _radius, Scalar(0,0,255), 3, 8, 0);
    }
    //Display orig image
    image = cimage;
    
    //For fps checker
//    time(&end);
//    ++counter;
//    sec = difftime(end, start);
//    fps = counter / sec;
    //Print fps
    //printf("FPS = %.2f\n", fps);
}

-(void)launchApplication:(NSString*) appID
{
    if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString: appID]]) {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:appID]];
    }
    else
    {
        NSLog(@"Error Launching App");
    }
}


@end

extern "C" {
    
    static RemoteCentral* remoteCentral = nil;
    static char buff[100];
    
    NSString* CreateNSString (const char* string)
    {
        if (string)
            return [NSString stringWithUTF8String: string];
        else
            return [NSString stringWithUTF8String: ""];
    }
    
    
    // When native code plugin is implemented in .mm / .cpp file, then functions
    // should be surrounded with extern "C" block to conform C function naming rules
    
    void _StartService () {
        // Start RemoteCentral service from Unity
        remoteCentral = [[RemoteCentral alloc] init];
        [remoteCentral initOccurred];
    }
    
    BOOL _remoteConnected() {
        return remoteCentral.discoveredPeripheral.state == CBPeripheralStateConnected;
    }
    
    void ConvertString(NSString *str)
    {
        const char *s= [str UTF8String];
        strcpy(buff,s);
        return;
    }
    
    void _getPeriphID() {
        ConvertString((remoteCentral.discoveredPeripheral.identifier.UUIDString));
        UnitySendMessage( "seebrightSDK" , "setRemoteStatus", buff);
    }
    
    void _getPacket(char**dataPtr)
    {
        if(remoteCentral.discoveredPeripheral.state == CBPeripheralStateConnected) {
            char* res = nil;
            if(dataAvail&&remoteCentral.dataChar != NULL)
            {
                *dataPtr=remoteCentral.dataChar;
            }
        }
        return;
    }
    
    void _launchApplication(char* appID)
    {
        NSLog(@"%s", appID);
        NSString *appStringID = [NSString stringWithUTF8String: appID];
        [remoteCentral launchApplication:appStringID];
    }
    
    void _StartTracking()
    {
        [remoteCentral StartTracking];
    }
    
    void _StopTracking()
    {
        [remoteCentral StopTracking];
    }
}