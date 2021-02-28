using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using Mirror;

public class GrindPlayerController : NetworkBehaviour
{

    public RoadMeshCreator roadMeshCreator;
    public PathCreator currentRail;
    public float grindSpeed;
    public bool isReversed;

    float currentGrindSpeed;

    public bool grindCooldownActive;

    public float dstTravelled;
    public float timeOnPath;
    GlobalPlayerController globalPlayerController;
    Rigidbody rb;
    CapsuleCollider cc;


    // Start is called before the first frame update
    void Awake()
    {
        globalPlayerController = GetComponent<GlobalPlayerController>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        isReversed = false;
        currentGrindSpeed = grindSpeed;
        grindCooldownActive = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentGrindSpeed = grindSpeed * globalPlayerController.currentSpeedMultiplier;
        globalPlayerController.recentAction = RecentActionType.Grind;
        rb.velocity = new Vector3(0,0,0);

        // Debug.DrawRay(rb.position, currentRail.path.GetDirectionAtDistance (dstTravelled,  EndOfPathInstruction.Stop), Color.black, 0.01f);
        // Debug.DrawRay(rb.position, -currentRail.path.GetDirectionAtDistance (dstTravelled,  EndOfPathInstruction.Stop), Color.red, 0.01f);

        if(!isReversed) {
            dstTravelled += currentGrindSpeed * Time.fixedDeltaTime;
            rb.rotation = Quaternion.LookRotation(currentRail.path.GetDirectionAtDistance (dstTravelled,  EndOfPathInstruction.Stop));
        } else {
            dstTravelled -= currentGrindSpeed * Time.fixedDeltaTime;
            transform.rotation = currentRail.path.GetRotationAtDistance(dstTravelled,  EndOfPathInstruction.Stop);
            transform.rotation = Quaternion.LookRotation(-currentRail.path.GetDirectionAtDistance (dstTravelled,  EndOfPathInstruction.Stop));
        }

        rb.position = currentRail.path.GetPointAtDistance(dstTravelled, EndOfPathInstruction.Stop) + 
            (currentRail.path.GetNormalAtDistance(dstTravelled) * (0.02f + roadMeshCreator.thickness + (cc.height / 2f)));

        float curTime = currentRail.path.GetClosestTimeOnPath (rb.position);
        timeOnPath = curTime;
        if(curTime > 0.99f || curTime < 0.01f) {
            globalPlayerController.EnableDefaultControls();
            rb.velocity = transform.forward * currentGrindSpeed;
            globalPlayerController.recentAction = RecentActionType.None;
            StartCoroutine(CoolDownTimer(0.1f));
        }


        if(globalPlayerController.input.jumpPressed && isLocalPlayer) {
            globalPlayerController.EnableDefaultControls();
            rb.velocity = (transform.forward + new Vector3(0, 1, 0) + (globalPlayerController.input.moveDirection * 0.4f)) * grindSpeed * 1.05f;
            globalPlayerController.IncreaseSpeedMultiplier(0.2f);
            globalPlayerController.recentAction = RecentActionType.SlideJump;
            StartCoroutine(CoolDownTimer(0.1f));
        }
        //Debug.Log(currentRail.path.GetClosestTimeOnPath (rb.position));

        globalPlayerController.input.jumpPressed = false;
        globalPlayerController.input.dashPressed = false;

    }

    public IEnumerator CoolDownTimer(float cooldownTimeSeconds) {
        grindCooldownActive = true;
        yield return new WaitForSeconds(cooldownTimeSeconds);
        grindCooldownActive = false;

    }

    public float GetCurrentGrindSpeed() {
        return currentGrindSpeed;
    }

    // void Update()
    // {
    //     dstTravelled += grindSpeed * Time.deltaTime;
    //     transform.position = currentRail.path.GetPointAtDistance(dstTravelled, EndOfPathInstruction.Reverse) + new Vector3(0,  (cc.height / 2) + roadMeshCreator.thickness + 0.01f , 0);
    //     transform.rotation = currentRail.path.GetRotationAtDistance(dstTravelled,  EndOfPathInstruction.Reverse);

    // }
}
