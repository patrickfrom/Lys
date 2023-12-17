using ImGuiNET;
using OpenTK.Mathematics;

namespace Lys.Utils;

public static class ImGuiExtensions
{
    public static bool DragFloat3(string label, ref Vector3 value)
    {
        var sysVector = new System.Numerics.Vector3(value.X, value.Y, value.Z);
        var result = ImGui.DragFloat3(label, ref sysVector);
        value = new Vector3(sysVector.X, sysVector.Y, sysVector.Z);
        return result;
    }
    
    public static bool DragFloat3(string label, ref Vector3 value, float speed = 1.0f)
    {
        var sysVector = new System.Numerics.Vector3(value.X, value.Y, value.Z);
        var result = ImGui.DragFloat3(label, ref sysVector, speed);
        value = new Vector3(sysVector.X, sysVector.Y, sysVector.Z);
        return result;
    }

    public static bool ColorEdit3(string label, ref Vector3 value)
    {
        var sysVector = new System.Numerics.Vector3(value.X, value.Y, value.Z);
        var result = ImGui.ColorEdit3(label, ref sysVector);
        value = new Vector3(sysVector.X, sysVector.Y, sysVector.Z);
        return result;
    }
}