﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Drawing;
using System;
using System.Text.RegularExpressions;
using DocImageUtility;
using System.ComponentModel;
using System.Runtime.InteropServices;

public class Panorama : SiteElement
{
    public const int FRONT_INDEX = 0;
    public const int BACK_INDEX = 1;
    public const int UP_INDEX = 2;
    public const int DOWN_INDEX = 3;
    public const int RIGHT_INDEX = 4;
    public const int LEFT_INDEX = 5;

    private string leftEyePath;
    private string rightEyePath;

    public Material leftEye;
    public Material rightEye;

    [SerializeField] private string defaultCamPath = "./defaultCam.json";
    [SerializeField] private string defaultLeftEyePath = "./leftEye.tif";
    [SerializeField] private string defaultRightEyePath = "./rightEye.tif";

    protected override IEnumerator ActivateCoroutine()
    {

        List<Camera> allCams = CAVECameraRig.allCameras;

        foreach (Camera cam in allCams)
        {

            Skybox camSkybox = cam.GetComponent<Skybox>();

            if (camSkybox == null)
            {

                camSkybox = cam.gameObject.AddComponent<Skybox>();

            }

            camSkybox.enabled = true;

            if (cam.stereoTargetEye == StereoTargetEyeMask.Left)
            {

                camSkybox.material = leftEye;

            }
            else
            {

                camSkybox.material = rightEye;

            }
        }

        yield return null;
    }

    protected override IEnumerator DeactivateCoroutine()
    {

        List<Camera> allCams = CAVECameraRig.allCameras;

        foreach (Camera cam in allCams)
        {

            Skybox camSkybox = cam.GetComponent<Skybox>();

            if (camSkybox != null)
            {

                camSkybox.enabled = false;

            }
        }

        yield return null;
    }

    protected override IEnumerator LoadCoroutine()
    {

        SerializableCAVECam camData = siteData as SerializableCAVECam;

        Debug.Log("Loading " + camData.name);

        leftEyePath = camData.left_eye;
        rightEyePath = camData.right_eye;

        if (!File.Exists(leftEyePath) || !File.Exists(rightEyePath))
        {
            StatusText.SetText("Failed to load files");
            Debug.LogErrorFormat("Could not load pano: Failed to find left pano {0} or right pano {1}", leftEyePath, rightEyePath);

        }

        else {

            yield return StartCoroutine(LoadAndSaveCubemapMaterials(rightEyePath, true));

            if (CAVECameraRig.isCAVE)
            {
                yield return StartCoroutine(LoadAndSaveCubemapMaterials(leftEyePath, false));
            }
            else
            {
                leftEye = rightEye;
            }

            loaded = true;

            yield return null;

        }
    }

    public IEnumerator LoadAndSaveCubemapMaterials(string path, bool isRightEye)
    {
        List<Texture2D> textures = new List<Texture2D>();

        if (Directory.Exists(GetCacheDirectory(path)))
        {
            StatusText.SetText("Loading textures from cache");


            yield return StartCoroutine(GetTexturesFromCache(path, textures));
        }
        else
        {
            StatusText.SetText("Loading textures from tif");

            yield return StartCoroutine(GetTexturesFromTif(path, textures));
        }

        int texSize = textures[0].width;
        TextureFormat format = textures[0].format;

        StatusText.SetText("Creating cubemap");

        yield return null;

        Cubemap cubemap = new Cubemap(texSize, format, false);

        yield return StartCoroutine(CreateCubemapFromTextures(textures, cubemap));

        cubemap.Apply();

        Material eyeMat = new Material(Shader.Find("Skybox/Cubemap"));
        eyeMat.SetTexture(Shader.PropertyToID("_Tex"), cubemap);

        if (isRightEye)
        {
            rightEye = eyeMat;
        }
        else
        {
            leftEye = eyeMat;
        }

        loaded = true;

    }

    protected override IEnumerator UnloadCoroutine()
    {
        leftEye = null;
        rightEye = null;
        yield return null;
    }

