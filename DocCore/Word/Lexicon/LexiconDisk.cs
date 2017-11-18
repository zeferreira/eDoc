using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace DocCore
{
    public class LexiconDisk : ILexicon
    {
        //Hashtable ht;
        SortedDictionary<int, Word> ht;
        private long maxLength;

        BinaryWriter bw;
        BinaryReader br;

        private string lexiconFileName;

        public string LexiconFileName
        {
            get { return lexiconFileName; }
        }

        public long MaxLength 
        { 
            get 
            {
                return this.maxLength;   
            } 
        }

        public long Quantity
        {
            get { return ht.Count; }
        }

        //implements singleton pattern
        private static LexiconDisk instance = null;
        private static readonly object padlock = new object();

        public static LexiconDisk Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new LexiconDisk();
                    }
                    return instance;
                }
            }
        }

        public LexiconDisk()
        {
            this.lexiconFileName = EngineConfiguration.Instance.LexiconFileName;

            //if (!File.Exists(this.lexiconFileName))
            //{
            //    throw new Exception("Lexicon Not Found: " + this.lexiconFileName);
            //}

            this.ht = new SortedDictionary<int, Word>();
            this.maxLength = int.MaxValue;
        }

        public LexiconDisk(string lexiconFileName)
        {
            this.lexiconFileName = EngineConfiguration.Instance.STRFolderBlockTempFiles + lexiconFileName + ".lex";
            //this.ht = new Hashtable();
            this.ht = new SortedDictionary<int, Word>();
            this.maxLength = int.MaxValue;
        }

        //public Word GetWord(string word)
        //{
        //    return ht[word.GetHashCode()] as Word;
        //}

        public Word GetWord(ref int wordID)
        {
            return ht[wordID] as Word;
        }

        public void AddNewWord(Word word)
        {
            this.ht.Add(word.WordID, word);
        }

        public bool HasWord(ref int wordID)
        {
            if (this.ht.ContainsKey(wordID))
            {
                return true;
            }

            return false;
        }

        public void LoadFromStorage()
        {
            this.br = new BinaryReader(new FileStream(lexiconFileName, FileMode.Open));

            for (int i = 0; (br.BaseStream.Position < br.BaseStream.Length); i++)
            {
                Word word = new Word();
                //word.Text = br.ReadString();
                word.WordID = br.ReadInt32();
                word.QuantityHits = br.ReadInt32();
                word.QuantityDocFrequency = br.ReadInt32();
                word.StartPositionInvertedFile = br.ReadInt64();
                word.EndPositionInvertedFile = br.ReadInt64();

                this.ht.Add(word.WordID, word);
            }
        }

        public void WriteToStorage()
        {
            FileStream fileStream = new FileStream(lexiconFileName, FileMode.Create);
            this.bw = new BinaryWriter(fileStream);

            foreach (KeyValuePair<int, Word> entry in this.ht)
            {
                Word item = (Word)entry.Value;

                //debug
                Debug.WriteLine(item.Text);
                Debug.WriteLine(item.StartPositionInvertedFile);
                Debug.WriteLine(item.EndPositionInvertedFile);
                //debug
                
                bw.Write(item.WordID); 
                //bw.Write(item.Text);
                bw.Write(item.QuantityHits);
                bw.Write(item.QuantityDocFrequency);
                bw.Write(item.StartPositionInvertedFile);
                bw.Write(item.EndPositionInvertedFile);
            }

            bw.Close();
            fileStream.Close();
        }

        public SortedDictionary<int, Word> GetDictionary()
        {
            return this.ht;
        }
    }
}
