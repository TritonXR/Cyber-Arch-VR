using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class TiffImage
{

    public TiffImage(string fileName)
    {
        this.FileName = fileName;
    }

    public string FileName { get; private set; }

    public int PageCount
    {
        get
        {
            // Get the frame dimension list from the image of the file and
            Image image = Image.FromFile(FileName);

            // Get the globally unique identifier (GUID)
            Guid frameGuid = image.FrameDimensionsList[0];

            // Create the frame dimension
            FrameDimension frameDimension = new FrameDimension(frameGuid);

            //Gets the total number of frames in the .tiff file
            return image.GetFrameCount(frameDimension);
        }
    }

    // Return the memory stream of a specific page
    public Image GetTiffSpecificPage(int pageNumber)
    {
        Image image = Image.FromFile(this.FileName);

        using (MemoryStream stream = new MemoryStream())
        {
            Guid frameGuid = image.FrameDimensionsList[0];

            FrameDimension objDimension = new FrameDimension(frameGuid);

            image.SelectActiveFrame(objDimension, pageNumber);

            image.Save(stream, ImageFormat.Png);

            Image streamImage = Image.FromStream(stream);

            return streamImage;
        }
    }

}