using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlayerController : MonoBehaviour
{

    public Vector3 wallNormal;
    GlobalPlayerController globalPlayerController;

    public List<GameObject> wallsCollidingWith;

    bool canAct = true;

    public float initialJumpForce = 17.5f;
    public float wallRunInitialJumpForce = 12.5f;
    public float wallSlideDownSpeed = 1f;

    public float defaultWallRunSpeed = 7.5f;
    public float wallRunDuration = 2f;
    float currentWallRunDuration;

    public bool isWallRunning;

    public Vector3 wallRunDirection;

    Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        globalPlayerController = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        wallNormal = new Vector3(0, 0, 0);
        wallRunDirection = new Vector3(0, 0, 0);
        currentWallRunDuration = wallRunDuration;
        isWallRunning = false;
        wallsCollidingWith = new List<GameObject>();
    }

    void OnEnable()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.useGravity = false;
        canAct = false;
        currentWallRunDuration = wallRunDuration;
        StartCoroutine(CanActCoolDown(0.15f));

        gameObject.transform.rotation = Quaternion.LookRotation(wallNormal);
        globalPlayerController.ResetJumpsAndDashes();
    }

    void OnDisable()
    {
        rb.useGravity = true;
    }

    void Update()
    {
        currentWallRunDuration -= Time.deltaTime;
        isWallRunning = wallRunDirection != new Vector3(0, 0, 0) && currentWallRunDuration > 0 && globalPlayerController.hasRecentlyJumped != RecentJumpType.Wall;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isWallRunning)
        {
            rb.velocity = wallRunDirection * ((defaultWallRunSpeed * currentWallRunDuration * 0.5f) + 4f);
        }
        else if (currentWallRunDuration < 0 && wallRunDirection != new Vector3(0, 0, 0))
        {
            rb.velocity = new Vector3(rb.velocity.normalized.x + wallNormal.normalized.x, 0f, rb.velocity.normalized.z + wallNormal.normalized.z);
        }
        else
        {
            rb.AddForce(Physics.gravity * wallSlideDownSpeed);
        }

        if (InputController.jumpPressed && canAct)
        {

            if (isWallRunning)
            {
                rb.velocity = new Vector3(rb.velocity.normalized.x + wallNormal.normalized.x, 1f, rb.velocity.normalized.z + wallNormal.normalized.z) * wallRunInitialJumpForce;
            }
            else
            {
                rb.velocity = new Vector3(wallNormal.normalized.x, 1f, wallNormal.normalized.z) * initialJumpForce;
            }

            globalPlayerController.hasRecentlyJumped = RecentJumpType.Wall;

            Debug.DrawRay(rb.position, new Vector3(wallNormal.normalized.x, 1f, wallNormal.normalized.z), Color.blue, 2f);
        }


        InputController.jumpPressed = false;
        InputController.dashPressed = false;
    }


    public IEnumerator CanActCoolDown(float cooldownTimeSeconds)
    {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        canAct = true;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Parkour") && this.enabled)
        {
            //Debug.DrawRay(other.GetContact(0).point, other.GetContact(0).normal ,Color.white, 1f);

            if (other.GetContact(0).normal == wallNormal)
            {
                return;
            }

            if (0.966f < Vector3.Dot(wallNormal, other.GetContact(0).normal))
            { //can transition the wall run
                Vector3 currentHorizontalVelocity = rb.velocity;
                currentHorizontalVelocity.y = 0;
                Debug.Log("New Wall");
                wallRunDirection = currentHorizontalVelocity.normalized;
            }
        }
    }
}
