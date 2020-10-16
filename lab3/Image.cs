using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;


namespace lab3
{
    class Image
    {
        public static Image<Bgr, byte> Scale(Image<Bgr, byte> sourceImage, double k)
        {
            Image<Bgr, byte> scaledImage = new Image<Bgr, byte>((int)(sourceImage.Width * k), (int)(sourceImage.Height * k));
            double I = 0;
            double J = 0;

            for (int i = 0; i < scaledImage.Width ; i++)
                for (int j = 0; j < scaledImage.Height ; j++)
                {
                    I = (int)(i / k);
                    J = (int)(j / k);

                    scaledImage[j, i] = sourceImage[(int)J, (int)I];
                }

            Smooth(sourceImage, scaledImage, k, I, J);

            return scaledImage;
        }

        public static Image<Bgr, byte> Share(Image<Bgr, byte> sourceImage, double sh)
        {
            Image<Bgr, byte> scaledImage = new Image<Bgr, byte>((int)sourceImage.Width, (int)sourceImage.Height);
            double newX = 0;
            double newY = 0;

            for (int x = 0; x < sourceImage.Width; x++)
                for (int y = 0; y < sourceImage.Height; y++)
                {
                    newX = (x - sh * (sourceImage.Height - y));
                    newY = y;

                    if (newX >= sourceImage.Width || newX < 0) continue;

                    scaledImage[y, x] = sourceImage[y, (int)newX];
                }

            Smooth(sourceImage, scaledImage, sh, newX, newY);

            return scaledImage;
        }

        public static Image<Bgr, byte> Rotate(Image<Bgr, byte> sourceImage, double a)
        {
            Image<Bgr, byte> scaledImage = new Image<Bgr, byte>((int)sourceImage.Width, (int)sourceImage.Height);
            double newX = 0;
            double newY = 0;

            double angleRadians = a * Math.PI / 180d;

            for (int x = 0; x < scaledImage.Width; x++)
                for (int y = 0; y < scaledImage.Height; y++)
                {
                    newX = Math.Abs(Math.Cos(angleRadians) * (x - scaledImage.Width / 2) - Math.Sin(angleRadians) * (y - scaledImage.Height / 2) + scaledImage.Width / 2);
                    newY = Math.Abs(Math.Sin(angleRadians) * (x - scaledImage.Width / 2) + Math.Cos(angleRadians) * (y - scaledImage.Height / 2) + scaledImage.Height / 2);

                    if (newX >= sourceImage.Width || newY>= sourceImage.Height) continue;

                    scaledImage[y, x] = sourceImage[(int)newY, (int)newX];
                }
            //Smooth(sourceImage, scaledImage, a, newX, newY);

            return scaledImage;
        }

        public static Image<Bgr, byte> Reflect(Image<Bgr, byte> sourceImage, int qX, int qY)
        {
            Image<Bgr, byte> scaledImage = new Image<Bgr, byte>((int)sourceImage.Width, (int)sourceImage.Height);

            double newX = 0;
            double newY = 0;

            for (int x = 0; x < scaledImage.Width; x++)
                for (int y = 0; y < scaledImage.Height; y++)
                {
                    if (qX == 1 && qY == 1)
                    {
                        newX = x;
                        newY = y;
                    }
                    else if (qX == -1 && qY == -1)
                    {
                        newX = (x * (qX) + sourceImage.Width);
                        newY = (y * (qY) + sourceImage.Height);
                    }
                    else if(qX == 1 && qY == -1)
                    {
                        newX = (x * (qX));
                        newY = (y * (qY) + sourceImage.Height);
                    }
                    else if(qX == -1 && qY == 1)
                    {
                        newX = (x * (qX) + sourceImage.Width);
                        newY = (y * (qY));
                    }

                    if (newX >= sourceImage.Width || newY >= sourceImage.Height) continue;

                    scaledImage[y, x] = sourceImage[(int)newY, (int)newX];
                }
            
            return scaledImage.Resize(640/2, 480/2, Inter.Linear);
        }

        public static Image<Bgr, byte> Smooth(Image<Bgr, byte> sourceImage, Image<Bgr, byte> scaledImage, double k, double I, double J)
        {
            for (int i = 0; i < scaledImage.Width - 1; i++)
                for (int j = 0; j < scaledImage.Height - 1; j++)
                {
                    //double I = (int)(i / k);
                    //double J = (int)(j / k);

                    //sglagivanie pikselov pri masshtabirovanii
                    double baseI = Math.Floor(I);
                    double baseJ = Math.Floor(J);

                    if (baseI >= sourceImage.Width - 1) continue;
                    if (baseJ >= sourceImage.Height - 1) continue;

                    double rI = I - baseI;
                    double rJ = J - baseJ;

                    double irI = 1 - rI;
                    double irJ = 1 - rJ;

                    Bgr c1 = new Bgr();// promegutoch. cvet po gorizont
                    c1.Blue = sourceImage.Data[(int)baseJ, (int)baseI, 0] * irI + sourceImage.Data[(int)baseJ, (int)(baseI + 1), 0] * rI;
                    c1.Green = sourceImage.Data[(int)baseJ, (int)baseI, 1] * irI + sourceImage.Data[(int)baseJ, (int)(baseI + 1), 1] * rI;
                    c1.Red = sourceImage.Data[(int)baseJ, (int)baseI, 2] * irI + sourceImage.Data[(int)baseJ, (int)(baseI + 1), 2] * rI;

                    Bgr c2 = new Bgr();
                    c2.Blue = sourceImage.Data[(int)(baseJ + 1), (int)baseI, 0] * irI + sourceImage.Data[(int)(baseJ + 1), (int)(baseI + 1), 0] * rI;
                    c2.Green = sourceImage.Data[(int)(baseJ + 1), (int)baseI, 1] * irI + sourceImage.Data[(int)(baseJ + 1), (int)(baseI + 1), 1] * rI;
                    c2.Red = sourceImage.Data[(int)(baseJ + 1), (int)baseI, 2] * irI + sourceImage.Data[(int)(baseJ + 1), (int)(baseI + 1), 2] * rI;

                    Bgr c = new Bgr();
                    c.Blue = c1.Blue * irJ + c2.Blue * rJ;
                    c.Green = c1.Green * irJ + c2.Green * rJ;
                    c.Red = c1.Red * irJ + c2.Red * rJ;

                    scaledImage[j, i] = c;
                }

            return scaledImage;
        }
    }
}
