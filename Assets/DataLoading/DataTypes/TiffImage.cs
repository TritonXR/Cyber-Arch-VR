using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

public class TiffImage {

    private string fileName;

    public bool allPagesLoaded;

    public Image[] pages;

    private int pageCount;
    private int finishedPages;

    public TiffImage(string fileName)
    {
        this.fileName = fileName;
    }

    public void LoadAllPages()
    {

        try
        {

            Bitmap bitmap = (Bitmap)Image.FromFile(fileName);
            pageCount = bitmap.GetFrameCount(FrameDimension.Page);

            pages = new Image[pageCount];

            for (int idx = 0; idx < pageCount; idx++)
            {
                int index = idx;
                Bitmap clonedBitmap = new Bitmap(bitmap);
                ThreadPool.QueueUserWorkItem(new WaitCallback(state => LoadPage(index, clonedBitmap)));

            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void LoadPage(int index, Bitmap bitmap)
    {

        try
        {

            Debug.Log("Loading tif page " + (index+1) + " of " + pageCount);

            // save each frame to a bytestream
            bitmap.SelectActiveFrame(FrameDimension.Page, index);
            MemoryStream byteStream = new MemoryStream();
            bitmap.Save(byteStream, ImageFormat.Tiff);

            // and then create a new Image from it
            pages[index] = Image.FromStream(byteStream);

            finishedPages++;

            if (finishedPages == pageCount)
            {
                allPagesLoaded = true;
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
}
