using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;

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

        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Obtener los parámetros del usuario
            int numLados = int.Parse(textBoxNumCaras.Text);
            double r1 = double.Parse(textBoxR1.Text);
            double r2 = double.Parse(textBoxR2.Text);
            double h1 = double.Parse(textBoxH1.Text);
            double h2 = double.Parse(textBoxH2.Text);
            double w1 = double.Parse(textBoxW1.Text);

            ComprobarLimitaciones(r1, r2, w1);

            // Generar el cilindro con los parámetros especificados

            FuncionGenerarVaso(numLados, h1, r1, r2, h2, w1, "cilindro.obj");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Valores predeterminados para los parámetros del cilindro
            int numLadosDefault = 20;
            double r1Default = 10.0;
            double r2Default = 16.0;
            double h1Default = 20.0;
            double h2Default = 10.0;
            double w1Default = 2.0;

            // Generar el cilindro con los valores predeterminados
            FuncionGenerarVaso(numLadosDefault, h1Default, r1Default, r2Default, h2Default, w1Default, "cilindro_default.obj");


            MessageBox.Show("Archivo cilindro_default.obj creado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string currentDirectory = Environment.CurrentDirectory;
            Process.Start(currentDirectory);
        }


        // CODIGO DEL VASO 3D

        public static void GuardarObjetoObj(StreamWriter writer, string line)
        {
            writer.WriteLine(line);
        }

        public static void FuncionGenerarVaso(int numSides1, double height1, double radius1, double r2, double h2, double w1, string filename)
        {
            try
            {

                using (StreamWriter writer = new StreamWriter(filename))
                {
                    // Vértices para el primer poliedro
                    writer.WriteLine("v 0.000000 0.000000 0.000000"); // Vértice central inferior
                    writer.WriteLine("v 0.000000 0.000000 " + height1.ToString("F6", CultureInfo.InvariantCulture)); // Vértice central superior
                    for (int i = 0; i < numSides1; i++)
                    {
                        double angle = 2 * Math.PI * i / numSides1;
                        double x1 = radius1 * Math.Cos(angle);
                        double y1 = radius1 * Math.Sin(angle);
                        double x2 = r2 * Math.Cos(angle); // Ajuste para el radio superior
                        double y2 = r2 * Math.Sin(angle); // Ajuste para el radio superior
                        writer.WriteLine("v " + x1.ToString("F6", CultureInfo.InvariantCulture) + " " + y1.ToString("F6", CultureInfo.InvariantCulture) + " 0.000000"); // Vértices inferiores
                        writer.WriteLine("v " + x2.ToString("F6", CultureInfo.InvariantCulture) + " " + y2.ToString("F6", CultureInfo.InvariantCulture) + " " + height1.ToString("F6", CultureInfo.InvariantCulture)); // Vértices superiores
                    }

                    // Calcular el radio inferior del segundo poliedro
                    double ratio = h2 / height1;
                    double diffRadius = radius1 - r2;
                    double radius2 = radius1 - diffRadius * ratio - w1;

                    // Vértices para el segundo poliedro
                    double radius3 = r2 - w1;
                    for (int i = 0; i < numSides1; i++)
                    {
                        double angle = 2 * Math.PI * i / numSides1;
                        double x = radius2 * Math.Cos(angle);
                        double y = radius2 * Math.Sin(angle);
                        double x1 = radius3 * Math.Cos(angle); // Ajuste para el radio superior
                        double y1 = radius3 * Math.Sin(angle); // Ajuste para el radio superior
                        writer.WriteLine("v " + x.ToString("F6", CultureInfo.InvariantCulture) + " " + y.ToString("F6", CultureInfo.InvariantCulture) + " " + h2.ToString("F6", CultureInfo.InvariantCulture)); // Vértices inferiores del segundo poliedro
                        writer.WriteLine("v " + x1.ToString("F6", CultureInfo.InvariantCulture) + " " + y1.ToString("F6", CultureInfo.InvariantCulture) + " " + height1.ToString("F6", CultureInfo.InvariantCulture)); // Vértices superiores del segundo poliedro
                    }

                    // Vértice central de la cara inferior del segundo poliedro
                    writer.WriteLine("v 0.000000 0.000000 " + h2.ToString("F6", CultureInfo.InvariantCulture)); // Vértice central inferior del segundo poliedro

                    // Normales
                    writer.WriteLine("vn -1.000000 0.000000 0.000000"); // Normal para las caras laterales
                    writer.WriteLine("vn 0.000000 0.000000 -1.000000"); // Normal para la cara inferior (invertida)

                    // Caras para el primer poliedro
                    // Cara inferior
                    for (int i = 0; i < numSides1; i++)
                    {
                        int v1 = i * 2 + 3; // Índice del primer vértice de la cara inferior
                        int v2 = (i + 1) % numSides1 * 2 + 3; // Índice del segundo vértice de la cara inferior
                                                              // Cara inferior
                        writer.WriteLine("f " + v1 + "//1 " + v2 + "//1 1//1");
                    }

                    // Caras laterales para el primer poliedro
                    for (int i = 0; i < numSides1; i++)
                    {
                        int v1 = i * 2 + 3; // Índice del primer vértice de la cara inferior
                        int v2 = (i + 1) % numSides1 * 2 + 3; // Índice del segundo vértice de la cara inferior
                                                              // Caras laterales (dos triángulos)
                        writer.WriteLine("f " + v1 + "//2 " + v2 + "//2 " + (v2 + 1) + "//2"); // Cara lateral (triángulo 1)
                        writer.WriteLine("f " + v1 + "//2 " + (v2 + 1) + "//2 " + (v1 + 1) + "//2"); // Cara lateral (triángulo 2)
                    }

                    // Caras para el segundo poliedro
                    // Cara inferior del segundo poliedro
                    for (int i = 0; i < numSides1; i++)
                    {
                        int v1 = i * 2 + 3 + 2 * numSides1; // Índice del primer vértice de la cara inferior del segundo poliedro
                        int v2 = (i + 1) % numSides1 * 2 + 3 + 2 * numSides1; // Índice del segundo vértice de la cara inferior del segundo poliedro
                        int v3 = 4 * numSides1 + 3; // Índice del vértice central de la cara inferior del segundo poliedro
                                                    // Cara inferior del segundo poliedro
                        writer.WriteLine("f " + v1 + "//1 " + v2 + "//1 " + v3 + "//1");
                    }

                    // Caras laterales para el segundo poliedro
                    for (int i = 0; i < numSides1; i++)
                    {
                        int v1 = i * 2 + 3 + 2 * numSides1; // Índice del primer vértice de la cara inferior del segundo poliedro
                        int v2 = (i + 1) % numSides1 * 2 + 3 + 2 * numSides1; // Índice del segundo vértice de la cara inferior del segundo poliedro
                                                                              // Caras laterales (dos triángulos)
                        writer.WriteLine("f " + v1 + "//2 " + (v2 + 1) + "//2 " + v2 + "//2"); // Cara lateral (triángulo 1)
                        writer.WriteLine("f " + v1 + "//2 " + (v1 + 1) + "//2 " + (v2 + 1) + "//2"); // Cara lateral (triángulo 2)
                    }

                    // Caras triangulares entre los vértices superiores de ambos poliedros
                    for (int i = 0; i < numSides1; i++)
                    {
                        int v1 = i * 2 + 4; // Índice del primer vértice superior del primer poliedro
                        int v2 = i * 2 + 4 + 2 * numSides1; // Índice del primer vértice superior del segundo poliedro
                        int v3 = (i + 1) % numSides1 * 2 + 4; // Índice del segundo vértice superior del primer poliedro
                        int v4 = (i + 1) % numSides1 * 2 + 4 + 2 * numSides1; // Índice del segundo vértice superior del segundo poliedro
                        writer.WriteLine("f " + v1 + "//1 " + v2 + "//1 " + v4 + "//1"); // Triángulo 1
                        writer.WriteLine("f " + v1 + "//1 " + v4 + "//1 " + v3 + "//1"); // Triángulo 2
                    }

                    Console.WriteLine("Archivo OBJ anidado generado exitosamente.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al escribir el archivo OBJ anidado: " + e.Message);
            }
        }

        private void ComprobarLimitaciones(double r1, double r2, double w1)
        {
            /// LIMITACIONES DE PARAMETROS

            // Verificar si radius1 es mayor que r2 pero no menor
            if (r1 < r2)
            {
                Console.WriteLine("1");
                MessageBox.Show("El valor de 'r1' no puede ser menor al de 'r2'", "R1 menor :(", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //radius1 = r2; // Ajustar radius1 igual a r2
            } 
            else if (r1 >= r2 && w1 >= 1)
            {
                Console.WriteLine("2");
                MessageBox.Show("Archivo cilindro.obj creado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            // Verificar si w1 es menor que 1
            else if (w1 < 1 && r1 >= r2)
            {
                Console.WriteLine("3");
                MessageBox.Show("El valor de w1 no puede ser menor a -> 1", "W1 menor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //w1 = 1; // Ajustar w1 a 1
            } 
            else if (w1 >= 1 && r1 >= r2) 
            {
                Console.WriteLine("4");
                MessageBox.Show("Archivo cilindro.obj creado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    } // CLASE





} // NAMESPACE
