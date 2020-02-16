using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NoteLoader : MonoBehaviour
{
    private const string customFolder = "CustomNotes";

    private List<string> bundlePaths;
    private List<CustomNotes.NoteDescriptor> notes;
    
    public CustomNotes.NoteDescriptor[] CreateAllNotes(Transform parent)
    {
        string customNotesFolderPath = Path.Combine(Application.dataPath, customFolder);
        
        if (!Directory.Exists(customNotesFolderPath))
        {
            Directory.CreateDirectory(customNotesFolderPath);
        }
        
        List<string> allBundlePaths = Directory.GetFiles(customNotesFolderPath, "*.note", SearchOption.TopDirectoryOnly).Concat(Directory.GetFiles(customNotesFolderPath, "*.bloq", SearchOption.TopDirectoryOnly)).ToList();
        
        notes = new List<CustomNotes.NoteDescriptor>();
        bundlePaths = new List<string>();

        /*GameObject defaultNoteSetGO = Instantiate(_DefaultNotes);
        CustomNotes.NoteDescriptor defaultNoteSet = defaultNoteSetGO.GetComponent<CustomNotes.NoteDescriptor>();

        defaultNoteSetGO.transform.parent = parent;
        defaultNoteSetGO.transform.position = new Vector3(0, 0, 4);
        defaultNoteSetGO.name = defaultNoteSet.NoteName + " by " + defaultNoteSet.AuthorName;

        if (defaultNoteSet.Icon == null)
            defaultNoteSet.Icon = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "FeetIcon")
                .FirstOrDefault().texture;

        defaultNoteSetGO.SetActive(false);
        notes.Add(defaultNoteSet);*/

        for (int i = 0; i < allBundlePaths.Count; i++)
        {
            CustomNotes.NoteDescriptor newNote = LoadNoteBundle(allBundlePaths[i], parent);
        }

        return notes.ToArray();
    }


    public CustomNotes.NoteDescriptor LoadNoteBundle(string bundlePath, Transform parent)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
        {
            return null;
        }

        CustomNotes.NoteDescriptor newNote = LoadNote(bundle, parent);
        if (newNote != null)
        {
            bundlePaths.Add(bundlePath);
            notes.Add(newNote);
        }

        return newNote;
    }
    
    private CustomNotes.NoteDescriptor LoadNote(AssetBundle bundle, Transform parent)
    {
        GameObject notePrefab = bundle.LoadAsset<GameObject>("assets/_customnote.prefab");
        if (notePrefab == null)
        {
            return null;
        }

        GameObject newNote = Instantiate(notePrefab);
        newNote.transform.position = new Vector3(0, 0, 4);
        newNote.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        newNote.transform.parent = parent;

        bundle.Unload(false);
        
        CustomNotes.NoteDescriptor customNote = newNote.GetComponent<CustomNotes.NoteDescriptor>();
        if (customNote == null)
        {
            // no customplatform component, abort
            Destroy(newNote);
            return null;
        }

        newNote.name = customNote.NoteName + " by " + customNote.AuthorName;

        //if (customNote.Icon == null)
            //customNote.Icon = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "FeetIcon").FirstOrDefault().texture;

        //AddManagers(newPlatform);

        newNote.SetActive(false);

        return customNote;
    }

    private void AddManagers(GameObject go)
    {
        AddManagers(go, go);
    }

    private void AddManagers(GameObject go, GameObject root)
    {
        // Replace materials for this object
        /* MaterialSwapper.ReplaceMaterialsForGameObject(go);

       
        // Add a tube light manager if there are tube light descriptors
        if (go.GetComponentInChildren<TubeLight>(true) != null)
        {
            TubeLightManager tlm = root.GetComponent<TubeLightManager>();
            if(tlm == null) tlm = root.AddComponent<TubeLightManager>();
            tlm.CreateTubeLights(go);
        }
        

        // Rotation effect manager
        if (go.GetComponentInChildren<RingRotationData>(true) != null)
        {
            RotationEventEffectManager rotManager = root.GetComponent<RotationEventEffectManager>();
            if (rotManager == null) rotManager = root.AddComponent<RotationEventEffectManager>();
            rotManager.CreateEffects(go);
        }

        // Add a trackRing controller if there are track ring descriptors
        if (go.GetComponentInChildren<TrackRings>(true) != null)
        {
            foreach (Ring trackRings in go.GetComponentsInChildren<TrackRings>(true))
            {
                GameObject ringPrefab = trackRings.trackLaneRingPrefab;

                // Add managers to prefabs (nesting)
                AddManagers(ringPrefab, root);
            }

            TrackRingsManagerSpawner trms = root.GetComponent<TrackRingsManagerSpawner>();
            if (trms == null) trms = root.AddComponent<TrackRingsManagerSpawner>();
            trms.CreateTrackRings(go);
        }

        // Add spectrogram manager
        if (go.GetComponentInChildren<Spectrogram>(true) != null)
        {
            foreach (Spectrogram spec in go.GetComponentsInChildren<Spectrogram>(true))
            {
                GameObject colPrefab = spec.columnPrefab;
                AddManagers(colPrefab, root);
            }

            SpectrogramColumnManager specManager = go.GetComponent<SpectrogramColumnManager>();
            if (specManager == null) specManager = go.AddComponent<SpectrogramColumnManager>();
            specManager.CreateColumns(go);
        }

        if (go.GetComponentInChildren<SpectrogramMaterial>(true) != null)
        {
            // Add spectrogram materials manager
            SpectrogramMaterialManager specMatManager = go.GetComponent<SpectrogramMaterialManager>();
            if (specMatManager == null) specMatManager = go.AddComponent<SpectrogramMaterialManager>();
            specMatManager.UpdateMaterials();
        }


        if (go.GetComponentInChildren<SpectrogramAnimationState>(true) != null)
        {
            // Add spectrogram animation state manager
            SpectrogramAnimationStateManager specAnimManager = go.GetComponent<SpectrogramAnimationStateManager>();
            if (specAnimManager == null) specAnimManager = go.AddComponent<SpectrogramAnimationStateManager>();
            specAnimManager.UpdateAnimationStates();
        }

        // Add Song event manager
        if (go.GetComponentInChildren<SongEventHandler>(true) != null)
        {
            foreach (SongEventHandler handler in go.GetComponentsInChildren<SongEventHandler>())
            {
                SongEventManager manager = handler.gameObject.AddComponent<SongEventManager>();
                manager._songEventHandler = handler;
            }
        }

        // Add EventManager 
        if (go.GetComponentInChildren<EventManager>(true) != null)
        {
            foreach (EventManager em in go.GetComponentsInChildren<EventManager>())
            {
                PlatformEventManager pem = em.gameObject.AddComponent<PlatformEventManager>();
                pem._EventManager = em;
            }
        }*/
    }
}