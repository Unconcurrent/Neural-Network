using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using NeuralNetwork;

namespace TensorTest
{
    public partial class NeuralForm : Form
    {
        Graphics g;
        Bitmap img;
        private static int size = 20;
        private int length = size * 9;
        private List<Dictionary<Neuron, Point>> neuronsPoints = new List<Dictionary<Neuron, Point>>();
        public NeuralForm(NeuralNetwork.NeuralNetwork n)
        {
            var bigger = 0;
            foreach (var layer in n.Layers)
            {
                if (layer.NeuronCount > bigger)
                    bigger = layer.NeuronCount;
            }


            img = new Bitmap(size * 5 * n.Layers.Count * 3, (size * 3) * (bigger) + size);

            InitializeComponent();
        }

        private void NeuralForm_Load(object sender, EventArgs e)
        {
            
            g = Graphics.FromImage(img);
            g.Clear(Color.White);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
        }

        public void UpdateNeural(NeuralNetwork.NeuralNetwork neural, string textToShow)
        {
            g.Clear(Color.White);
            neuronsPoints.Clear();
            var x = size * 3;
            var y = size * 3;
            textToShow = textToShow ?? String.Empty;

            foreach (var layer in neural.Layers)
            {
                var dictionary = new Dictionary<Neuron, Point>();
                foreach (var neuron in layer.Neurons)
                {
                    dictionary.Add(neuron, new Point(x, y));
                    y += size * 3;
                }
                neuronsPoints.Add(dictionary);
                x += length;
                y = size * 3;
            }

            x = size * 3;
            y = size * 3;



            for (int i = 0; i < neuronsPoints.Count; i++)
            {
                var neurons = neuronsPoints[i];
                for (int j = 0; j < neurons.Count; j++)
                {
                    var neuronPoint = neurons.ToList()[j];
                    var y1 = 1;

                    for (int k = 0; k < neuronPoint.Key.Dendrites.Count; k++)
                    {
                        if((neuronPoint.Key.Dendrites[k].Weight > 0.4d &
                           neuronPoint.Key.Dendrites[k].Weight < 1) | (i == neural.LayerCount - 1 & neuronPoint.Key.Dendrites[k].Weight > 0))
                            g.DrawLine(
                                new Pen(Color.Black, 
                                1),
                                x,
                                y,
                                x - length,
                                size * 3 * y1);
                        if(neuronPoint.Key.Dendrites[k].Weight > 1 &
                           neuronPoint.Key.Dendrites[k].Weight < 10)
                            g.DrawLine(
                                new Pen(Color.Black,
                                    2),
                                x,
                                y,
                                x - length,
                                size * 3 * y1);
                        if (neuronPoint.Key.Dendrites[k].Weight > 20)
                            g.DrawLine(
                                new Pen(Color.Black,
                                    3),
                                x,
                                y,
                                x - length,
                                size * 3 * y1);
                        y1++;
                    }
                    y += size * 3;
                }
                x += length;
                y = size * 3;
                
            }

            foreach (var point in neuronsPoints)
            {
                foreach (var neuron in point)
                {
                    var text = neuron.Key.Value.ToString();
                    DrawCicle(
                        text.Length > 4 ? text.Substring(0, 4) : text, 
                        neuron.Value.X, 
                        neuron.Value.Y);
                }
            }

            g.DrawString(textToShow, new Font(FontFamily.GenericSansSerif, size), new SolidBrush(Color.Black), length * neuronsPoints.Count - length / 2, size * 3);
            g.Flush(FlushIntention.Flush);
                
                
            pictureBox1.Image = img;
            pictureBox1.Refresh();
            Invalidate();
            Application.DoEvents();
        }

        public void DrawCicle(string text, int centerX, int centerY)
        {
            g.DrawCircle(new Pen(Color.Black), centerX, centerY, size);
            g.DrawString(text, DefaultFont, new SolidBrush(Color.Black), centerX - size / 2, centerY - size / 2);
        }

        private void NeuralForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.ConsoleEventCallback(0);
            Environment.Exit(0);
        }
    }

    public static class GraphicsExtensions
    {
        public static void DrawCircle(this Graphics g, Pen pen,
            float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
            float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }
    }
}
