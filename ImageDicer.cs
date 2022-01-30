using System;
using System.Security.Cryptography;
using System.Drawing;
namespace Image_Dicer
{
    public class ImageDicer
    {
        public static int[] primes = new int[] {
        4051, 24570149, 4570079, 1924573137, 124570247,
        80141, 124570079, 4570001, 1924573033, 180043,
            2570047, 1924573219, 2570189, 24570001, 624570127,
                 624570041 };
        private static int rotate(int xx, int rr)
        {
            rr &= 31;
            return (xx << (int)rr) | (xx >> (int)(32 - rr));
        }
        private static int pack(byte a, byte r, byte g, byte b)
        {
            int rr = (int)(((int)a << 24) & 0xFF000000) | (((int)r << 16) & 0x00FF0000) | (((int)g << 8) & 0x0000FF00) | (((int)b << 0) & 0x00000000FF);
            return rr;
        }
        private static int prand(Color lastPixel, int bi, int bj, int iter, int pixel, int key)
        {
            int p0 = primes[0];
            int p1 = primes[1];
            int p2 = primes[2];
            int p3 = primes[3];
            int p4 = primes[4];

            //TODO: use lastPixel for recurrant random

            int k = (key * p0) ^ (bi * p1) ^ (bj * p2) ^ (iter * p3) ^ (pixel * p4);
            long i = 1, y;
            y = (0x6c078965 * (k ^ (k >> 30)) + i) & 0xffffffff;
            y = y ^ (y >> 11);
            y = y ^ ((y << 7) & 0x9d2c5680);
            y = y ^ ((y << 15) & 0xefc60000);
            y = y ^ (y >> 18);
            int h = (int)y;

            return h;
        }
        /// <summary>
        /// Encrypt in_img into out_img.
        /// iterations, iblockx, iblocky and key are needed to decrypt.
        /// </summary>
        /// <param name="in_img">In image.</param>
        /// <param name="out_img">Out image.</param>
        /// <param name="reverse">If set to <c>true</c> reverse.</param>
        /// <param name="iterations">Iterations.</param>
        /// <param name="iblockx">Size of a puzzle pice X.</param>
        /// <param name="iblocky">Size of a puzzle pice Y.</param>
        /// <param name="key">Cipher key to use</param>
        /// <param name="mask">Use color mask to change image colors.</param>
        public static void Encrypt(Bitmap in_img, out Bitmap out_img, bool reverse, int iterations = 1, int iblockx = 512, int iblocky = 512, int key = 0xF9009C9, bool mask = false)
        {
            //* Increase iterations for a fuzzier image, but takes longer.
            //* Blockx/y is for a puzzle. Set to 1 for single pixel scramble
            //* TODO: add boolto make the block size logarithmic with each pass (block/2 .. )
            //TODO: make sure block is a power of 2 or 1
            if (iterations == 0)
            {
                iterations = 1;
            }
            if (iblockx == 0)
            {
                iblockx = 1;
            }
            if (iblocky == 0)
            {
                iblocky = 1;
            }

            key = (int)((key ^ 0x1c000101));

            out_img = new Bitmap(in_img);
            Color lastPixel = Color.FromArgb(127, 63, 71, 19);

            for (int xxiter = 0; xxiter < iterations; xxiter++)
            {

                int blockx = iblockx;
                int blocky = iblocky;
                //if (logblocksize)
                //{
                // this wont work in reverse. Fix this.
                //    blockx /=2;
                //    blocky /=2;
                //}
                int blocksW = (int)Math.Ceiling((double)out_img.Width / (double)blockx);
                int blocksH = (int)Math.Ceiling((double)out_img.Height / (double)blocky);

                for (int i = 0; i < (blocksW * blockx); i += blockx)
                {
                    for (int j = 0; j < (blocksH * blocky); j += blocky)
                    {

                        int p0 = primes[5];
                        int p1 = primes[6];
                        int p2 = primes[7];
                        int p3 = primes[8];

                        //top left coords of puzzle piece
                        int a0, a1, b0, b1;
                        int ir,jr,itr;
                        if (!reverse)
                        {
                            ir = i;
                            jr = j;
                            itr = xxiter;
                        }
                        else
                        {
                            //reverse.
                            //original w - i - 1
                            ir = (blocksW * blockx) - (i) - blockx;
                            jr = (blocksH * blocky) - (j) - blocky;
                            itr = iterations - xxiter - 1;
                        }

                        //If you us pseudorandom blocks you end up with an image halo. Using each block makes sure the image is mostly noise.
                        a0 =ir;// (int)((uint)(prand(lastPixel, ir, jr, itr, p0, key)) % (uint)out_img.Width);
                        a1 =jr;// (int)((uint)(prand(lastPixel, ir, jr, itr, p1, key)) % (uint)out_img.Height);
                        b0 = (int)((uint)(prand(lastPixel, ir, jr, itr, p2, key)) % (uint)out_img.Width);
                        b1 = (int)((uint)(prand(lastPixel, ir, jr, itr, p3, key)) % (uint)out_img.Height);

                        int idx0 = a1 * out_img.Height + a0;
                        int idx1 = b1 * out_img.Height + b0;

                        //swap puzzle pieces (bit blt would be faster)
                        for (int xx = 0; xx < blockx; xx++)
                        {
                            for (int yy = 0; yy < blocky; yy++)
                            {
                                int xpix1, ypix1, xpix2, ypix2;
                                if (!reverse)
                                {
                                    xpix1 = (a0 + xx) % out_img.Width;
                                    ypix1 = (a1 + yy) % out_img.Height;
                                    xpix2 = (b0 + xx) % out_img.Width;
                                    ypix2 = (b1 + yy) % out_img.Height;
                                }
                                else
                                {
                                    int xxr = blockx - xx - 1;
                                    int yyr = blocky - yy - 1;
                                    xpix1 = (a0 + xxr) % out_img.Width;
                                    ypix1 = (a1 + yyr) % out_img.Height;
                                    xpix2 = (b0 + xxr) % out_img.Width;
                                    ypix2 = (b1 + yyr) % out_img.Height;
                                }
                                var pix1 = out_img.GetPixel(xpix1, ypix1);
                                var pix2 = out_img.GetPixel(xpix2, ypix2);

                                if (mask)
                                {
                                    //MASK AND SHIFT color

                                    int cmask = pack(0,(byte)(idx0 % 255), (byte)(idx1 % 255), (byte)((idx0*idx1) % 255));
                                    int r1 = pix1.ToArgb();
                                    int r2 = pix2.ToArgb();
                                    int r1m = 0;
                                    int r2m = 0;
                                   
                                    // a b
                                    //c1 = a ^ b
                                    //t = a ^ mask
                                    //c2 = c1 c1 = c2
                                    //t = c1 ^ mask
                                    //b = c1 ^ t
                                    //c1 = b c2 = a;
                                    if (!reverse)
                                    {
                                        r1m = r1 ^ cmask;//c
                                        r2m = r2 ^ cmask;//d
                                    }
                                    else
                                    {
                                        // 1=d, 2=c
                                        r1m = r1 ^ cmask;
                                        r2m = r2 ^ cmask;
                                    }

                                    pix1 = Color.FromArgb(r1m);
                                    pix2 = Color.FromArgb(r2m);
                                }

                                //swap
                                out_img.SetPixel(xpix1, ypix1, pix2);
                                out_img.SetPixel(xpix2, ypix2, pix1);

                                lastPixel = pix2;

                            }
                        }



                    }
                }

            }
        }
    }
}
