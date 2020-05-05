using UnityEngine;
using UnityEditor;

[CreateAssetMenu (fileName = "ChainExecutionButton", menuName = "Chain Execution Button")]
public class ChainExecutionButton : ScriptableObject
{
    public char input;
    public Color color;
}