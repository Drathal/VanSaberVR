using EzySlice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HelperClass;

public class SaberHandler : MonoBehaviour
{
    public NoteType _type;
    public SteamVR_Controller.Device device;
    public SteamVR_TrackedObject controllerInHand;
    GameObject _tip;
    GameObject _base;
    GameObject _Controller;
    private void Start()
    {
        _tip = GetChildByName(gameObject, "Tip");
        _base = GetChildByName(gameObject, "Base");
        _Controller = _type == NoteType.LEFT ? EngineStart.Instance.LeftController : EngineStart.Instance.RightController;
        controllerInHand = _Controller.GetComponentInChildren<SteamVR_TrackedObject>(true);
    }

    private Vector3 previousPos;
    private Vector3 previousBotPos;
    void Update()
    {
        if (device == null)
        {
            if ((int)controllerInHand.index != -1)
            {
                device = SteamVR_Controller.Input((int)controllerInHand.index);
            }
        }
        else
        {
            float ControllerDistance = Vector3.Distance(gameObject.transform.position, device.transform.pos);

            if (ControllerDistance > 0.2f)
            {
                CustomSaberManager.Instance.setSaber(gameObject, _Controller, _type);
            }
        }

        previousPos = _tip.transform.position;
        previousBotPos = _base.transform.position;
    }

    IEnumerator LongVibration(float length, float strength)
    {
        strength = Mathf.Clamp01(strength);
        var startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime <= length)
        {
            int valveStrength = Mathf.RoundToInt(Mathf.Lerp(0, 3999, strength));

            device.TriggerHapticPulse((ushort)valveStrength);

            yield return null;
        }
    }

    public void Vribrate()
    {
        device.TriggerHapticPulse(3999);
    }

    private void OnTriggerExit(Collider other)
    {
        _handler = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_handler != null)
        {
            if (_handler._obstacle != null)
            {
                Vribrate();
            }
        }
    }

    MoveHandler _handler;
    Vector3 center;
    Vector3 halfSize;
    Quaternion orientation;
    float Angle;

    private void OnTriggerEnter(Collider other)
    {
        _handler = other.transform.parent.GetComponent<MoveHandler>();

        if (_handler != null)
        {
            if (_handler._note != null)
            {
                if (_handler._wasHit || _handler._note._type != _type)
                {
                    return;
                }

                if (_handler._note._type == NoteType.BOMB)
                {
                    //miss
                    return;
                }

                Vector3 cutDir = other.transform.InverseTransformVector(_tip.transform.position - previousPos);                                                                           
                                
                if (OkCut(cutDir, out Angle))
                {
                    _handler._wasHit = true;                    
                    
                    int cutScore = 0;

                    if (Angle <= -90)
                    {
                        cutScore += 70;
                    }
                    else if (Angle > -90 && Angle < -30)
                    {
                        cutScore += Angle < -70 ? 70 : (int)Mathf.Abs(Angle);
                    }
                    
                    if (Angle < -149)
                    {
                        cutScore += 30;
                    }
                    else if (Angle > -149 && Angle <= -91)
                    {
                        cutScore += (int)Mathf.Abs(Angle) - 90;
                    }

                    ThreePointsToBox(_tip.transform.position, _base.transform.position, (previousBotPos + previousPos) / 2, out center, out halfSize, out orientation);
                    float cutToCenter = Mathf.Abs(Vector3.Distance(other.transform.position, center));
                    float var = 15 - (cutToCenter * 10);
                    cutScore += (int)var;

                    UIRotator.Instance.UpdateScore(cutScore);
                    Cut(other.gameObject, _handler);
                    StartCoroutine(LongVibration(0.2f, 3999));
                }
                else
                {
                    UIRotator.Instance.UpdateScore(0, true);
                }
            }
            else
            {
                if (_handler._obstacle != null)
                {
                    Vribrate();
                }
            }
        }
    }

    public bool OkCut(Vector3 to, out float _angle)
    {
        _angle = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;

        if (_handler._note._cutDirection == CutDirection.NONDIRECTION)
            return true;

        bool goodEnoughCut = _angle > -150 && _angle < -30;

        if (goodEnoughCut)
            return true;
        
        return false;
    }
    
    public void Cut(GameObject other, MoveHandler _handler)
    {
        /* if (_currentSettings.ShowSparks == 1)
        {
            GameObject newSpark = PoolHandler.Instance.GetPooledSpark();
            newSpark.transform.position = other.transform.position;

            ParticleSystem.MainModule tempSpark = newSpark.GetComponent<ParticleSystem>().main;
            if (newSpark.GetComponent<ParticleSystem>())
            {
                tempSpark.startColor = new ParticleSystem.MinMaxGradient(other.gameObject.GetComponent<Renderer>().material.GetColor("_Color"));
                tempSpark.stopAction = ParticleSystemStopAction.Disable;
                newSpark.SetActive(true);
            }
        }*/

        if (_currentSettings.ShowDebris == 1)
        {
            Material newCutMat = other.gameObject.GetComponent<MeshRenderer>().material;
            _Sliced hull = other.gameObject.Slice(other.gameObject.transform.position, orientation * _tip.transform.up, newCutMat);

            if (hull != null)
            {
                hull.CreateLowerHull(other.gameObject, newCutMat, GameHandler.Instance._Angle);
                hull.CreateUpperHull(other.gameObject, newCutMat, GameHandler.Instance._Angle);
            }
        }

        if (_handler.newBar != null)
            Destroy(_handler.newBar);

        other.transform.parent.position = new Vector3(0, 1000, 0);
        other.transform.parent.rotation = Quaternion.identity;
        GameHandler.Instance._objects.Remove(_handler);
        other.transform.parent.transform.SetParent(_handler._objectHandler._Pool.transform);        
        other.transform.parent.gameObject.SetActive(false);

        _handler = null;

    }

    public void ThreePointsToBox(Vector3 p0, Vector3 p1, Vector3 p2, out Vector3 center, out Vector3 halfSize, out Quaternion orientation) //https://github.com/hrincarp/GGJ2017-cart/
    {
        Vector3 up = Vector3.Cross(p1 - p2, p0 - p2).normalized;

        // Continue only if normal exists
        if (up.sqrMagnitude > 0.00001f)
        {

            Vector3 forward = (p0 - p1).normalized;
            Vector3 left = Vector3.Cross(forward, up);

            orientation = new Quaternion();
            orientation.SetLookRotation(forward, up);

            float a = Mathf.Abs((new UnityEngine.Plane(left, p0)).GetDistanceToPoint(p2));
            float b = Vector3.Magnitude(p0 - p1);

            Vector3 pc = (p0 + p1) * 0.5f;

            center = pc - left * a * 0.5f;
            halfSize = new Vector3(a * 0.5f, 0.0f, b * 0.5f);
        }
        else
        {
            center = Vector3.zero;
            halfSize = Vector3.zero;
            orientation = Quaternion.identity;
        }
    }
}

