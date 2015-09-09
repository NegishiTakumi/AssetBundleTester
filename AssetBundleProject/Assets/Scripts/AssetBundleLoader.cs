using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections;


public class AssetBundleLoader : MonoBehaviour
{

    // Use this for initialization
    public string BundleURL;
    public string AssetName;
    public int version;
    public UnityEngine.UI.Image image;
    public UnityEngine.UI.Text text;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DownloadAndCache());
            Debug.Log("Space is pressed");
        }
    }

    private IEnumerator DownloadAndCache()
    {

        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL+"?time=" + DateTime.Now.Second, version))
        {
            //ダウンロード進捗率を表示するだけ。
            while (!www.isDone)
            { // ダウンロードの進捗を表示
                text.text = "Download rate : " + www.progress*100 + "%";
                yield return null;
            }
            //yield return www;
            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);
            
            AssetBundle bundle = www.assetBundle;
            bundle.GetAllAssetNames().ToList().ForEach(Debug.Log);
            Debug.Log(bundle.mainAsset.name);

            if (AssetName == "")
            {
                var tex = (Texture2D) bundle.LoadAsset(bundle.mainAsset.name);
                image.sprite = Sprite.Create(tex as Texture2D, new Rect(0, 0, 2000, 1100), Vector2.zero);
            }
            else
                Instantiate(bundle.LoadAsset(AssetName));
            // Unload the AssetBundles compressed contents to conserve memory
            bundle.Unload(false);
            Debug.Log("Completed");
        } // memory is freed from the web stream (www.Dispose() gets called implicitly)
    }
}
