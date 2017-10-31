using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Drawing;
using System.Threading;
using System;

public class ImageToColorArray
{

    public Bitmap bitmap;

    private int numThreadsPerImage = 5;
    private UnityEngine.Color[] finalColorArray;

    private List<ImagePieceToColorArray> converters;

    private bool finished = false;

    public ImageToColorArray(Bitmap originalImage, int numThreadsPerImage)
    {

        bitmap = originalImage;
        this.numThreadsPerImage = numThreadsPerImage;

    }

    public void Convert()
    {

        try
        {
            Debug.Log("Starting conversion of entire file on thread " + Thread.CurrentThread.ManagedThreadId);
            converters = new List<ImagePieceToColorArray>();

            int picHeight = bitmap.Height;

            for (int i = 0; i < numThreadsPerImage; i++)
            {

                int start = i * (picHeight / numThreadsPerImage);
                int end = (i + 1) * (picHeight / numThreadsPerImage);

                if (i == numThreadsPerImage - 1)
                {
                    end = picHeight;
                }

                ImagePieceToColorArray newConverter = new ImagePieceToColorArray(new Bitmap(bitmap), start, end);
                converters.Add(newConverter);

                ThreadPool.QueueUserWorkItem(new WaitCallback(state => newConverter.Convert()));
            }

        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Exception on thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, e);
        }
    }

    public bool IsFinished()
    {

        foreach (ImagePieceToColorArray converter in converters)
        {

            if (!converter.finished)
            {
                return false;
            }

        }

        if (finished == false)
        {
            CombinePieces();
        }

        return finished;

    }

    private void CombinePieces()
    {

        List<UnityEngine.Color> colorList = new List<UnityEngine.Color>();

        for (int i = 0; i < converters.Count; i++)
        {
            UnityEngine.Color[] colorArray = converters[i].GetFinalArray();

            colorList.AddRange(colorArray);

        }

        finalColorArray = colorList.ToArray();
        finished = true;
     
    }

    public UnityEngine.Color[] GetFinalArray()
    {

        if (IsFinished())
        {
            return finalColorArray;
        }
        else
        {
            Debug.LogError("Trying to get final color array before conversion is finished!");
            return null;
        }

    }
}
