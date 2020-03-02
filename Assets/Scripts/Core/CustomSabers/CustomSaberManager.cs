using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static HelperClass;

public class CustomSaberManager : MonoBehaviour
{
    public static CustomSaberManager Instance;
    private CustomSaberLoader saberLoader;
    private SaberDescriptor[] sabers;
    //public Saber LeftSaber;
    //public Saber RightSaber;
    private int saberIndex = 0;

    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Saber Manager");
        CustomSaberManager newManager = go.AddComponent<CustomSaberManager>();
        newManager.saberLoader = new CustomSaberLoader();
        newManager.RefreshSabers();

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
        saberLoader = new CustomSaberLoader();
    }

    public SaberDescriptor AddSaber(string path)
    {
        SaberDescriptor newSaber = saberLoader.LoadPlatformBundle(path, transform);
        if (newSaber != null)
        {
            var saberList = sabers.ToList();
            saberList.Add(newSaber);
            sabers = saberList.ToArray();
        }

        return newSaber;
    }

    public void RefreshSabers()
    {
        if (sabers != null)
        {
            Transform[] ts = _currentMenuObjects._SaberContent.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == _currentMenuObjects._SaberContent.name)
                {
                    continue;
                }

                Destroy(t.gameObject);
            }

            foreach (SaberDescriptor saber in sabers)
            {
                Destroy(saber.gameObject);
            }
        }

        sabers = saberLoader.CreateAllSabers(transform);

        for (int i = 0; i < sabers.Length; i++)
        {
            GameObject _saber = Instantiate(_currentMenuObjects._SaberListPreFab);
            _saber.transform.SetParent(_currentMenuObjects._SaberContent.transform);
            _saber.transform.localPosition = _currentMenuObjects._SaberListPreFab.transform.localPosition;
            _saber.transform.localScale = _currentMenuObjects._SaberListPreFab.transform.localScale;
            _saber.transform.localRotation = _currentMenuObjects._SaberListPreFab.transform.localRotation;
            _saber.name = sabers[i].SaberName;
            SaberSelect select = _saber.AddComponent<SaberSelect>();
            select.index = i;
            _saber.GetComponentInChildren<Button>().onClick.AddListener(() => select.GoToTarget());

            Transform[] ts = _saber.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "Name")
                {
                    t.gameObject.GetComponent<Text>().text = sabers[i].SaberName;
                }

                if (t.gameObject.name == "Artist")
                {
                    t.gameObject.GetComponent<Text>().text = sabers[i].AuthorName;
                }

                if (t.gameObject.name == "Cover")
                {
                    //Image tmp = t.gameObject.GetComponent<Image>();
                    //tmp.sprite = platforms[i].icon;
                }
            }

            _saber.SetActive(true);
        }

        // Check if this path was loaded and update our platform index
        for (int i = 0; i < sabers.Length; i++)
        {
            if (currentSaber.SaberName + currentSaber.AuthorName ==
                sabers[i].SaberName + sabers[i].AuthorName)
            {
                saberIndex = i;
                break;
            }
        }

        if (_currentSettings.LastKnownSaberSet != "")
        {
            for (int i = 0; i < sabers.Length; i++)
            {
                if (sabers[i].SaberName == _currentSettings.LastKnownSaberSet)
                {
                    ChangeToSaber(i);
                    break;
                }
            }
        }
    }


    public int currentSaberIndex
    {
        get { return saberIndex; }
    }

    public SaberDescriptor currentSaber
    {
        get { return sabers[saberIndex]; }
    }

    public SaberDescriptor GetSaber(int i)
    {
        return sabers.ElementAt(i);
    }

    public GameObject _LeftSaber;
    public GameObject _RightSaber; 

    public void ChangeToSaber(int index)
    {
        if(_LeftSaber != null)
            _LeftSaber.transform.SetParent(currentSaber.transform);

        if (_RightSaber != null)
            _RightSaber.transform.SetParent(currentSaber.transform);

        currentSaber.gameObject.SetActive(false);
        saberIndex = index % sabers.Length;

        Transform[] _Children = currentSaber.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform _child in _Children)
        {
            if (_child.gameObject.name == "LeftSaber")
            {
                _child.gameObject.transform.SetParent(EngineStart.Instance.LeftController.transform);
                _child.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                _child.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                _LeftSaber = _child.gameObject;

                BoxCollider _LedftColl = _LeftSaber.GetComponent<BoxCollider>();

                if (_LedftColl == null)
                    _LedftColl = _LeftSaber.AddComponent<BoxCollider>();

                _LedftColl.size = new Vector3(0.09f, 0.09f, _LeftSaber.transform.localScale.z);
                _LedftColl.center = new Vector3(0f, 0f, _LeftSaber.transform.localScale.z * 0.5f);
            }
            else if(_child.gameObject.name == "RightSaber")
            {
                _child.gameObject.transform.SetParent(EngineStart.Instance.RightController.transform);
                _child.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                _child.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                _RightSaber = _child.gameObject;

                BoxCollider _RightColl = _RightSaber.GetComponent<BoxCollider>();

                if(_RightColl == null)
                    _RightColl = _RightSaber.AddComponent<BoxCollider>();


                _RightColl.size = new Vector3(0.09f, 0.09f, _RightSaber.transform.localScale.z);
                _RightColl.center = new Vector3(0f, 0f, _RightSaber.transform.localScale.z * 0.5f);

            }
        }

        currentSaber.gameObject.SetActive(true);
    }

    public void LoadCurrentSabers()
    {
        if (_LeftSaber != null)
        {
            _LeftSaber.transform.SetParent(currentSaber.transform);
            setLeftSaber();
        }

        if (_RightSaber != null)
        {
            _RightSaber.transform.SetParent(currentSaber.transform);
            setRightSaber();
        }
    }

    public void setLeftSaber()
    {
        if (EngineStart.Instance.LeftController.GetComponent<FixedJoint>())
            if (EngineStart.Instance.LeftController.GetComponent<FixedJoint>().connectedBody != null)
                EngineStart.Instance.LeftController.GetComponent<FixedJoint>().connectedBody = null;

        _LeftSaber.transform.rotation = EngineStart.Instance.LeftController.transform.rotation;
        _LeftSaber.transform.position = EngineStart.Instance.LeftController.transform.position;

        var jointL = AddFixedJoint(EngineStart.Instance.LeftController);

        Rigidbody _leftRid = _LeftSaber.GetComponent<Rigidbody>();

        if (_leftRid == null)
            _leftRid = _LeftSaber.AddComponent<Rigidbody>();

        _leftRid.useGravity = false;

        jointL.connectedBody = _leftRid;

        SaberHandler _leftSaberScript = _LeftSaber.GetComponent<SaberHandler>();

        if(_leftSaberScript == null)
            _leftSaberScript = _LeftSaber.AddComponent<SaberHandler>();

        _leftSaberScript._type = NoteType.LEFT;
    }

    public void setRightSaber()
    {
        if (EngineStart.Instance.RightController.GetComponent<FixedJoint>())
            if (EngineStart.Instance.RightController.GetComponent<FixedJoint>().connectedBody != null)
                EngineStart.Instance.RightController.GetComponent<FixedJoint>().connectedBody = null;

        _RightSaber.transform.rotation = EngineStart.Instance.RightController.transform.rotation;
        _RightSaber.transform.position = EngineStart.Instance.RightController.transform.position;

        var jointR = AddFixedJoint(EngineStart.Instance.RightController);

        Rigidbody _rightRid = _RightSaber.GetComponent<Rigidbody>();

        if(_rightRid == null)
            _rightRid = _RightSaber.AddComponent<Rigidbody>();

        _rightRid.useGravity = false;


        jointR.connectedBody = _rightRid;

        SaberHandler _RightSaberScript = _RightSaber.GetComponent<SaberHandler>();

        if (_RightSaberScript == null)
            _RightSaberScript = _RightSaber.AddComponent<SaberHandler>();

        _RightSaberScript._type = NoteType.RIGHT;
    }

    private FixedJoint AddFixedJoint(GameObject obj)
    {
        FixedJoint fx = obj.AddComponent<FixedJoint>();
        fx.breakForce = Mathf.Infinity;
        fx.breakTorque = Mathf.Infinity;
        return fx;
    }
}