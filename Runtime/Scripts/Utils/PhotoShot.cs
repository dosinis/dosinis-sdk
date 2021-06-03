using System.IO;
using UnityEngine;

namespace DosinisSDK.Utils
{
    /// <summary>
    ///Screen Recorder will save individual images of active scene in any resolution and of a specific image format
    // including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.
    //
    // You can compile these images into a video using ffmpeg:
    // ffmpeg -i screen_3840x2160_%d.ppm -y test.avi
    /// </summary>
    public class PhotoShot : MonoBehaviour
    {
        [SerializeField] private int captureWidth = 3840;
        [SerializeField] private int captureHeight = 2160;

        [SerializeField] private GameObject hideGameObject;

        [SerializeField] private bool optimizeForManyScreenshots = true;
        public enum Format { RAW, JPG, PNG, PPM };
        [SerializeField] private Format format = Format.PNG;

        [SerializeField] private string folder;

        private Rect rect;
        private RenderTexture renderTexture;
        private Texture2D screenShot;
        private int counter = 0;

        private bool captureScreenshot = false;
        private bool captureVideo = false;

        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        private string GetFileName(int width, int height)
        {
            if (folder == null || folder.Length == 0)
            {
                folder = Application.dataPath;
                if (Application.isEditor)
                {
                    var stringPath = folder + "/..";
                    folder = Path.GetFullPath(stringPath);
                }
                folder += "/screenshots";

                Directory.CreateDirectory(folder);

                string mask = string.Format("screen_{0}x{1}*.{2}", width, height, format.ToString().ToLower());
                counter = Directory.GetFiles(folder, mask, SearchOption.TopDirectoryOnly).Length;
            }

            var filename = string.Format("{0}/screen_{1}x{2}_{3}.{4}", folder, width, height, counter, format.ToString().ToLower());

            ++counter;

            return filename;
        }

        private void ExportScreenshot()
        {
            string filename = GetFileName((int)rect.width, (int)rect.height);

            byte[] fileHeader = null;
            byte[] fileData = null;

            if (format == Format.RAW)
            {
                fileData = screenShot.GetRawTextureData();
            }
            else if (format == Format.PNG)
            {
                fileData = screenShot.EncodeToPNG();
            }
            else if (format == Format.JPG)
            {
                fileData = screenShot.EncodeToJPG();
            }
            else
            {
                string headerStr = string.Format("P6\n{0} {1}\n255\n", rect.width, rect.height);
                fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
                fileData = screenShot.GetRawTextureData();
            }

            new System.Threading.Thread(() =>
            {
                var f = File.Create(filename);
                if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
                f.Write(fileData, 0, fileData.Length);
                f.Close();
                Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));
            }).Start();

            if (hideGameObject != null) hideGameObject.SetActive(true);

            if (optimizeForManyScreenshots == false)
            {
                Destroy(renderTexture);
                renderTexture = null;
                screenShot = null;
            }
        }

        private void Update()
        {
            captureScreenshot |= Input.GetKeyDown("c");
            captureVideo = Input.GetKey("v");

            if (captureScreenshot || captureVideo)
            {
                captureScreenshot = false;

                if (hideGameObject != null)
                    hideGameObject.SetActive(false);

                if (renderTexture == null)
                {
                    rect = new Rect(0, 0, captureWidth, captureHeight);
                    renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
                    screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
                }

                cam.targetTexture = renderTexture;
                cam.Render();

                RenderTexture.active = renderTexture;
                screenShot.ReadPixels(rect, 0, 0);

                cam.targetTexture = null;
                RenderTexture.active = null;

                ExportScreenshot();
            }
        }
    }

}