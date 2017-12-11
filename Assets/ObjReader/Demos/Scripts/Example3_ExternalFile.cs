using UnityEngine;
using System.Collections;

public class Example3_ExternalFile : MonoBehaviour {

	public string objFileName = "Car_obj.txt";
	public Material standardMaterial;
	public Material transparentMaterial;

	IEnumerator Start () {
		
		objFileName = Application.dataPath + "/ObjReader/Demos/Sample Files/" + objFileName;

        yield return null;

		ObjReader.use.ConvertFile (objFileName, true, standardMaterial, transparentMaterial);
		
	}
}