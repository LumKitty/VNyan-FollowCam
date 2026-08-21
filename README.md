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
Static: This axis will ignore wherever the bone is and be relative to zero instead (i.e. between your feet). Recommended for the Y axis to reduce shaking

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

### Converting existing cameras to followcam profiles
This plugin includes a feature to quickly copy your current VNyan camera to a FollowCam profile, allowing you to quickly convert your existing setup for use with follow cams.  
**Important: All tracking should be disabled while doing this. Your model should be at 0,0,0 and facing forwards**  
With the follower cam disabled, the copy buttons will copy the main VNyan camera settings to follow cam settings. This will automatically adjust based on your chosen bone, and static/non-static axis  
There are separate copy buttons for both camera position, and camera look-at. For most use cases you will want to click both  
You can then save to a new profile, and easily switch between profiles using ```_lum_followcam_load```  
  
Repeat this process for each VNyan camera you need to convert.
Warning: Ensure you have found Lerp and Min Threshold settings that you like before doing a bulk conversion
