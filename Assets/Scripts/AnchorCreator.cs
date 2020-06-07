using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator : MonoBehaviour
{
    public AudioClip cowboy1;
    public AudioClip cowboy2;
    public AudioClip gunshot;
    public AudioSource cowboy;
    bool doneBefore = false;
    [SerializeField]
    GameObject Gun, Shot, Ghost;

    float timeLeft = 1f;

    //removes all anchors when the button is pressed
    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_Anchors)
        {
            m_AnchorManager.RemoveAnchor(anchor);
            Destroy(anchor.gameObject);
        }
        m_Anchors.Clear();
        doneBefore = false;
    }

    //makes the hats shoot and kill eachother when the button is pressed
    public void Shoot()
    {
        //makes sure there are never more or less than 2 hats when they are shooting eachother
        if (m_Anchors.Count != 2)
        {
            return;
        }
        GameObject Shot1 = Instantiate(Shot, m_Anchors[0].transform);
        GameObject Shot2 = Instantiate(Shot, m_Anchors[1].transform);
        cowboy.PlayOneShot(gunshot, 5f);

        //decides randomly which hat to remove between the 2 of them
        int whoDead = Random.Range(0,2);
        if (whoDead == 1)
        {
            //instantiates a Ghost that floats into the sky
            Instantiate(Ghost, m_Anchors[0].transform.position, Quaternion.identity);
            //SIDENOTE: this is super dumb, in order to remove one of the anchors you need to 
            //remove it using the the anchor manager script
            //remove the anchor gameobject from the scene
            //remove the anchor from the list of anchors
            m_AnchorManager.RemoveAnchor(m_Anchors[0]);
            Destroy(m_Anchors[0].gameObject);
            m_Anchors.RemoveAt(0);
        }
        else
        {
            Instantiate(Ghost, m_Anchors[1].transform.position, Quaternion.identity);
            m_AnchorManager.RemoveAnchor(m_Anchors[1]);
            Destroy(m_Anchors[1].gameObject);
            m_Anchors.RemoveAt(1);
        }
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_Anchors = new List<ARAnchor>();
        cowboy = GetComponent<AudioSource>();
        cowboy.PlayOneShot(cowboy2, 1f);
    }

    void Update()
    {
        //if there are 2 hats it deploys their guns and makes them look at eachother
        //KNOWN BUG: if there is one hat already in the scene the second hat will spawn without a gun
        if (m_Anchors.Count == 2)
        {
            //doneBefore makes sure it only happens once and is reset when you delete all the anchors (line 29)
            if (doneBefore == false)
            {
                Instantiate(Gun, m_Anchors[0].transform);
                Instantiate(Gun, m_Anchors[1].transform);
                doneBefore = true;
            }
            else
            {
                m_Anchors[0].transform.LookAt(m_Anchors[1].transform);
                m_Anchors[1].transform.LookAt(m_Anchors[0].transform);
            }
        }
        //checks to make sure there will never be more than 2 hats
        if (m_Anchors.Count >= 2)
        {
            return;
        }

        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return;

        if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.FeaturePoint))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;
            var anchor = m_AnchorManager.AddAnchor(hitPose);
            if (anchor == null)
            {
                Logger.Log("Error creating anchor");
            }
            else
            {
                //play sounds when you put the hats on your stuffies
                if (m_Anchors.Count == 0)
                {
                    cowboy.PlayOneShot(cowboy1, 1f);
                }
                else
                {
                    //Instantiate(Gun, m_Anchors[1].transform);
                    cowboy.PlayOneShot(cowboy2, 1f);
                }
                m_Anchors.Add(anchor);
            }
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_Anchors;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;
}
