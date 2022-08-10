using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    NetworkManager networkManager;
    float speed = 10;

    [SerializeField]
    bool isOwner;

    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    private void Update()
    {
        if (!IsInsideBounderies() || !isOwner)
            return;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        var newPos = new Vector2(speed * inputX, speed *inputY) * Time.deltaTime;
        //networkManager.OnPlayerMove(newPos);
        PlayerMove(newPos);
    }

    public void PlayerMove(Vector2 newPos)
    {
        transform.Translate(newPos);
    }

    private bool IsInsideBounderies()
    {
        bool state = true;
        if (transform.position.x > 20)
        {
            transform.position = new Vector3(19.9f, transform.position.y, transform.position.z);
            state = false;
        }
        if (transform.position.x < -10)
        {
            transform.position = new Vector3(-9.9f, transform.position.y, transform.position.z);
            state = false;
        }
        if (transform.position.y > 10)
        {
            transform.position = new Vector3(transform.position.x, 9.9f, transform.position.z);
            state = false;
        }
        if (transform.position.y < -10)
        {
            transform.position = new Vector3(transform.position.x, -9.9f, transform.position.z);
            state = false;
        }
        return state;
    }
}
