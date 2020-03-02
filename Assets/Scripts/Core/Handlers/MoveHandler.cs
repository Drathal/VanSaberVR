using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClass;

public class MoveHandler : MonoBehaviour
{
    public float _height; // Obstacle
    private float length; // Obstacle
    private float _obstacleDuration = 0; // Obstacle
    public ObstacleData _obstacle; // Obstacle
    
    Quaternion _startRotation = default; // Note
    Quaternion _finalRotation = default; // Note
    public bool _wasHit = false; // Note or Bomb
    private bool _wasMissed = false; // Note
    private bool _canMakeNoise = true; // Note or Bomb
    public NoteData _note; // Note

    private float _Offset;
    private float _OffsetPercent;
    public float _objectTime;
    public float _oldObjectTime;
    public float _newObjectTime;
    public Color _startBarColor;
    public Color _endBarColor;
    
    public float _laneAngle;
    public GameHandler _objectHandler;
    public GameObject newBar;
    public Material newBarMat;

    public Vector3 _startPos; //
    public Vector3 _midPos;   // Both Note 
    public Vector3 _endPos;   // and Obstacle
    public Vector3 _timePos;  //

    public void UpdateObjects()
    {
        _Offset = AudioHandler.Instance.SongTime() - (_objectTime - _objectHandler._spawnOffset + (_objectHandler._totalDistance / _objectHandler._BeatPerMin));
        _OffsetPercent = _Offset / (_objectHandler._afterDistance / _objectHandler._noteSpeed);

        if (_OffsetPercent >= 1f && _wasMissed || _OffsetPercent >= 1f + (Mathf.Abs(_obstacleDuration) * _objectHandler._BeatPerSec) || _OffsetPercent >= 0.5f && _obstacleDuration < 0)
        {
            if (newBar != null)
            {
                Destroy(newBar);
            }

            transform.position = new Vector3(0, 1000, 0);
            transform.rotation = Quaternion.identity;
            _objectHandler._objects.Remove(this);
            transform.SetParent(_objectHandler._Pool.transform);
            gameObject.SetActive(false);
            return;
        }

        if (_note != null)
        {
            if (_note._type != NoteType.BOMB && _OffsetPercent < 0.65f)
            {
                Quaternion _lookPos = default;
                _lookPos.SetLookRotation(transform.position - new Vector3(EngineStart.Instance.PlayerHead.transform.position.x, transform.position.y, EngineStart.Instance.PlayerHead.transform.position.z), transform.up);
                transform.GetChild(0).localRotation = Quaternion.Lerp(_startRotation, _finalRotation, _OffsetPercent < 0.125 ? _OffsetPercent * 8 : 1);
                transform.rotation = _lookPos;
            }

            if (_OffsetPercent >= 0.65f && !_wasMissed)
            {
                if (_note._type != NoteType.BOMB)
                {
                    _wasMissed = true;
                    UIRotator.Instance.UpdateScore(0, true);
                }
            }
        }

        if (_OffsetPercent > 0)
        {
            if (_objectHandler._song.TargetBeatMapSet._beatmapCharacteristicName.Contains("360"))
                SpawnBar();

            _timePos.x = _midPos.x;
            _timePos.y = _midPos.y;
            _timePos.z = Mathf.LerpUnclamped(_midPos.z + EngineStart.Instance.PlayerHead.transform.position.z * (_OffsetPercent > 0.5f ? 1 : _OffsetPercent * 2), _endPos.z + EngineStart.Instance.PlayerHead.transform.position.z, _OffsetPercent);
        }
        else
        {
            _timePos = Vector3.LerpUnclamped(_startPos, _midPos, _OffsetPercent + 1);
        }

        Vector3 vector3 = Quaternion.Euler(0.0f, _laneAngle, 0.0f) * _timePos;
        transform.position = vector3;

        if (newBar != null && _OffsetPercent >= 0.5f)
        {
            Destroy(newBar);
        }      
    }

    bool _canSpawn = true;
    void SpawnBar()
    {
        if (newBar != null)
        {
            if (newBar.activeInHierarchy)
                newBarMat.SetColor("_TintColor", Color.Lerp(_startBarColor, _endBarColor, _OffsetPercent));
            else
                newBar.SetActive(true);
        }

        if (!_canSpawn)
            return;

        _canSpawn = false;
        UIRotator.Instance.Move(_laneAngle);
        //newBar = Instantiate(EnvironmentSpin.Instance._spawnBar, transform.parent);
        //newBar.transform.position = Quaternion.Euler(0.0f, _laneAngle, 0.0f) * new Vector3(_midPos.x, 0, 0);
        //newBar.transform.rotation = Quaternion.Euler(0.0f, _laneAngle, 0.0f) * newBar.transform.rotation;
        //newBar.transform.SetParent(transform.parent);

        //newBarMat = newBar.GetComponent<MeshRenderer>().material;

        //Color temp = newBarMat.GetColor("_TintColor");
        //_startBarColor = new Color(temp.r, temp.g, temp.b, 0);
        //_endBarColor = new Color(temp.r, temp.g, temp.b, 0.15f);
        //newBar.SetActive(false);
    }

