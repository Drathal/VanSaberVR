using CustomFloorPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClass;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;

    public GameObject _ActiveObjects;
    public GameObject _EnvironmentSpin;
    public GameObject _Pool;
    public GameObject _UIRotator;

    public bool _SetupComplete = false;
    public Map _song;
    public TubeLight[] _tubeLights;
    public List<MoveHandler> _objects;
    public int _noteIndex;
    public int _eventIndex;
    public int _obstilcleIndex;

    public float _totalDistance;
    public float _afterDistance;
    public float _speed = 120;

    public float _Angle;
    public float _BeatPerMin;
    public float _BeatPerSec;
    public float _SecPerBeat;
    public float _spawnOffset;
    public float _noteSpeed;
    public float BeatsTime;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject _Manager = new GameObject("Game Manager");
        GameHandler newManager = _Manager.AddComponent<GameHandler>();

        newManager._ActiveObjects = new GameObject("Active Objects");
        newManager._ActiveObjects.transform.SetParent(_Manager.transform);

        newManager._Pool = new GameObject("Pool Objects");
        newManager._Pool.transform.SetParent(_Manager.transform);
        newManager._Pool.AddComponent<PoolHandler>();

        newManager._UIRotator = new GameObject("UIRotator");
        newManager._UIRotator.transform.SetParent(_Manager.transform);
        newManager._UIRotator.AddComponent<UIRotator>();
        //Vector3 pos = _currentMenuObjects._ScoreUI.transform.localPosition;
        _currentMenuObjects._ScoreUI.transform.SetParent(newManager._UIRotator.transform);
        //_currentMenuObjects._ScoreUI.transform.localPosition = pos;

        newManager._EnvironmentSpin = new GameObject("Environment Spin");
        newManager._EnvironmentSpin.transform.SetParent(_Manager.transform);
        newManager._EnvironmentSpin.AddComponent<EnvironmentSpinHandler>();

        newManager.SetupGame();

    }

    public void SetupGame()
    {
        CustomMenuManager.Instance.gameObject.SetActive(false);
        _noteIndex = 0;
        _eventIndex = 0;
        _obstilcleIndex = 0;
        _song = CustomMenuManager.Instance.CurrentMap;
        _objects = new List<MoveHandler>();
        _noteSpeed = _song.TargetDifficulty._noteJumpMovementSpeed;

        if (_noteSpeed < 12)
            _noteSpeed = 12;

        UpdateBeats();

        NoteManager.Instance.LoadCurrentNotes();
        CustomSaberManager.Instance.LoadCurrentSabers();
        EventHander.OnLoad(this);

        _tubeLights = Resources.FindObjectsOfTypeAll<TubeLight>();
        EventHander.Instance.CreateMatsForLights(_tubeLights);

        EnvironmentSpinHandler.Instance.StartUp(this);

        AudioHandler.Instance.stopAllAudio();
        AudioHandler.Instance.setAllAudioTime(0);
       

        _SetupComplete = true;
        
        AudioHandler.Instance.playAllAudio(3);
    }

    public void UpdateBeats()
    {
        _BeatPerMin = _song._beatsPerMinute;
        _BeatPerSec = 60 / _BeatPerMin;
        _SecPerBeat = _BeatPerMin / 60;

        UpdateSpawnTime();
    }

    public void UpdateSpawnTime()
    {
        _totalDistance = _speed * _BeatPerSec * 2;
        _afterDistance = _noteSpeed * _BeatPerSec * 2 * 2f;
        _spawnOffset = _totalDistance / _speed + _afterDistance * 0.5f / _noteSpeed;
    }

    public void Update()
    {
        if (_SetupComplete)
        {
            if (AudioHandler.Instance.currentPlaybackSource.isPlaying)
            {
                BeatsTime = AudioHandler.Instance.SongTime();

                UpdateNotes();
                UpdateObstilcles();
                UpdateEvents();
                UpdateOnTimeEvents();
            }
            else
            {
                if (_noteIndex == _song.TargetDifficulty.level._notes.Count &&
                _obstilcleIndex == _song.TargetDifficulty.level._obstacles.Count)
                {
                    CustomMenuManager.Instance.gameObject.SetActive(true);
                    _currentMenuObjects._ScoreUI.transform.SetParent(_currentMenuObjects._menu.transform);
                    //_currentMenuObjects._ScoreUI.transform.localPosition = _currentMenuObjects._ScoreUI.transform.localPosition;
                    Destroy(gameObject);
                }
            }
        }

    }

    public void UpdateNotes()
    {
        for (int i = _noteIndex; i < _song.TargetDifficulty.level._notes.Count; i++)
        {
            if (_noteIndex < _song.TargetDifficulty.level._notes.Count && _song.TargetDifficulty.level._notes[_noteIndex].TimeInSec() - _spawnOffset < BeatsTime)
            {
                float x = GetX(_song.TargetDifficulty.level._notes[_noteIndex]._lineIndex);
                float y = GetY(_song.TargetDifficulty.level._notes[_noteIndex]._lineLayer);
                float z = _totalDistance + (_afterDistance / 2);

                Vector3 _startZ = new Vector3(x, y, z);
                Vector3 _midZ = new Vector3(x, y, z - _totalDistance);
                Vector3 _endZ = new Vector3(x, y, z - (_afterDistance + _totalDistance));

                GameObject cube = null;

                if (_song.TargetDifficulty.level._notes[_noteIndex]._type == NoteType.BOMB)
                {
                    cube = PoolHandler.Instance.GetPooledBombNote();
                }
                else
                {
                    cube = PoolHandler.Instance.GetPooledNote(_song.TargetDifficulty.level._notes[_noteIndex]);
                }

                var handling = cube.GetComponent<MoveHandler>();
                handling.SetupNote(_startZ, _midZ, _endZ, this, _song.TargetDifficulty.level._notes[_noteIndex], EnvironmentSpinHandler.Instance.currentRotation);
                cube.SetActive(true);
                _objects.Add(handling);

                _noteIndex++;
            }
            else
                break;
        }
    }

    public void UpdateObstilcles()
    {
        for (int i = _obstilcleIndex; i < _song.TargetDifficulty.level._obstacles.Count; i++)
        {
            if (_obstilcleIndex < _song.TargetDifficulty.level._obstacles.Count && _song.TargetDifficulty.level._obstacles[_obstilcleIndex].TimeInSec() - _spawnOffset < BeatsTime)
            {
                float x = GetX(_song.TargetDifficulty.level._obstacles[_obstilcleIndex]._lineIndex);
                float y = _song.TargetDifficulty.level._obstacles[_obstilcleIndex]._type != (float)ObstacleType.CEILING ? 0.1f : 1.3f;
                float z = _totalDistance + (_afterDistance / 2);

                Vector3 _startZ = new Vector3(x, 0, z);
                Vector3 _midZ = new Vector3(x, y, z - _totalDistance);
                Vector3 _endZ = new Vector3(x, y, z - (_afterDistance + _totalDistance));

                GameObject wall = PoolHandler.Instance.GetPooledObstacle();

                var wallHandling = wall.GetComponent<MoveHandler>();
                wallHandling.SetupObstacle(_song.TargetDifficulty.level._obstacles[_obstilcleIndex], this, _startZ, _midZ, _endZ, EnvironmentSpinHandler.Instance.currentRotation);
                wall.SetActive(true);
                _objects.Add(wallHandling);

                _obstilcleIndex++;
            }
            else
                break;
        }
    }
    public void UpdateEvents()
    {
        for (int i = _eventIndex; i < _song.TargetDifficulty.level._events.Count; i++)
        {
            if (_eventIndex < _song.TargetDifficulty.level._events.Count && _song.TargetDifficulty.level._events[_eventIndex].TimeInSec() - _spawnOffset < BeatsTime)
            {
                if (EventHander.Instance.LaneChange(_song.TargetDifficulty.level._events[_eventIndex]))
                {
                    _Angle += EventHander.Instance.RotationValue(_song.TargetDifficulty.level._events[_eventIndex].Value);
                    EnvironmentSpinHandler.Instance.Move(_Angle);
                }
                else
                {
                    _lateEvents.Add(_song.TargetDifficulty.level._events[_eventIndex]);
                }

                _eventIndex++;
            }
            else
                break;
        }
    }

    List<EventData> _lateEvents = new List<EventData>();
    public void UpdateOnTimeEvents()
    {
        for (int i = 0; i < _lateEvents.Count; i++)
        {
            if (_lateEvents[i].TimeInSec() < BeatsTime)
            {
                EventHander.Instance.EventNoteForGame(_lateEvents[i]);
                _lateEvents.Remove(_lateEvents[i]);
            }
            else
                break;
        }
    }
    
    private float GetY(float lineLayer)
    {
        float delta = (1.9f - 1.4f);

        if ((int)lineLayer >= 1000 || (int)lineLayer <= -1000)
        {
            return 1.4f - delta - delta + (((int)lineLayer) * (delta / 1000f));
        }

        if ((int)lineLayer > 2)
        {

            return 1.4f - delta + ((int)lineLayer * delta);
        }

        if ((int)lineLayer < 0)
        {
            return 1.4f - delta + ((int)lineLayer * delta);
        }

        if (lineLayer == 0)
        {
            return 0.85f;
        }
        if (lineLayer == 1)
        {
            return 1.4f;
        }

        return 1.9f;
    }
    public float GetX(float noteindex)
    {
        float num = -1.5f;

        if (noteindex >= 1000 || noteindex <= -1000)
        {
            if (noteindex <= -1000)
                noteindex += 2000;

            num = (num + ((noteindex) * (0.6f / 1000)));
        }
        else
        {
            num = (num + noteindex) * 0.6f;
        }

        return num;
    }
}
