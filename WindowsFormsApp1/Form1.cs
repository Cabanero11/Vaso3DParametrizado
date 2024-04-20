using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // PARA EVENTOS
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            // Obtener los parámetros del usuario
            int numLados = int.Parse(textBoxNumCaras.Text);
            double r1 = double.Parse(textBoxR1.Text);
            double r2 = double.Parse(textBoxR2.Text);
            double h1 = double.Parse(textBoxH1.Text);
            double h2 = double.Parse(textBoxR2.Text);
            double w1 = double.Parse(textBoxH1.Text);

            // Generar el cilindro con los parámetros especificados
            FuncionGenerarVaso(numLados, r1, r2, h1, h2, w1, "cilindro.obj");

            MessageBox.Show("Archivo cilindro.obj creado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string currentDirectory = Environment.CurrentDirectory;
            Process.Start(currentDirectory);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Valores predeterminados para los parámetros del cilindro
            int numLadosDefault = 20;
            double r1Default = 10.0;
            double r2Default = 10.0;
            double h1Default = 20.0;
            double h2Default = 15.0;
            double w1Default = 2.0;

            // Generar el cilindro con los valores predeterminados
            FuncionGenerarVaso(numLadosDefault, r1Default, r2Default, h1Default, h2Default, w1Default, "cilindro_default.obj");

            MessageBox.Show("Archivo cilindro_default.obj creado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // CODIGO DEL VASO 3D

        public static void GuardarObjetoObj(StreamWriter writer, string line)
        {
            writer.WriteLine(line);
        }

        public static void FuncionGenerarVaso(int numCaras, double r1, double r2, double h1, double h2, double w1, string filename)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    // Vértices para el primer poliedro
                    GuardarObjetoObj(writer, "v 0.000000 0.000000 0.000000"); // Vértice central inferior
                    GuardarObjetoObj(writer, "v 0.000000 0.000000 " + h1); // Vértice central superior
                    for (int i = 0; i < numCaras; i++)
                    {
                        double angle = 2 * Math.PI * i / numCaras;
                        double x = r1 * Math.Cos(angle);
                        double y = r1 * Math.Sin(angle);
                        GuardarObjetoObj(writer, "v " + String.Format("{0:F6}", x).Replace(",", ".") + " " + String.Format("{0:F6}", y).Replace(",", ".") + " 0.000000"); // Vértices inferiores
                        GuardarObjetoObj(writer, "v " + String.Format("{0:F6}", x).Replace(",", ".") + " " + String.Format("{0:F6}", y).Replace(",", ".") + " " + String.Format("{0:F6}", h1).Replace(",", ".")); // Vértices superiores
                    }

                    // Vértices para el segundo poliedro
                    r2 = r1 - w1; // Ajustamos el radio del segundo poliedro
                    for (int i = 0; i < numCaras; i++)
                    {
                        double angle = 2 * Math.PI * i / numCaras;
                        double x = r2 * Math.Cos(angle);
                        double y = r2 * Math.Sin(angle);
                        GuardarObjetoObj(writer, "v " + String.Format("{0:F6}", x).Replace(",", ".") + " " + String.Format("{0:F6}", y).Replace(",", ".") + " " + String.Format("{0:F6}", h2).Replace(",", ".")); // Vértices inferiores del segundo poliedro
                        GuardarObjetoObj(writer, "v " + String.Format("{0:F6}", x).Replace(",", ".") + " " + String.Format("{0:F6}", y).Replace(",", ".") + " " + String.Format("{0:F6}", h1).Replace(",", ".")); // Vértices superiores del segundo poliedro
                    }
                    // Vértice central de la cara inferior del segundo poliedro
                    GuardarObjetoObj(writer, "v 0.000000 0.000000 " + String.Format("{0:F6}", h2).Replace(",", ".")); // Vértice central inferior del segundo poliedro

                    // Normales
                    GuardarObjetoObj(writer, "vn -1.000000 0.000000 0.000000"); // Normal para las caras laterales
                    GuardarObjetoObj(writer, "vn 0.000000 0.000000 -1.000000"); // Normal para la cara inferior (invertida)

                    // Caras para el primer poliedro
                    // Cara inferior
                    for (int i = 0; i < numCaras; i++)
                    {
                        int v1 = i * 2 + 3; // Índice del primer vértice de la cara inferior
                        int v2 = (i + 1) % numCaras * 2 + 3; // Índice del segundo vértice de la cara inferior
                                                              // Cara inferior
                        GuardarObjetoObj(writer, "f " + v1 + "//1 " + v2 + "//1 1//1");
                    }

                    // Caras laterales para el primer poliedro
                    for (int i = 0; i < numCaras; i++)
                    {
                        int v1 = i * 2 + 3; // Índice del primer vértice de la cara inferior
                        int v2 = (i + 1) % numCaras * 2 + 3; // Índice del segundo vértice de la cara inferior
                                                              // Caras laterales (dos triángulos)
                        GuardarObjetoObj(writer, "f " + v1 + "//2 " + v2 + "//2 " + (v2 + 1) + "//2"); // Cara lateral (triángulo 1)
                        GuardarObjetoObj(writer, "f " + v1 + "//2 " + (v2 + 1) + "//2 " + (v1 + 1) + "//2"); // Cara lateral (triángulo 2)
                    }

                    // Caras para el segundo poliedro
                    // Cara inferior del segundo poliedro
                    for (int i = 0; i < numCaras; i++)
                    {
                        int v1 = i * 2 + 3 + 2 * numCaras; // Índice del primer vértice de la cara inferior del segundo poliedro
                        int v2 = (i + 1) % numCaras * 2 + 3 + 2 * numCaras; // Índice del segundo vértice de la cara inferior del segundo poliedro
                        int v3 = 4 * numCaras + 3; // Índice del vértice central de la cara inferior del segundo poliedro
                                                    // Cara inferior del segundo poliedro
                        GuardarObjetoObj(writer, "f " + v1 + "//1 " + v2 + "//1 " + v3 + "//1");
                    }

                    // Caras laterales para el segundo poliedro
                    for (int i = 0; i < numCaras; i++)
                    {
                        int v1 = i * 2 + 3 + 2 * numCaras; // Índice del primer vértice de la cara inferior del segundo poliedro
                        int v2 = (i + 1) % numCaras * 2 + 3 + 2 * numCaras; // Índice del segundo vértice de la cara inferior del segundo poliedro
                                                                              // Caras laterales (dos triángulos)
                        GuardarObjetoObj(writer, "f " + v1 + "//2 " + (v2 + 1) + "//2 " + v2 + "//2"); // Cara lateral (triángulo 1)
                        GuardarObjetoObj(writer, "f " + v1 + "//2 " + (v1 + 1) + "//2 " + (v2 + 1) + "//2"); // Cara lateral (triángulo 2)
                    }

                    // Caras triangulares entre los vértices superiores de ambos poliedros
                    for (int i = 0; i < numCaras; i++)
                    {
                        int v1 = i * 2 + 4; // Índice del primer vértice superior del primer poliedro
                        int v2 = i * 2 + 4 + 2 * numCaras; // Índice del primer vértice superior del segundo poliedro
                        int v3 = (i + 1) % numCaras * 2 + 4; // Índice del segundo vértice superior del primer poliedro
                        int v4 = (i + 1) % numCaras * 2 + 4 + 2 * numCaras; // Índice del segundo vértice superior del segundo poliedro
                        GuardarObjetoObj(writer, "f " + v1 + "//1 " + v2 + "//1 " + v4 + "//1"); // Triángulo 1
                        GuardarObjetoObj(writer, "f " + v1 + "//1 " + v4 + "//1 " + v3 + "//1"); // Triángulo 2
                    }

                    Console.WriteLine("Archivo OBJ anidado generado exitosamente.");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error al escribir el archivo OBJ anidado: " + e.Message);
            }
        }


    } // CLASE
} // NAMESPACE
