using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Sprites;


/// <summary>
/// Serializable representation of a Vector3
/// </summary>
public readonly record struct Vector
{
    /// <summary> The X coordinate </summary>
    public float X { get; }
    /// <summary> The Y coordinate </summary>
    public float Y { get; }
    /// <summary> The Z coordinate </summary>
    public float Z { get; }

    /// <summary>
    /// Creates a new Vector with the specified properties
    /// </summary>
    public Vector(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Formats the vector
    /// </summary>
    public override string ToString() => $"({X}, {Y}, {Z})";

    /// <summary>
    /// (0, 0, 0)
    /// </summary>
    public static Vector Zero => new(0, 0, 0);
    /// <summary>
    /// (1, 1, 1)
    /// </summary>
    public static Vector One => new(1, 1, 1);

    /// <summary>
    /// Converts to a Vector3
    /// </summary>
    public static implicit operator Vector3(Vector v) => new(v.X, v.Y, v.Z);
    /// <summary>
    /// Converts to a SerializeableVector
    /// </summary>
    public static implicit operator Vector(Vector3 v) => new(v.x, v.y, v.z);
}