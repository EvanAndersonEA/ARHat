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
    GameObject Gun, Shot;

    float timeLeft = 1f;

    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_Anchors)
        {
            m_AnchorManager.RemoveAnchor(anchor);
        }
        m_Anchors.Clear();
        doneBefore = false;
    }

    public void Shoot()
    {
        GameObject Shot1 = Instantiate(Shot, m_Anchors[0].transform);
        GameObject Shot2 = Instantiate(Shot, m_Anchors[1].transform);
        cowboy.PlayOneShot(gunshot, 5f);

        int whoDead = Random.Range(0,1);
        if (whoDead == 1)
        {
            m_AnchorManager.RemoveAnchor(m_Anchors[0]);
        }
        else
        {
            m_AnchorManager.RemoveAnchor(m_Anchors[1]);
        }
        m_Anchors.Clear();

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
        if (m_Anchors.Count == 2)
        {
            if(doneBefore == false)
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
                if (m_Anchors.Count == 0)
                {
                    cowboy.PlayOneShot(cowboy1, 1f);
                }
                else
                {
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
