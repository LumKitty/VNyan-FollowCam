using Haukcode.HighResolutionTimer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using static VNyan_FollowCam._Settings;
using static VNyan_FollowCam.Functions;

namespace VNyan_FollowCam {     
    internal class HighResTimer {
        bool KeepRunning = false;
        internal Thread TimerThread;
        HighResolutionTimer Timer = new HighResolutionTimer();

        internal bool IsRunning { get { return KeepRunning; } }
        
        private void TimerThreadCode() {
            Timer.Start();
            while (KeepRunning) {
                Timer.WaitForTrigger();
                FollowCam.UpdateCamera();
            }
            Timer.Stop();
        }
        
        internal HighResTimer(int FPS) {
            Log($"Setting timer to {FPS} ({1000d / FPS})");
            Timer.SetPeriod(1000d / FPS);
            TimerThread = new Thread(new ThreadStart(TimerThreadCode));
            TimerThread.Name = "Timer";
        }

        internal void Start() {
            if (!KeepRunning) {
                FollowCam.PrevTime = Time.realtimeSinceStartupAsDouble;
                KeepRunning = true;
                TimerThread.Start();
            }
        }

        internal void Stop() {
            KeepRunning = false;
            TimerThread = new Thread(new ThreadStart(TimerThreadCode));
            TimerThread.Name = "Timer";
        }

        internal void SetFrequency(int FPS) {
            Timer.SetPeriod(1000d / FPS);
        }

    }
}