    public void SetupObstacle(ObstacleData obstacle, GameHandler refNotesSpawner, Vector3 startPos, Vector3 midPos, Vector3 endPos, float angle)
    {
        _obstacleDuration = 0;
        _objectHandler = refNotesSpawner;
        _laneAngle = angle;
        transform.rotation = Quaternion.Euler(0.0f, _laneAngle, 0.0f);
        _obstacle = obstacle;
        _wasMissed = false;
        _obstacleDuration = _obstacle._duration;

        if (CustomMenuManager.Instance.CurrentMap.TargetDifficulty._customData != null)
            if (CustomMenuManager.Instance.CurrentMap.TargetDifficulty._customData.EditorOffset != 0)
                _obstacle._time -= (float)CustomMenuManager.Instance.CurrentMap.TargetDifficulty._customData.EditorOffset / 100;

        _objectTime = _obstacle.TimeInSec();
        _startPos = startPos;
        _midPos = midPos;
        _endPos = endPos;
        _startPos.z += _currentSettings.ArmDistance;
        _midPos.z += _currentSettings.ArmDistance;
        _endPos.z += _currentSettings.ArmDistance;
        _canSpawn = true;
        _height = (_obstacle._type != (float)ObstacleType.CEILING) ? 3f : 1.5f;

        var _distance = (_endPos - _midPos).magnitude / (_objectHandler._afterDistance / _objectHandler._noteSpeed);
        length = _distance * (_obstacle._duration * _objectHandler._BeatPerSec);

        if (_obstacle._width >= 1000 ||
            (((int)_obstacle._type >= 1000 && (int)_obstacle._type <= 4000) ||
             ((int)_obstacle._type >= 4001 && (int)_obstacle._type <= 4005000)))
        {
            Mode mode = ((int)_obstacle._type >= 4001 && (int)_obstacle._type <= 4100000)
                ? Mode.preciseHeightStart
                : Mode.preciseHeight;
            int height = 0;
            int startHeight = 0;
            if (mode == Mode.preciseHeightStart)
            {
                int value = (int)_obstacle._type;
                value -= 4001;
                height = value / 1000;
                startHeight = value % 1000;
            }
            else
            {
                int value = (int)_obstacle._type;
                height = value - 1000;
            }

            float num = 0;
            if ((_obstacle._width >= 1000) || (mode == Mode.preciseHeightStart))
            {

                float width = _obstacle._width - 1000;
                float precisionLineWidth = 0.6f / 1000;
                num = width * precisionLineWidth;
                Vector3 b = new Vector3((num - 0.6f) * 0.5f, 4 * ((float)startHeight / 1000), 0f);
                _startPos = startPos + b;
                _midPos = midPos + b;
                _endPos = endPos + b;

            }
            else
                num = _obstacle._width * 0.6f;

            float multiplier = 1f;
            if ((int)_obstacle._type >= 1000)
            {
                multiplier = height / 1000f;
            }

            BuildWall(num * 0.98f, _height * multiplier, length);
        }
        else
        {
            float num = _obstacle._width * 0.6f;
            Vector3 b = new Vector3((num - 0.6f) * 0.5f, 0f, 0f);
            _startPos = startPos + b;
            _midPos = midPos + b;
            _endPos = endPos + b;

            BuildWall(num * 0.98f, _height, length);
        }
    }

