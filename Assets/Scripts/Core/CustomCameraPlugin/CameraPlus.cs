using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Wacki;

public class CameraPlusManager : MonoBehaviour
{
	public static CameraPlusManager Instance;
	protected const int OnlyInThirdPerson = 3;
	protected const int OnlyInFirstPerson = 4;
	public readonly Config Config = new Config(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg"));
	protected Camera _mainCamera;

	public bool ThirdPerson
	{
		get { return _thirdPerson; }
		set
		{
			_thirdPerson = value;
			_cameraCube.gameObject.SetActive(_thirdPerson);
			_cameraPreviewQuad.gameObject.SetActive(_thirdPerson);

			if (value)
			{
				_cam.cullingMask &= ~(1 << OnlyInFirstPerson);
				_cam.cullingMask |= 1 << OnlyInThirdPerson;
			}
			else
			{
				_cam.cullingMask &= ~(1 << OnlyInThirdPerson);
				_cam.cullingMask |= 1 << OnlyInFirstPerson;
			}
		}
	}

	protected bool _thirdPerson;

	public Vector3 ThirdPersonPos;
	public Vector3 ThirdPersonRot;
	protected RenderTexture _camRenderTexture;
	protected Material _previewMaterial;
	protected Camera _cam;
	protected Transform _cameraCube;
	protected ScreenCameraBehaviour _screenCamera;
	protected GameObject _cameraPreviewQuad;

	protected int _prevScreenWidth;
	protected int _prevScreenHeight;
	protected int _prevAA;
	protected float _prevRenderScale;

	private void Awake()
	{
		if (Instance != null) return;
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
	public static void OnLoad(Camera mainCamera)
	{
		if (Instance != null)
		{
			return;
		}

		GameObject go = new GameObject("CameraPlus");
		CameraPlusManager newManager = go.AddComponent<CameraPlusManager>();
		newManager.Startup(mainCamera);
	}

	public void Startup(Camera mainCamera)
	{
		_mainCamera = mainCamera;

		XRSettings.showDeviceView = false;

		Config.ConfigChangedEvent += PluginOnConfigChangedEvent;
		SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;

		var gameObj = Instantiate(_mainCamera.gameObject);
		gameObj.SetActive(false);
		gameObj.name = "Camera Plus";
		gameObj.tag = "Untagged";
		while (gameObj.transform.childCount > 0) DestroyImmediate(gameObj.transform.GetChild(0).gameObject);
		DestroyImmediate(gameObj.GetComponent("CameraRenderCallbacksManager"));
		DestroyImmediate(gameObj.GetComponent("AudioListener"));
		DestroyImmediate(gameObj.GetComponent("MeshCollider"));

		if (SteamVRCompatibility.IsAvailable)
		{
			DestroyImmediate(gameObj.GetComponent(SteamVRCompatibility.SteamVRCamera));
			DestroyImmediate(gameObj.GetComponent(SteamVRCompatibility.SteamVRFade));
		}

		_screenCamera = new GameObject("Screen Camera").AddComponent<ScreenCameraBehaviour>();

		if (_previewMaterial == null)
		{
			_previewMaterial = new Material(Shader.Find("Hidden/BlitCopyWithDepth"));
		}

		_cam = gameObj.GetComponent<Camera>();
		_cam.stereoTargetEye = StereoTargetEyeMask.None;
		_cam.enabled = true;

		gameObj.SetActive(true);

		var camera = _mainCamera.transform;
		transform.position = camera.position;
		transform.rotation = camera.rotation;

		gameObj.transform.parent = transform;
		gameObj.transform.localPosition = Vector3.zero;
		gameObj.transform.localRotation = Quaternion.identity;
		gameObj.transform.localScale = Vector3.one;

		var cameraCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cameraCube.SetActive(ThirdPerson);
		_cameraCube = cameraCube.transform;
		_cameraCube.localScale = new Vector3(0.15f, 0.15f, 0.22f);
		_cameraCube.name = "CameraCube";

		var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		DestroyImmediate(quad.GetComponent<Collider>());
		quad.GetComponent<MeshRenderer>().material = _previewMaterial;
		quad.transform.parent = _cameraCube;
		quad.transform.localPosition = new Vector3(-1f * ((_cam.aspect - 1) / 2 + 1), 0, 0.22f);
		quad.transform.localEulerAngles = new Vector3(0, 180, 0);
		quad.transform.localScale = new Vector3(_cam.aspect, 1, 1);
		_cameraPreviewQuad = quad;

		ReadConfig();

		if (ThirdPerson)
		{
			ThirdPersonPos = Config.Position;
			ThirdPersonRot = Config.Rotation;

			transform.position = ThirdPersonPos;
			transform.eulerAngles = ThirdPersonRot;

			_cameraCube.position = ThirdPersonPos;
			_cameraCube.eulerAngles = ThirdPersonRot;
		}

		SceneManagerOnSceneLoaded(new Scene(), LoadSceneMode.Single);
	}

	protected virtual void OnDestroy()
	{
		Config.ConfigChangedEvent -= PluginOnConfigChangedEvent;
		SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
	}

	protected virtual void PluginOnConfigChangedEvent(Config config)
	{
		ReadConfig();
	}

	protected virtual void ReadConfig()
	{
		ThirdPerson = Config.thirdPerson;

		if (!ThirdPerson)
		{
			transform.position = _mainCamera.transform.position;
			transform.rotation = _mainCamera.transform.rotation;
		}
		else
		{
			ThirdPersonPos = Config.Position;
			ThirdPersonRot = Config.Rotation;
		}

		CreateScreenRenderTexture();
		SetFOV();
	}

	protected virtual void CreateScreenRenderTexture()
	{
		_prevScreenWidth = Screen.width;
		_prevScreenHeight = Screen.height;

		var replace = false;
		if (_camRenderTexture == null)
		{
			_camRenderTexture = new RenderTexture(1, 1, 24);
			replace = true;
		}
		else
		{
			if (Config.antiAliasing != _prevAA || Config.renderScale != _prevRenderScale)
			{
				replace = true;

				_cam.targetTexture = null;
				_screenCamera.SetRenderTexture(null);

				_camRenderTexture.Release();

				_prevAA = Config.antiAliasing;
				_prevRenderScale = Config.renderScale;
			}
		}

		if (!replace)
		{
			Console.WriteLine("Don't need to replace");
			return;
		}

		GetScaledScreenResolution(Config.renderScale, out var scaledWidth, out var scaledHeight);
		_camRenderTexture.width = scaledWidth;
		_camRenderTexture.height = scaledHeight;

		_camRenderTexture.antiAliasing = Config.antiAliasing;
		_camRenderTexture.Create();

		_cam.targetTexture = _camRenderTexture;
		_previewMaterial.SetTexture("_MainTex", _camRenderTexture);
		_screenCamera.SetRenderTexture(_camRenderTexture);

	}

	protected virtual void GetScaledScreenResolution(float scale, out int scaledWidth, out int scaledHeight)
	{
		var ratio = (float)Screen.height / Screen.width;
		scaledWidth = Mathf.Clamp(Mathf.RoundToInt(Screen.width * scale), 1, int.MaxValue);
		scaledHeight = Mathf.Clamp(Mathf.RoundToInt(scaledWidth * ratio), 1, int.MaxValue);
	}

	protected virtual void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		var pointer = Resources.FindObjectsOfTypeAll<ViveUILaserPointer>().FirstOrDefault();
		if (pointer == null) return;
		var movePointer = pointer.gameObject.AddComponent<CameraMoverPointer>();
		movePointer.Init(this, _cameraCube);
	}

	protected virtual void LateUpdate()
	{
		if (Screen.width != _prevScreenWidth || Screen.height != _prevScreenHeight)
		{
			CreateScreenRenderTexture();
		}

		var camera = _mainCamera.transform;

		if (ThirdPerson)
		{
			transform.position = ThirdPersonPos;
			transform.eulerAngles = ThirdPersonRot;
			_cameraCube.position = ThirdPersonPos;
			_cameraCube.eulerAngles = ThirdPersonRot;
			return;
		}

		transform.position = Vector3.Lerp(transform.position, camera.position,
			Config.positionSmooth * Time.deltaTime);

		transform.rotation = Quaternion.Slerp(transform.rotation, camera.rotation,
			Config.rotationSmooth * Time.deltaTime);
	}

	protected virtual void SetFOV()
	{
		if (_cam == null) return;
		var fov = (float)(57.2957801818848 *
						   (2.0 * Mathf.Atan(
								Mathf.Tan((float)(Config.fov * (Math.PI / 180.0) * 0.5)) /
								_mainCamera.aspect)));
		_cam.fieldOfView = fov;
	}

	protected virtual void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			ThirdPerson = !ThirdPerson;
			if (!ThirdPerson)
			{
				transform.position = _mainCamera.transform.position;
				transform.rotation = _mainCamera.transform.rotation;
			}
			else
			{
				ThirdPersonPos = Config.Position;
				ThirdPersonRot = Config.Rotation;
			}

			Config.thirdPerson = ThirdPerson;
			Config.Save();
		}
	}
}