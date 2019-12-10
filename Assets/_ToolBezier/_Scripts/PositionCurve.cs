using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCurve : MonoBehaviour
{

    [Header("Preinstantiate The Path")]
    [Tooltip ("True if you want a predraw (Predraw destroy the path if there is one)")]
    public bool preInstantiate;
    [Tooltip ("How many segment you want to draw at the beginning of the game")]
    public int howManySegment;

    [Header("How The Path Is Draw")]
    [Tooltip ("How Many point are produce between two anchor")]
    public float smoothness;
    [Tooltip ("Is The Path Draw Progressivily")]
    public bool progressifInstantiateRoad;
    [Tooltip ("If progressifInstantiateRoad is true, how fast the road is draw")]
    public float speedOfDraw;
    [Tooltip("Position Between Each Point")]
    public int distanceBetweenAnchor;
    [Tooltip("If you want to see the next segment draw")]
    public bool previsualisation = false;


    [Header("RotationTest")]
    public float rotationValue;
    public float rotationOfTheLastRoadPart;


    [Header ("Not Tooltiped yet or other Stuff")]


    public GameObject curveHolder;
    public GameObject anchor;
    public GameObject physicalRoad;
    public GameObject roadHolder;
    public GameObject Instanciator;

    GameObject bufferRoad;

    public Vector3 posOfTheNewPoints;

    bool waitForTheEndOfPreinitialising = false;




    [SerializeField]
    public List<StructSegment> curveTab;

    // Start is called before the first frame update
    void Start()
    {
        if (preInstantiate == true)
            InitialisationOfThePath();

        if (previsualisation == true)
        {
            InstantiateNewPoints();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (previsualisation == true)
        {
            PathPrevisualisation();
        }
    }

    public void PathPrevisualisation()
    {
        DestroyTheEndOfThePathFrom(1);
        DestroyTheEndOfTheRoadFrom(1);
        InstantiateNewPoints();
    }

    public void DestroyTheEndOfTheRoadFrom (int HowManySegment)
    {
        for (int i = 0; i < HowManySegment; i++)
        {
            Destroy(roadHolder.transform.GetChild(roadHolder.transform.childCount-1).gameObject);
        }
    }

    public void DestroyTheEndOfThePathFrom (int howManySegment)
    {
        for (int i = 0; i < howManySegment; i++)
        {
            Destroy(curveTab[curveTab.Count-1].pointsAnchors.gameObject);
            Destroy(curveTab[curveTab.Count-1].pointsAfterAnchors.gameObject);
            curveTab.Remove(curveTab[curveTab.Count-1]);
        }
    }



    public void InitialisationOfThePath()
    {
        waitForTheEndOfPreinitialising = true;
        int curveChild = curveHolder.transform.childCount;
        for (int i = 0; i < curveChild; i++)
        {
            Destroy(curveHolder.transform.GetChild(0));
        }

        DestroyTheRoad();
        EmptyTheStructCurve();

        for (int i =0; i< howManySegment; i++)
        {
            InstantiateNewPoints(new Vector3 (0 + distanceBetweenAnchor * i,0,0));
        }
        waitForTheEndOfPreinitialising = false;
    }

    public void EmptyTheStructCurve()
    {
        curveTab.Clear();
    }

    public void DestroyTheRoad()
    {
        int roadChild = roadHolder.transform.childCount;
        for (int i = 0; i < roadChild; i++)
        {
            Destroy(roadHolder.transform.GetChild(0).gameObject);
        }
    }

    //Permet de poser un nouveau point dans la scène
    public void InstantiateNewPoints()
    {
        print("Instanciate new point");
        GameObject newPoint = Instantiate(anchor, Instanciator.transform.position, Quaternion.identity);
        newPoint.name = "Point" + curveTab.Count.ToString();
        newPoint.transform.parent = curveHolder.transform;
        Vector3 posOfNextPoint;
        if (curveTab.Count == 0)
        {
            posOfNextPoint = new Vector3(distanceBetweenAnchor * 0.5f, 0, 0);
        }
        else
        {
            posOfNextPoint = Vector3.LerpUnclamped(curveTab[curveTab.Count - 1].pointsAfterAnchors.transform.position, newPoint.transform.position, 2f);
        }

        GameObject pointAfterAnchor = Instantiate(anchor, posOfNextPoint , Quaternion.identity);
        pointAfterAnchor.transform.parent = curveHolder.transform;
        pointAfterAnchor.name = "Point" + curveTab.Count.ToString() + "AfterAnchor";

        AddToTheCurve(newPoint, pointAfterAnchor);
    }

    public void InstantiateNewPoints(Vector3 positionOfTheAnchor)
    {
        print("Instanciate new point");
        GameObject newPoint = Instantiate(anchor, positionOfTheAnchor, Quaternion.identity);
        newPoint.name = "Point" + curveTab.Count.ToString();
        newPoint.transform.parent = curveHolder.transform;

        Vector3 posOfNextPoint;
        if (curveTab.Count == 0)
        {
            posOfNextPoint = new Vector3(distanceBetweenAnchor * 0.5f, 0, 0);
        }
        else
        {
            posOfNextPoint = Vector3.LerpUnclamped(curveTab[curveTab.Count - 1].pointsAfterAnchors.transform.position, newPoint.transform.position, 2f);
        }

        GameObject pointAfterAnchor = Instantiate(anchor, posOfNextPoint, Quaternion.identity);
        pointAfterAnchor.transform.parent = curveHolder.transform;
        pointAfterAnchor.name = "Point" + curveTab.Count.ToString() + "AfterAnchor";

        AddToTheCurve(newPoint, pointAfterAnchor);
    }

    //Permet de mettre un nouveau point dans le tableau de structure!
    void AddToTheCurve(GameObject anchor, GameObject afterAnchor)
    {
        if (curveTab.Count > 0)
        {
            curveTab.Add(new StructSegment
            {
                pointsAnchors = anchor.transform,
                pointsBeforeAnchors = curveTab[curveTab.Count - 1].pointsAfterAnchors,
                pointsAfterAnchors = afterAnchor.transform
            });
        }
        else
        {
            curveTab.Add(new StructSegment
            {
                pointsAnchors = anchor.transform,
                pointsAfterAnchors = afterAnchor.transform
            });
        }

        if (progressifInstantiateRoad == true && waitForTheEndOfPreinitialising == false)
            StartCoroutine(DrawNewSegment(speedOfDraw));
        else
        {
            DrawNewSegment();
        }
    }


    //Permet de faire la curve proprement
    Vector3 GetPoint(StructSegment CurveToDraw,StructSegment CurveBefore, float time)
    {
        Vector3 pos = Vector3.Lerp(Vector3.Lerp(CurveBefore.pointsAnchors.transform.position, CurveToDraw.pointsBeforeAnchors.transform.position, time),
                            Vector3.Lerp(CurveToDraw.pointsBeforeAnchors.transform.position, CurveToDraw.pointsAnchors.transform.position, time), time);
        return pos;
    }

    //Permet d'afficher la courbe
    void DrawNewSegment()
    {
        GameObject holderSegmentRoad = Instantiate(anchor, Vector3.zero, Quaternion.identity, roadHolder.transform);
        //Only Working for last segment
        for (float i = 0; i < smoothness; i++)
        {
            holderSegmentRoad.name = "HolderRoadSegment(" + roadHolder.transform.childCount + ")";
            if (curveTab.Count != 1)
            {
                GameObject roadJustCreated = Instantiate(physicalRoad, GetPoint(curveTab[curveTab.Count - 1], curveTab[curveTab.Count - 2],(i / (float)smoothness)), Quaternion.identity, holderSegmentRoad.transform);
            
                if (bufferRoad == null)
                {
                    bufferRoad = roadJustCreated;
                }
                else
                {
                    bufferRoad.transform.LookAt(roadJustCreated.transform);
                    bufferRoad.transform.localRotation = Quaternion.Euler(bufferRoad.transform.rotation.eulerAngles.x, bufferRoad.transform.rotation.eulerAngles.y, (i / smoothness) * rotationValue + rotationOfTheLastRoadPart);
                    if (i == smoothness - 1)
                    {
                        roadJustCreated.transform.localRotation = Quaternion.Euler(bufferRoad.transform.rotation.eulerAngles.x, bufferRoad.transform.rotation.eulerAngles.y,
                                                                  bufferRoad.transform.rotation.eulerAngles.z + (1/smoothness) * rotationOfTheLastRoadPart);
                    }
                    bufferRoad = roadJustCreated;
                }
            }
                
        }
    }

    IEnumerator DrawNewSegment(float duration)
    {
        for (int i = 0; i < smoothness; i++)
        {
            GameObject roadJustCreated = null;
            if (curveTab.Count != 1)
            {
                roadJustCreated = Instantiate(physicalRoad, GetPoint(curveTab[curveTab.Count - 1], curveTab[curveTab.Count - 2], (i / (float)smoothness)), Quaternion.identity, roadHolder.transform);

                if (bufferRoad == null)
                {
                    bufferRoad = roadJustCreated;
                }
                else
                {
                    bufferRoad.transform.LookAt(roadJustCreated.transform);
                    if (i == smoothness - 1)
                        roadJustCreated.transform.LookAt(bufferRoad.transform);
                    bufferRoad = roadJustCreated;
                }

                yield return new WaitForSeconds(duration);
            }
            else
            {
                yield return null;
            }
        }
        
    }
}
