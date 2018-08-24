﻿using ExecutorOpenDSS.Classes;
using ExecutorOpenDSS.Classes_Principais;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutorOpenDSS
{
    class ArqManip
    {

        //Verifica se o arquivo existe antes de deletá-lo
        public static void SafeDelete(string arquivo)
        {
            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
            }
        }

        //Escreve uma lista de strings em um arquivo
        public static void GravaListArquivoTXT(List<string> conteudo, string arquivo, MainWindow janela)
        {
            //Verifica se existe conteudo
            if (conteudo != null)
            {
                //Tenta escrever no arquivo
                try
                {
                    File.WriteAllLines(arquivo, conteudo);
                }

                //Caso não consiga, exibe mensagem de erro
                catch
                {
                    janela.Disp("Não foi possível escrever no arquivo " + arquivo, "E");
                }
            }
        }
        
        //Grava CONTEUDO em arquivo FID 
        public static void GravaEmArquivoTensaoBarraTrafos(string conteudo, string fid, MainWindow _janela)
        {
            //
            while (true)
            {
                try
                {
                    // TODO consertar esquema de bloqueio de arquivo
                    string arq = _janela._indiceArq == 0 ? fid : fid + _janela._indiceArq + ".txt";

                    //
                    //string arq = fid;

                    using (StreamWriter file = new StreamWriter(arq, true))
                    {
                        file.WriteLine(conteudo); //.WriteLineAsync(
                    }
                    break;
                }
                catch
                {
                    _janela.Disp("Arquivo " + fid + " bloqueado. Tentando outro nome de arquivo");
                    _janela._indiceArq++;
                }
                if (_janela._indiceArq > 10)
                {
                    _janela.Disp("Não foi possível escrever o arquivo " + fid + ". Verifique se possui permissão de escrita no local");
                    break;
                }
            }
        }
                
        //Grava CONTEUDO em arquivo FID 
        public static void GravaEmArquivo(string conteudo, string fid, MainWindow _janela )
        {
            //
            while(true)
            {
                try
                {
                    // TODO consertar esquema de bloqueio de arquivo
                    string arq = _janela._indiceArq == 0 ? fid : fid + _janela._indiceArq + ".txt";
                    
                    //
                    //string arq = fid;

                    using (StreamWriter file = new StreamWriter(arq, true))
                    {
                        file.WriteLineAsync(conteudo);
                    }
                    break;
                }
                catch 
                {
                        _janela.Disp("Arquivo " + fid + " bloqueado. Tentando outro nome de arquivo");
                        _janela._indiceArq++;
                }
                if (_janela._indiceArq > 10)
                {
                    _janela.Disp("Não foi possível escrever o arquivo " + fid + ". Verifique se possui permissão de escrita no local");
                    break;
                }
            }       
        }

        //funcao gravaPerdas em arquivo
        static public void GravaPerdas(ResultadoFluxo perdasTotais, string nomeAlim, string fid, MainWindow jan)
        {
            /*
            // armazena resultados em classe
            ResPotenciaEEnergia res = new ResPotenciaEEnergia(perdasTotais);

            // grava perdas arquivo
            ArqManip.GravaPerdasAlimArquivo(nomeAlim, res, fid, jan);
             * */

            // grava perdas arquivo
            ArqManip.GravaPerdasAlimArquivo(nomeAlim, perdasTotais, fid, jan);
        }

        //Tradução da função gravaPerdasAlimArquivo
        static public void GravaPerdasAlimArquivo(string nomeAlim, ResultadoFluxo perdas, string fid, MainWindow janela)
        {
            // cria string com o formato de saida das perdas
            string conteudo = perdas._energyMeter.formataResultado(nomeAlim);

            //Grava em arquivo
            ArqManip.GravaEmArquivo(conteudo, fid, janela);
        }
  
        //Grava a lista de alimentadores não convergentes em um txt
        static public void gravaLstAlimNaoConvergiram(ParamGeraisDSS paramGerais, MainWindow _janela)
        {
            string nomeAlim = paramGerais.getNomeAlimAtual();

            while (true)
            {
                try
                {
                    // 
                    string arquivoNome = paramGerais._parGUI._pathRaizGUI + paramGerais._arquivoResAlimNaoConvergiram;
                    
                    using (StreamWriter file = new StreamWriter(arquivoNome, true))
                    {
                        file.WriteLineAsync(nomeAlim);
                    }
                    break;
                }
                catch
                {
                    _janela.Disp("Arquivo dos alimentadores não convergentes bloqueado!");
                }
            }       
        }

        internal static List<string> leAlimentadoresArquivoTXT(string p)
        {
            //TODO obter do projeto Conversor


            throw new NotImplementedException();
        }

        internal static void GravaDictionaryExcel(FileInfo file, Dictionary<string, double> mapAlimLoadMult)
        {
            using (var package = new ExcelPackage(file))
            {
                ExcelWorksheet plan = package.Workbook.Worksheets.Add("Ajustes");
                int linha = 1;

                foreach (KeyValuePair<string, double> kvp in mapAlimLoadMult)
                {
                    plan.Cells[linha, 1].Value = kvp.Key;
                    plan.Cells[linha, 2].Value = kvp.Value;
                    linha++;
                }
                package.Save();
            }
        }
    }
}
