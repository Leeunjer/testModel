

using UnityEngine;

public static class Constants
{
    public const float Gravity = -9.81f;

    //public static LayerMask GroundLayMast => 1 << LayerMask.NameToLayer("Ground");
    public static LayerMask GroundLayerMask => LayerMask.GetMask("Ground");
}
