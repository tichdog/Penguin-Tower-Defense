using UnityEngine;

public class GearRotate : MonoBehaviour
{
    [SerializeField] private float speed = 180f;

    private bool rotate;

    public void SetRotate(bool value)
    {
        rotate = value;
    }

    private void Update()
    {
        if (!rotate) return;

        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}
