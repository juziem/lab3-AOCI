using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace lab3
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> sourceImage;
        PointF[] pts = new PointF[4];
        PointF[] destPoints = new PointF[4];

        int c = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//masshtab
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                sourceImage = new Image<Bgr, byte>(fileName).Resize(640, 480, Inter.Linear);

                imageBox1.Image = sourceImage;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double k;

            if (sourceImage == null )
            {
                MessageBox.Show("Загрузите изображение.");
                textBox1.Clear();
            }
            else
            {
                if (textBox1.Text == "")
                    MessageBox.Show("Введите число.");
                else
                {
                    k = double.Parse(textBox1.Text);

                    if (k < 0 || k > 10) 
                    { 
                        MessageBox.Show("Выход за допустимые значения. Диапазон 0-10");
                        k = 0;
                        textBox1.Clear();
                    }
                    else
                        imageBox2.Image = Image.Scale(sourceImage, k);
                    }               
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double sh;

            if (sourceImage == null)
            {
                MessageBox.Show("Загрузите изображение.");
                textBox2.Clear();
            }
            else
            {
                if (textBox2.Text == "")
                    MessageBox.Show("Введите число.");
                else
                {
                    sh = double.Parse(textBox2.Text);

                    if(sh<0 || sh>10)
                    {
                        MessageBox.Show("Выход за допустимые значения. Диапазон 0-10");
                        sh = 0;
                        textBox2.Clear();
                    }
                    else
                        imageBox2.Image = Image.Share(sourceImage, sh);
                    }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double a;
            if (sourceImage == null)
            {
                MessageBox.Show("Загрузите изображение.");
                textBox3.Clear();
            }
            else
            {
                if (textBox3.Text == "")
                    MessageBox.Show("Введите число.");
                else
                {
                    a = double.Parse(textBox3.Text);

                    if (a < 0 || a > 360)
                    {
                        MessageBox.Show("Выход за допустимые значения. Диапазон 0-360");
                        a = 0;
                        textBox3.Clear();
                    }
                    else
                        imageBox2.Image = Image.Rotate(sourceImage, a);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (sourceImage == null)
            {
                MessageBox.Show("Загрузите изображение.");
                textBox4.Clear();
                textBox5.Clear();
            }
            else
            {
                int qX = 0;
                int qY = 0;

                if(textBox4.Text == "" || textBox5.Text == "")
                    MessageBox.Show("Введите значение в оба поля.");
                else
                {
                    qX = int.Parse(textBox4.Text);
                    qY = int.Parse(textBox5.Text);
                }
                

                if (qX == 1 || qX == -1)
                {
                    if (qY == 1 || qY == -1)
                    {
                        imageBox2.Image = Image.Reflect(sourceImage, qX, qY);
                    }
                    else
                    {
                        MessageBox.Show("Выход за допустимые значения. Введите 1 или -1");
                        qY = 0;
                        textBox4.Clear();
                        textBox5.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Выход за допустимые значения. Введите 1 или -1");
                    qX = 0;
                    textBox4.Clear();
                    textBox5.Clear();
                }
            }
        }

        private void imageBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var imgCopy = sourceImage.Copy();

            int x = (int)(e.Location.X / imageBox1.ZoomScale);
            int y = (int)(e.Location.Y / imageBox1.ZoomScale);
            
            pts[c] = new PointF(x, y);
            c++;
            if (c >= 4) 
                c = 0;

            int radius = 2;
            int thickness = 2;
            var color = new Bgr(Color.Blue).MCvScalar;

            for(int i = 0; i < 4; i++) 
            {
                // функция, рисующая на изображении круг с заданными параметрами
                CvInvoke.Circle(imgCopy, Point.Round(pts[i]), radius, color, thickness);
            }

            imageBox1.Image = imgCopy;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (sourceImage == null)
            {
                MessageBox.Show("Загрузите изображение.");
            }
            else
            {
                destPoints[0] = new PointF(0, 0);
                destPoints[1] = new PointF(0, sourceImage.Height - 1);
                destPoints[2] = new PointF(sourceImage.Width - 1, sourceImage.Height - 1);
                destPoints[3] = new PointF(sourceImage.Width - 1, 0);

                var homographyMatrix = CvInvoke.GetPerspectiveTransform(pts, destPoints);
                var destImage = new Image<Bgr, byte>(sourceImage.Size);
                CvInvoke.WarpPerspective(sourceImage, destImage, homographyMatrix, destImage.Size);

                imageBox2.Image = destImage;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 44 && (e.KeyChar < 48 || e.KeyChar > 57))
                e.Handled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 44 && ( e.KeyChar < 48 || e.KeyChar > 57))
                e.Handled = true;
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 44 && (e.KeyChar < 48 || e.KeyChar > 57))
                e.Handled = true;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 44 && e.KeyChar != 45 && (e.KeyChar < 48 || e.KeyChar > 57))
                e.Handled = true;
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 44 && e.KeyChar != 45 && (e.KeyChar < 48 || e.KeyChar > 57))
                e.Handled = true;
        }
    }
}
