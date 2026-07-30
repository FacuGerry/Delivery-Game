using UnityEngine;

[CreateAssetMenu(fileName = "KeyBindings", menuName = "GameSettings/KeyBindings")]

public class KeyBindingsSO : ScriptableObject
{
    [Header("Restart Car")]
    public KeyCode restartCar;

    [Header("Pause")]
    // Suggestion: [Baja] - 3 KeyCode separadas para pausa es código repetitivo. Una List<KeyCode> de pausas o un único 'Pause' Action del Input System sería más limpio.
    public KeyCode pause;
    public KeyCode pause2;
    public KeyCode pause3;
}