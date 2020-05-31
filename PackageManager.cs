using System.Collections.Generic;
using System.IO;
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



        public static void downloadToFile(string url, string file)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "VrcPackageManager");
                Debug.Log("Downloading file " + file + " from " + url);
                byte[] data = client.DownloadData(url);
                File.WriteAllBytes(file, data);
            }
        }

        // static bool installing = false;
        // static Queue<Package> toInstall = new Queue<Package>();
        public static void DownloadAndRun(string url)
        {
            string file = Path.Combine(Application.dataPath, Path.GetFileName(url));
            downloadToFile(url, file);
            Debug.Log("Executing " + file);
            if (file.EndsWith("unitypackage"))
            {
                AssetDatabase.ImportPackage(file, false);
            }
            else
            {
                var proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = file;
                // proc.Exited += (a, b) => installing = false;
                proc.Start();
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
            // if (GUILayout.Button("[ALL IN SECTION]"))
            //     foreach (var pkg in packages3)
            //         toInstall.Enqueue(pkg);

            // if (toInstall.Count > 0 && !installing)
            // {
            //     Package pkg = toInstall.Dequeue();
            //     Debug.Log("Auto installing: " + pkg.GetName());
            //     pkg.Install();
            // }
        }



        public static GUILayoutOption[] options = null;
        public abstract class Package
        {
            public abstract void FetchInfo();
            public abstract string GetName();
            public abstract string GetDownload();
            private bool fetched;
            public void OnGUI()
            {

                if (GUILayout.Button(GetName()))
                {
                    Install();
                }
            }
            public void Install()
            {
                if (!fetched)
                {
                    FetchInfo();
                    fetched = true;
                }
                DownloadAndRun(GetDownload());
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
            public override string GetDownload() { return data.downloadUrls.sdk2; }
        }
        public class SDK3 : Package
        {
            private VRCStruct data;
            public override string GetName() { return "VRChat SDK3 (Udon)"; }
            public override void FetchInfo()
            {
                data = download<VRCStruct>("https://api.vrchat.cloud/api/1/config");
            }
            public override string GetDownload() { return data.downloadUrls.sdk3; }
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
            public override string GetDownload()
            {
                if (data.assets.Length > 0)
                {
                    return data.assets[0].browser_download_url;
                }
                return data.zipball_url;
            }
        }
    }
}