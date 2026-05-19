using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    [SerializeField] private Transform[] _points;

    public Transform[] Points => _points;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_points == null || _points.Length < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < _points.Length - 1; i++)
        {
            if (_points[i] != null && _points[i + 1] != null)
            {
                Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
            }
        }
    }
#endif
}