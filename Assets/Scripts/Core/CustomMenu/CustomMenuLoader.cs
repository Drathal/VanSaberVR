using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CustomMenuLoader : MonoBehaviour
{

    private const string customFolder = "CustomMenus";
    private List<string> bundlePaths;
    private List<CustomMenus.MenuDescriptor> _menus;

    public CustomMenus.MenuDescriptor[] CreateAllMenus(Transform parent)
    {
        string customMenusFolderPath = Path.Combine(Application.dataPath, customFolder);

        if (!Directory.Exists(customMenusFolderPath))
        {
            Directory.CreateDirectory(customMenusFolderPath);
        }

        List<string> allBundlePaths = Directory.GetFiles(customMenusFolderPath, "*.menu", SearchOption.TopDirectoryOnly).ToList();

        _menus = new List<CustomMenus.MenuDescriptor>();
        bundlePaths = new List<string>();

        for (int i = 0; i < allBundlePaths.Count; i++)
        {
            CustomMenus.MenuDescriptor newMenu = LoadMenuBundle(allBundlePaths[i], parent);
        }

        return _menus.ToArray();
    }
    public CustomMenus.MenuDescriptor LoadMenuBundle(string bundlePath, Transform parent)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
        {
            return null;
        }

        CustomMenus.MenuDescriptor newMenu = LoadMenu(bundle, parent);
        if (newMenu != null)
        {
            bundlePaths.Add(bundlePath);
            _menus.Add(newMenu);
        }

        return newMenu;
    }

    private CustomMenus.MenuDescriptor LoadMenu(AssetBundle bundle, Transform parent)
    {
        GameObject menuPrefab = bundle.LoadAsset<GameObject>("_CustomMenu");
        if (menuPrefab == null)
        {
            return null;
        }

        GameObject newMenu = Instantiate(menuPrefab);
        //newNote.transform.position = new Vector3(0, 0, 4);
        //newNote.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        newMenu.transform.parent = parent;

        bundle.Unload(false);

        CustomMenus.MenuDescriptor customMenu = newMenu.GetComponent<CustomMenus.MenuDescriptor>();
        if (customMenu == null)
        {
            // no custommenu component, abort
            Destroy(newMenu);
            return null;
        }

        newMenu.name = customMenu.MenuName + " by " + customMenu.AuthorName;

        //if (customNote.Icon == null)
        //customNote.Icon = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "FeetIcon").FirstOrDefault().texture;

        //AddManagers(newPlatform);

        newMenu.SetActive(false);

        return customMenu;
    }
}
