using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static HelperClass;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance;
    private PlatformLoader platformLoader;
    private CustomFloorPlugin.CustomPlatform[] platforms;
    private int platformIndex = 0;

    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Platform Manager");
        PlatformManager newManager = go.AddComponent<PlatformManager>();
        newManager.platformLoader = new PlatformLoader();
        newManager.RefreshPlatforms();

    }

    public void EnableDisableDefaults()
    {
        if (!currentPlatform.gameObject.activeInHierarchy)
            currentPlatform.gameObject.SetActive(true);

        GameObject _Dynamic = GetChildByName(Instance.GetPlatform(0).gameObject, "Dynamic");

        GetChildByName(_Dynamic, "RotatingLeftLazer").SetActive(Instance.currentPlatform.hideRotatingLasers ? false : true);
        GetChildByName(_Dynamic, "RotatingRightLazer").SetActive(Instance.currentPlatform.hideRotatingLasers ? false : true);
        GetChildByName(_Dynamic, "BackTopLazer").SetActive(Instance.currentPlatform.hideBackLasers ? false : true);
        GetChildByName(_Dynamic, "BigRings").SetActive(Instance.currentPlatform.hideBigRings ? false : true);
        GetChildByName(_Dynamic, "SmallRings").SetActive(Instance.currentPlatform.hideSmallRings ? false : true);
        GetChildByName(_Dynamic, "PlayersPlatform").SetActive(Instance.currentPlatform.hideDefaultPlatform ? false : true);
        GetChildByName(_Dynamic, "TopLeftRightLazers").SetActive(Instance.currentPlatform.hideDoubleColorLasers ? false : true);
        GetChildByName(_Dynamic, "Highway").SetActive(Instance.currentPlatform.hideHighway ? false : true);
        //GetChildByName(_Dynamic, "").SetActive(Instance.currentPlatform.hideTrackLights ? false : true);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        platformLoader = new PlatformLoader();
    }

    public CustomFloorPlugin.CustomPlatform AddPlatform(string path)
    {
        CustomFloorPlugin.CustomPlatform newPlatform = platformLoader.LoadPlatformBundle(path, transform);
        if (newPlatform != null)
        {
            var platList = platforms.ToList();
            platList.Add(newPlatform);
            platforms = platList.ToArray();
        }

        return newPlatform;
    }

    public void RefreshPlatforms()
    {
        if (platforms != null)
        {
            Transform[] ts = _currentMenuObjects._PlatformContent.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == _currentMenuObjects._PlatformContent.name)
                {
                    continue;
                }

                Destroy(t.gameObject);
            }

            foreach (CustomFloorPlugin.CustomPlatform platform in platforms)
            {
                Destroy(platform.gameObject);
            }
        }

        platforms = platformLoader.CreateAllPlatforms(transform);

        for (int i = 0; i < platforms.Length; i++)
        {
            GameObject _plat = Instantiate(_currentMenuObjects._PlatListPreFab);
            _plat.transform.SetParent(_currentMenuObjects._PlatformContent.transform);
            _plat.transform.localPosition = _currentMenuObjects._PlatListPreFab.transform.localPosition;
            _plat.transform.localScale = _currentMenuObjects._PlatListPreFab.transform.localScale;
            _plat.transform.localRotation = _currentMenuObjects._PlatListPreFab.transform.localRotation;
            _plat.name = platforms[i].platName;
            PlatSelect _newSelect = _plat.AddComponent<PlatSelect>();
            _newSelect.index = i;
            _plat.GetComponentInChildren<Button>().onClick.AddListener(() => _newSelect.GoToTarget());
            //_plat.platformIndex = i;

            Transform[] ts = _plat.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "Name")
                {
                    t.gameObject.GetComponent<Text>().text = platforms[i].platName;
                }

                if (t.gameObject.name == "Artist")
                {
                    t.gameObject.GetComponent<Text>().text = platforms[i].platAuthor;
                }

                if (t.gameObject.name == "Cover")
                {
                    //Image tmp = t.gameObject.GetComponent<Image>();
                    //tmp.sprite = platforms[i].icon;
                }
            }

            _plat.SetActive(true);
        }

        // Check if this path was loaded and update our platform index
        for (int i = 0; i < platforms.Length; i++)
        {
            if (currentPlatform.platName + currentPlatform.platAuthor ==
                platforms[i].platName + platforms[i].platAuthor)
            {
                platformIndex = i;
                break;
            }
        }

        if (_currentSettings.LastKnownPlatform != "")
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i].platName == _currentSettings.LastKnownPlatform)
                {
                    ChangeToPlatform(i);
                    break;
                }
            }
        }
        //if (platforms.Length > 0)
        //ChangeToPlatform(platformIndex);
    }
    
    public int currentPlatformIndex
    {
        get { return platformIndex; }
    }

    public CustomFloorPlugin.CustomPlatform currentPlatform
    {
        get { return platforms[platformIndex]; }
    }

    public CustomFloorPlugin.CustomPlatform[] GetPlatforms()
    {
        return platforms;
    }

    public CustomFloorPlugin.CustomPlatform GetPlatform(int i)
    {
        return platforms.ElementAt(i);
    }
    
    public void ChangeToPlatform(int index)
    {
        currentPlatform.gameObject.SetActive(false);
        platformIndex = index % platforms.Length;
        currentPlatform.gameObject.SetActive(true);
    }
}