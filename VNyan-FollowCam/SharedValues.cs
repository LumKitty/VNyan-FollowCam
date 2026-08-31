using System;
using System.Collections.Generic;
using System.Text;

namespace VNyan_FollowCam {
    internal class SharedValues {
        public const long MMFPos_CamPosX = 0;
        public const long MMFPos_CamPosY = 1 * sizeof(float);
        public const long MMFPos_CamPosZ = 2 * sizeof(float);
        public const long MMFPos_CamRotW = 3 * sizeof(float);
        public const long MMFPos_CamRotX = 4 * sizeof(float);
        public const long MMFPos_CamRotY = 5 * sizeof(float);
        public const long MMFPos_CamRotZ = 6 * sizeof(float);
        public const long MMFPos_CamFOV = 7 * sizeof(float);
        public const long MMFPos_Settings = 8 * sizeof(float);
    }
}
