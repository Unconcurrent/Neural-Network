using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace TensorTest
{
    public class Program
    {
        static NeuralNetwork.NeuralNetwork NeuralNetwork = new NeuralNetwork.NeuralNetwork(0.2, new[] { 8, 18, 8 });
        static List<double> experienceList = new List<double>();
        static void Main(string[] args)
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            var form = new NeuralForm(NeuralNetwork);
            form.Show();

            var r = new Random(1);
            if (!File.Exists("neural.net"))
            {
                var ii = 0;
                while (ii++ <= 100000)
                {                    
                    var input = new double[8];

                    for (int i = 0; i < input.Length; i++)
                    {
                        input[i] = r.Next(100) > 50 ? 1 : 0;
                    }

                    var output = new double[input.Length];

                    for (int i = 0; i < input.Length; i++)
                    {
                        output[i] = input[i] == 1 ? 0 : 1;
                    }

                    NeuralNetwork.Train(input, output);



                    var networkOutput = new double[input.Length];
                    for (int j = 0; j < NeuralNetwork.Layers.Last().Neurons.Count; j++)
                    {
                        var neuron = NeuralNetwork.Layers.Last().Neurons[j];
                        networkOutput[j] = neuron.Value;
                    }

                    for (int j = 0; j < output.Length; j++)
                    {
                        if (experienceList.Count > 999)
                        {
                            experienceList.Remove(experienceList.First());
                        }
                        experienceList.Add(output[j] == Math.Round(networkOutput[j]) ? 1 : 0);
                    }

                    double temp = 0;
                    foreach (var exp in experienceList)
                    {
                        temp += exp;
                    }

                    double loss = 100 - ((temp / experienceList.Count) * 100);




                    if (ii % 100 == 0)
                    {
                        Console.CursorTop = 0;
                        Console.CursorLeft = 0;
                        Console.WriteLine($"Iteration: {ii}  ");
                        form.UpdateNeural(NeuralNetwork, $"Loss: {loss:##.###}%");
                    }
                }
            }
            else
            {
                NeuralNetwork.Load("neural.net");
                form.Hide();
                form = new NeuralForm(NeuralNetwork);
                form.Show();
            }
            
            

            while (true)
            {
                Console.Clear();

                var input = new double[8];
                var networkOutput = new double[8];
                var output = new double[8];

                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = r.Next(100) > 50 ? 1 : 0;
                }

                for (int i = 0; i < input.Length; i++)
                {
                    output[i] = input[i] == 1 ? 0 : 1;
                }

                var response = NeuralNetwork.Run(input);

                for (int i = 0; i < response.Length; i++)
                {
                    networkOutput[i] = Math.Round(response[i]);
                }
                DrawMap(input, networkOutput);

                for (int i = 0; i < 10; i++)
                {
                    NeuralNetwork.Train(input, output);
                }




                for (int j = 0; j < output.Length; j++)
                {
                    if (experienceList.Count > 999)
                    {
                        experienceList.Remove(experienceList.First());
                    }
                    experienceList.Add(output[j] == Math.Round(networkOutput[j]) ? 1 : 0);
                }

                double temp = 0;
                foreach (var exp in experienceList)
                {
                    temp += exp;
                }

                double loss = 100 - ((temp / experienceList.Count) * 100);




                form.UpdateNeural(NeuralNetwork, $"Loss: {loss:##.###}%");
                //Thread.Sleep(300);
            }
            
        }

        static void DrawMap(double[] mapDoubles, double[] i)
        {
            for (var j = 0; j < mapDoubles.Length; j++)
            {
                var mapDouble = mapDoubles[j];

                Console.BackgroundColor = mapDouble == 1 ? ConsoleColor.Blue : ConsoleColor.Red;
                Console.Write(i[j] == 0 ? "  " : "()");
                Console.ResetColor();

                if (j == 3)
                    Console.Write("  ");
                if (j == 2 | j == 4)
                    Console.Write("\r\n");
            }
        }

        public static bool ConsoleEventCallback(int eventType)
        {
            NeuralNetwork.Save("neural.net");
            
            return false;
        }

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
        // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
