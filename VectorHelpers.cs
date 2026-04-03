using Godot;

namespace ImpishPuppets;

public static class VectorHelpers
{
    public static Vector3 ToVec3pos(this Vector2 vec) => new(vec.X, -vec.Y, 0);
    public static Vector2 ToVec2pos(this Vector3 vec) => new(vec.X, -vec.Y);
    public static Vector2 ToVec2scale(this Vector3 vec) => new(vec.X, vec.Y);
    public static Vector3 ToVec3scale(this Vector2 vec) => new(vec.X, vec.Y, 1);
    public static Vector4 ToVec4(this Rect2 rect) => new(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);
    public static Rect2 Scale(this Rect2 rect, Vector2 vec) => new(rect.Position*vec, rect.Size*vec);
}