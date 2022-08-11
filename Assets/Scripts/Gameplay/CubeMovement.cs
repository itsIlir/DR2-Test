using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    readonly float speed = 10;

    [SerializeField]
    bool isOwner;

    private static readonly Bounds PlayBounds = new Bounds(Vector3.zero, new Vector3(10, 10));

    private void Update()
    {
        if (!isOwner)
            return;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        var newPos = (Vector2)transform.position + new Vector2(speed * inputX, speed * inputY) * Time.deltaTime;
        PlayerMove(newPos);
    }

    public void PlayerMove(Vector2 newPos)
    {
        newPos = PlayBounds.ClosestPoint(newPos);
        transform.position = newPos;
    }
}
