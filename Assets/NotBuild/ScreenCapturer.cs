using UnityEngine;
using System.Collections;

public class ScreenCapturer : MonoBehaviour {

	public int Width = 1024;
	public int Height = 1024;
	Camera mCamera;
	// Use this for initialization
	void Start () {
		mCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	[ContextMenu("Capture")]
	void Capture()
	{
		Rect rect = new Rect (0, 0, Width, Height);

		// 创建一个RenderTexture对象  
		RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);  
		// 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
		mCamera.targetTexture = rt;  
		mCamera.Render();  
		//ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
		//ps: camera2.targetTexture = rt;  
		//ps: camera2.Render();  
		//ps: -------------------------------------------------------------------  
		
		// 激活这个rt, 并从中中读取像素。  
		RenderTexture.active = rt;  
		Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24,false);  
		screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
		screenShot.Apply();  
		
		// 重置相关参数，以使用camera继续在屏幕上显示  
		mCamera.targetTexture = null;  
		//ps: camera2.targetTexture = null;  
		RenderTexture.active = null; // JC: added to avoid errors  
		GameObject.Destroy(rt);
		// 最后将这些纹理数据，成一个png图片文件  
		byte[] bytes = screenShot.EncodeToPNG();  
		string filename = Application.dataPath + "/Screenshot.png";  
		System.IO.File.WriteAllBytes(filename, bytes);  
		Debug.Log(string.Format("截屏了一张照片: {0}", filename));  
	}
}
