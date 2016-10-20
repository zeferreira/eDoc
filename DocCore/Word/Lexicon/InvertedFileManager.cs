using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DocCore
{
    /// <summary>
    /// This code is bad. Have memory allocation activities that does not need but
    /// the constructor is a better version of Singleton.
    /// </summary>

    public class InvertedFileManager
    {
        EngineConfiguration conf;
        IDocumentIndex docIndex;

        BinaryReader br;
        BinaryWriter bw;

        string invertedfileName;

        private InvertedFileManager()
        {
            this.invertedfileName = string.Empty;
            this.docIndex = FactoryDocumentIndex.GetDocumentIndex();
        }

        public static InvertedFileManager Instance { get { return Nested.Instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly InvertedFileManager Instance = new InvertedFileManager();
        }

        public void AddWordOccurrence(WordOccurrenceNode wordOccur)
        {
            invertedfileName = GetFileName(wordOccur.Word.WordID);

            //create the file or add entry to file
            try
            {
                bw = new BinaryWriter(new FileStream(invertedfileName, FileMode.Append));

                bw.Write(wordOccur.Doc.DocID);
                bw.Write(wordOccur.QuantityHits);

                foreach (WordHit hit in wordOccur.Hits)
                {
                    bw.Write(hit.Position);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("\n Cannot create file or write to file." + e.Message);
                return;
            }
            finally
            {
                bw.Close();
            }
        }

        
        public List<WordOccurrenceNode> GetWordOccurrencies(Word word)
        {
            invertedfileName = GetFileName(word.WordID);
            List<WordOccurrenceNode> result = new List<WordOccurrenceNode>();

            try
            {
                //open the file
                br = new BinaryReader(new FileStream(invertedfileName, FileMode.Open));

                //reading the file
                for (int i = 0; (i < conf.MaxResultList) && (br.BaseStream.Position < br.BaseStream.Length); i++)
                {
                    int tempDocumentHashOne = br.ReadInt32();
                    int hitsCount = br.ReadInt32();

                    WordOccurrenceNode node = new WordOccurrenceNode();

                    node.Hits = new List<WordHit>();

                    for (int y = 0; y < hitsCount; y++)
                    {
                        WordHit hit = new WordHit();
                        hit.Position = br.ReadInt32();
                        node.Hits.Add(hit);
                    }

                    node.Word = word;
                    node.QuantityHits = hitsCount;
                    node.Doc = this.docIndex.Search(tempDocumentHashOne);
                    result.Add(node);
                }

                return result;
            }
            catch (IOException e)
            {
                throw e;
            }
            finally
            {
                br.Close();
            }
        }


        private string GetFileName(int wordID)
        {
            conf = EngineConfiguration.Instance;

            return conf.PathFolderIndex + @"\" + wordID.ToString() + ".ind";
        }
    }
}
