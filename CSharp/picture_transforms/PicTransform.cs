using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace test
{
    class main
    {
        static void Main(string[] args)
        {
            Picture pic = new Picture();
            switch (args.Length)
            {
                case 1:
                    pic.transforms(args[0]);
                    break;
                case 2:
                    pic.transforms(args[0], args[1].ToLower());
                    break;
                case 3:
                    long quality;
                    try
                    {
                        quality = long.Parse(args[2]);
                    } catch(System.FormatException e) {
                        Console.WriteLine(e);
                        return;
                    }
                    pic.transforms(args[0], args[1].ToLower(), quality);
                    break;
                default:
                    readme();
                    return;
            }
        }
        
        //输出用法
        public static void readme()
        {
            string info = "usage: [exe] path [ImageFormat] [quality]\n\n" +
                "ImageFormat (default=jpg):\njpg、png、bmp、tiff、gif\n\n" +
                "quality (default=90, jpg only):\n0-100\n";
            Console.WriteLine(info);
        }
    }
    
    //文件类
    class FileClass
    {
        //获取目录及子目录下的文件
        public static string[] GetFileList(string path)
        {
            List<string> fileList = new List<string>();
            
            if (Directory.Exists(path) == true)
            {
                foreach (string file in Directory.GetFiles(path))
                    fileList.Add(file);
                foreach (string directory in Directory.GetDirectories(path))
                    fileList.AddRange(GetFileList(directory));
            }
            
            return fileList.ToArray();
        }
        
        //获取新的目录名称
        public static string GetNewDirectorie(string path)
        {
            string p = "";
            p = path.Replace("\"","")+"_1\\";
            DirectoryInfo dir = new DirectoryInfo(p);
            dir.Create();
            return p;
        }
    }
    
    //图片转换类
    class Picture
    {
        Bitmap image;
        int SuccessCount;
        int ErrorCount;
        string NewPath = "";
        
        //转换多个文件
        public void transforms(string path,string ext="jpg",long quality=90)
        {
            string[] files = null;
            
            SuccessCount = 0;
            ErrorCount = 0;
            files = FileClass.GetFileList(path);
            
            if(files.Length == 0) return;
            NewPath = FileClass.GetNewDirectorie(path);
            
            foreach(string file in files)
            {
                transform(file, ext , quality);
            }
            
            ErrorCount = files.Length - SuccessCount;
            result();
        }
        
        //转换一个文件
        private void transform(string file, string ext="jpg", long quality=90)
        {
            string file_new;
            string[] files;
            
            files = file.Split('.');
            file_new = NewPath + files[0].Substring(files[0].LastIndexOf('\\')+1) + "." + ext;
            
            try
            {
                image = new Bitmap(file, true);
            } catch (System.ArgumentException e)
            {
                Console.WriteLine(file + "\n" + e);
                return;
            }
            
            switch(ext)
            {
                case "jpg":
                    ImageCodecInfo Encoder = GetEncoder(ImageFormat.Jpeg);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    image.Save(file_new, Encoder, myEncoderParameters);
                    break;
                case "png":
                    image.Save(file_new, ImageFormat.Png);
                    break;
                case "bmp":
                    image.Save(file_new, ImageFormat.Bmp);
                    break;
                case "tiff":
                    image.Save(file_new, ImageFormat.Tiff);
                    break;
                case "gif":
                    image.Save(file_new, ImageFormat.Gif);
                    break;
            }
            
            image.Dispose();
            SuccessCount++;
        }
        
        //选择图片格式
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)  
            {
                if (codec.FormatID == format.Guid) return codec;  
            }
            return null;
        }
        
        //输出结果
        private void result()
        {
            Console.WriteLine("Success: "+SuccessCount.ToString()+"\nError: "+ErrorCount.ToString());
        }
    }
}
