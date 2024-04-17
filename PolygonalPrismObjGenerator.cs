using System;
using System.IO;
using System.Text;

namespace PolygonalPrismObjGenerator
{
    public class PolygonalPrismObjGenerator
    {
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

            double[] baseCenterVertex = new double[] { position[0], position[1], position[2] - height / 2 };
            double[] topCenterVertex = new double[] { position[0], position[1], position[2] + height / 2 };

            double[] centralBaseVertex = baseCenterVertex;

            double[][] rotatedBaseVertices = RotatePolygonalPrism(baseVertices, orientation);
            for (int i = 0; i < numFaces; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    rotatedBaseVertices[i][j] += position[j];
                }
            }

            double[][] topVertices = new double[numFaces][];
            for (int i = 0; i < numFaces; i++)
            {
                topVertices[i] = new double[3];
                for (int j = 0; j < 3; j++)
                {
                    topVertices[i][j] = rotatedBaseVertices[i][j];
                    topVertices[i][2] += height;
                }
            }

            double[] centralTopVertex = topCenterVertex;

            double[][] allVertices;
            if (numFaces == 7)
            {
                allVertices = new double[16][];
                for (int i = 0; i < numFaces; i++)
                {
                    allVertices[i] = rotatedBaseVertices[i];
                }
                allVertices[numFaces] = centralBaseVertex;
                for (int i = 0; i < numFaces; i++)
                {
                    allVertices[numFaces + 1 + i] = topVertices[i];
                }
                allVertices[2 * numFaces + 1] = centralTopVertex;
            }
            else
            {
                // Aquí puedes manejar otros polígonos según sea necesario
                // En este ejemplo solo estamos considerando un heptágono
                Console.WriteLine("Solo se admite un heptágono para este ejemplo.");
                return;
            }

            int[][] faces = GenerateFaces(numFaces);

            double[][] faceNormals = new double[faces.Length][];
            for (int i = 0; i < faces.Length; i++)
            {
                int[] face = faces[i];
                double[] v1 = allVertices[face[0] - 1];
                double[] v2 = allVertices[face[1] - 1];
                double[] v3 = allVertices[face[2] - 1];
                double[] normal = CalculateNormal(v1, v2, v3);
                faceNormals[i] = normal;

                if (!IsCounterclockwise(v1, v2, v3, normal))
                {
                    int temp = face[1];
                    face[1] = face[2];
                    face[2] = temp;
                }
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
                            writer.Write($"{coordinate:F2} ");
                        }
                        writer.WriteLine();
                    }

                    foreach (double[] normal in faceNormals)
                    {
                        writer.Write("vn ");
                        foreach (double coordinate in normal)
                        {
                            writer.Write($"{coordinate:F2} ");
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

        private static bool IsCounterclockwise(double[] v1, double[] v2, double[] v3, double[] normal)
        {
            double[] u = { v2[0] - v1[0], v2[1] - v1[1], v2[2] - v1[2] };
            double[] v = { v3[0] - v1[0], v3[1] - v1[1], v3[2] - v1[2] };
            double[] crossProduct = { u[1] * v[2] - u[2] * v[1], u[2] * v[0] - u[0] * v[2], u[0] * v[1] - u[1] * v[0] };
            double dotProduct = crossProduct[0] * normal[0] + crossProduct[1] * normal[1] + crossProduct[2] * normal[2];
            return dotProduct >= 0;
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
                default:
                    Console.WriteLine("Solo se admite un heptágono para este ejemplo.");
                    return null;
            }
        }
    }
}
