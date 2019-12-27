using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class Instantiator : MonoBehaviour
{

    [SerializeField] Transform handTransform;

    public PositionCurve positionHolder;
    public SteamVR_Action_Boolean triggerAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            positionHolder.posOfTheNewPoints = this.transform.position;
            positionHolder.rotationOfTheLastRoadPart += positionHolder.rotationValue;
            positionHolder.InstantiateNewPoints();
        }

        if (triggerAction.GetStateDown(SteamVR_Input_Sources.Any))
        {
            positionHolder.posOfTheNewPoints = this.transform.position;
            positionHolder.rotationOfTheLastRoadPart += positionHolder.rotationValue;
            positionHolder.rotationValue = handTransform.localEulerAngles.z;
            positionHolder.InstantiateNewPoints(GetNewPosition());
        }

        if (positionHolder.previsualisation)
        {
            positionHolder.rotationValue = handTransform.localEulerAngles.z;
            positionHolder.PathPrevisualisation(GetNewPosition());
        }
    }

    public Vector3 GetNewPosition()
    {
        GameObject lastGo = positionHolder.roadHolder.transform.GetChild(positionHolder.roadHolder.transform.childCount - 2).
            GetChild((int)(positionHolder.smoothness - 1)).gameObject;
        GameObject instantiator = positionHolder.Instanciator;
        instantiator.transform.parent = lastGo.transform;
        instantiator.transform.localPosition = Vector3.zero;
        instantiator.transform.localRotation = handTransform.localRotation;

        Vector3 newPos = instantiator.transform.forward * positionHolder.distanceBetweenAnchor + lastGo.transform.position;
        print(newPos + "   " + handTransform.localRotation);
        return newPos;
    }
}
