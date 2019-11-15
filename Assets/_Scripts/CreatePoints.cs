using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Valve.VR;
using PathCreation.Examples;

public class CreatePoints : MonoBehaviour
{
    public GameDatas gameDatas;

    public SteamVR_Action_Boolean triggerAction;
    [SerializeField] PathFollower pathFollower;
    [SerializeField] PathCreator m_pathCreator;
    [SerializeField] GameObject m_pathHolder;
    [SerializeField] Transform m_handTransform;
    [SerializeField] Transform m_playerTransform;
    [SerializeField] Transform m_target;

    [SerializeField] float m_distanceFocus;
    [SerializeField] float m_distanceFromLastPoint;
    [SerializeField] Transform m_parentHand;

    private void Start()
    {
        m_target.position = m_handTransform.position + m_handTransform.forward * m_distanceFocus;
        pathFollower.speed = gameDatas.playerSpeed;
    }

    void Update()
    {
        m_target.position = GetInstanceDotPosition(m_handTransform, m_playerTransform, m_parentHand);
        if (Input.GetKeyDown(KeyCode.A) || triggerAction.GetStateDown(SteamVR_Input_Sources.Any))
        {
            //m_pathCreator.bezierPath.AddSegmentToEnd(GetInstanceDotPositionRay(m_handTransform, m_playerTransform));
            m_pathCreator.bezierPath.AddSegmentToEnd(m_target.position);
          //  SetLastPointRotation(m_handTransform);
        }
        //m_target.position = GetInstanceDotPositionRay(m_handTransform, m_playerTransform);
    }


    private Vector3 GetInstanceDotPosition(Transform handTransform, Transform playerTransform, Transform handParent)
    {
        Vector3 finalDotPosition = m_pathCreator.bezierPath.GetPoint(m_pathCreator.bezierPath.NumPoints - 1);
        Vector3 finalDotRotation = m_pathHolder.transform.GetChild(m_pathHolder.transform.childCount - 1).localRotation * Vector3.forward;
        //Vector3 finalDotRotation = m_pathCreator.bezierPath.GetAnchorNormalAngle(m_pathCreator.bezierPath.NumPoints - 1);
        //Vector3 focusRay = handTransform.forward * m_distanceFocus + finalDotPosition;
        //Vector3 localForward = m_parentHand.InverseTransformDirection(handTransform.forward);
        Vector3 localForward = handTransform.localRotation * Vector3.forward;
        Vector3 adaptatifForward = (handTransform.localRotation * m_pathHolder.transform.GetChild(m_pathHolder.transform.childCount - 1).rotation) * Vector3.forward;
        Debug.Log("fwd cumulé: " + (localForward + finalDotRotation) + " / local fwd: " + localForward + " / fwd finaldot : " + finalDotRotation);
        Vector3 newDotPosition = finalDotPosition + ((localForward) * m_distanceFromLastPoint);
        return newDotPosition;
    }


    /// <summary>
    /// Permet de viser la direction dans laquel la piste va continuer sa trajectoire.
    /// </summary>
    /// <param name="handTransform"></param>
    /// <param name="playerTransform"></param>
    /// <returns></returns>
    private Vector3 GetInstanceDotPositionRay(Transform handTransform, Transform playerTransform)
    {
        Vector3 finalDotPosition = m_pathCreator.bezierPath.GetPoint(m_pathCreator.bezierPath.NumPoints - 1);
        Vector3 focusRay = handTransform.forward * m_distanceFocus + playerTransform.position;
        Vector3 pointToRayDirection = (focusRay - finalDotPosition).normalized;
        Vector3 newDotPosition = finalDotPosition + pointToRayDirection * m_distanceFromLastPoint;
        return newDotPosition;
    }

    private void SetLastPointRotation(Transform handTransform)
    {
        m_pathCreator.bezierPath.SetAnchorNormalAngle(m_pathCreator.bezierPath.NumAnchorPoints - 1, handTransform.eulerAngles.z);
    }
}
