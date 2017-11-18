using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace DocCore
{
    /// <summary>
    /// This code is bad. Have memory allocation activities that does not need but
    /// the constructor is a better version of Singleton.
    /// </summary>

    public class InvertedFileDisk : IInvertedFile
    {
        EngineConfiguration conf;
        IRepositoryDocument docIndex;
        ILexicon lexicon;

        private Hashtable dictionaryTemp;
        private Stream stream;

        BinaryReader br;
        BinaryWriter bw;

        string invertedfileName;

        private InvertedFileDisk()
        {
            this.invertedfileName = string.Empty;
            this.docIndex = FactoryRepositoryDocument.GetRepositoryDocument();
            this.lexicon = FactoryLexicon.GetLexicon();
            this.dictionaryTemp = new Hashtable();
        }

        public static InvertedFileDisk Instance { get { return Nested.Instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly InvertedFileDisk Instance = new InvertedFileDisk();
        }

        public void AddWordOccurrence(WordOccurrenceNode wordOccur)
        {
            //create the file or add entry to file
            try
            {
                if (dictionaryTemp.Contains(wordOccur.Word.WordID))
                    stream = (Stream)dictionaryTemp[wordOccur.Word.WordID];
                else
                {
                    byte[] bufferStream = new byte[8192];
                    stream = new MemoryStream(bufferStream);
                    dictionaryTemp[wordOccur.Word.WordID] = stream;
                }

                bw = new BinaryWriter(stream);

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
            List<WordOccurrenceNode> result = new List<WordOccurrenceNode>();

            try
            {
                Stream stream = new FileStream(invertedfileName, FileMode.Open);;

                stream.Position = word.StartPositionInvertedFile;
                //open the file
                br = new BinaryReader(stream);

                //reading the file
                for (int i = 0; (i < conf.MaxResultList) && (br.BaseStream.Position < word.EndPositionInvertedFile); i++)
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
                    node.Doc = this.docIndex.Read(tempDocumentHashOne);
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

        public void WriteToStorage()
        {
            string indexFileName = @"G:\IndexFiles\index.dat";
            bw = new BinaryWriter(new FileStream(indexFileName, FileMode.OpenOrCreate));

            foreach (KeyValuePair<int,MemoryStream> item in dictionaryTemp)
            {
                int key = item.Key;
                Word w = lexicon.GetWord(ref key);
                w.StartPositionInvertedFile = (int)bw.BaseStream.Position;
                w.EndPositionInvertedFile = (int)(bw.BaseStream.Position + (int)item.Value.Length);

                bw.Write(item.Value.ToArray(),0,(int)item.Value.Length);
            }
            

            GC.ReRegisterForFinalize(dictionaryTemp);
            GC.Collect();
        }
    }
}
