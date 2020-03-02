using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolHandler : MonoBehaviour
{
    public static PoolHandler Instance;
    public int PoolAmount = 100;

    public List<GameObject> PooledNoteLeft;
    public List<GameObject> PooledSpark;
    public List<GameObject> PooledNoteRight;
    public List<GameObject> PooledNoteDotLeft;
    public List<GameObject> PooledNoteDotRight;
    public List<GameObject> PooledNoteBomb;
    public List<GameObject> PooledObstacle;
    public List<GameObject> PooledChunks;
    public List<GameObject> PooledHitSound;
    public GameObject Spark;
    public GameObject NoteLeft;
    public GameObject NoteRight;
    public GameObject NoteDotLeft;
    public GameObject NoteDotRight;
    public GameObject NoteBomb;
    public GameObject Chunk;
    public GameObject obstaclePrefab;
    public GameObject sound;
    public AudioClip[] HitSound;
    public TextMesh text;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    public void Update()
    {
        for (int i = 0; i < GameHandler.Instance._objects.Count; i++)
        {
            GameHandler.Instance._objects[i].UpdateObjects();
        }
    }

    public void SetPools()
    {
        /*PooledSpark = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = Instantiate(Spark, transform);
            newNote.transform.position = new Vector3(0, 1000f, 0);
            newNote.SetActive(false);
            PooledSpark.Add(newNote);
        }

        PooledHitSound = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = Instantiate(sound, transform);
            newNote.transform.position = new Vector3(0, 0, 0);
            newNote.SetActive(false);
            PooledHitSound.Add(newNote);
        }

        PooledChunks = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = new GameObject();
            newNote.transform.SetParent(transform);
            newNote.transform.position = new Vector3(0, 1000, 0);
            //newNote.SetActive(false);
            PooledChunks.Add(newNote);
        }*/

        PooledObstacle = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = Instantiate(obstaclePrefab, transform);
            newNote.transform.position = new Vector3(0, 1000, 0);
            newNote.SetActive(false);
            PooledObstacle.Add(newNote);
        }

        PooledNoteLeft = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = Instantiate(NoteLeft, transform);
            newNote.transform.position = new Vector3(0, 1000, 0);
            PooledNoteLeft.Add(newNote);
        }

        PooledNoteRight = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = Instantiate(NoteRight, transform);
            newNote.transform.position = new Vector3(0, 1000, 0);
            PooledNoteRight.Add(newNote);
        }

        PooledNoteDotLeft = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = Instantiate(NoteDotLeft, transform);
            newNote.transform.position = new Vector3(0, 1000, 0);
            PooledNoteDotLeft.Add(newNote);
        }

        PooledNoteDotRight = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = Instantiate(NoteDotRight, transform);
            newNote.transform.position = new Vector3(0, 1000, 0);
            PooledNoteDotRight.Add(newNote);
        }

        PooledNoteBomb = new List<GameObject>();

        for (int i = 0; i < PoolAmount; i++)
        {
            GameObject newNote = Instantiate(NoteBomb, transform);
            newNote.transform.position = new Vector3(0, 1000, 0);
            PooledNoteBomb.Add(newNote);
        }
    }

    public GameObject GetPooledSpark()
    {
        for (int i = 0; i < PooledSpark.Count; i++)
        {
            if (!PooledSpark[i].activeInHierarchy)
            {
                PooledSpark[i].transform.localRotation = Quaternion.identity;
                return PooledSpark[i];
            }
        }

        GameObject newSpark = Instantiate(Spark, transform);
        PooledSpark.Add(newSpark);
        return newSpark;
    }

    public GameObject GetPooledBombNote()
    {
        for (int i = 0; i < PooledNoteBomb.Count; i++)
        {
            if (!PooledNoteBomb[i].activeInHierarchy)
            {
                PooledNoteBomb[i].transform.localRotation = Quaternion.identity;
                return PooledNoteBomb[i];
            }
        }

        GameObject newBomb = Instantiate(NoteBomb, transform);
        MoveHandler _cubeHandler = newBomb.GetComponent<MoveHandler>();

        if (_cubeHandler == null)
        {
            _cubeHandler = newBomb.AddComponent<MoveHandler>();
        }

        PooledNoteBomb.Add(newBomb);
        return newBomb;
    }

    public GameObject GetPooledChunk()
    {
        for (int i = 0; i < PooledChunks.Count; i++)
        {
            PooledChunks[i].transform.localRotation = Quaternion.identity;
            return PooledChunks[i];
        }

        GameObject newChunk = new GameObject();
        newChunk.transform.SetParent(transform);
        PooledChunks.Add(newChunk);
        return newChunk;
    }

    public GameObject GetPooledNote(NoteData _note, bool isAny = false)
    {
        if (_note._type == NoteType.LEFT && _note._cutDirection != CutDirection.NONDIRECTION)
        {
            for (int i = 0; i < PooledNoteLeft.Count; i++)
            {
                if (!PooledNoteLeft[i].activeInHierarchy)
                {
                    PooledNoteLeft[i].transform.localRotation = Quaternion.identity;
                    return PooledNoteLeft[i];
                }
            }

            GameObject newLeftNote = Instantiate(NoteLeft, transform);
            MoveHandler _cubeHandler = newLeftNote.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                newLeftNote.AddComponent<MoveHandler>();
            }
            PooledNoteLeft.Add(newLeftNote);
            return newLeftNote;
        }
        else if (_note._type == NoteType.RIGHT && _note._cutDirection != CutDirection.NONDIRECTION)
        {
            for (int i = 0; i < PooledNoteRight.Count; i++)
            {
                if (!PooledNoteRight[i].activeInHierarchy)
                {
                    PooledNoteRight[i].transform.localRotation = Quaternion.identity;
                    return PooledNoteRight[i];
                }
            }

            GameObject newRightNote = Instantiate(NoteRight, transform);
            MoveHandler _cubeHandler = newRightNote.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                newRightNote.AddComponent<MoveHandler>();
            }
            PooledNoteRight.Add(newRightNote);
            return newRightNote;
        }
        else if (_note._type == NoteType.LEFT && _note._cutDirection == CutDirection.NONDIRECTION)
        {
            for (int i = 0; i < PooledNoteDotLeft.Count; i++)
            {
                if (!PooledNoteDotLeft[i].activeInHierarchy)
                {
                    PooledNoteDotLeft[i].transform.localRotation = Quaternion.identity;
                    return PooledNoteDotLeft[i];
                }
            }

            GameObject newLeftDotNote = Instantiate(NoteDotLeft, transform);
            MoveHandler _cubeHandler = newLeftDotNote.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                newLeftDotNote.AddComponent<MoveHandler>();
            }
            PooledNoteDotLeft.Add(newLeftDotNote);
            return newLeftDotNote;
        }
        else if (_note._type == NoteType.RIGHT && _note._cutDirection == CutDirection.NONDIRECTION)
        {
            for (int i = 0; i < PooledNoteDotRight.Count; i++)
            {
                if (!PooledNoteDotRight[i].activeInHierarchy)
                {
                    PooledNoteDotRight[i].transform.localRotation = Quaternion.identity;
                    return PooledNoteDotRight[i];
                }
            }

            GameObject newRightDotNote = Instantiate(NoteDotRight, transform);
            MoveHandler _cubeHandler = newRightDotNote.GetComponent<MoveHandler>();

            if (_cubeHandler == null)
            {
                newRightDotNote.AddComponent<MoveHandler>();
            }
            PooledNoteDotRight.Add(newRightDotNote);
            return newRightDotNote;
        }

        return null;
    }
    public GameObject GetPooledObstacle()
    {
        for (int i = 0; i < PooledObstacle.Count; i++)
        {
            if (!PooledObstacle[i].activeInHierarchy)
            {
                return PooledObstacle[i];
            }
        }

        GameObject newobsticale = Instantiate(obstaclePrefab, transform);
        MoveHandler _obsticaleHandler = newobsticale.GetComponent<MoveHandler>();

        if (_obsticaleHandler == null)
        {
            newobsticale.AddComponent<MoveHandler>();
        }
        PooledObstacle.Add(newobsticale);
        return newobsticale;
    }

    public GameObject GetPooledHitSound(string name)
    {
        for (int i = 0; i < PooledHitSound.Count; i++)
        {
            if (!PooledHitSound[i].activeInHierarchy)
            {
                foreach (AudioClip clip in HitSound)
                {
                    if (clip.name == name)
                    {
                        PooledHitSound[i].GetComponent<AudioSource>().clip = clip;
                        return PooledHitSound[i];
                    }
                }
            }
        }

        foreach (AudioClip clip in HitSound)
        {
            if (clip.name == name)
            {
                GameObject newPurple = Instantiate(sound, transform);
                newPurple.GetComponent<AudioSource>().clip = clip;
                newPurple.name = newPurple.name;
                PooledHitSound.Add(newPurple);
                return newPurple;
            }
        }

        return null;
    }

    public void ChangeNoteColor(Color color, NoteData _note)
    {

    }
    public void ChangeWallColor(Color color)
    {

    }
}
