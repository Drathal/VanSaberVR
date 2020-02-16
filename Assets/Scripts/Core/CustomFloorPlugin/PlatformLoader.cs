using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlatformLoader : MonoBehaviour
{
    private const string customFolder = "CustomPlatforms";

    private List<string> bundlePaths;
    private List<CustomFloorPlugin.CustomPlatform> platforms;

    /// <summary>
    /// Loads AssetBundles and populates the platforms array with CustomPlatform objects
    /// </summary>
    public CustomFloorPlugin.CustomPlatform[] CreateAllPlatforms(Transform parent)
    {
        string customPlatformsFolderPath = Path.Combine(Application.dataPath, customFolder);
        
        // Create the CustomPlatforms folder if it doesn't already exist
        if (!Directory.Exists(customPlatformsFolderPath))
        {
            Directory.CreateDirectory(customPlatformsFolderPath);
        }

        // Find AssetBundles in our CustomPlatforms directory
        string[] allBundlePaths = Directory.GetFiles(customPlatformsFolderPath, "*.plat");

        platforms = new List<CustomFloorPlugin.CustomPlatform>();
        bundlePaths = new List<string>();
        
        /*GameObject defaultPlatformGO = Instantiate(_DefaultPlatform);
        CustomFloorPlugin.CustomPlatform defaultPlatform = defaultPlatformGO.GetComponent<CustomFloorPlugin.CustomPlatform>();

        defaultPlatformGO.transform.parent = parent;
        defaultPlatformGO.name = defaultPlatform.platName + " by " + defaultPlatform.platAuthor;

        if (defaultPlatform.icon == null)
            defaultPlatform.icon = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "FeetIcon")
                .FirstOrDefault();

        defaultPlatformGO.SetActive(false);
        platforms.Add(defaultPlatform);*/
        
        // Populate the platforms array
        for (int i = 0; i < allBundlePaths.Length; i++)
        {
            CustomFloorPlugin.CustomPlatform newPlatform = LoadPlatformBundle(allBundlePaths[i], parent);
        }

        return platforms.ToArray();
    }


    public CustomFloorPlugin.CustomPlatform LoadPlatformBundle(string bundlePath, Transform parent)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
        {
            return null;
        }

        CustomFloorPlugin.CustomPlatform newPlatform = LoadPlatform(bundle, parent);
        if (newPlatform != null)
        {
            bundlePaths.Add(bundlePath);
            platforms.Add(newPlatform);
        }

        return newPlatform;
    }

    /// <summary>
    /// Instantiate a platform from an assetbundle.
    /// </summary>
    /// <param name="bundle">An AssetBundle containing a CustomPlatform</param>
    /// <returns></returns>
    private CustomFloorPlugin.CustomPlatform LoadPlatform(AssetBundle bundle, Transform parent)
    {
        GameObject platformPrefab = bundle.LoadAsset<GameObject>("_CustomPlatform");
        if (platformPrefab == null)
        {
            return null;
        }

        GameObject newPlatform = Instantiate(platformPrefab);
        newPlatform.transform.parent = parent;

        bundle.Unload(false);

        // Collect author and name
        CustomFloorPlugin.CustomPlatform customPlatform = newPlatform.GetComponent<CustomFloorPlugin.CustomPlatform >();
        if (customPlatform == null)
        {
            // Check for old platform 
            global::CustomPlatform legacyPlatform = newPlatform.GetComponent<global::CustomPlatform>();
            if (legacyPlatform != null)
            {
                // Replace legacyplatform component with up to date one
                customPlatform = newPlatform.AddComponent<CustomFloorPlugin.CustomPlatform>();
                customPlatform.platName = legacyPlatform.platName;
                customPlatform.platAuthor = legacyPlatform.platAuthor;
                customPlatform.hideDefaultPlatform = true;
                // Remove old platform data
                Destroy(legacyPlatform);
            }
            else
            {
                // no customplatform component, abort
                Destroy(newPlatform);
                return null;
            }
        }

        newPlatform.name = customPlatform.platName + " by " + customPlatform.platAuthor;

        if (customPlatform.icon == null)
            customPlatform.icon = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "FeetIcon")
                .FirstOrDefault();

        //AddManagers(newPlatform);

        newPlatform.SetActive(false);

        return customPlatform;
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

public class CustomPlatform : MonoBehaviour
{
    public string platName = "MyCustomPlatform";
    public string platAuthor = "MyName";
}