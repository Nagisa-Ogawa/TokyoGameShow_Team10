using UnityEngine;

[CreateAssetMenu(menuName ="Minion/Param")]
public class Param : ScriptableObject
{
    public float initSpeed = 2f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float neighborDistance = 1f;
    public float neighborFOV = 90f;
    public float separationWeight = 5f;
    public float alignmentWeight = 2f;
    public float bindingWeight = 3f;

}
