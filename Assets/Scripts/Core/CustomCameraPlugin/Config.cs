using System;
using System.IO;
using UnityEngine;

public class Config
{
	public string FilePath { get; }
	public float fov = 100;
	public int antiAliasing = 2;
	public float renderScale = 1;
	public float positionSmooth = 20;
	public float rotationSmooth = 10;

	public bool thirdPerson = false;

	public float posx;
	public float posy = 2;
	public float posz = -1.2f;

	public float angx = 15;
	public float angy;
	public float angz;

	public event Action<Config> ConfigChangedEvent;

	private readonly FileSystemWatcher _configWatcher;
	private bool _saving;

	public Vector3 Position
	{
		get
		{
			return new Vector3(posx, posy, posz);
		}
	}

	public Vector3 Rotation
	{
		get { return new Vector3(angx, angy, angz); }
	}

	public Config(string filePath)
	{
		FilePath = filePath;

		if (File.Exists(FilePath))
		{
			Load();
			var text = File.ReadAllText(FilePath);
			if (text.Contains("rotx"))
			{

				var oldRotConfig = new OldRotConfig();
				//ConfigSerializer.LoadConfig(oldRotConfig, FilePath);

				var euler = new Quaternion(oldRotConfig.rotx, oldRotConfig.roty, oldRotConfig.rotz,
						oldRotConfig.rotw)
					.eulerAngles;
				angx = euler.x;
				angy = euler.y;
				angz = euler.z;

				Save();
			}
		}
		else
		{
			Save();
		}

		_configWatcher = new FileSystemWatcher(Environment.CurrentDirectory)
		{
			NotifyFilter = NotifyFilters.LastWrite,
			Filter = "cameraplus.cfg",
			EnableRaisingEvents = true
		};
		_configWatcher.Changed += ConfigWatcherOnChanged;
	}

	~Config()
	{
		_configWatcher.Changed -= ConfigWatcherOnChanged;
	}

	public void Save()
	{
		_saving = true;
		//ConfigSerializer.SaveConfig(this, FilePath);
	}

	public void Load()
	{
		string[] lines = File.ReadAllLines(@FilePath);

		for (int i = 0; i < lines.Length; i++)
		{
			string[] val = lines[i].Split('=');
			switch (val[0])
			{
				case "fov":
					fov = float.Parse(val[1]);
					break;
				case "antiAliasing":
					antiAliasing = int.Parse(val[1]);
					break;
				case "renderScale":
					renderScale = float.Parse(val[1]);
					break;
				case "positionSmooth":
					positionSmooth = float.Parse(val[1]);
					break;
				case "rotationSmooth":
					rotationSmooth = float.Parse(val[1]);
					break;
				case "thirdPerson":
					thirdPerson = val[1] == "True" ? true : false;
					break;
				case "posx":
					posx = float.Parse(val[1]);
					break;
				case "posy":
					posy = float.Parse(val[1]);
					break;
				case "posz":
					posz = float.Parse(val[1]);
					break;
				case "angx":
					angx = float.Parse(val[1]);
					break;
				case "angy":
					angy = float.Parse(val[1]);
					break;
				case "angz":
					angz = float.Parse(val[1]);
					break;
			}
		}
		//ConfigSerializer.LoadConfig(this, FilePath);
	}

	private void ConfigWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
	{
		if (_saving)
		{
			_saving = false;
			return;
		}

		Load();

		if (ConfigChangedEvent != null)
		{
			ConfigChangedEvent(this);
		}
	}

	public class OldRotConfig
	{
		public float rotx;
		public float roty;
		public float rotz;
		public float rotw;
	}
}