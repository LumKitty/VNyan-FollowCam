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
        internal static bool _MainFollowCamActive = false;
        internal static MemoryMappedViewAccessor? mmfAccess = null;

        internal static void DummyUpdateVRnyanCameraPos(Vector3 CamPos, Quaternion CamRot, ulong Squence, double Timestamp) { }
        internal static bool DummyGetVRNyanControllingCamera() { return false; }
        internal static void DummySetMainFollowCamActive(bool Active) { _MainFollowCamActive = Active; }
        internal static bool DummyGetMainFollowCamActive() { return _MainFollowCamActive; }
        private static System.Reflection.MethodInfo? VRnyan_Get_MMF = null;

        internal static Action<Vector3, Quaternion, ulong, double> UpdateVRnyanCameraPos = DummyUpdateVRnyanCameraPos;
        internal static Func<bool> GetVRNyanControllingCamera = DummyGetVRNyanControllingCamera;
        internal static Action<bool> SetMainFollowCamActive = DummySetMainFollowCamActive;
        internal static Func<bool> GetMainFollowCamActive = DummyGetMainFollowCamActive;

        internal static bool VRNyanControllingCamera { get { return GetVRNyanControllingCamera(); } }
        internal static bool MainFollowCamActive { get { return GetMainFollowCamActive(); } set { SetMainFollowCamActive(value); } }

        public static void UpdateMMF(Vector3 CamPos, Quaternion CamRot, string Source = "Local") {

            if (mmfAccess != null) {
                //Log($"Local UpdateMMF called from {Source}");
                mmfAccess.Write(SharedValues.MMFPos_CamPosX, CamPos.x);
                mmfAccess.Write(SharedValues.MMFPos_CamPosY, CamPos.y);
                mmfAccess.Write(SharedValues.MMFPos_CamPosZ, CamPos.z);
                mmfAccess.Write(SharedValues.MMFPos_CamRotW, CamRot.w);
                mmfAccess.Write(SharedValues.MMFPos_CamRotX, CamRot.x);
                mmfAccess.Write(SharedValues.MMFPos_CamRotY, CamRot.y);
                mmfAccess.Write(SharedValues.MMFPos_CamRotZ, CamRot.z);
                mmfAccess.Write(SharedValues.MMFPos_CamFOV, Camera.main.fieldOfView);
            } else {
                Log($"Local UpdateMMF called from {Source} but it was null - Attempting to connect MMF");
                mmfAccess = (MemoryMappedViewAccessor)VRnyan_Get_MMF?.Invoke(null, null);
                if (mmfAccess != null) {
                    Log("Success! Updating MMF");
                    mmfAccess.Write(SharedValues.MMFPos_CamPosX, CamPos.x);
                    mmfAccess.Write(SharedValues.MMFPos_CamPosY, CamPos.y);
                    mmfAccess.Write(SharedValues.MMFPos_CamPosZ, CamPos.z);
                    mmfAccess.Write(SharedValues.MMFPos_CamRotW, CamRot.w);
                    mmfAccess.Write(SharedValues.MMFPos_CamRotX, CamRot.x);
                    mmfAccess.Write(SharedValues.MMFPos_CamRotY, CamRot.y);
                    mmfAccess.Write(SharedValues.MMFPos_CamRotZ, CamRot.z);
                    mmfAccess.Write(SharedValues.MMFPos_CamFOV, Camera.main.fieldOfView);
                } else {
                    Log("Failed to attach MMF");
                }
            }
        }

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
                        Log("Connecting: GetMMF");
                        VRnyan_Get_MMF = type.GetMethod("Get_MMF", BindingFlags.Static | BindingFlags.Public);

                        if (VRnyan_GetUpdateCameraPos == null || VRnyan_Get_GetVRNyanControllingCamera == null || VRnyan_Get_SetMainFollowCamActive == null) {
                            Log("Couldn't find position methods");
                            Log("VRnyan_GetUpdateCameraPos: " + VRnyan_GetUpdateCameraPos.ToString());
                            Log("VRnyan_Get_VRNyanControllingCamer: " + VRnyan_Get_GetVRNyanControllingCamera.ToString());
                            Log("VRnyan_Set_FollowCamActive: " + VRnyan_Get_SetMainFollowCamActive.ToString());
                            Log("VRnyan_Get_FollowCamActive: " + VRnyan_Get_GetMainFollowCamActive.ToString());
                        } else {
                            Log("Got methods - Connecting up variables&methods");
                            Log("VRnyan_GetUpdateCameraPos");
                            UpdateVRnyanCameraPos = (Action<Vector3, Quaternion, ulong, double>)VRnyan_GetUpdateCameraPos?.Invoke(null, null);
                            
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
