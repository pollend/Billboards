using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.GZip;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Custom_Scenery.CustomScenery
{
    public class BillBoard : Deco
    {

        [Serialized] 
        public string Image;

        public string ImageSource;

        public string BannerPath { get; set; }


        private readonly List<Banner> _banners = new List<Banner>();

        // GUI
        private Vector2 _scrollPosition = Vector2.zero;
        private bool _show;
        private Rect _windowPosition = new Rect(20, 20, 350, 320);

        private void Start()
        {
            BannerPath = FilePaths.getFolderPath("Banners");

            if (!string.IsNullOrEmpty(Image))
            {
                Texture2D tex = new Texture2D(1, 1);

                byte[] image = Convert.FromBase64String(Image);
                tex.LoadImage(image);

                SetTexture(tex);
            }
            //StartCoroutine(LoadGUISkin());

            StartCoroutine(ReloadLocalBanners());
        }

   

        private IEnumerator ReloadLocalBanners()
        {
            _banners.Clear();

            string[] files = Directory.GetFiles(System.IO.Path.Combine(BannerPath, "square"));

            foreach (string file in files)
            {
                Texture2D tex = LoadFromImage(file);

                yield return null;

                _banners.Add(new Banner {Source = file, Texture = tex});
            }
        }

        public override bool canBeSelected()
        {
            return true;
        }

        public override void onContextClick(bool isInBuildingTool)
        {
            _show = true;
            base.onContextClick(isInBuildingTool);
        }



        private void OnGUI()
        {
            //GUI.skin = _skin;

            if (_show)
            {
                _windowPosition = GUI.Window(Mathf.RoundToInt(transform.position.x * transform.position.z),
                    _windowPosition, BillboardWindow, "Billboard");

                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    _show = false;
                }
            }
        }

        private void BillboardWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 310, 30));
            if (GUI.Button(new Rect(320, 5, 20, 20), "x"))
            {
                _show = false;
            }

            GUI.Label(new Rect(10, 100, 350, 20), "Banner images are located in:");
            GUI.Label(new Rect(10, 120, 300, 20), BannerPath, new GUIStyle() {fontSize = 10, wordWrap = true});


            _scrollPosition = GUI.BeginScrollView(new Rect(10, 145, 330, 170), _scrollPosition,
                new Rect(0, 0, 310, (_banners.Count / 5 + 1) * 60));

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < _banners.Count + 1; y++)
                {
                    if (_banners.ElementAtOrDefault(x + y * 5) == null)
                        break;

                    if (GUI.Button(new Rect(x * 60, y * 60, 50, 50), _banners[x + y * 5].Texture,
                        new GUIStyle() {padding = new RectOffset(0, 0, 0, 0), border = new RectOffset(0, 0, 0, 0)}))
                    {
                        ImageSource = _banners[x + y * 5].Source;
                        SetTexture(LoadFromImage(ImageSource));
                    }
                }
            }

            GUI.EndScrollView();

        }

        private void SetTexture(Texture2D tex)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                if (renderer.gameObject.name.StartsWith("Board"))
                {
                    renderer.material.mainTexture = tex;
                }
            }
        }

        public string LoadRandomFromDir(string dir)
        {
            string[] files = Directory.GetFiles(dir);

            int i = Mathf.Max(0, Mathf.RoundToInt(Random.value * files.Count() - 1));

            string file = files[i];

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

                Image = Convert.ToBase64String(tex.EncodeToPNG());
            }

            return tex;
        }

       

   
    }
}
