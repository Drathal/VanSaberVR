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