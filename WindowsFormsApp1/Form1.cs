using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Click += btnGenerar_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // EVENTO DE LLAMAR AL BOTON
        private void btnGenerar_Click(object sender, EventArgs e)
        {
            int numFaces = 20;
            double sideLength = 1.0;
            double height = 2.0;
            double[] position = new double[] { 0, 0, 0 };
            double[] orientation = new double[] { 0, 0, 0 };

            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            string outputPath = Path.Combine(exeFolder, "vaso.obj");

            GeneratePolygonalPrismObjFile(numFaces, sideLength, height, position, orientation, outputPath);

            MessageBox.Show("¡El archivo se ha generado y guardado exitosamente en la carpeta de la aplicación!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        public static void GeneratePolygonalPrismObjFile(int numFaces, double sideLength, double height, double[] position, double[] orientation, string outputFileName)
        {
            double[][] baseVertices = new double[numFaces][];
            double angleIncrement = 2 * Math.PI / numFaces;

            for (int i = 0; i < numFaces; i++)
            {
                double x = sideLength / 2 * Math.Cos(i * angleIncrement);
                double y = sideLength / 2 * Math.Sin(i * angleIncrement);
                baseVertices[i] = new double[] { x, y, -height / 2 };
            }

            // CASOS DE NUMFACES
            if (numFaces == 7)
            {
                Console.WriteLine("Solo se admite un Icosagon para este ejemplo.");
                return;
            }
            else if (numFaces == 20)
            {
                baseVertices = new double[20][];
                for (int i = 0; i < numFaces; i++)
                {
                    double x = sideLength / 2 * Math.Cos(i * angleIncrement);
                    double y = sideLength / 2 * Math.Sin(i * angleIncrement);
                    baseVertices[i] = new double[] { x, y, -height / 2 };
                }
            }
            else
            {
                Console.WriteLine("Le pepe");
                return;
            }

            double[][] allVertices = baseVertices;

            // LAS CARAS SE GUARDAN ACA
            int[][] faces = GenerateFaces(numFaces);

            double[][] faceNormals = new double[faces.Length][];
            for (int i = 0; i < faces.Length; i++)
            {
                int[] face = faces[i];
                double[] v1 = allVertices[face[0]];
                double[] v2 = allVertices[face[1]];
                double[] v3 = allVertices[face[2]];
                double[] normal = CalculateNormal(v1, v2, v3);
                faceNormals[i] = normal;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(outputFileName))
                {
                    foreach (double[] vertex in allVertices)
                    {
                        writer.Write("v ");
                        foreach (double coordinate in vertex)
                        {
                            string coordinateString = coordinate.ToString("F2", CultureInfo.InvariantCulture).Replace(',', '.');
                            writer.Write($"{coordinateString} ");
                        }
                        writer.WriteLine();
                    }

                    foreach (double[] normal in faceNormals)
                    {
                        writer.Write("vn ");
                        foreach (double coordinate in normal)
                        {
                            string coordinateString = coordinate.ToString("F2", CultureInfo.InvariantCulture).Replace(',', '.');
                            writer.Write($"{coordinateString} ");
                        }
                        writer.WriteLine();
                    }

                    foreach (int[] face in faces)
                    {
                        writer.Write("f ");
                        foreach (int vertexIndex in face)
                        {
                            writer.Write($"{vertexIndex}//{vertexIndex} ");
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Error al escribir en el archivo: {e.Message}");
            }
        }

        private static double[] CalculateNormal(double[] v1, double[] v2, double[] v3)
        {
            double[] u = { v2[0] - v1[0], v2[1] - v1[1], v2[2] - v1[2] };
            double[] v = { v3[0] - v1[0], v3[1] - v1[1], v3[2] - v1[2] };
            double[] normal = new double[3];
            normal[0] = u[1] * v[2] - u[2] * v[1];
            normal[1] = u[2] * v[0] - u[0] * v[2];
            normal[2] = u[0] * v[1] - u[1] * v[0];
            double length = Math.Sqrt(normal[0] * normal[0] + normal[1] * normal[1] + normal[2] * normal[2]);
            normal[0] /= length;
            normal[1] /= length;
            normal[2] /= length;
            return normal;
        }

        private static double[] RotateVertex(double[] vertex, double[] orientation)
        {
            double x = vertex[0] * (Math.Cos(orientation[1]) * Math.Cos(orientation[2])) - vertex[1] * (Math.Cos(orientation[1]) * Math.Sin(orientation[2])) + vertex[2] * Math.Sin(orientation[1]);
            double y = vertex[0] * (Math.Cos(orientation[0]) * Math.Sin(orientation[2]) + Math.Sin(orientation[0]) * Math.Sin(orientation[1]) * Math.Cos(orientation[2])) + vertex[1] * (Math.Cos(orientation[0]) * Math.Cos(orientation[2]) - Math.Sin(orientation[0]) * Math.Sin(orientation[1]) * Math.Sin(orientation[2])) - vertex[2] * Math.Sin(orientation[0]) * Math.Cos(orientation[1]);
            double z = vertex[0] * (Math.Sin(orientation[0]) * Math.Sin(orientation[2]) - Math.Cos(orientation[0]) * Math.Sin(orientation[1]) * Math.Cos(orientation[2])) + vertex[1] * (Math.Sin(orientation[0]) * Math.Cos(orientation[2]) + Math.Cos(orientation[0]) * Math.Sin(orientation[1]) * Math.Sin(orientation[2])) + vertex[2] * Math.Cos(orientation[0]) * Math.Cos(orientation[1]);
            return new double[] { x, y, z };
        }

        private static double[][] RotatePolygonalPrism(double[][] vertices, double[] orientation)
        {
            double[][] rotatedVertices = new double[vertices.Length][];
            for (int i = 0; i < vertices.Length; i++)
            {
                rotatedVertices[i] = RotateVertex(vertices[i], orientation);
            }
            return rotatedVertices;
        }

        public static int[][] GenerateFaces(int numFaces)
        {
            switch (numFaces)
            {
                case 7:
                    return new int[][]
                    {
                        // First base heptagon
                        new int[] {1, 2, 8}, new int[] {2, 3, 8}, new int[] {3, 4, 8}, new int[] {4, 5, 8}, new int[] {5, 6, 8}, new int[] {6, 7, 8}, new int[] {7, 1, 8},
                        // Second base heptagon
                        new int[] {9, 10, 16}, new int[] {10, 11, 16}, new int[] {11, 12, 16}, new int[] {12, 13, 16}, new int[] {13, 14, 16}, new int[] {14, 15, 16}, new int[] {15, 9, 16},
                        // Side triangles
                        new int[] {1, 2, 9}, new int[] {2, 3, 10}, new int[] {3, 4, 11}, new int[] {4, 5, 12}, new int[] {5, 6, 13}, new int[] {6, 7, 14}, new int[] {7, 1, 15},
                        // Additional side triangles to complete the rectangle
                        new int[] {9, 10, 2}, new int[] {10, 11, 3}, new int[] {11, 12, 4}, new int[] {12, 13, 5}, new int[] {13, 14, 6}, new int[] {14, 15, 7}, new int[] {15, 9, 1}
                    };
                case 20:
                    return new int[][]
            {
                // Bottom face
                new int[] { 0, 1, 2, 3, 4 },
                new int[] { 1, 2, 3, 4, 5 },
                new int[] { 2, 3, 4, 5, 6 },
                new int[] { 3, 4, 5, 6, 7 },
                new int[] { 4, 5, 6, 7, 8 },
                new int[] { 5, 6, 7, 8, 9 },
                new int[] { 6, 7, 8, 9, 10 },
                new int[] { 7, 8, 9, 10, 11 },
                new int[] { 8, 9, 10, 11, 12 },
                new int[] { 9, 10, 11, 12, 13 },
                new int[] { 10, 11, 12, 13, 14 },
                new int[] { 11, 12, 13, 14, 15 },
                new int[] { 12, 13, 14, 15, 16 },
                new int[] { 13, 14, 15, 16, 17 },
                new int[] { 14, 15, 16, 17, 18 },
                new int[] { 15, 16, 17, 18, 19 },
                new int[] { 16, 17, 18, 19, 0 },
                new int[] { 17, 18, 19, 0, 1 },
                new int[] { 18, 19, 0, 1, 2 },
                new int[] { 19, 0, 1, 2, 3 },

                // Sides
                new int[] { 0, 1, numFaces + 1 },
                new int[] { 1, 2, numFaces + 2 },
                new int[] { 2, 3, numFaces + 3 },
                new int[] { 3, 4, numFaces + 4 },
                new int[] { 4, 5, numFaces + 5 },
                new int[] { 5, 6, numFaces + 6 },
                new int[] { 6, 7, numFaces + 7 },
                new int[] { 7, 8, numFaces + 8 },
                new int[] { 8, 9, numFaces + 9 },
                new int[] { 9, 10, numFaces + 10 },
                new int[] { 10, 11, numFaces + 11 },
                new int[] { 11, 12, numFaces + 12 },
                new int[] { 12, 13, numFaces + 13 },
                new int[] { 13, 14, numFaces + 14 },
                new int[] { 14, 15, numFaces + 15 },
                new int[] { 15, 16, numFaces + 16 },
                new int[] { 16, 17, numFaces + 17 },
                new int[] { 17, 18, numFaces + 18 },
                new int[] { 18, 19, numFaces + 19 },
                new int[] { 19, 0, numFaces },
            };

                default:
                    Console.WriteLine("Le pepe");
                    return null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string currentDirectory = Environment.CurrentDirectory;
            Process.Start(currentDirectory);
        }
    }
}
