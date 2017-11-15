using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                ThreadPool.QueueUserWorkItem(new WaitCallback(state => LoadPage(index)));
            } 
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void LoadPage(int index)
    {

        Bitmap bitmap = (Bitmap)Image.FromFile(fileName);


        //  Debug.LogWarning("Original bitmap size: " + originalBitmap.GetFrameCount(FrameDimension.Page));


        Debug.LogWarning("New bitmap size: " + bitmap.GetFrameCount(FrameDimension.Page));
    

        try
        {

            using (MemoryStream stream = new MemoryStream())
            {
                Debug.Log("Loading tif page " + (index + 1) + " of " + pageCount);

                // save each frame to a bytestream
                Guid frameGuid = bitmap.FrameDimensionsList[0];

                FrameDimension objDimension = new FrameDimension(frameGuid);

                bitmap.SelectActiveFrame(objDimension, index);

                bitmap.Save(stream, ImageFormat.Png);

                // and then create a new Image from it
                pages[index] = Image.FromStream(stream);
            }

            Bitmap testBit = new Bitmap(pages[index]);

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
