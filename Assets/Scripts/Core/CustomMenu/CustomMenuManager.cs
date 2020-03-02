using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static HelperClass;

public class CustomMenuManager : MonoBehaviour
{
    public static CustomMenuManager Instance;
    private CustomMenuLoader _menuLoader;
    private CustomMenus.MenuDescriptor[] _menus;

    public List<Map> AllSongs = new List<Map>();
    public Map CurrentSong;
    public int CurrentSongIndex;

    public GameObject SelectorPanelPrefab;
    public GameObject SelectorPanel;
    private int _menuIndex = 0;

    public int CurrentMapIndex
    {
        get
        {
            return CurrentSongIndex;
        }
        set
        {
            CurrentSongIndex = value;
        }
    }

    public Map CurrentMap
    {
        get
        {
            return AllSongs[CurrentSongIndex];
        }
    }

    public Map SetMap(int index)
    {
        CurrentSong = AllSongs[index];
        CurrentSongIndex = index;
        return CurrentSong;
    }

    public int currentMenuIndex
    {
        get { return _menuIndex; }
    }
    public CustomMenus.MenuDescriptor currentMenu
    {
        get { return _menus[_menuIndex]; }
    }

    public static void OnLoad(GameObject _SelectorPanel, GameObject _SelectorPanelPrefab)
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Menu Manager");
        CustomMenuManager newManager = go.AddComponent<CustomMenuManager>();
        newManager._menuLoader = new CustomMenuLoader();
        newManager.SelectorPanel = _SelectorPanel;
        newManager.SelectorPanelPrefab = _SelectorPanelPrefab;
        newManager.RefreshMenus();
    }
    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _menuLoader = new CustomMenuLoader();
    }

    public void RefreshMenus()
    {
        if (_menus != null)
        {
            Transform[] ts = GetChildByName(SelectorPanel.gameObject, "MenuContent").transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "MenuContent")
                {
                    continue;
                }

                Destroy(t.gameObject);
            }

            foreach (CustomMenus.MenuDescriptor note in _menus)
            {
                Destroy(note.gameObject);
            }
        }

        _menus = _menuLoader.CreateAllMenus(transform);
        
        for (int i = 0; i < _menus.Length; i++)
        {
            MenuSelect _note = Instantiate(SelectorPanelPrefab).AddComponent<MenuSelect>();

            _note.transform.SetParent(GetChildByName(SelectorPanel, "MenuContent").transform);
            _note.transform.localPosition = SelectorPanelPrefab.transform.localPosition;
            _note.transform.localScale = SelectorPanelPrefab.transform.localScale;
            _note.transform.localRotation = SelectorPanelPrefab.transform.localRotation;
            _note.name = _menus[i].MenuName;
            _note.Index = i;

            Transform[] ts = _note.transform.GetComponentsInChildren<Transform>();
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "name")
                {
                    t.gameObject.GetComponent<Text>().text = _menus[i].MenuName;
                }

                if (t.gameObject.name == "icon")
                {
                    //Image tmp = t.gameObject.GetComponent<Image>();
                    //tmp.mainTexture = notes[i].Icon;
                }
            }
        }

        if (_currentSettings.LastKnownMenu != "")
        {
            for (int i = 0; i < _menus.Length; i++)
            {
                if (_menus[i].MenuName == _currentSettings.LastKnownMenu)
                {
                    SetMenu(i);
                    BuildMenu(SelectorPanel);
                    break;
                }
            }
        }
    }

    public void SetMenu(int index)
    {
        currentMenu.gameObject.SetActive(false);
        _menuIndex = index % _menus.Length;
        currentMenu.gameObject.SetActive(true);
    }
    
    public void BuildMenu(GameObject selector)
    {
        selector.SetActive(false);

        GameObject _menu = currentMenu.gameObject;

        _currentMenuObjects = new CustomMenuObjects()
        {
            _menu = currentMenu.gameObject,
            _MenusButton = GetChildByName(_menu, "MenusButton"),
            _PlayButton = GetChildByName(_menu, "PlayButton"),
            _EditorButton = GetChildByName(_menu, "EditorButton"),
            _SongContent = GetChildByName(_menu, "SongContent"),
            _SongListPreFab = GetChildByName(_menu, "SongListPreFab"),
            _DifficultyContent = GetChildByName(_menu, "DifficultyContent"),
            _DiffListPreFab = GetChildByName(_menu, "DiffListPreFab"),
            _LoadedText = GetChildByName(_menu, "LoadedText"),
            _CustomNoteApplyButton = GetChildByName(_menu, "NoteApplyButton"),
            _CustomSaberApplyButton = GetChildByName(_menu, "SaberApplyButton"),
            _CustomPlatformApplyButton = GetChildByName(_menu, "PlatApplyButton"),
            _PlatformContent = GetChildByName(_menu, "PlatContent"),
            _PlatListPreFab = GetChildByName(_menu, "PlatListPreFab"),
            _NoteContent = GetChildByName(_menu, "NoteContent"),
            _NoteListPreFab = GetChildByName(_menu, "NoteListPreFab"),
            _SaberContent = GetChildByName(_menu, "SaberContent"),
            _SaberListPreFab = GetChildByName(_menu, "SaberListPreFab"),
            _Combo = GetChildByName(_menu, "ComboValue"),
            _Score = GetChildByName(_menu, "ScoreValue"),
            _ScoreUI = GetChildByName(_menu, "ScoreUI"),
            _TimeRemain = GetChildByName(_menu, "New Text")
            //_Multiplier = GetChildByName(_menu, "")
        };

        _currentMenuObjects._MenusButton.GetComponent<Button>().onClick.AddListener(MenuSelectButton);
        _currentMenuObjects._PlayButton.GetComponent<Button>().onClick.AddListener(PlayButton);
        _currentMenuObjects._EditorButton.GetComponent<Button>().onClick.AddListener(EditorButton);

        _currentMenuObjects._CustomNoteApplyButton.GetComponent<Button>().onClick.AddListener(SelectNoteButton);
        _currentMenuObjects._CustomSaberApplyButton.GetComponent<Button>().onClick.AddListener(SelectSaberButton);
        _currentMenuObjects._CustomPlatformApplyButton.GetComponent<Button>().onClick.AddListener(SelectPlatButton);

        StartCoroutine(LoadSongs());
    }

    public void SelectPlatButton()
    {
        _currentSettings.LastKnownPlatform = PlatformManager.Instance.currentPlatform.platName;
        SettingsHandler.Instance.WriteSettings(_currentSettings);

    }

    public void SelectNoteButton()
    {
        _currentSettings.LastKnownNoteSet = NoteManager.Instance.currentNote.NoteName;
        SettingsHandler.Instance.WriteSettings(_currentSettings);
    }

    public void SelectSaberButton()
    {
        _currentSettings.LastKnownSaberSet = CustomSaberManager.Instance.currentSaber.SaberName;
        SettingsHandler.Instance.WriteSettings(_currentSettings);
    }
    public void MenuSelectButton()
    {
        SelectorPanel.SetActive(true);
    }

    public void EditorButton()
    {

    }

    public void PlayButton()
    {
        GameHandler.OnLoad();
    }

    public void UpdateDiffs()
    {
        Transform[] ts1 = _currentMenuObjects._DifficultyContent.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts1)
        {
            if (t.gameObject.name == "DifficultyContent")
                continue;

            Destroy(t.gameObject);
        }

        foreach (DifficultyBeatmapSets BeatmapSet in CurrentMap._difficultyBeatmapSets)
        {
            foreach (DifficultyBeatmap Beatmap in BeatmapSet._difficultyBeatmaps)
            {
                GameObject button = Instantiate(_currentMenuObjects._DiffListPreFab);
                button.transform.SetParent(_currentMenuObjects._DifficultyContent.transform);
                button.transform.localScale = new Vector3(1, 1, 1);
                button.transform.localPosition =
                    new Vector3(button.transform.localPosition.x, button.transform.localPosition.y, 0);
                button.name = Beatmap._difficulty;
                Transform[] ts = button.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in ts)
                {
                    if (t.gameObject.name == "difficulty")
                    {
                        t.gameObject.GetComponent<Text>().text = BeatmapSet._beatmapCharacteristicName + " " + Beatmap._difficulty;
                        button.GetComponent<Button>().onClick.AddListener(() => button.AddComponent<DiffSelect>().GoToTarget(BeatmapSet, Beatmap));
                        button.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public IEnumerator LoadSongs()
    {
        string _dir = Application.dataPath + "/Songs/";
        string[] datFiles = Directory.GetFiles(_dir, "info.dat", SearchOption.AllDirectories);
        string[] jsonFiles = Directory.GetFiles(_dir, "info.json", SearchOption.AllDirectories);
        string[] combinedFiles = datFiles.Concat(jsonFiles).ToArray();
        yield return null;

        int id = 0;

        foreach (string _newFile in combinedFiles)
        {
            bool _dat = (Path.GetExtension(_newFile) == ".dat");
            Map _song;

            if (!_dat)
            {
                _song = JsonMapFile.LoadChart(_newFile);
            }
            else
            {
                _song = DatFile.LoadChart(_newFile);
            }

            if (_song == null)
                continue;

            _song.path = _newFile;

            string path = Path.GetDirectoryName(_newFile);
            string[] eggFiles = Directory.GetFiles(path, "*.egg", SearchOption.AllDirectories);
            string[] oggFiles = Directory.GetFiles(path, "*.ogg", SearchOption.AllDirectories);
            string[] combinedAudioFiles = eggFiles.Concat(oggFiles).ToArray();

            StartCoroutine(AudioHandler.Instance.RecieveAudio(combinedAudioFiles[0], _song));

            yield return null;

            string[] jpgFiles = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);

            if (jpgFiles.Length > 0)
            {

                Texture2D tex = null;
                byte[] fileData;

                if (File.Exists(jpgFiles[0]))
                {
                    fileData = File.ReadAllBytes(jpgFiles[0]);
                    tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);
                }

                _song.icon = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            }

            yield return null;
            
            AllSongs.Add(_song);

            GameObject Song = Instantiate(_currentMenuObjects._SongListPreFab);
            Song.transform.SetParent(_currentMenuObjects._SongContent.transform);
            Song.transform.localScale = new Vector3(1, 1, 1);
            Song.transform.localPosition = new Vector3(Song.transform.localPosition.x, Song.transform.localPosition.y, 0);
            Song.name = id.ToString();
            Song.AddComponent<SongSelect>().arrayID = id;

            Transform[] ts = Song.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "Artist")
                {
                    t.gameObject.GetComponent<Text>().text = _song._songAuthorName;
                }
                if (t.gameObject.name == "SongName")
                {
                    t.gameObject.GetComponent<Text>().text = _song._songName;
                }
                if (t.gameObject.name == "Cover")
                {
                    t.gameObject.GetComponent<Image>().sprite = _song.icon;
                }

                yield return null;

            }

            yield return null;
            Song.SetActive(true);
            id++;
        }

        _currentMenuObjects._LoadedText.GetComponent<Text>().text = combinedFiles.Length.ToString() + " total songs read successfully.";

        yield return null;
    }
}

public class CustomMenuObjects
{
    public GameObject _menu;
    public GameObject _MenusButton;
    public GameObject _PlayButton;
    public GameObject _EditorButton;
    public GameObject _SongContent;
    public GameObject _SongListPreFab;
    public GameObject _DifficultyContent;
    public GameObject _DiffListPreFab;
    public GameObject _LoadedText;
    public GameObject _CustomNoteApplyButton;
    public GameObject _CustomSaberApplyButton;
    public GameObject _CustomPlatformApplyButton;    
    public GameObject _PlatformContent;
    public GameObject _PlatListPreFab;
    public GameObject _NoteContent;
    public GameObject _NoteListPreFab;
    public GameObject _SaberContent;
    public GameObject _SaberListPreFab;
    //public GameObject _SongTime;
    //public GameObject _Bpm;
    //public GameObject _NoteCount;
    //public GameObject _WallCount;
    //public GameObject _BombCount;

    public GameObject _Combo;
    public GameObject _Score;
    public GameObject _TimeRemain;
    public GameObject _Multiplier;
    public GameObject _ScoreUI;
}
