using UnityEngine;

public class CameraTransform
{
    public Vector3 Position { get; private set; }
    public Vector3 Rotation { get; private set; }

    public CameraTransform(Vector3 position, Vector3 rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}