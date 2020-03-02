using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static HelperClass;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;
    private NoteLoader noteLoader;

    private CustomNotes.NoteDescriptor[] notes;
    private int noteIndex = 0;

    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Note Manager");
        NoteManager newManager = go.AddComponent<NoteManager>();
        newManager.noteLoader = new NoteLoader();
        newManager.RefreshNotes();
    }

    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        noteLoader = new NoteLoader();
    }

    public CustomNotes.NoteDescriptor AddNote(string path)
    {
        CustomNotes.NoteDescriptor newNote = noteLoader.LoadNoteBundle(path, transform);
        if (newNote != null)
        {
            var noteList = notes.ToList();
            noteList.Add(newNote);
            notes = noteList.ToArray();
        }

        return newNote;
    }

    public void RefreshNotes()
    {
        if (notes != null)
        {
            Transform[] ts = _currentMenuObjects._NoteContent.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == _currentMenuObjects._NoteContent.name)
                {
                    continue;
                }

                Destroy(t.gameObject);
            }

            foreach (CustomNotes.NoteDescriptor note in notes)
            {
                Destroy(note.gameObject);
            }
        }

        notes = noteLoader.CreateAllNotes(transform);

        for (int i = 0; i < notes.Length; i++)
        {
            GameObject _note = Instantiate(_currentMenuObjects._NoteListPreFab);
            _note.transform.SetParent(_currentMenuObjects._NoteContent.transform);
            _note.transform.localPosition = _currentMenuObjects._NoteListPreFab.transform.localPosition;
            _note.transform.localScale = _currentMenuObjects._NoteListPreFab.transform.localScale;
            _note.transform.localRotation = _currentMenuObjects._NoteListPreFab.transform.localRotation;
            _note.name = notes[i].NoteName;
            NoteSelect _noteSelect = _note.AddComponent<NoteSelect>();
            _noteSelect.index = i;
            _note.GetComponentInChildren<Button>().onClick.AddListener(() => _noteSelect.GoToTarget());

            Transform[] ts = _note.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "Name")
                {
                    t.gameObject.GetComponent<Text>().text = notes[i].NoteName;
                }

                if (t.gameObject.name == "Artist")
                {
                    t.gameObject.GetComponent<Text>().text = notes[i].AuthorName;
                }

                if (t.gameObject.name == "Cover")
                {
                    //Image tmp = t.gameObject.GetComponent<Image>();
                    //tmp.sprite = notes[i].Icon;
                }
            }

            _note.SetActive(true);
        }
        
        for (int i = 0; i < notes.Length; i++)
        {
            if (currentNote.NoteName + currentNote.AuthorName ==
                notes[i].NoteName + notes[i].AuthorName)
            {
                noteIndex = i;
                break;
            }
        }

        if (_currentSettings.LastKnownNoteSet != "")
        {
            for (int i = 0; i < notes.Length; i++)
            {
                if (notes[i].NoteName == _currentSettings.LastKnownNoteSet)
                {
                    ChangeToNote(i);
                    break;
                }
            }
        }

        //if (notes.Length > 0)
        //ChangeToNote(noteIndex);
    }
    

    public int currentNoteIndex
    {
        get { return noteIndex; }
    }

    public CustomNotes.NoteDescriptor currentNote
    {
        get { return notes[noteIndex]; }
    }

    public CustomNotes.NoteDescriptor[] GetNotes()
    {
        return notes;
    }

    public CustomNotes.NoteDescriptor GetNote(int i)
    {
        return notes.ElementAt(i);
    }
    
    public void ChangeToNote(int index)
    {
        currentNote.gameObject.SetActive(false);
        noteIndex = index % notes.Length;
        currentNote.gameObject.SetActive(true);
    }

    public void LoadCurrentNotes()
    {
        GameObject _Wall = GetChildByName(currentNote.gameObject, "Wall");

        if (_Wall == null)
        {
            _Wall = GetChildByName(GetNote(0).gameObject, "Wall");
        }

        GameObject _NoteLeft = GetChildByName(currentNote.gameObject, "NoteLeft");

        if (_NoteLeft == null)
        {
            _NoteLeft = GetChildByName(GetNote(0).gameObject, "NoteLeft");
        }

        GameObject _NoteRight = GetChildByName(currentNote.gameObject, "NoteRight");

        if (_NoteRight == null)
        {
            _NoteRight = GetChildByName(GetNote(0).gameObject, "NoteRight");
        }

        GameObject _NoteDotLeft = GetChildByName(currentNote.gameObject, "NoteDotLeft");

        if (_NoteDotLeft == null)
        {
            _NoteDotLeft = GetChildByName(GetNote(0).gameObject, "NoteDotLeft");
        }

        GameObject _NoteDotRight = GetChildByName(currentNote.gameObject, "NoteDotRight");

        if (_NoteDotRight == null)
        {
            _NoteDotRight = GetChildByName(GetNote(0).gameObject, "NoteDotRight");
        }

        GameObject _NoteBomb = GetChildByName(currentNote.gameObject, "NoteBomb");

        if (_NoteBomb == null)
        {
            _NoteBomb = GetChildByName(GetNote(0).gameObject, "NoteBomb");
        }

        if (_Wall != null)
        {
            MoveHandler _wallHandler = _Wall.GetComponent<MoveHandler>();

            if (_wallHandler == null)
            {
                _wallHandler = _Wall.AddComponent<MoveHandler>();
            }

            Transform[] _Children = _Wall.transform.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < _Children.Length; i++)
            {
                BoxCollider _wallCollider = _Children[i].gameObject.GetComponent<BoxCollider>();

                if (_wallCollider == null)
                {
                    _wallCollider = _Children[i].gameObject.AddComponent<BoxCollider>();
                }

                _wallCollider.isTrigger = true;
            }

            PoolHandler.Instance.obstaclePrefab = _Wall;
            PoolHandler.Instance.obstaclePrefab.SetActive(false);
        }

        BoxCollider _collider = null;
        MoveHandler _cubeHandler = null;

        if (_NoteLeft != null)
        {
            _collider = _NoteLeft.transform.GetChild(0).gameObject.GetComponent<BoxCollider>();

            if (_collider == null)
            {
                _collider = _NoteLeft.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
            }

            _cubeHandler = _NoteLeft.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteLeft.AddComponent<MoveHandler>();
            }

            _collider.size = new Vector3(1.384117f, 1.28684f, 2.170774f);
            _collider.center = new Vector3(-0.001176417f, -0.002926767f, -0.4800174f);
            _collider.isTrigger = true;
            PoolHandler.Instance.NoteLeft = _NoteLeft;
            PoolHandler.Instance.NoteLeft.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            PoolHandler.Instance.NoteLeft.SetActive(false);
        }

        _collider = null;
        _cubeHandler = null;

        if (_NoteRight != null)
        {
            _collider = _NoteRight.transform.GetChild(0).gameObject.GetComponent<BoxCollider>();

            if (_collider == null)
            {
                _collider = _NoteRight.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
            }

            _cubeHandler = _NoteRight.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteRight.AddComponent<MoveHandler>();
            }

            _collider.size = new Vector3(1.384117f, 1.28684f, 2.170774f);
            _collider.center = new Vector3(-0.001176417f, -0.002926767f, -0.4800174f);
            _collider.isTrigger = true;
            PoolHandler.Instance.NoteRight = _NoteRight;
            PoolHandler.Instance.NoteRight.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            PoolHandler.Instance.NoteRight.SetActive(false);
        }

        _collider = null;
        _cubeHandler = null;

        if (_NoteDotLeft != null)
        {
            _collider = _NoteDotLeft.transform.GetChild(0).gameObject.GetComponent<BoxCollider>();

            if (_collider == null)
            {
                _collider = _NoteDotLeft.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
            }

            _cubeHandler = _NoteDotLeft.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteDotLeft.AddComponent<MoveHandler>();
            }

            _collider.size = new Vector3(1.384117f, 1.28684f, 2.170774f);
            _collider.center = new Vector3(-0.001176417f, -0.002926767f, -0.4800174f);
            _collider.isTrigger = true;
            PoolHandler.Instance.NoteDotLeft = _NoteDotLeft;
            PoolHandler.Instance.NoteDotLeft.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            PoolHandler.Instance.NoteDotLeft.SetActive(false);
        }

        _collider = null;
        _cubeHandler = null;

        if (_NoteDotRight != null)
        {
            _collider = _NoteDotRight.transform.GetChild(0).gameObject.GetComponent<BoxCollider>();

            if (_collider == null)
            {
                _collider = _NoteDotRight.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
            }

            _cubeHandler = _NoteDotRight.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteDotRight.AddComponent<MoveHandler>();
            }

            _collider.size = new Vector3(1.384117f, 1.28684f, 2.170774f);
            _collider.center = new Vector3(-0.001176417f, -0.002926767f, -0.4800174f);
            _collider.isTrigger = true;
            PoolHandler.Instance.NoteDotRight = _NoteDotRight;
            PoolHandler.Instance.NoteDotRight.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            PoolHandler.Instance.NoteDotRight.SetActive(false);
        }

        _collider = null;
        _cubeHandler = null;

        if (_NoteBomb != null)
        {
            SphereCollider _bombCollider = _NoteBomb.transform.GetChild(0).gameObject.GetComponent<SphereCollider>();

            if (_bombCollider == null)
            {
                _bombCollider = _NoteBomb.transform.GetChild(0).gameObject.AddComponent<SphereCollider>();
            }

            _cubeHandler = _NoteBomb.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteBomb.AddComponent<MoveHandler>();
            }

            _bombCollider.radius = 0.4f;
            _bombCollider.center = new Vector3(0, 0, 0);
            _bombCollider.isTrigger = true;
            PoolHandler.Instance.NoteBomb = _NoteBomb;
            PoolHandler.Instance.NoteBomb.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            PoolHandler.Instance.NoteBomb.SetActive(false);
        }

        PoolHandler.Instance.SetPools();
    }
}