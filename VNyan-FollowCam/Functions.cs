using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using UnityEngine.UIElements;
using VNyanInterface;

namespace VNyan_FollowCam {
    internal static class Functions {
        internal static void Log(string Message, int LogLevel = 1) {
            if (LogLevel <= _Settings.GlobalSettings.LogLevel) {
                UnityEngine.Debug.Log($"[FollowCam] {Message}");
            }
        }
        internal static VNyanVector3 FromVector3(UnityEngine.Vector3 Position) {
            VNyanVector3 Result = new VNyanVector3();
            Result.X = Position.x;
            Result.Y = Position.y;
            Result.Z = Position.z;
            return Result;
        }
        internal static VNyanQuaternion FromQuaternion(UnityEngine.Quaternion Rotation) {
            VNyanQuaternion Result = new VNyanQuaternion();
            Result.W = Rotation.w;
            Result.X = Rotation.x;
            Result.Y = Rotation.y;
            Result.Z = Rotation.z;
            return Result;
        }
            
    }
}
