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

    [SerializeField] GameObject m_lastPathGoTemp;
    [SerializeField] LineRenderer m_line;
    private void Start()
    {
        m_target.position = m_handTransform.position + m_handTransform.forward * m_distanceFocus;
        pathFollower.speed = gameDatas.playerSpeed;
        m_pathCreator.bezierPath.AddSegmentToEnd(GetInstanceDotPosition(m_handTransform, m_playerTransform, m_parentHand));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || triggerAction.GetStateDown(SteamVR_Input_Sources.Any))
        {
            m_pathCreator.bezierPath.AddSegmentToEnd(GetInstanceDotPosition(m_handTransform, m_playerTransform, m_parentHand));
          //  m_pathCreator.bezierPath.AddSegmentToEnd(GetInstanceDotPositionRay(m_handTransform, m_playerTransform));
            // SetLastPointRotation(m_handTransform);
        }
      //  m_target.position = GetInstanceDotPositionRay(m_handTransform, m_playerTransform);
        m_target.position = GetInstanceDotPosition(m_handTransform, m_playerTransform, m_parentHand);
        MoveLastSegment();
        SetLine();
    }


    public void MoveLastSegment()
    {
        print("0");
        m_pathCreator.bezierPath.DeleteSegment(m_pathCreator.bezierPath.NumPoints-1);
        print("0.5");
        m_pathCreator.bezierPath.AddSegmentToEnd(GetInstanceDotPosition(m_handTransform, m_playerTransform, m_parentHand));
        print("1");

    }

    private void SetLine()
    {
        m_line.SetPosition(0, m_pathCreator.bezierPath.GetPoint(m_pathCreator.bezierPath.NumPoints - 1));
        m_line.SetPosition(1, m_target.position);
    }

    private Vector3 GetInstanceDotPosition(Transform handTransform, Transform playerTransform, Transform handParent)
    {

        GameObject lastPathGo = m_pathHolder.transform.GetChild(m_pathHolder.transform.childCount - 1).gameObject;
        m_lastPathGoTemp.transform.parent = lastPathGo.transform;
        m_lastPathGoTemp.transform.localPosition = Vector3.zero;
        m_lastPathGoTemp.transform.localRotation = handTransform.localRotation;


        //sortir le game object tempon
        if (m_lastPathGoTemp.transform.parent != null)
            m_lastPathGoTemp.transform.parent = null;
        Vector3 finalDotPosition = m_pathCreator.bezierPath.GetPoint(m_pathCreator.bezierPath.NumPoints - 1);

        Vector3 newDotPosition = m_lastPathGoTemp.transform.forward * m_distanceFromLastPoint + finalDotPosition;
        print(newDotPosition);
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
