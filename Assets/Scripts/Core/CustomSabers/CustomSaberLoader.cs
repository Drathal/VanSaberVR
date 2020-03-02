using System;
using CustomSaber;
using System.Collections.Generic;
using System.IO;
using static HelperClass;
using UnityEngine;

public class CustomSaberLoader : MonoBehaviour
{
    private const string customFolder = "CustomSabers";

    private List<string> bundlePaths;
    private List<SaberDescriptor> sabers;

    /// <summary>
    /// Loads AssetBundles and populates the platforms array with CustomPlatform objects
    /// </summary>
    public SaberDescriptor[] CreateAllSabers(Transform parent)
    {
        string customPlatformsFolderPath = Path.Combine(Application.dataPath, customFolder);

        // Create the CustomPlatforms folder if it doesn't already exist
        if (!Directory.Exists(customPlatformsFolderPath))
        {
            Directory.CreateDirectory(customPlatformsFolderPath);
        }

        // Find AssetBundles in our CustomSabers directory
        string[] allBundlePaths = Directory.GetFiles(customPlatformsFolderPath, "*.saber");

        sabers = new List<SaberDescriptor>();
        bundlePaths = new List<string>();
        
        // Populate the array
        for (int i = 0; i < allBundlePaths.Length; i++)
        {
            LoadPlatformBundle(allBundlePaths[i], parent);
        }

        return sabers.ToArray();
    }


    public SaberDescriptor LoadPlatformBundle(string bundlePath, Transform parent)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
        {
            return null;
        }

        SaberDescriptor newPlatform = LoadSaber(bundle, parent);
        if (newPlatform != null)
        {
            bundlePaths.Add(bundlePath);
            sabers.Add(newPlatform);
        }

        return newPlatform;
    }

    private SaberDescriptor LoadSaber(AssetBundle bundle, Transform parent)
    {
        GameObject platformPrefab = bundle.LoadAsset<GameObject>("_customsaber");
        if (platformPrefab == null)
        {
            return null;
        }

        GameObject newSaber = Instantiate(platformPrefab);
        newSaber.transform.parent = parent;

        bundle.Unload(false);

        // Collect author and name
        SaberDescriptor customSaber = newSaber.GetComponent<SaberDescriptor>();
        if (customSaber == null)
        {
            // no component, abort
            Destroy(newSaber);
            return null;
        }

        newSaber.name = customSaber.SaberName + " by " + customSaber.AuthorName;

        // if (customPlatform.icon == null)
        //customPlatform.icon = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "FeetIcon")
        // .FirstOrDefault();

        AddManagers(newSaber);

        newSaber.SetActive(false);

        foreach (Transform child in newSaber.transform)
        {
            if (child.name == "RightSaber" || child.name == "LeftSaber")
            {
                GameObject _Tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _Tip.name = "Tip";
                _Tip.transform.SetParent(child);
                _Tip.transform.localScale = new Vector3(0, 0, 0);
                _Tip.transform.localPosition = new Vector3(0, 0, child.localScale.z);
                GameObject _base = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _base.name = "Base";
                _base.transform.SetParent(child);
                _base.transform.localScale = new Vector3(0, 0, 0);
                _base.transform.localPosition = new Vector3(0, 0, 0);
            }
        }


       return customSaber;
    }

    private void AddManagers(GameObject go)
    {
        AddManagers(go, go);
    }

    private void AddManagers(GameObject go, GameObject root)
    {
        if (go.GetComponentInChildren<CustomTrail>(true) != null)
        {
            CustomTrail tlm;
            foreach (Transform child in go.transform)
            {
                if (child.GetComponent<CustomTrail>())
                {
                    tlm = child.GetComponent<CustomTrail>();

                    GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    trail.transform.SetParent(child);
                    Destroy(trail.GetComponent<BoxCollider>());
                    trail.transform.position = tlm.PointStart.position;
                    trail.transform.localEulerAngles = new Vector3(90, 0, 0);
                    TrailHandler newTrail = trail.AddComponent<TrailHandler>();                    
                    newTrail.pointEnd = tlm.PointEnd.gameObject;
                    newTrail.pointStart = tlm.PointStart.gameObject;
                    newTrail.mat = tlm.TrailMaterial;
                    newTrail.Onload();
                }
            }

           // tlm.CreateTubeLights(go);
        }
        
/* 
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