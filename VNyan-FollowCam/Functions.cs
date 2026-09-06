using System;
using System.Collections.Generic;
using System.Text;

namespace VNyan_FollowCam {
    internal static class Functions {
        internal static void Log(string Message, int LogLevel = 1) {
            if (LogLevel <= _Settings.GlobalSettings.LogLevel) {
                UnityEngine.Debug.Log($"[FollowCam] {Message}");
            }
        }
    }
}