    private IEnumerator LoadCamFromFile(string camJSONPath)
    {

        SerializableCAVECam camFile;

        if (File.Exists(camJSONPath))
        {

            string camJson = File.ReadAllText(camJSONPath);

            camFile = JsonUtility.FromJson<SerializableCAVECam>(camJson);

            Debug.Log("Found Cam. Loading");

            yield return null;

        }

        else
        {

            if (File.Exists(defaultCamPath))
            {

                Debug.LogWarning("No CAVECam JSON found at " + camJSONPath + ". Attempting to use default path: " + defaultCamPath);

                string objJSON = File.ReadAllText(defaultCamPath);

                camFile = JsonUtility.FromJson<SerializableCAVECam>(objJSON);

                yield return null;

            }
            else
            {

                if (!File.Exists(defaultLeftEyePath) || !File.Exists(defaultRightEyePath))
                {
                    Debug.LogErrorFormat("Cannot load CAVECam, and no defaults set. Please create defaults to prevent this error");
                    yield break;
                }

                camFile = new SerializableCAVECam(defaultLeftEyePath, defaultRightEyePath, "Default Text");

                File.WriteAllText(defaultCamPath, JsonUtility.ToJson(camFile));

                Debug.LogWarningFormat("No CAVECam JSON found at {0}, and no default JSON at {1}. Creating a blank cam with left eye {2} and right eye {3}.", camJSONPath, defaultCamPath, defaultLeftEyePath, defaultRightEyePath);

                yield return null;

            }
        }

        siteData = camFile;
        Load();
    }

    
    public IEnumerator GetTexturesFromCache(string filePath, List<Texture2D> textures)
    {

        string cacheDirectory = GetCacheDirectory(filePath);
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        if (!Directory.Exists(cacheDirectory))
        {
            yield return StartCoroutine(GetTexturesFromTif(filePath, textures));
            yield break;
        }

        string[] facePaths = new string[6];

        string frontPath = cacheDirectory + "/" + fileName + "_front.png";
        string backPath = cacheDirectory + "/" + fileName + "_back.png";
        string leftPath = cacheDirectory + "/" + fileName + "_left.png";
        string rightPath = cacheDirectory + "/" + fileName + "_right.png";
        string upPath = cacheDirectory + "/" + fileName + "_up.png";
        string downPath = cacheDirectory + "/" + fileName + "_down.png";

        facePaths[FRONT_INDEX] = frontPath;
        facePaths[BACK_INDEX] = backPath;
        facePaths[LEFT_INDEX] = leftPath;
        facePaths[RIGHT_INDEX] = rightPath;
        facePaths[DOWN_INDEX] = downPath;
        facePaths[UP_INDEX] = upPath;

        textures.Clear();

        if (File.Exists(frontPath) && File.Exists(backPath) && File.Exists(leftPath) && File.Exists(rightPath) && File.Exists(upPath) && File.Exists(downPath))
        {

            for (int i = 0; i < 6; i++)
            {

                string statusText = string.Format("Stage 1 of 2: Loading textures: \n{0} of {1}", i+1, 6);
                StatusText.SetText(statusText);

                string fullPath = Path.GetFullPath(facePaths[i]);

                yield return null;

                fullPath = fullPath.Replace("\\", "/");
                string fullPathWWW = "file://" + fullPath;

                Debug.Log("Loading image into texture from " + fullPath);

                yield return null;

                Texture2D newTex = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                WWW newWWW = new WWW(fullPathWWW);

                yield return newWWW;

                newWWW.LoadImageIntoTexture(newTex);

                while (!newWWW.isDone)
                {
                    yield return null;
                }

                Debug.Log("Done loading");

                //  byte[] bytes = File.ReadAllBytes(fullPath);
                // newTex.LoadImage(bytes);

                yield return null;

                textures.Add(newTex);

            }
        }
        else
        {

            yield return StartCoroutine(GetTexturesFromTif(filePath, textures));
            yield break;

        }

        yield return null;
    }
    

    public IEnumerator GetTexturesFromTif(string tifPath, List<Texture2D> textures)
    {

        List<Image> images = new List<Image>();

        yield return StartCoroutine(LoadTifPages(tifPath, images));

        Debug.Log("Loaded Tif Images");

        yield return StartCoroutine(LoadImagesAsTextures(images, textures));

        CacheTextures(textures, tifPath);

    }

    public static string GetCacheDirectory(string filePath)
    {

        string fileName = Path.GetFileNameWithoutExtension(filePath);

        string destinationFolder = GameManager.cacheDirectory + "/" + fileName;

        return destinationFolder;

    }

    public void CacheTextures(List<Texture2D> textures, string imagePath)
    {

        string cacheDirectory = GetCacheDirectory(imagePath);

        if (Directory.Exists(cacheDirectory))
        {

            foreach (string file in Directory.GetFiles(cacheDirectory))
            {
                File.Delete(file);
            }

            Directory.Delete(cacheDirectory);
        }

        Directory.CreateDirectory(cacheDirectory);

        string fileName = Path.GetFileNameWithoutExtension(imagePath);

        string cachedPathFormat = cacheDirectory + "/" + fileName + "_{0}.png";

        for (int i = 0; i < textures.Count; i++)
        {

            string finalpath = "";

            switch (i)
            {
                case FRONT_INDEX:
                    finalpath = string.Format(cachedPathFormat, "front");
                    break;

                case BACK_INDEX:
                    finalpath = string.Format(cachedPathFormat, "back");
                    break;

                case LEFT_INDEX:
                    finalpath = string.Format(cachedPathFormat, "left");
                    break;

                case RIGHT_INDEX:
                    finalpath = string.Format(cachedPathFormat, "right");
                    break;

                case UP_INDEX:
                    finalpath = string.Format(cachedPathFormat, "up");
                    break;

                case DOWN_INDEX:
                    finalpath = string.Format(cachedPathFormat, "down");
                    break;
            }

            byte[] bytes = textures[i].EncodeToPNG();

            File.WriteAllBytes(finalpath, bytes);

        }
    }
    

