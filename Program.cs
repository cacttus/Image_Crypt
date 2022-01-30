using System;
using System.Drawing;
namespace Image_Dicer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    ShowHelp();
                }
                else
                {
                    string in_loc = args[0];
                    string out_loc = args[1];
                    bool overwrite = true;
                    int xsiz = 512;
                    int ysiz = 512;
                    int key = 2;
                    int iter = 1;
                    bool decrypt = false;
                    bool mask = true;
                    if (!System.IO.File.Exists(in_loc))
                    {
                        Console.WriteLine("File " + in_loc + " could not be found.");
                    }
                    else if (System.IO.File.Exists(out_loc) && overwrite == false)
                    {
                        Console.WriteLine("File " + out_loc + " already exists. Specify -o to overwrite..");
                    }
                    else
                    {
                        Bitmap in_img = new Bitmap(in_loc);
                        Bitmap out_img = null;
                        Bitmap dec_img = null;
                        ImageDicer.Encrypt(in_img, out out_img, decrypt, iter, xsiz, ysiz, key, mask);
                        out_img.Save(out_loc);
                        //Test
                        ImageDicer.Encrypt(out_img, out dec_img, true, iter, xsiz, ysiz, key, mask);
                        dec_img.Save("../../decrypt.png");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }
        private static void ShowHelp()
        {
            Console.WriteLine("Help:");
            Console.WriteLine(" Usage:");
            Console.WriteLine("   ImageDicer [-e|-d] [-x=512] [-y=512] [-i=1] [-k=0x190C70F4] [-o] 'InputImage' 'OutputImage'");
            Console.WriteLine(" Options:");
            Console.WriteLine("   -e: encrypt image");
            Console.WriteLine("   -d: decrypt image");
            Console.WriteLine("   -o: overwrite output image");
            Console.WriteLine("   -x: x block size");
            Console.WriteLine("   -y: y block size");
            Console.WriteLine("   -i: iterations");
            Console.WriteLine("   -k: hash key");
            Console.WriteLine(" Remarks:");
            Console.WriteLine("   Suports .Png, .Jpg, .Gif, .Bmp");

        }
    }

}
