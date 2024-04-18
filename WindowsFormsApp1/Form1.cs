using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;

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
            double altura = double.Parse(textBoxH1.Text);
            double radio = double.Parse(textBoxR1.Text);

            // Generar el cilindro con los parámetros especificados
            FuncionGenerarCilindro(numLados, altura, radio, "cilindro.obj");

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
            double alturaDefault = 10.0;
            double radioDefault = 5.0;

            // Generar el cilindro con los valores predeterminados
            FuncionGenerarCilindro(numLadosDefault, alturaDefault, radioDefault, "cilindro_default.obj");

            MessageBox.Show("Archivo cilindro_default.obj creado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // CODIGO DEL CILINDRO



        // TRANSFORMACION DEL CODIGO DEL DE JAVA AAAA
        private void FuncionGenerarCilindro(int numLados, double altura, double radio, string nombreArchivo)
        {
            List<List<double>> vertices = new List<List<double>>();
            List<List<int>> caras = new List<List<int>>();

            // Crear vértices
            for (int i = 0; i < numLados; i++)
            {
                double angulo = 2 * Math.PI * i / numLados;
                double x = radio * Math.Cos(angulo);
                double y = radio * Math.Sin(angulo);
                vertices.Add(new List<double>() { x, y, 0 }); // Parte inferior del cilindro
                vertices.Add(new List<double>() { x, y, altura }); // Parte superior del cilindro
            }

            // Calcular caras
            for (int i = 0; i < numLados + 1; i++)
            {
                // Cara lateral
                caras.Add(new List<int>() { i * 2, (i * 2 + 2) % (numLados * 2), (i * 2 + 3) % (numLados * 2), i * 2 + 1 });

                // Cara inferior
                caras.Add(new List<int>() { i * 2, (i * 2 + 2) % (numLados * 2), (i * 2 + 4) % (numLados * 2), i * 2 + 6 });
            }

            // Guardar los vértices y caras en un archivo .obj
            GuardarObjetoObj(vertices, caras, nombreArchivo);
        }

        private void GuardarObjetoObj(List<List<double>> vertices, List<List<int>> caras, string nombreArchivo)
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US"); // Establecer cultura en-US para formatear números decimales con puntos
                using (StreamWriter writer = new StreamWriter(nombreArchivo))
                {
                    foreach (List<double> vertex in vertices)
                    {
                        string formattedVertex = $"{vertex[0]:F2} {vertex[1]:F2} {vertex[2]:F2}"; // Formatear vértice
                        formattedVertex = formattedVertex.Replace(',', '.'); // Reemplazar comas por puntos
                        writer.WriteLine($"v {formattedVertex}"); // Escribir vértice en el archivo
                    }

                    foreach (List<int> face in caras)
                    {
                        writer.WriteLine($"f {face[0] + 1}//{face[0] + 1} {face[1] + 1}//{face[1] + 1} {face[2] + 1}//{face[2] + 1} {face[3] + 1}//{face[3] + 1}"); // Escribir cara en el archivo
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Error al escribir en el archivo: {e.Message}");
            }
        }





       
    }

   
}