    public void BuildWall(float width, float height, float length, float _thickness = 0.02f)
    {
        int _frameCount = 0;

        Transform[] _Children = transform.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < _Children.Length; i++)
        {
            if (_Children[i].gameObject.name == "WallCore")
            {
                _Children[i].localScale = new Vector3(width, height, length);
                _Children[i].localPosition = new Vector3(0f, height * 0.5f, length * 0.5f);
            }
            else if (_Children[i].gameObject.name == "frame")
            {
                switch (_frameCount)
                {
                    case 0:
                        _Children[i].localScale = new Vector3(width, _thickness, _thickness);
                        _Children[i].localPosition = new Vector3(0f, height, 0f);
                        break;
                    case 1:
                        _Children[i].localScale = new Vector3(width, _thickness, _thickness);
                        _Children[i].localPosition = new Vector3(0f, 0f, 0f);
                        break;
                    case 2:
                        _Children[i].localScale = new Vector3(width, _thickness, _thickness);
                        _Children[i].localPosition = new Vector3(0f, height, length);
                        break;
                    case 3:
                        _Children[i].localScale = new Vector3(width, _thickness, _thickness);
                        _Children[i].localPosition = new Vector3(0f, 0f, length);
                        break;
                    case 4:
                        _Children[i].localScale = new Vector3(_thickness, height, _thickness);
                        _Children[i].localPosition = new Vector3(-width * 0.5f, height * 0.5f, 0f);
                        break;
                    case 5:
                        _Children[i].localScale = new Vector3(_thickness, height, _thickness);
                        _Children[i].localPosition = new Vector3(width * 0.5f, height * 0.5f, 0f);
                        break;
                    case 6:
                        _Children[i].localScale = new Vector3(_thickness, height, _thickness);
                        _Children[i].localPosition = new Vector3(-width * 0.5f, height * 0.5f, length);
                        break;
                    case 7:
                        _Children[i].localScale = new Vector3(_thickness, height, _thickness);
                        _Children[i].localPosition = new Vector3(width * 0.5f, height * 0.5f, length);
                        break;
                    case 8:
                        _Children[i].localScale = new Vector3(_thickness, _thickness, length);
                        _Children[i].localPosition = new Vector3(-width * 0.5f, height, length * 0.5f);
                        break;
                    case 9:
                        _Children[i].localScale = new Vector3(_thickness, _thickness, length);
                        _Children[i].localPosition = new Vector3(width * 0.5f, height, length * 0.5f);
                        break;
                    case 10:
                        _Children[i].localScale = new Vector3(_thickness, _thickness, length);
                        _Children[i].localPosition = new Vector3(-width * 0.5f, 0f, length * 0.5f);
                        break;
                    case 11:
                        _Children[i].localScale = new Vector3(_thickness, _thickness, length);
                        _Children[i].localPosition = new Vector3(width * 0.5f, 0f, length * 0.5f);
                        break;
                }

                _frameCount++;
            }
        }
    }

    public void SetupNote(Vector3 startPos, Vector3 midPos, Vector3 endPos, GameHandler _notesSpawner, NoteData note, float angle)
    {
        _obstacleDuration = 0;
        _wasHit = false;
        _wasMissed = false;
        _objectHandler = _notesSpawner;
        _laneAngle = angle;
        _note = note;

        if (CustomMenuManager.Instance.CurrentMap.TargetDifficulty._customData != null)
            if (CustomMenuManager.Instance.CurrentMap.TargetDifficulty._customData.EditorOffset != 0)
                _note._time -= (float)CustomMenuManager.Instance.CurrentMap.TargetDifficulty._customData.EditorOffset / 100* GameHandler.Instance._BeatPerSec;

        _objectTime = _note.TimeInSec();
        _startPos = startPos;
        _midPos = midPos;
        _endPos = endPos;

        _startPos.z += _currentSettings.ArmDistance;
        _midPos.z += _currentSettings.ArmDistance;
        _endPos.z += _currentSettings.ArmDistance;
        _canSpawn = true;
        _canMakeNoise = true;
        _startRotation = transform.rotation;
        
        if (note._type != NoteType.BOMB)
            SetRotation();
    }

    public void SetRotation()
    {
        _finalRotation = default;
        switch (_note._cutDirection)
        {
            case CutDirection.BOTTOM:
                _finalRotation.eulerAngles = new Vector3(0f, 0f, 180f);
                break;
            case CutDirection.TOP:
                _finalRotation = Quaternion.identity;
                break;
            case CutDirection.RIGHT:
                _finalRotation.eulerAngles = new Vector3(0f, 0f, 90f);
                break;
            case CutDirection.LEFT:
                _finalRotation.eulerAngles = new Vector3(0f, 0f, -90f);
                break;
            case CutDirection.BOTTOMLEFT:
                _finalRotation.eulerAngles = new Vector3(0f, 0f, -135f);
                break;
            case CutDirection.BOTTOMRIGHT:
                _finalRotation.eulerAngles = new Vector3(0f, 0f, 135f);
                break;
            case CutDirection.TOPLEFT:
                _finalRotation.eulerAngles = new Vector3(0f, 0f, -45f);
                break;
            case CutDirection.TOPRIGHT:
                _finalRotation.eulerAngles = new Vector3(0f, 0f, 45f);
                break;
            default:
                _finalRotation = Quaternion.identity;
                break;
        }

        if ((int)_note._cutDirection >= 1000 && (int)_note._cutDirection <= 1360)
        {
            int angle = 1000 - (int)_note._cutDirection;
            _finalRotation.eulerAngles = new Vector3(0f, 0f, 1000 - (int)_note._cutDirection);
        }
        else if ((int)_note._cutDirection >= 2000 && (int)_note._cutDirection <= 2360)
        {
            int angle = 2000 - (int)_note._cutDirection;
            _finalRotation.eulerAngles = new Vector3(0f, 0f, 2000 - (int)_note._cutDirection);
        }
    }
}