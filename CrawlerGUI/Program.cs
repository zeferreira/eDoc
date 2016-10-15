using System;
using System.Collections.Generic;
using System.Text;
using DocCore;
using System.IO;
using System.Configuration;

namespace CrawlerGUI
{
    class Program
    {
        static void Main(string[] args)
        {
            int niveis = 1;
            List<string> inst = GetInstitutions(@"D:\projetos\edoc\CodeRepository\eDoc\CrawlerGUI\DataSearch\Inst\brasil.txt");
            List<string> themes = GetThemes(@"D:\projetos\edoc\CodeRepository\eDoc\CrawlerGUI\DataSearch\Themes\computer_pt_br.txt");

            foreach (string theme in themes)
            {
                foreach (string instAtual in inst)
                {
                    string tema = Uri.EscapeUriString(theme);
                    string instituicao = Uri.EscapeUriString(instAtual);

                    BaixarTema(niveis, tema, instituicao);
                }
            }

            Console.WriteLine("Fim");
            Console.ReadLine();
        }

        private static List<string> GetInstitutions(string file)
        {
            var strInstFile = File.ReadAllLines(file);
            List<string> instList = new List<string>(strInstFile);

            return instList;
        }

        private static List<string> GetThemes(string file)
        {
            var strInstFile = File.ReadAllLines(file);
            List<string> instList = new List<string>(strInstFile);

            return instList;
        }

        private static void BaixarTema(int niveis, string tema, string instituicao)
        {
            string folderForFiles = ConfigurationManager.AppSettings["folderForFiles"].ToString() + @"\";

            int nivel = niveis;
            Crawler cr = new Crawler(folderForFiles);

            for (int i = 0; i < nivel; i++)
            {
                string url = @"https://scholar.google.com.br/scholar?start=" + i.ToString() + @"0&q=filetype:pdf+" + tema + "+" + instituicao + "&hl=pt-BR&as_sdt=0,5";

                string content = cr.GetWebPageContent(url);

                List<string> urlList = cr.GetDocumentListFromPage(content);

                foreach (string item in urlList)
                {
                    Console.WriteLine(item);

                    try
                    {
                        cr.DownloadFile(item);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Falha ao tentar baixar: " + item);
                        //throw;
                    }

                }

                Console.WriteLine("Page: " + i.ToString());
            }
        }

    }
}
