using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HelperClass
{
    public static GameObject SelectedPlat;
    public static GameObject SelectedNote;
    public static GameObject SelectedMenu;
    public static GameObject SelectedSaberSet;
    public static Settings _currentSettings;
    public static CustomMenuObjects _currentMenuObjects;
    
    public static GameObject GetChildByName(GameObject parent, string childName)
    {
        Transform[] _Children = parent.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform _child in _Children)
        {
            if (_child.gameObject.name == childName)
            {
                return _child.gameObject;
            }
        }

        return null;
    }

    public const int RGB_INT_OFFSET = 2000000000;
    public static int ColourToInt(Color color)
    {
        int r = Mathf.FloorToInt(color.r * 255f);
        int g = Mathf.FloorToInt(color.g * 255f);
        int b = Mathf.FloorToInt(color.b * 255f);
        return RGB_INT_OFFSET + (((r & 0x0ff) << 16) | ((g & 0x0ff) << 8) | (b & 0x0ff));
    }

    public static Color ColourFromInt(int rgb)
    {
        rgb = rgb - RGB_INT_OFFSET;
        int red = (rgb >> 16) & 0x0ff;
        int green = (rgb >> 8) & 0x0ff;
        int blue = (rgb) & 0x0ff;
        return new Color(red / 255f, green / 255f, blue / 255f, 1);
    }
}

public enum NoteType
{
    LEFT = 0,
    RIGHT = 1,
    BOMB = 3
}

public enum EventColorType
{
    LightsOff = 0,
    Blue = 1,
    BlueUnk = 2,
    Bluefade = 3,
    unused = 4,
    Red = 5,
    RedUnk = 6,
    RedFade = 7,
}

public enum CutDirection
{
    TOP = 1,
    BOTTOM = 0,
    LEFT = 2,
    RIGHT = 3,
    TOPLEFT = 6,
    TOPRIGHT = 7,
    BOTTOMLEFT = 4,
    BOTTOMRIGHT = 5,
    NONDIRECTION = 8
}

public enum ObstacleType
{
    WALL = 0,
    CEILING = 1
}

public enum Mode
{
    preciseHeight,
    preciseHeightStart
};