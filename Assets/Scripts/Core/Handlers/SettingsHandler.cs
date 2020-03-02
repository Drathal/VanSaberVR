using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    public static SettingsHandler Instance;

    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Settings Handler");
        SettingsHandler newManager = go.AddComponent<SettingsHandler>();

        if (File.Exists(@Application.dataPath + "/Data.mjd"))
        {
            newManager.ReadSettings();
        }
        else
        {
            Settings newSettings = new Settings()
            {
                UserName = "CloneSaberTester",
                LeftColor = Color.red,
                RightColor = Color.blue,
                WallFrameColor = Color.white,
                LeftLightColor = Color.red,
                RightLightColor = Color.blue,
                ShowDebris = 1,
                ShowSparks = 1,
                CutSoundLevel = 30,
                ArmDistance = 1.5f,
                PlayerHeight = 1,
                LastKnownMenu = "",
                LastKnownPlatform = "",
                LastKnownSaberSet = "",
                LastKnownNoteSet = ""
            };

            newManager.WriteSettings(newSettings);
            newManager.ReadSettings();
        }
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
    public void WriteSettings(Settings settings)
    {
        if (File.Exists(@Application.dataPath + "/Data.mjd"))
            File.Delete(@Application.dataPath + "/Data.mjd");

        NetSocket _socketWriter = new NetSocket();
        PacketWriter writer = new PacketWriter(Opcodes.SETTINGS_WRITE);
        writer.WriteString(_socketWriter.Encrypt(settings.UserName));
        writer.WriteInt32(HelperClass.ColourToInt(settings.LeftColor));
        writer.WriteInt32(HelperClass.ColourToInt(settings.RightColor));
        writer.WriteInt32(HelperClass.ColourToInt(settings.WallFrameColor));
        writer.WriteInt32(HelperClass.ColourToInt(settings.LeftLightColor));
        writer.WriteInt32(HelperClass.ColourToInt(settings.RightLightColor));
        writer.WriteInt32(settings.ShowDebris);
        writer.WriteInt32(settings.ShowSparks);
        writer.WriteInt32(settings.CutSoundLevel);
        writer.WriteFloat(settings.ArmDistance);
        writer.WriteFloat(settings.PlayerHeight);
        writer.WriteString(_socketWriter.Encrypt(settings.LastKnownMenu));
        writer.WriteString(_socketWriter.Encrypt(settings.LastKnownPlatform));
        writer.WriteString(_socketWriter.Encrypt(settings.LastKnownSaberSet));
        writer.WriteString(_socketWriter.Encrypt(settings.LastKnownNoteSet));
        byte[] endData = _socketWriter.FinalisePacket(writer);
        File.WriteAllBytes(@Application.dataPath + "/Data.mjd", endData);
    }

    public void ReadSettings()
    {
        byte[] startData = File.ReadAllBytes(@Application.dataPath + "/Data.mjd");
        new NetSocket().HandleData(startData);
    }
}
