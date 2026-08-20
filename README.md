### Follow camera for VNyan
Very early beta, here's some rough instructions

Installation: Copy VNyan-FollowCam.dll to your plugin directory

### Usage:
Camera offset: 
Where the camera should be, relative to the bone you selected, e.g. Hips + x/y/z values  
Off = Don't do anything, use main VNyan camera position - This will create a static camera  
Absolute = Camera will follow you around, but ignore your rotation, so will always point in the same in-world direction. Probably use this for Beat Saber  
Relative = Camera will follow you around, taking rotation into consideration. Use this if you always want a front view, or behind view  
Lerp: Increasing this will make the camera move faster in response to movements  
Min threshold: Movements below this value will cause no movement at all. Use this to prevent minor hip wiggles from moving the camera  

Look at bone:
The camera will always look at the selected bone, adjusted by the x/y/z values - A common use would be to look at hips + Z:1m to give
the impression of looking at your head, without moving the camera if you bend over  
Off/Absolute/Relative = Same as for camera offset
Lerp & threshold = Same as for camera offset, but specified in degrees instead of meters  

Examples:
Camera Off + Rotation Absolute will simulate a static camera following your around on stage  
Camera and rotation Relative will simulate a behind view camera like you might get in a 3rd person game  
Camera and rotation Absolute will always have the camera looking forwards, but it can move around to stay the same distance from you

Triggers:  
```_lum_followcam_enable``` - Activate the followcam  
```_lum_followcam_disable``` - Deactivate and revert to VNyan's regular camera  
```_lum_followcam_offsetoff``` - Revert to regular camera only for the position  
```_lum_followcam_offsetabs``` - Switch to absolute mode for calculating the bone offset  
```_lum_followcam_offsetrel``` - Switch to relative move for calculating the bone offset  
```_lum_followcam_rotationoff``` - Revert to regular camera only for the rotation (probably not useful)  
```_lum_followcam_rotationabs``` - Switch to absolute mode for calculating the lookat bone offset  
```_lum_followcam_rotationrel``` - Switch t- relative more for caluclating the lookat bone offset  
```_lum_followcam_load``` - Load in a profile file (full path to be specified on text1)  
