using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Model : SiteElement
{
    public GameObject model;

    private float rotationSpeed = 0.1f;
    private bool rotate = true;

    protected override IEnumerator ActivateCoroutine()
    {
        if (model != null)
        {

            model.SetActive(true);
            yield return null;

        }
    }

    protected override IEnumerator DeactivateCoroutine()
    {
        if (model != null)
        {
            model.SetActive(false);

            yield return null;
        }
    }

    protected override IEnumerator LoadCoroutine()
    {

        SerializableModel modelData = siteData as SerializableModel;

        GameObject pivot = new GameObject("Position Pivot");

        Debug.Log("Loading Model");
        yield return null;

        string path = modelData.filePath;

        if (!File.Exists(path))
        {
            Debug.LogError("Failed to find file: " + path);
            failed = true;
            yield break;
        }


        if (Path.GetExtension(path) == ".dae")
        {
            string colladaString = File.ReadAllText(path);
            model = ColladaImporter.Import(colladaString);
            LoadTextures(model, colladaString);

            if (model == null)
            {
                Debug.LogError("Failed to load model from " + path);
            }

            yield return null;
        }
        else if (Path.GetExtension(path) == ".obj")
        {

            Debug.LogWarning("LOADING OBJ");
            yield return null;


            Debug.LogWarning("Preprocessing Model");
            yield return StartCoroutine(PreprocessObjFile(path));
            Debug.LogWarning("DONE");


            Debug.LogWarning("Loading Model...");
            yield return null;


            List<GameObject> loadedObjects = new List<GameObject>();
            yield return StartCoroutine(LoadObjModel(path, loadedObjects));


            Debug.LogWarning("DONE");

            model = new GameObject(modelData.name);
            model.transform.parent = this.transform;


            pivot.transform.parent = model.transform;
            pivot.transform.localPosition = Vector3.zero;
            pivot.transform.localRotation = Quaternion.identity;

            foreach (GameObject obj in loadedObjects)
            {

                obj.transform.parent = pivot.transform;

            }
        }
        else
        {

            Debug.LogError("Failed to load model, unsupported file extension: " + path);
            yield break;
        }

        if (model == null)
        {
            yield break;
        }

        model.SetActive(false);
    }

    protected override IEnumerator UnloadCoroutine()
    {
        throw new NotImplementedException();
    }


    private void CenterModel(GameObject positionPivot)
    {


        Vector3 summedPositions = Vector3.zero;

        MeshRenderer[] allRenderers = model.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in allRenderers)
        {
            summedPositions += renderer.bounds.center;
        }

        summedPositions /= allRenderers.Length;

        Vector3 distance = summedPositions - positionPivot.transform.position;

        positionPivot.transform.position -= distance;

    }

    private void FixModelRotation(GameObject positionPivot)
    {

        GameObject rotationPivot = new GameObject("Rotation Pivot");
        rotationPivot.transform.parent = model.transform;
        rotationPivot.transform.localPosition = Vector3.zero;
        rotationPivot.transform.localRotation = Quaternion.identity;
        positionPivot.transform.parent = rotationPivot.transform;

        rotationPivot.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));

    }

    private IEnumerator LoadObjModel(string path, List<GameObject> objList)
    {
        Material defaultMat = new Material(Shader.Find("Standard"));
        ObjReader.ObjData loadData = ObjReader.use.ConvertFileAsync("file://" + Path.GetFullPath(path), true, defaultMat);

        int sameFrames = 0;
        float prevProgress = 0;

        int cycleNum = 0;

        while (!loadData.isDone)
        {

            string progressDots = ".";
            if (cycleNum > 0)
            {
                if (cycleNum > 1)
                {
                    progressDots += "..";
                    cycleNum = 0;
                }
                else
                {
                    progressDots += ".";
                    cycleNum++;
                }
            }
            else
            {
                cycleNum++;
            }

            float progress = loadData.progress;


            if (progress == prevProgress)
            {
                sameFrames++;
            }
            else
            {
                sameFrames = 0;
                prevProgress = progress;
            }

            if (sameFrames > 90)
            {
                yield break;
            }

            yield return null;
        }

        if (loadData.gameObjects != null)
        {

            objList.AddRange(loadData.gameObjects);

        }
    }

    private IEnumerator PreprocessObjFile(string filepath)
    {
        Debug.Log("Processing file " + filepath);
        yield return null;

        StringBuilder resultString = new StringBuilder();

        Debug.Log("Reading file into memory...");
        yield return null;

        string[] file = File.ReadAllLines(filepath);

        Debug.Log("Done!");
        yield return null;

        HashSet<string> seenIndices = new HashSet<string>();

        int groupNum = 0;

        for (int lineIndex = 0; lineIndex < file.Length; lineIndex++)
        {
            if (lineIndex % 50000 == 0)
            {
                Debug.LogFormat("Processing Line {0} of file with {1} vertices active in hash set", lineIndex, seenIndices.Count);
                yield return null;

            }

            string line = file[lineIndex];

            resultString.AppendLine(line);

            if (string.IsNullOrEmpty(line) || (line[0] != 'f' && line[0] != 'g'))
            {
                continue;
            }

            if (line[0] == 'f')
            {

                string[] lineValues = line.Split(' ', '/');

                for (int i = 1; i < lineValues.Length; i++)
                {
                    string index = lineValues[i];
                    if (!seenIndices.Contains(index))
                    {
                        seenIndices.Add(index);

                    }

                    if (seenIndices.Count >= ObjReader.use.maxPoints)
                    {

                        if (lineIndex < file.Length - 1 && file[lineIndex + 1][0] != 'g')
                        {

                            resultString.AppendLine("g group" + groupNum++);

                            Debug.LogFormat("Processed group {0} of {1} vertices", groupNum - 1, seenIndices.Count);

                            seenIndices.Clear();

                            yield return null;
                        }

                    }
                }
            }
            else if (line[0] == 'g')
            {
                seenIndices.Clear();
            }
        }

        File.WriteAllText(filepath, resultString.ToString());

        yield return null;

    }

    private IEnumerator LoadTextures(GameObject go, string originalUrl)
    {
        string path = originalUrl;
        int lastSlash = path.LastIndexOf('/', path.Length - 1);
        if (lastSlash >= 0) path = path.Substring(0, lastSlash + 1);
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                if (m.mainTexture != null)
                {
                    Texture2D texture = null;
                    string texUrl = path + m.mainTexture.name;
                    yield return StartCoroutine(LoadTexture(texUrl, retval => texture = retval));
                    if (texture != null)
                    {
                        m.mainTexture = texture;
                    }
                }
            }
        }
    }

    private IEnumerator LoadTexture(string url, System.Action<Texture2D> result)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error != null)
        {

            Debug.LogError("Failed to load texture: " + url);

        }
        else
        {
        }
        result(www.texture);
    }
}

[System.Serializable]
public class SerializableModel : SerializableSiteElement
{
    public string filePath;
}