    public IEnumerator LoadTifPages(string imagePath, List<Image> tifImages)
    {
        StatusText.SetText("Stage 1: Loading Pages From Tif File");

        yield return null;

        TiffImage loadedTiff = new TiffImage(imagePath);

        loadedTiff.LoadAllPages();

        while (!loadedTiff.allPagesLoaded)
        {
            yield return null;
        }

        tifImages.AddRange(loadedTiff.pages);
       
        yield return null;

    }

    public void LoadImageSubsetAsTextures(List<Image> images, ImageToColorArray[] converters, int startIndex, int endIndex, int numThreadsPerImage)
    {

        try
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                Image img = images[i];
                Bitmap newBitmap = new Bitmap(img);

                ImageToColorArray converter = new ImageToColorArray(newBitmap, numThreadsPerImage);

                converters[i] = converter;

                converter.Convert();

            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    public IEnumerator LoadImagesAsTextures(List<Image> images, List<Texture2D> textures)
    {

        for (int i = 0; i < images.Count; i++)
        {
            Bitmap testBit = new Bitmap(images[i]);
        }


        int numConverterThreads = 1;
        int numThreadsPerImage = 2;

        StatusText.SetText("Stage 2: Starting threads to convert images");

        yield return null;

        ImageToColorArray[] converters = new ImageToColorArray[images.Count];
        int numConvertersPerThread = images.Count / numConverterThreads;

        for (int i = 0; i < numConverterThreads; i++)
        {

            int startIndex = numConvertersPerThread * i;
            int endIndex = numConvertersPerThread * (i + 1);

            if (i == numConverterThreads-1)
            {
                endIndex = images.Count;
            }

            Debug.LogFormat("Creating converter that starts at index {0} and ends at index {1}", startIndex, endIndex);

            ThreadPool.QueueUserWorkItem(new WaitCallback(state => LoadImageSubsetAsTextures(images, converters, startIndex, endIndex, numThreadsPerImage)));

        }

        StatusText.SetText("Stage 2: Converting images");

        yield return null;

        for (int i = 0; i < converters.Length; i++)
        {

            while (converters[i] == null || !converters[i].IsFinished())
            {
                yield return null;
            }
        }

        for (int i = 0; i < converters.Length; i++)
        {

            ImageToColorArray converter = converters[i];

            StatusText.SetText("Stage 3: Saving images:\n" + (i + 1) + " of " + converters.Length);

            yield return null;

            Texture2D newTex = new Texture2D(converter.width, converter.height);
            UnityEngine.Color[] pixels = converter.GetFinalArray();

            Debug.Log("pixels size is: " + pixels.Length);

            newTex.SetPixels(pixels);
            textures.Add(newTex);

        }
    }

    public IEnumerator CreateCubemapFromTextures(List<Texture2D> textures, Cubemap newCubemap)
    {
        Texture2D frontFace = textures[FRONT_INDEX];
        Texture2D backFace = textures[BACK_INDEX];
 
        Texture2D upFace = textures[UP_INDEX];
        Texture2D downFace = textures[DOWN_INDEX];

        Texture2D leftFace = textures[LEFT_INDEX];
        Texture2D rightFace = textures[RIGHT_INDEX];

        Debug.LogFormat("Setting Cubemap Faces from {0} textures", textures.Count);

        StatusText.SetText("Final Stage: Setting cubemap faces:\n1 of 6");
        yield return null;
        newCubemap.SetPixels(frontFace.GetPixels(), CubemapFace.PositiveZ);

        StatusText.SetText("Final Stage: Setting cubemap faces:\n 2 of 6");
        yield return null;
        newCubemap.SetPixels(upFace.GetPixels(), CubemapFace.PositiveY);

        StatusText.SetText("Final Stage: Setting cubemap faces:\n 3 of 6");
        yield return null;
        newCubemap.SetPixels(leftFace.GetPixels(), CubemapFace.NegativeX);

        StatusText.SetText("Final Stage: Setting cubemap faces:\n 4 of 6");
        yield return null;
        newCubemap.SetPixels(rightFace.GetPixels(), CubemapFace.PositiveX);

        StatusText.SetText("Final Stage: Setting cubemap faces:\n 5 of 6");
        yield return null;
        newCubemap.SetPixels(backFace.GetPixels(), CubemapFace.NegativeZ);

        StatusText.SetText("Final Stage: Setting cubemap faces:\n 6 of 6");
        yield return null;
        newCubemap.SetPixels(downFace.GetPixels(), CubemapFace.NegativeY);

        yield return null;
    }
}


[System.Serializable]
public class SerializableCAVECam : SerializableSiteElement
{

    public string left_eye;
    public string right_eye;

    public SerializableCAVECam(string leftEye, string rightEye, string description)
    {

        this.left_eye = leftEye;
        this.right_eye = rightEye;
        this.description = description;

    }
}
