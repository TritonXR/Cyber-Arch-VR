
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace DocImageUtility
{
    public class NewTiffImage
    {
        private string TIFF_CODEC = "image/tiff";
        private long ENCODING_SCHEME = (long)EncoderValue.CompressionCCITT4;
        public string SelectedPages { get; set; }
        public int NumberOfPages { get; set; }
        public string FilePath { get; set; }
        public bool newfile = true;
        public bool newdocument = true;
        Image Document;
        EncoderParameters encoderParams;
        public int SelectedPageCount { get { return Pages.Length; } }

        //First method to call in order to load the file and get its contents and pages
        public void Load(string path)
        {
            this.FilePath = path;
            //Get the frame dimension list from the image of the file and
            Image tiffImage = Image.FromFile(path);
            //get the globally unique identifier (GUID)
            Guid objGuid = tiffImage.FrameDimensionsList[0];
            //create the frame dimension
            FrameDimension dimension = new FrameDimension(objGuid);
            //Gets the total number of frames in the .tiff file
            NumberOfPages = tiffImage.GetFrameCount(dimension);
            tiffImage.Dispose();
        }
        //Split Method
        public void Split(string splittedFileName)
        {
            //Get its file information
            ImageCodecInfo codecInfo = GetCodecInfo(TIFF_CODEC);
            EncoderParameters encoderParams = new EncoderParameters(2);
            encoderParams.Param[0] = new
              EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
            encoderParams.Param[1] = new
             EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ENCODING_SCHEME);
            //Load the Document
            FileStream fs = new FileStream(this.FilePath, FileMode.OpenOrCreate);
            Image image = Image.FromStream(fs);
            //Get file frame/pages
            FrameDimension frameDim = new FrameDimension(image.FrameDimensionsList[0]);
            //Check if selected pages is null or 0 value
            if (SelectedPages != null)
            {
                // Delete / Overwrite existing file if updating Splitted Image File
                var file = new FileInfo(splittedFileName);
                if (file.Exists) file.Delete();
                //for each frame/pages create the new document
                for (int i = 0; i < Pages.Length; i++)
                {
                    //check whether selected pages is not greater than the file pages
                    if (Pages.Length >= (i + 1))
                    {
                        //Selected image frame
                        image.SelectActiveFrame(frameDim, Pages[i]);
                        //check whether file is new document
                        if (newfile == true)
                        {
                            image.Save(splittedFileName, codecInfo, encoderParams);
                            newfile = false;
                        }
                        else
                        //append the document depending on the selected frame from the original image
                        {
                            encoderParams.Param[0] = new
                             EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag,
                             (long)EncoderValue.FrameDimensionPage);
                            image.SaveAdd(image, encoderParams);
                        }
                    }
                }
                fs.Close();
            }
        }
        //Merge Method
        public void Merge(string MergeFileName, string source)
        {
            //Set its Image Format as TIFF
            ImageCodecInfo codecInfo = GetCodecInfo(TIFF_CODEC);
            //Load the Document
            FileStream fs = new FileStream(source, FileMode.OpenOrCreate);
            Image image = Image.FromStream(fs);
            //Get file frame/pages
            FrameDimension frameDim = new FrameDimension(image.FrameDimensionsList[0]);
            if (newdocument == true)
            {
                //Set its Image Type
                encoderParams = new EncoderParameters(2);
                encoderParams.Param[0] = new
                  EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag,
                  (long)EncoderValue.MultiFrame);
                encoderParams.Param[1] = new
                  EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ENCODING_SCHEME);
                //Check if selected pages is null or 0 value
                if (SelectedPages != null)
                {
                    //for each frame/pages create the new document
                    for (int i = 0; i < image.GetFrameCount(frameDim); i++)
                    {
                        //check whether selected pages is not greater than the file pages
                        if (Pages.Length >= (i + 1))
                        {
                            //Selected image frame
                            image.SelectActiveFrame(frameDim, Pages[i]);
                            //check whether file is new document
                            if (newfile == true)
                            {
                                //create new filename
                                Document = image;
                                //save
                                Document.Save(MergeFileName, codecInfo, encoderParams);
                                encoderParams.Param[0] =
                                new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag,
                                (long)EncoderValue.FrameDimensionPage);
                                newfile = false;
                                newdocument = false;
                            }
                            else
                            {
                                //append the document depending on the selected frame from the original image
                                Document.SaveAdd(image, encoderParams);
                            }
                        }
                    }
                    fs.Close();
                }
            }
            else
            {
                //Check if selected pages is null or 0 value
                if (SelectedPages != null)
                {
                    //for each frame/pages create the new document
                    for (int i = 0; i < image.GetFrameCount(frameDim); i++)
                    {
                        //check whether selected pages is not greater than the file pages
                        if (Pages.Length >= (i + 1))
                        {
                            //Selected image frame
                            image.SelectActiveFrame(frameDim, Pages[i]);
                            //check whether file is new document
                            //append the document depending on the selected frame from the original image
                            Document.SaveAdd(image, encoderParams);
                        }
                    }
                    fs.Close();
                }
            }
        }
        //Check whether selected pages is valid
        public int[] Pages
        {
            get
            {
                ArrayList ps = new ArrayList();
                string[] ss = SelectedPages.Split(new char[] { ',', ' ', ';' });
                foreach (string s in ss)
                    if (Regex.IsMatch(s, @"\d+-\d+"))
                    {
                        int start = int.Parse(s.Split(new char[] { '-' })[0]);
                        int end = int.Parse(s.Split(new char[] { '-' })[1]);
                        if (start > end)
                            return new int[] { 0 };
                        while (start <= end)
                        {
                            ps.Add(start - 1);
                            start++;
                        }
                    }
                    else
                    {
                        int i;
                        int.TryParse(s, out i);
                        if (i > 0)
                            ps.Add(int.Parse(s) - 1);
                    }
                return ps.ToArray(typeof(int)) as int[];
            }
        }
        private ImageCodecInfo GetCodecInfo(string codec)
        {
            ImageCodecInfo codecInfo = null;
            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.MimeType == codec)
                {
                    codecInfo = info;
                    break;
                }
            }
            return codecInfo;
        }
    }
}