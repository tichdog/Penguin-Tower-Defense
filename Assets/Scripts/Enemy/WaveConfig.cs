using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Wave/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] private Wave[] _waves;

    public Wave[] Waves => _waves;
}