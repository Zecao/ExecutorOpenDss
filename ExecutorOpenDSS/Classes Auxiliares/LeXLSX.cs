﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS
{
    class LeXLSX
    {

        public static double[,] Numerico(string arquivoOriginal)
        {

            if (File.Exists(arquivoOriginal))
            {
                //Nome do arquivo temporário
                string nomeArquivo = arquivoOriginal + "~Temp";

                //Deleta o arquivo temporário, se ele existe
                ArqManip.SafeDelete(nomeArquivo);

                //Cria o arquivo temporário
                File.Copy(arquivoOriginal, nomeArquivo);

                var file = new FileInfo(nomeArquivo);
                using (var package = new ExcelPackage(file))
                {
                    ExcelWorksheet plan = package.Workbook.Worksheets.First();
                    int ultimaLinha = plan.Dimension.End.Row;
                    int ultimaColuna = plan.Dimension.End.Column;
                    double[,] resultado = new Double[ultimaLinha,ultimaColuna];
                    for (int linha = 1; linha <= ultimaLinha; linha++)
                    {
                        for (int coluna = 1; coluna <= ultimaColuna; coluna++)
                        {
                            try
                            {
                                resultado[linha - 1, coluna - 1] = double.Parse(plan.Cells[linha, coluna].Value.ToString());
                            }
                            catch {
                                resultado[linha - 1, coluna - 1] = Double.NaN;
                            }
                        }
                    }
                    resultado = EliminaNan(resultado);
                    return resultado;
                }  
            }
            else
            {
                throw new FileNotFoundException("Arquivo " + arquivoOriginal + " não encontrado.");
                //return null;
            }
        }

        public static string[,] LeTudo(string arquivoOriginal)
        {
            if (File.Exists(arquivoOriginal))
            {
                //Nome do arquivo temporário
                string nomeArquivo = arquivoOriginal + "~Temp";

                //Deleta o arquivo temporário, se ele existe
                ArqManip.SafeDelete(nomeArquivo);

                //Cria o arquivo temporário
                File.Copy(arquivoOriginal, nomeArquivo);

                var file = new FileInfo(nomeArquivo);
                using (var package = new ExcelPackage(file))
                {
                    ExcelWorksheet plan = package.Workbook.Worksheets.First();
                    int ultimaLinha = plan.Dimension.End.Row;
                    int ultimaColuna = plan.Dimension.End.Column;
                    string[,] resultado = new String[ultimaLinha, ultimaColuna];
                    for (int linha = 1; linha <= ultimaLinha; linha++)
                    {
                        for (int coluna = 1; coluna <= ultimaColuna; coluna++)
                        {
                            try
                            {
                                resultado[linha - 1, coluna - 1] = plan.Cells[linha, coluna].Value.ToString();
                            }
                            catch { }
                        }
                    }
                    return resultado;
                }
            }
            else
            {
                throw new FileNotFoundException("Arquivo " + arquivoOriginal + " não encontrado.");
                //return null;
            }
        }

        public static string[,] LeIntervalo(string arquivoOriginal, int linhaInicial, int colunaInicial, int linhaFinal = 2147483647, int colunaFinal = 2147483647)
        {
            if (linhaInicial > linhaFinal || colunaInicial > colunaFinal)
            {
                throw new IndexOutOfRangeException();
            }
            if (File.Exists(arquivoOriginal))
            {
                //Nome do arquivo temporário
                string nomeArquivo = arquivoOriginal + "~Temp";

                //Deleta o arquivo temporário, se ele existe
                ArqManip.SafeDelete(nomeArquivo);

                //Cria o arquivo temporário
                File.Copy(arquivoOriginal, nomeArquivo);

                var file = new FileInfo(nomeArquivo);
                using (var package = new ExcelPackage(file))
                {
                    ExcelWorksheet plan = package.Workbook.Worksheets.First();
                    int ultimaLinha = plan.Dimension.End.Row;
                    ultimaLinha = ultimaLinha < linhaFinal ? ultimaLinha : linhaFinal;
                    int ultimaColuna = plan.Dimension.End.Column;
                    ultimaColuna = ultimaColuna < colunaFinal ? ultimaColuna : colunaFinal;
                    string[,] resultado = new String[ultimaLinha-linhaInicial+1, ultimaColuna-colunaInicial+1];
                    for (int linha = linhaInicial; linha <= ultimaLinha; linha++)
                    {
                        for (int coluna = colunaInicial; coluna <= ultimaColuna; coluna++)
                        {
                            try
                            {
                                resultado[linha - linhaInicial, coluna - colunaInicial] = plan.Cells[linha, coluna].Value.ToString();
                            }
                            catch { }
                        }
                    }
                    return resultado;
                }
            }
            else
            {
                throw new FileNotFoundException("Arquivo " + arquivoOriginal + " não encontrado.");
            }
        }

        private static double[,] EliminaNan(double[,] entrada)
        {

            List<double> temp = new List<double>(); ;
            List<List<double>> aux = new List<List<double>>();
            List<List<double>> aux2 = new List<List<double>>();
            int linhas = entrada.GetLength(0);
            int colunas = entrada.GetLength(1);
            int cont=0;
            for (int linha = 0; linha < linhas; linha++)
            {
                temp = new List<double>();
                bool testa = true;
                for (int coluna = 0; coluna < colunas; coluna++)
                {
                    temp.Add(entrada[linha, coluna]);
                    testa = Double.IsNaN(entrada[linha, coluna]) ? testa : false;
                }
                if (!testa)
                {
                    aux.Add(new List<double>());
                    aux[cont].AddRange(temp);
                    cont++;
                }
            }
            linhas = cont;
            cont = 0;
            //temp = new List<double>();
            for (int coluna = 0; coluna < colunas; coluna++)
            {
                temp = new List<double>();
                bool testa = true;
                for (int linha = 0; linha < linhas; linha++)
                {
                    temp.Add(aux[linha][coluna]);
                    testa = Double.IsNaN(aux[linha][coluna]) ? testa : false;
                }
                if (!testa)
                {
                    aux2.Add(new List<double>());
                    aux2[cont].AddRange(temp);
                    cont++;
                }
            }
            colunas = cont;
            double[,] saida = new Double[linhas, colunas];
            for (int coluna = 0; coluna < colunas; coluna++)
            {
                for (int linha = 0; linha < linhas; linha++)
                {
                    saida[linha,coluna] = aux2[coluna][linha];
                }
            }
            return saida;
        }

        public static Dictionary<string, double> XLSX2Dictionary(string nomeArquivoCompleto, int coluna = 2 )
        {
            Dictionary<string, double> saida = new Dictionary<string, double>();

            if (File.Exists(nomeArquivoCompleto))
            {
                var file = new FileInfo(nomeArquivoCompleto);
                
                // TODO opcao de executar com o arquivo aberto
                using (var package = new ExcelPackage(file))
                {
                    ExcelWorksheet plan = package.Workbook.Worksheets.First();
                    int ultimaLinha = plan.Dimension.End.Row;
                    double valor;
                    string texto;
                    for (int i = 1; i <= ultimaLinha; i++)
                    {
                        try
                        {
                            texto = plan.Cells[i, 1].Text;
                            valor = double.Parse(plan.Cells[i, coluna].Value.ToString());
                            saida.Add(texto, valor);
                        }
                        catch { }
                    }
                }
                return saida;
            }
            else
            {
                throw new FileNotFoundException("Arquivo " + nomeArquivoCompleto + " não encontrado.");
            }
        }
    }
}
