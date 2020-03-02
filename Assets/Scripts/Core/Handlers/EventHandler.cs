using CustomFloorPlugin;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CustomFloorPlugin.TubeLight;
using static HelperClass;

public class EventHander : MonoBehaviour
{
    public List<GameObject> _currentRings;
    public static List<Material> _eventMats;
    public GameHandler _objectHandler;
    public static EventHander Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void OnLoad(GameHandler objectHandler)
    {
        if (Instance != null)
        {
            var _eventHander = FindObjectsOfType<EventHander>().FirstOrDefault();
            _eventHander._objectHandler = objectHandler;
            return;
        }

        GameObject go = new GameObject("Event Manager");
        EventHander newManager = go.AddComponent<EventHander>();

        newManager._objectHandler = objectHandler;
        newManager._currentRings = new List<GameObject>();
    }

    public void EventNoteForGame(EventData EventNote)
    {
        switch (EventNote.Type)
        {
            case LightsID.Static - 1:
                EventColor(EventNote.Value, (int)LightsID.Static);
                break;
            case LightsID.BackLights - 1:
                EventColor(EventNote.Value, (int)LightsID.BackLights);
                break;
            case LightsID.BigRingLights - 1:
                EventColor(EventNote.Value, (int)LightsID.BigRingLights);
                break;
            case LightsID.LeftLasers - 1:
                EventColor(EventNote.Value, (int)LightsID.LeftLasers);
                break;
            case LightsID.RightLasers - 1:
                EventColor(EventNote.Value, (int)LightsID.RightLasers);
                break;
            case LightsID.TrackAndBottom - 1:
                EventColor(EventNote.Value, (int)LightsID.TrackAndBottom);
                break;
            case LightsID.Unused5 - 1:
                EventColor(EventNote.Value, (int)LightsID.Unused5);
                break;
            case LightsID.Unused6 - 1:
                EventColor(EventNote.Value, (int)LightsID.Unused6);
                break;
            case LightsID.Unused7 - 1:
                EventColor(EventNote.Value, (int)LightsID.Unused7);
                break;
            case LightsID.RingsRotationEffect - 1:
                break;
            case LightsID.RingsStepEffect - 1:
                break;
            case LightsID.Unused10 - 1:
                EventColor(EventNote.Value, (int)LightsID.Unused10);
                break;
            case LightsID.Unused11 - 1:
                EventColor(EventNote.Value, (int)LightsID.Unused11);
                break;
            case LightsID.RingSpeedLeft - 1:
                break;
            case LightsID.RingSpeedRight - 1:
                break;
            case LightsID.Unused14 - 1:
                    EventColor(EventNote.Value, (int)LightsID.Unused14);
                break;
            case LightsID.Unused15 - 1:
                    EventColor(EventNote.Value, (int)LightsID.Unused15);
                break;
            default:
                break;
        }
    }

    public bool LaneChange(EventData eventData)
    {
        if (eventData.Type == LightsID.Unused14 - 1 || eventData.Type == LightsID.Unused15 - 1)
        {
            if (eventData.Value < 9)
                return true;
        }

        return false;
    }

    public float RotationValue(int index)
    {
        switch (index)
        {
            case 0:
                return -60f;
            case 1:
                return -45f;
            case 2:
                return -30f;
            case 3:
                return -15f;
            case 4:
                return 15f;
            case 5:
                return 30f;
            case 6:
                return 45f;
            case 7:
                return 60f;
            default:
                break;
        }
        return 0;
    }

    public void SetLight(int _lightID, Color _color)
    {
        _eventMats[_lightID].SetFloat("_InnerGlow", 0);
        _eventMats[_lightID].SetInt("_OuterGlow", 10);
        _eventMats[_lightID].SetColor("_GlowColor", _color);
    }

    public void LightOff(int _lightID)
    {
        _eventMats[_lightID].SetFloat("_InnerGlow", 0);
        _eventMats[_lightID].SetInt("_OuterGlow", 0);
        _eventMats[_lightID].SetColor("_GlowColor", Color.black);
    }
    public void EventColor(int _eventColorType, int _lightID)
    {
        switch ((EventColorType)_eventColorType)
        {
            case EventColorType.LightsOff:
                LightOff(_lightID);
                break;
            case EventColorType.Blue:
            case EventColorType.BlueUnk:
                SetLight(_lightID, _currentSettings.RightLightColor);
                break;
            case EventColorType.Bluefade:
                StartCoroutine(Fade(_lightID, _currentSettings.RightLightColor, 0.1f));
                break;
            case EventColorType.Red:
            case EventColorType.RedUnk:
                SetLight(_lightID, _currentSettings.LeftLightColor);
                break;
            case EventColorType.RedFade:
                StartCoroutine(Fade(_lightID, _currentSettings.LeftLightColor, 0.1f));
                break;
            default:
                if (_eventColorType > 7)
                {
                    SetLight(_lightID, ColourFromInt(_eventColorType));
                }
                break;
        }
    }

    IEnumerator Fade(int _lightID, Color color, float time)
    {
        _eventMats[_lightID].SetInt("_OuterGlow", 10);

        for (float i = 0; i < time || _eventMats[_lightID].GetColor("_GlowColor") != Color.black; i += Time.smoothDeltaTime)
        {            
            _eventMats[_lightID].SetColor("_GlowColor", Color.Lerp(color, Color.black, i));
            yield return null;
        }
    }

    public void CreateMatsForLights(TubeLight[] parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshFilter _mesh = cube.GetComponent<MeshFilter>();
        _eventMats = new List<Material>();

        for (int i = 0; i < (int)LightsID.Unused15; i++)
        {
            Material _newMaterial = new Material(Shader.Find("Custom/Glow"));
            _newMaterial.SetFloat("_InnerGlow", 0);
            _newMaterial.SetInt("_OuterGlow", 0);
            _newMaterial.SetColor("_GlowColor", Color.black);
            _eventMats.Add(_newMaterial);
        }

        foreach (TubeLight _child in parent)
        {
            MeshFilter meshFilter = _child.gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh = _mesh.mesh;
            //_child.gameObject.transform.localScale = new Vector3(_child.width, _child.length, _child.width);

            switch (_child.lightsID)
            {
                case LightsID.Static:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.Static];
                    break;
                case LightsID.BackLights:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.BackLights];
                    break;
                case LightsID.BigRingLights:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.BigRingLights];
                    break;
                case LightsID.LeftLasers:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.LeftLasers];
                    break;
                case LightsID.RightLasers:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.RightLasers];
                    break;
                case LightsID.TrackAndBottom:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.TrackAndBottom];
                    break;
                case LightsID.Unused5:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.Unused5];
                    break;
                case LightsID.Unused6:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.Unused6];
                    break;
                case LightsID.Unused7:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.Unused7];
                    break;
                case LightsID.RingsRotationEffect:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.RingsRotationEffect];
                    break;
                case LightsID.RingsStepEffect:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.RingsStepEffect];
                    break;
                case LightsID.Unused10:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.Unused10];
                    break;
                case LightsID.Unused11:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.Unused11];
                    break;
                case LightsID.RingSpeedLeft:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.RingSpeedLeft];
                    break;
                case LightsID.RingSpeedRight:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.RingSpeedRight];
                    break;
                case LightsID.Unused14:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.Unused14];
                    break;
                case LightsID.Unused15:
                    _child.gameObject.GetComponent<MeshRenderer>().material = _eventMats[(int)LightsID.Unused15];
                    break;
            }
        }
        cube.SetActive(false);
    }
}
