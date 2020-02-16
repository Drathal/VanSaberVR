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
            _note.GetComponentInChildren<Button>().onClick.AddListener(() => _note.AddComponent<NoteSelect>().GoToTarget(i));
            //_note.noteIndex = i;

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

        //if (notes.Length > 0)
            //ChangeToNote(noteIndex);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextNote();
        }
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

    public void NextNote()
    {
        ChangeToNote(noteIndex + 1);
    }

    public void PrevPlatform()
    {
        ChangeToNote(noteIndex - 1);
    }

    public void ChangeToNote(int index, bool save = true)
    {
        currentNote.gameObject.SetActive(false);
        noteIndex = index % notes.Length;
        currentNote.gameObject.SetActive(true);
    }

    public void LoadCurrentNotes()
    {
        /*var _objectPooler = FindObjectsOfType<ObjectPooler>().FirstOrDefault();

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
            MovableObject _wallHandler = _Wall.GetComponent<MovableObject>();

            if (_wallHandler == null)
            {
                _wallHandler = _Wall.AddComponent<MovableObject>();
            }
            
            _objectPooler.obstaclePrefab = _Wall;
            _objectPooler.obstaclePrefab.SetActive(false);
        }

        BoxCollider _collider = null;
        MovableObject _cubeHandler = null;

        if (_NoteLeft != null)
        {
            _collider = _NoteLeft.GetComponent<BoxCollider>();

            if (_collider == null)
            {
                _collider = _NoteLeft.AddComponent<BoxCollider>();
            }

            _cubeHandler = _NoteLeft.GetComponent<MovableObject>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteLeft.AddComponent<MovableObject>();
            }

            _collider.size = new Vector3(1.384117f, 1.28684f, 2.170774f);
            _collider.center = new Vector3(-0.001176417f, -0.002926767f, -0.4800174f);
            _collider.isTrigger = true;
            _objectPooler.NoteLeft = _NoteLeft;
            _objectPooler.NoteLeft.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            _objectPooler.NoteLeft.SetActive(false);
        }

        _collider = null;
        _cubeHandler = null;

        if (_NoteRight != null)
        {
            _collider = _NoteRight.GetComponent<BoxCollider>();

            if (_collider == null)
            {
                _collider = _NoteRight.AddComponent<BoxCollider>();
            }

            _cubeHandler = _NoteRight.GetComponent<MovableObject>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteRight.AddComponent<MovableObject>();
            }

            _collider.size = new Vector3(1.384117f, 1.28684f, 2.170774f);
            _collider.center = new Vector3(-0.001176417f, -0.002926767f, -0.4800174f);
            _collider.isTrigger = true;
            _objectPooler.NoteRight = _NoteRight;
            _objectPooler.NoteRight.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            _objectPooler.NoteRight.SetActive(false);
        }

        _collider = null;
        _cubeHandler = null;

        if (_NoteDotLeft != null)
        {
            _collider = _NoteDotLeft.GetComponent<BoxCollider>();

            if (_collider == null)
            {
                _collider = _NoteDotLeft.AddComponent<BoxCollider>();
            }

            _cubeHandler = _NoteDotLeft.GetComponent<MovableObject>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteDotLeft.AddComponent<MovableObject>();
            }

            _collider.size = new Vector3(1.384117f, 1.28684f, 2.170774f);
            _collider.center = new Vector3(-0.001176417f, -0.002926767f, -0.4800174f);
            _collider.isTrigger = true;
            _objectPooler.NoteDotLeft = _NoteDotLeft;
            _objectPooler.NoteDotLeft.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            _objectPooler.NoteDotLeft.SetActive(false);
        }

        _collider = null;
        _cubeHandler = null;

        if (_NoteDotRight != null)
        {
            _collider = _NoteDotRight.GetComponent<BoxCollider>();

            if (_collider == null)
            {
                _collider = _NoteDotRight.AddComponent<BoxCollider>();
            }

            _cubeHandler = _NoteDotRight.GetComponent<MovableObject>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteDotRight.AddComponent<MovableObject>();
            }

            _collider.size = new Vector3(1.384117f, 1.28684f, 2.170774f);
            _collider.center = new Vector3(-0.001176417f, -0.002926767f, -0.4800174f);
            _collider.isTrigger = true;
            _objectPooler.NoteDotRight = _NoteDotRight;
            _objectPooler.NoteDotRight.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            _objectPooler.NoteDotRight.SetActive(false);
        }

        _collider = null;
        _cubeHandler = null;

        if (_NoteBomb != null)
        {
            SphereCollider _bombCollider = _NoteBomb.GetComponent<SphereCollider>();

            if (_bombCollider == null)
            {
                _bombCollider = _NoteBomb.AddComponent<SphereCollider>();
            }

            _cubeHandler = _NoteBomb.GetComponent<MovableObject>();

            if (_cubeHandler == null)
            {
                _cubeHandler = _NoteBomb.AddComponent<MovableObject>();
            }

            _bombCollider.radius = 0.4f;
            _bombCollider.center = new Vector3(0, 0, 0);
            _bombCollider.isTrigger = true;
            _objectPooler.NoteBomb = _NoteBomb;
            _objectPooler.NoteBomb.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            _objectPooler.NoteBomb.SetActive(false);
        }

        _objectPooler.SetPooledNotes();*/
    }
}