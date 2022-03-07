using UnityEngine;
using Vuforia;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using Firebase.Storage;
using Firebase.Extensions;



// Get a reference to the storage service, using the default Firebase App

// Create a storage reference from our storage service

public class SimpleCloudRecoEventHandler : MonoBehaviour
{
    CloudRecoBehaviour mCloudRecoBehaviour;
    bool mIsScanning = false;
    string mTargetMetadata = "";

    public ImageTargetBehaviour ImageTargetTemplate;

    public void OnInitialized(CloudRecoBehaviour cloudRecoBehaviour)
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnInitError(CloudRecoBehaviour.InitError initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }

    public void OnUpdateError(CloudRecoBehaviour.QueryError updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());

    }

    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;

        if (scanning)
        {

        }
    }

    // Register cloud reco callbacks
    void Awake()
    {
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        mCloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
        mCloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
        mCloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
        mCloudRecoBehaviour.RegisterOnStateChangedEventHandler(OnStateChanged);
        mCloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);
    }

    // Here we handle a cloud target recognition event
    public void OnNewSearchResult(CloudRecoBehaviour.CloudRecoSearchResult cloudRecoSearchResult)
    {
        // Store the target metadata
        mTargetMetadata = cloudRecoSearchResult.MetaData;
        Debug.Log(mTargetMetadata);

        metaData loadAsset = new metaData();
        int extPos = mTargetMetadata.IndexOf(".");
        loadAsset.name = mTargetMetadata.Substring(0, extPos);
        loadAsset.extension = mTargetMetadata.Substring(extPos);
        loadAsset.fullName = mTargetMetadata;

        // Stop the scanning by disabling the behaviour
        mCloudRecoBehaviour.enabled = false;


        IEnumerator GetAssetBundle()
        {
            {
                var bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, loadAsset.name));
                yield return bundleLoadRequest;

                var myLoadedAssetBundle = bundleLoadRequest.assetBundle;

                if (myLoadedAssetBundle == null)
                {
                    Debug.Log("Failed to load asset bundle");
                    yield break;
                }
                var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(loadAsset.fullName);
                yield return assetLoadRequest;

                GameObject prefab;

                prefab = assetLoadRequest.asset as GameObject;

                if (loadAsset.extension == ".fbx")
                {
                    prefab.transform.localScale = new Vector3(3, 3, 3);
                    prefab.transform.localRotation = new Quaternion(0, 90, 180, 0);
                }
                else
                {
                    prefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    prefab.transform.localRotation = new Quaternion(0, 90, 180, 0);
                }

                Instantiate(prefab, ImageTargetTemplate.transform, false);
                Debug.Log(prefab.tag);

                myLoadedAssetBundle.Unload(false);
            }

        }

        // Build augmentation based on target 
        if (ImageTargetTemplate)
        {
            StartCoroutine(GetAssetBundle());
            mCloudRecoBehaviour.EnableObservers(cloudRecoSearchResult, ImageTargetTemplate.gameObject);
            /* Enable the new result with the same ImageTargetBehaviour: */
        }



    }
    //Unregister cloud reco callbacks when the handler is destroyed
    void OnDestroy()
    {
        mCloudRecoBehaviour.UnregisterOnInitializedEventHandler(OnInitialized);
        mCloudRecoBehaviour.UnregisterOnInitErrorEventHandler(OnInitError);
        mCloudRecoBehaviour.UnregisterOnUpdateErrorEventHandler(OnUpdateError);
        mCloudRecoBehaviour.UnregisterOnStateChangedEventHandler(OnStateChanged);
        mCloudRecoBehaviour.UnregisterOnNewSearchResultEventHandler(OnNewSearchResult);
    }


    public void ResetScanning()
    {
        if (!mIsScanning)
        {
            // Reset Behaviour
            var rendererComponents = mCloudRecoBehaviour.GetComponentsInChildren<Renderer>(true);

            foreach (var component in rendererComponents)
                //destroy component;
                Destroy(component);

            mCloudRecoBehaviour.enabled = true;
            mTargetMetadata = "";
            OnStateChanged(true);
        }
    }


    public class metaData
    {
        public string name { get; set; }

        public string extension { get; set; }

        public string fullName { get; set; }
    }
}
