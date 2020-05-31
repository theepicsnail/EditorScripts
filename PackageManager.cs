using System.Net;
using UnityEditor;
using UnityEngine;

namespace Snail
{
    public class PackageManager : EditorWindow
    {
        public static T download<T>(string url)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "VrcPackageManager");
                Debug.Log("Downloading " + url);
                byte[] data = client.DownloadData(url);
                return JsonUtility.FromJson<T>(System.Text.Encoding.UTF8.GetString(data));
            }
        }

        // Add menu item named "My Window" to the Window menu
        [MenuItem("Tools/Snail/Package Manager")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(PackageManager));
        }

        void OnGUI()
        {
            GUILayout.Label("SDK2 Packages", EditorStyles.boldLabel);
            foreach (var pkg in packages2)
                pkg.OnGUI();
                
            GUILayout.Label("SDK3 Packages", EditorStyles.boldLabel);
            foreach (var pkg in packages3)
                pkg.OnGUI();
            
        }
        public static GUILayoutOption[] options = null;
        public abstract class Package
        {
            public abstract void FetchInfo();
            public abstract void ShowGui();
            public abstract string GetName();
            private bool show;
            private bool fetched;
            public void OnGUI()
            {
                show = EditorGUILayout.Foldout(show, GetName());
                if (!show) return;
                if (!fetched)
                {
                    FetchInfo();
                    fetched = true;
                }
                ShowGui();
            }
        }

        private Package[] packages2 = {
            new SDK2(),
            new GithubPackage("rurre/PumkinsAvatarTools"),
            new GithubPackage("Xiexe/Xiexes-Unity-Shaders"),
        };
        private Package[] packages3 = {
            new SDK3(),
            new GithubPackage("GotoFinal/GotoUdon"),
            new GithubPackage("Merlin-san/UdonSharp")
        };

        [System.Serializable]
        public class VRCStruct
        {
            [System.Serializable]
            public class Urls
            {
                public string sdk2;
                public string sdk3;
            }
            public Urls downloadUrls;
        }
        public class SDK2 : Package
        {

            private VRCStruct data;
            public override string GetName() { return "VRChat SDK2 (Avatars)"; }

            public override void FetchInfo()
            {
                data = download<VRCStruct>("https://api.vrchat.cloud/api/1/config");
            }
            public override void ShowGui()
            {
                if (GUILayout.Button("Download"))
                {
                    Application.OpenURL(data.downloadUrls.sdk2);
                }
            }
        }
        public class SDK3 : Package
        {
            private VRCStruct data;
            public override string GetName() { return "VRChat SDK3 (Udon)"; }
            public override void FetchInfo()
            {
                data = download<VRCStruct>("https://api.vrchat.cloud/api/1/config");
            }
            public override void ShowGui()
            {
                if (GUILayout.Button("Download"))
                {
                    Application.OpenURL(data.downloadUrls.sdk3);
                }
            }
        }

        [System.Serializable]
        public class Github
        {
            public string name;
            public string zipball_url;
            public GithubAsset[] assets;

            [System.Serializable]
            public class GithubAsset
            {
                public string name;
                public string browser_download_url;
            }
        }

        public class GithubPackage : Package
        {
            private string userrepo;
            private Github data;
            public override void FetchInfo()
            {
                data = download<Github>($"https://api.github.com/repos/{userrepo}/releases/latest");
            }
            public GithubPackage(string userrepo)
            {
                this.userrepo = userrepo;
            }
            public override string GetName() { return userrepo; }
            public override void ShowGui()
            {
                if (GUILayout.Button("Zip File"))
                {
                    Application.OpenURL(data.zipball_url);
                }
                foreach (var asset in data.assets)
                {
                    if (GUILayout.Button(asset.name))
                        Application.OpenURL(asset.browser_download_url);
                }
            }
        }

    }
}