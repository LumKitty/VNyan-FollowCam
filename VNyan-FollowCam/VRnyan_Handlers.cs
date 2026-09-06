using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Text;
using UnityEngine;
using static VNyan_FollowCam.Functions;
using static VNyan_FollowCam.VRnyan_Handlers;

namespace VNyan_FollowCam {
    internal class VRnyan_Handlers {
        internal static int VRnyanConnectionAttempts = 0;
        internal static bool VRnyanConnectionSucceeded = false;

        //internal static bool VRNyanControllingCamera = false;
        //internal static bool MainFollowCamActive = false;
        internal static bool _MainFollowCamActive = false;

        internal static void DummyUpdateVRnyanCameraPos(Vector3 CamPos, Quaternion CamRot) { }
        internal static bool DummyGetVRNyanControllingCamera() { return false; }
        internal static void DummySetMainFollowCamActive(bool Active) { _MainFollowCamActive = Active; }
        internal static bool DummyGetMainFollowCamActive() { return _MainFollowCamActive; }

        internal static Action<Vector3, Quaternion> UpdateVRnyanCameraPos = DummyUpdateVRnyanCameraPos;
        internal static Func<bool> GetVRNyanControllingCamera = DummyGetVRNyanControllingCamera;
        internal static Action<bool> SetMainFollowCamActive = DummySetMainFollowCamActive;
        internal static Func<bool> GetMainFollowCamActive = DummyGetMainFollowCamActive;

        internal static bool VRNyanControllingCamera { get { return GetVRNyanControllingCamera(); } }
        internal static bool MainFollowCamActive { get { return GetMainFollowCamActive(); } set { SetMainFollowCamActive(value); } }

        internal static void ConnectVRnyan() {
            try {
                if (!VRnyanConnectionSucceeded && VRnyanConnectionAttempts < 5) {
                    VRnyanConnectionAttempts++;
                    Log("Looking for VRnyan");
                    var type = Type.GetType("VRnyan.FollowCam_Handlers, VRnyan", throwOnError: false);
                    if (type != null) {
                        Log("Found VRnyan, getting methods");
                        Log("Connecting: GetUpdateCameraPos");
                        System.Reflection.MethodInfo? VRnyan_GetUpdateCameraPos = type.GetMethod("Get_UpdateCameraPos", BindingFlags.Static | BindingFlags.Public);
                        Log("Connecting: GetVRNyanControllingCamera");
                        System.Reflection.MethodInfo? VRnyan_Get_GetVRNyanControllingCamera = type.GetMethod("Get_GetVRNyanControllingCamera", BindingFlags.Static | BindingFlags.Public);
                        Log("Connecting: SetMainFollowCamActive");
                        System.Reflection.MethodInfo? VRnyan_Get_SetMainFollowCamActive = type.GetMethod("Get_SetMainFollowCamActive", BindingFlags.Static | BindingFlags.Public);
                        Log("Connecting: GetMainFollowCamActive");
                        System.Reflection.MethodInfo? VRnyan_Get_GetMainFollowCamActive = type.GetMethod("Get_GetMainFollowCamActive", BindingFlags.Static | BindingFlags.Public);

                        if (VRnyan_GetUpdateCameraPos == null || VRnyan_Get_GetVRNyanControllingCamera == null || VRnyan_Get_SetMainFollowCamActive == null) {
                            Log("Couldn't find position methods");
                            Log("VRnyan_GetUpdateCameraPos: " + VRnyan_GetUpdateCameraPos.ToString());
                            Log("VRnyan_Get_VRNyanControllingCamer: " + VRnyan_Get_GetVRNyanControllingCamera.ToString());
                            Log("VRnyan_Set_FollowCamActive: " + VRnyan_Get_SetMainFollowCamActive.ToString());
                            Log("VRnyan_Get_FollowCamActive: " + VRnyan_Get_GetMainFollowCamActive.ToString());
                        } else {
                            Log("Got methods - Connecting up variables&methods");
                            Log("VRnyan_GetUpdateCameraPos");
                            UpdateVRnyanCameraPos = (Action<Vector3, Quaternion>)VRnyan_GetUpdateCameraPos?.Invoke(null, null);
                            
                            Log("VRnyan_Get_VRNyanControllingCamera");
                            GetVRNyanControllingCamera =             (Func<bool>)VRnyan_Get_GetVRNyanControllingCamera?.Invoke(null, null);

                            Log("VRnyan_Set_MainFollowCamActive");
                            SetMainFollowCamActive =               (Action<bool>)VRnyan_Get_SetMainFollowCamActive?.Invoke(null, null);

                            Log("VRnyan_Get_MainFollowCamActive");
                            GetMainFollowCamActive =                 (Func<bool>)VRnyan_Get_GetMainFollowCamActive?.Invoke(null, null);

                            VRnyanConnectionSucceeded = true;
                        }
                    } else {
                        Log("Did not find VRnyan assembly");
                    }
                }
            } catch (Exception ex) {
                Log("ERR: " + ex.ToString());
            }
        }
    }
}
