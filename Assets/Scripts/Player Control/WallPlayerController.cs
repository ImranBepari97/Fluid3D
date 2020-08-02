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
    public float currentWallRunDuration;

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

        globalPlayerController.ResetJumpsAndDashes();
    }

    void OnDisable()
    {
        rb.useGravity = true;
    }

    void Update()
    {
        currentWallRunDuration -= Time.deltaTime;
        isWallRunning = wallRunDirection != new Vector3(0, 0, 0) && currentWallRunDuration > 0 && globalPlayerController.recentAction != RecentActionType.WallJump;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isWallRunning) { //regular wall running
            rb.velocity = wallRunDirection * ((defaultWallRunSpeed * currentWallRunDuration * 0.5f) + 4f);
            gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
        else if (currentWallRunDuration < 0 && wallRunDirection != new Vector3(0, 0, 0)) { //stopped wall running, detach from wall
            rb.velocity = new Vector3(rb.velocity.normalized.x + wallNormal.normalized.x, 0f, rb.velocity.normalized.z + wallNormal.normalized.z);
            gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
        else { //sliding down wall
            rb.AddForce(Physics.gravity * wallSlideDownSpeed);
            gameObject.transform.rotation = Quaternion.LookRotation(wallNormal);
        }

        if (InputController.jumpPressed && canAct) {

            globalPlayerController.IncreaseSpeedMultiplier(0.1f);
            if (isWallRunning) {
                rb.velocity = new Vector3(
                    rb.velocity.normalized.x + wallNormal.normalized.x + (InputController.moveDirection.x * 0.4f), 
                    1f, 
                    rb.velocity.normalized.z + wallNormal.normalized.z + (InputController.moveDirection.z * 0.4f)) * wallRunInitialJumpForce;
            } else {
                rb.velocity = new Vector3(
                    wallNormal.normalized.x + (InputController.moveDirection.x * 0.4f), 
                    1f, 
                    wallNormal.normalized.z + (InputController.moveDirection.z * 0.4f)) * initialJumpForce;
            }

            globalPlayerController.recentAction = RecentActionType.WallJump;

            Debug.DrawRay(rb.position, new Vector3(wallNormal.normalized.x, 1f, wallNormal.normalized.z), Color.blue, 2f);
            globalPlayerController.EnableDefaultControls();
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

            if (other.GetContact(0).normal == wallNormal) {
                return;
            }

            if (0.966f < Vector3.Dot(wallNormal, other.GetContact(0).normal)) { //can transition the wall run
                Vector3 currentHorizontalVelocity = rb.velocity * 1.25f;
                currentHorizontalVelocity.y = 0;
                Debug.Log("New Wall");
                wallRunDirection = currentHorizontalVelocity.normalized;
            }
        }
    }
}
