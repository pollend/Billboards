using System;
using UnityEngine;
using System.Collections;
using System.IO;
using MiniJSON;
using System.Collections.Generic;
using System.Linq;
using Custom_Scenery;
using Random = UnityEngine.Random;

public class BillBoard : Deco
{
    private bool _show;

    private Rect _windowRect = new Rect(20, 20, 350, 320);

    [Serialized]
    public bool Pn = true;

    [Serialized]
    public string Image;
    
    public string url = "https://parkitectnexus.com/api/screenshots";
    
    private Vector2 _scrollPosition = Vector2.zero;

    private List<KeyValuePair<string, Texture2D>> _banners = new List<KeyValuePair<string, Texture2D>>();
    public string Path { get; set; }

    private GUISkin _skin;

    public override void Start()
    {
        base.Start();

        //get the full location of the assembly with DaoTests in it
        string fullPath = System.Reflection.Assembly.GetAssembly(typeof(Main)).Location;

        //get the folder that's in
        Path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fullPath).Replace("bin", ""), "");

        Debug.Log(Path);
        
        if (string.IsNullOrEmpty(Image))
        {
            StartCoroutine(SetBanner());
        }
        else
        {
            StartCoroutine(LoadImage());
        }

        StartCoroutine(LoadSkin());
    }

    private IEnumerator LoadSkin()
    {
        char dsc = System.IO.Path.DirectorySeparatorChar;

        using (WWW www = new WWW("file://" + Path + dsc + "assetbundle" + dsc + "guiskin"))
        {
            if (www.error != null)
                throw new Exception("Loading had an error:" + www.error);

            yield return www;

            AssetBundle bundle = www.assetBundle;

            _skin = bundle.LoadAsset<GUISkin>("ParkitectGUISkin");

            bundle.Unload(false);
        }
    }

    private IEnumerator LoadImage()
    {
        if (Pn)
        {
            WWW www = new WWW(Image);

            yield return www;

            Texture2D texture = www.texture;

            SetTexture(texture);
        }
        else
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                if (renderer.gameObject.name.StartsWith("Board"))
                {
                    renderer.material.SetTexture("_MainTex", LoadFromImage(Image));
                }
            }
        }
    }

    public IEnumerator SetBanner()
    {
        string[] files = Directory.GetFiles(System.IO.Path.Combine(Path, "Banners"));

        foreach (string file in files)
        {
            Texture2D tex = LoadFromImage(file);

            yield return null;

            _banners.Add(new KeyValuePair<string, Texture2D>(file, tex));
        }

        if (Pn)
        {
            WWW www = new WWW("https://parkitectnexus.com/api/screenshots");

            yield return www;

            if (www.error == null)
            {
                Dictionary<string, object> dict = Json.Deserialize(www.text) as Dictionary<string, object>;

                Dictionary<string, object> meta = dict["meta"] as Dictionary<string, object>;

                Dictionary<string, object> pagination = meta["pagination"] as Dictionary<string, object>;

                Int64 total = (Int64)pagination["total"];
                Int64 per_page = (Int64)pagination["per_page"];

                int pages = Mathf.CeilToInt(total / per_page);

                www = new WWW("https://parkitectnexus.com/api/screenshots?page=" + Mathf.RoundToInt(Random.value * pages));

                yield return www;

                dict = Json.Deserialize(www.text) as Dictionary<string, object>;

                List<object> screenshots = dict["data"] as List<object>;

                Dictionary<string, object> screenshot =
                    screenshots[Mathf.CeilToInt(Random.value * screenshots.Count - 1)] as Dictionary<string, object>;

                Dictionary<string, object> resource = screenshot["image"] as Dictionary<string, object>;

                Image = resource["url"] as string;
            }
            else
            {
                Debug.Log(www.error);
            }
        }
        else
        {
            Image = LoadRandomFromDir(System.IO.Path.Combine(Path, "Banners"));
        }

        StartCoroutine(LoadImage());
    }

    private void OnMouseDown()
    {
        _show = true;
    }

    private void OnGUI()
    {
        GUI.skin = _skin;

        if (_show)
        {
            _windowRect = GUI.Window(Mathf.RoundToInt(transform.position.x * transform.position.z), _windowRect, BillboardWindow, "Billboard");
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            _show = false;
        }
    }

    private void BillboardWindow(int windowID)
    {
        bool orgPn = Pn;

        if (GUI.Button(new Rect(320, 5, 20, 20), "x"))
        {
            _show = false;
        }

        Pn = GUI.Toggle(new Rect(10, 35, 300, 20), Pn, "Random screenshots from ParkitectNexus.com");
        Pn = !GUI.Toggle(new Rect(10, 55, 300, 20), !Pn, "Local images");
        if (GUI.Button(new Rect(10, 75, 70, 20), "Refresh"))
        {
            StartCoroutine(SetBanner());
        }

        if (Pn != orgPn)
        {
            StartCoroutine(SetBanner());
        }

        GUI.Label(new Rect(10, 100, 350, 20), "Banner images are located in:");
        GUI.Label(new Rect(10, 120, 300, 20), System.IO.Path.Combine(Path, "Banners"), new GUIStyle() { fontSize = 10, wordWrap = true });

        if (!Pn)
        {
            _scrollPosition = GUI.BeginScrollView(new Rect(10, 145, 330, 170), _scrollPosition, new Rect(0, 0, 310, (_banners.Count / 5 + 1) * 60));

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < _banners.Count + 1; y++)
                {
                    if (_banners.ElementAtOrDefault(x + y * 5).Value == null)
                        break;

                    if (GUI.Button(new Rect(x * 60, y * 60, 50, 50), _banners[x + y * 5].Value))
                    {
                        SetTexture(_banners[x + y * 5].Value);
                        Image = _banners[x + y*5].Key;
                    }
                }
            }

            GUI.EndScrollView();
        }
    }

    private void SetTexture(Texture2D tex)
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (renderer.gameObject.name.StartsWith("Board"))
            {
                renderer.material.SetTexture("_MainTex", tex);
            }
        }
    }
    
    public string LoadRandomFromDir(string dir)
    {
        string[] files = Directory.GetFiles(dir);

        string file = files[Mathf.CeilToInt(Random.value * files.Count() - 1)];

        return file;
    }

    public Texture2D LoadFromImage(string path)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(path))
        {
            fileData = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }

        return tex;
    }
}
