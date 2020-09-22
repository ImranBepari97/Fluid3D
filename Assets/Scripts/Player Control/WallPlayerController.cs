using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlayerController : MonoBehaviour
{

    public Vector3 wallNormal;
    GlobalPlayerController gpc;

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
        gpc = GetComponent<GlobalPlayerController>();
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

        gpc.ResetJumpsAndDashes();
    }

    void OnDisable()
    {
        rb.useGravity = true;
        if(gpc.recentAction == RecentActionType.WallRunning || 
            gpc.recentAction == RecentActionType.OnWall) {
            gpc.recentAction = RecentActionType.None;
        }
    }

    void Update()
    {
        currentWallRunDuration -= Time.deltaTime;
        isWallRunning = wallRunDirection != new Vector3(0, 0, 0) && currentWallRunDuration > 0 && gpc.recentAction != RecentActionType.WallJump;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        gpc.recentAction = RecentActionType.None;

        if (isWallRunning) { //regular wall running
            gpc.recentAction = RecentActionType.WallRunning;
            //rb.velocity = wallRunDirection * ((defaultWallRunSpeed * currentWallRunDuration * Mathf.Log10(gpc.currentSpeedMultiplier + 1.2f)) + 7f);
            rb.velocity = wallRunDirection * ((defaultWallRunSpeed * currentWallRunDuration * 0.5f) + 4f);
            Debug.Log(rb.velocity.magnitude);
            //gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
            gameObject.transform.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(rb.velocity), 540f * Time.deltaTime);
        }
        else if (currentWallRunDuration < 0 && wallRunDirection != new Vector3(0, 0, 0)) { //stopped wall running, detach from wall
            gpc.recentAction = RecentActionType.None;
            rb.velocity = new Vector3(rb.velocity.normalized.x + wallNormal.normalized.x, 0f, rb.velocity.normalized.z + wallNormal.normalized.z);
            gameObject.transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
        else { //sliding down wall
            gpc.recentAction = RecentActionType.OnWall;
            rb.AddForce(Physics.gravity * wallSlideDownSpeed);
            gameObject.transform.rotation = Quaternion.LookRotation(-wallNormal);
        }

        if (InputController.jumpPressed && canAct) {

            gpc.IncreaseSpeedMultiplier(0.2f);
            if (isWallRunning) {
                rb.velocity = new Vector3(
                    rb.velocity.normalized.x + wallNormal.normalized.x + (InputController.moveDirection.x * 0.4f), 
                    1f, 
                    rb.velocity.normalized.z + wallNormal.normalized.z + (InputController.moveDirection.z * 0.4f)) * wallRunInitialJumpForce;
            } else {
                rb.velocity = new Vector3(
                    wallNormal.normalized.x + (InputController.moveDirection.x * 0.4f), 
                    2f, 
                    wallNormal.normalized.z + (InputController.moveDirection.z * 0.4f)).normalized * initialJumpForce * 1.25f;

                    gameObject.transform.rotation = Quaternion.LookRotation(wallNormal);
            }

            gpc.recentAction = RecentActionType.WallJump;

            Debug.DrawRay(rb.position, rb.velocity, Color.blue, 2f);
            gpc.EnableDefaultControls();
        }


        InputController.jumpPressed = false;
        InputController.dashPressed = false;
    }


    public IEnumerator CanActCoolDown(float cooldownTimeSeconds)
    {
        yield return new WaitForSeconds(cooldownTimeSeconds);
        canAct = true;
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Parkour") && this.enabled)
        {
            //Debug.DrawRay(other.GetContact(0).point, other.GetContact(0).normal ,Color.white, 1f);
            if (other.GetContact(0).normal == wallNormal) {
                return;
            }

            if (0.966f > Vector3.Dot(wallNormal, other.GetContact(0).normal)) { //can transition the wall run
                wallNormal = other.GetContact(0).normal;
                currentWallRunDuration += 0.15f;
                Vector3 currentHorizontalVelocity = rb.velocity;
                currentHorizontalVelocity.y = 0;
                wallRunDirection = currentHorizontalVelocity.normalized;
            }
        }
    }
}
