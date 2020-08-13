using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class GrindPlayerController : MonoBehaviour
{

    public RoadMeshCreator roadMeshCreator;
    public PathCreator currentRail;
    public float grindSpeed;
    public bool isReversed;

    float currentGrindSpeed;

    public float dstTravelled;
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentGrindSpeed = grindSpeed * globalPlayerController.currentSpeedMultiplier;
        globalPlayerController.recentAction = RecentActionType.Grind;

        Debug.DrawRay(rb.position, currentRail.path.GetDirectionAtDistance (dstTravelled,  EndOfPathInstruction.Stop), Color.black, 0.01f);

        Debug.DrawRay(rb.position, -currentRail.path.GetDirectionAtDistance (dstTravelled,  EndOfPathInstruction.Stop), Color.red, 0.01f);

        if(!isReversed) {
            dstTravelled += currentGrindSpeed * Time.fixedDeltaTime;
            rb.rotation = Quaternion.LookRotation(currentRail.path.GetDirectionAtDistance (dstTravelled,  EndOfPathInstruction.Stop));
        } else {
            dstTravelled -= currentGrindSpeed * Time.fixedDeltaTime;
            transform.rotation = currentRail.path.GetRotationAtDistance(dstTravelled,  EndOfPathInstruction.Stop);
            transform.rotation =  Quaternion.LookRotation(-currentRail.path.GetDirectionAtDistance (dstTravelled,  EndOfPathInstruction.Stop));
        }

        rb.position = currentRail.path.GetPointAtDistance(dstTravelled, EndOfPathInstruction.Stop) + new Vector3(0,  (cc.height / 2) + roadMeshCreator.thickness / 2 + 0.01f , 0);

        float curTime = currentRail.path.GetClosestTimeOnPath (rb.position);
        if(curTime > 0.99f || curTime < 0.01f) {
            globalPlayerController.EnableDefaultControls();
            rb.velocity = transform.forward * currentGrindSpeed;
            globalPlayerController.recentAction = RecentActionType.None;
        }


        if(InputController.jumpPressed) {
            globalPlayerController.EnableDefaultControls();
            rb.velocity = (transform.forward + new Vector3(0, 1, 0) + (InputController.moveDirection * 0.4f)) * grindSpeed * 1.05f;
            globalPlayerController.IncreaseSpeedMultiplier(0.2f);
            globalPlayerController.recentAction = RecentActionType.SlideJump;
        }
        //Debug.Log(currentRail.path.GetClosestTimeOnPath (rb.position));

        InputController.jumpPressed = false;
        InputController.dashPressed = false;

    }

    // void Update()
    // {
    //     dstTravelled += grindSpeed * Time.deltaTime;
    //     transform.position = currentRail.path.GetPointAtDistance(dstTravelled, EndOfPathInstruction.Reverse) + new Vector3(0,  (cc.height / 2) + roadMeshCreator.thickness + 0.01f , 0);
    //     transform.rotation = currentRail.path.GetRotationAtDistance(dstTravelled,  EndOfPathInstruction.Reverse);

    // }
}
