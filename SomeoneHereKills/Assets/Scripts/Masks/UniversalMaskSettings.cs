using UnityEngine;

[CreateAssetMenu(fileName = "MaskSettings", menuName = "Mask/MaskSettings")]
public class UniversalMaskSettings : ScriptableObject
{
    [Header("Scale")]
    public float maskScale = 0.05f;

    [Header("Offsets")]
    public Vector3 baseOffset = Vector3.zero;
    public Vector3 cheeksOffset = new Vector3(0, 0, -0.01f);
    public Vector3 eyesOffset = new Vector3(0, 0, -0.02f);
    public Vector3 hornsOffset = new Vector3(0, 0.2f, -0.03f);
}
