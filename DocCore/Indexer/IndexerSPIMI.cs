using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using HtmlAgilityPack;
using System.Collections;

namespace DocCore
{
    public class IndexerSPIMI :IIndexer
    {
        IRepositoryDocument repDocumentFinal;
        
        EngineConfiguration conf;
        ILexicon finalLexicon;
        long totalWordQuantity;
        long initialFileSize;
        string strFolderBlockTempFiles;

        string strFinalIndexFile;

        public long TotalWordQuantity
        {
            get { return totalWordQuantity; }
            set { totalWordQuantity = value; }
        }

        long totalDocumentQuantity;

        public long TotalDocumentQuantity 
        { 
            get 
            { 
                return this.totalDocumentQuantity; 
            } 
        }
        
        string indexFileFolder;
        int totalIndexedDocuments;
        List<Block> blocksToMerge = new List<Block>();

        private static readonly object _lock = new object();
        List<Thread> docIndexQueue;
        List<Thread> indexBlockWriterQueue;
        bool isRunning = false;

        int threadQuantLimit = 7;

        int tmpQuantThreadAlive = 0;
        int tmpQuantThreadAliveBlock = 0;

        int tmpblockNumber = 0;
        int tmpLexiconNumber = 0;

        public bool IsRunning
        {
            get {
                lock (_lock)
                {
                    return this.isRunning;
                }
            }
        }

        private static IndexerSPIMI instance = null;
        private static readonly object padlock = new object();

        public static IndexerSPIMI Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new IndexerSPIMI();
                    }
                    return instance;
                }
            }
        }

        //Lexicon must fit in the main memory

        IndexerSPIMI()
        {
            this.indexFileFolder = EngineConfiguration.Instance.PathFolderIndex;
            this.docIndexQueue = new List<Thread>();
            this.indexBlockWriterQueue = new List<Thread>();
            isRunning = false;

            this.repDocumentFinal = FactoryRepositoryDocument.GetRepositoryDocument();
            this.totalIndexedDocuments = repDocumentFinal.GetTotalQuantity();
            this.totalDocumentQuantity = totalIndexedDocuments;
            this.finalLexicon = new LexiconDisk();
            this.initialFileSize = (1024 *1024);
            this.strFinalIndexFile = EngineConfiguration.Instance.FinalIndexFile;
            this.strFolderBlockTempFiles = EngineConfiguration.Instance.STRFolderBlockTempFiles;

            this.conf = EngineConfiguration.Instance;

            //for debug
            TextWriterTraceListener traceListener = new TextWriterTraceListener(System.IO.File.CreateText(EngineConfiguration.Instance.TraceDebugFile));
            Debug.Listeners.Add(traceListener);
        }
        /// <summary>
        /// This algorithm wait's for the processed next file and start a new thread for process a new file. 
        /// The problem is that he waits for full process of file before start a new request to the disc.
        /// Get a new approach 
        /// </summary>
        public void Index()
        {
            this.isRunning = true;
            DateTime start = DateTime.Now;

            threadQuantLimit = 7;
                
            tmpQuantThreadAliveBlock = 0;

            tmpblockNumber = 0;
            tmpLexiconNumber = 0;

            Block tmpBlock = new Block();
            tmpBlock.BlockFileName = strFolderBlockTempFiles + tmpblockNumber.ToString();
            tmpBlock.Lexicon = new LexiconDisk(tmpLexiconNumber.ToString());

            this.blocksToMerge.Add(tmpBlock);

            List<Document> listOfDocuments = repDocumentFinal.List();

            IEnumerator<Document> iterator = listOfDocuments.GetEnumerator();

            while(!ItsDone())
            {
                for (; (tmpQuantThreadAlive < threadQuantLimit) && iterator.MoveNext(); tmpQuantThreadAlive++)
                {
                    Document doc = iterator.Current;
                    Thread thread = new Thread(() => IndexFile(doc, tmpBlock));
                    docIndexQueue.Add(thread);
                    thread.Start();
                }

                foreach (Thread th in docIndexQueue)
                {
                    if (!th.IsAlive)
                    {
                        docIndexQueue.Remove(th);
                        tmpQuantThreadAlive--;
                        break;
                    }
                }

                //write a block here
                //TO DO 
                float tmpMemoryQuant = Useful.GetTotalMemoryProcessInGB();
                if ((tmpMemoryQuant >= 3) || ItsDone())
                {
                    //wait for threads writing in the current block
                    
                    WaitThreadFinishDoc();

                    Thread thread = new Thread(() => WriteBlockToDisk(tmpBlock));
                    indexBlockWriterQueue.Add(thread);
                    thread.Start();

                    if(!ItsDone())
                    {
                        tmpblockNumber++;
                        tmpLexiconNumber++;

                        tmpBlock = new Block();
                        tmpBlock.BlockFileName = strFolderBlockTempFiles + tmpblockNumber.ToString();
                        tmpBlock.Lexicon = new LexiconDisk(tmpLexiconNumber.ToString());
                        this.blocksToMerge.Add(tmpBlock);
                    }
                    
                    // another process may be using the block
                }

            }

            //clean the threads (doc)
            WaitThreadFinishDoc();

            //clean the threads (blocks)
            WaitThreadBlockWrite();

            //merge blocks here
            Merge(blocksToMerge);

            //clear the tmpblockfiles
            ClearTmpBlockFiles(blocksToMerge);
            this.isRunning = false;
        }

        public List<WordOccurrenceNode> GetWordOccurrencies(Word word)
        {
            List<WordOccurrenceNode> result = new List<WordOccurrenceNode>();

            try
            {
                //many threads need to read the file
                using (var stream = File.Open(this.strFinalIndexFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    stream.Position = word.StartPositionInvertedFile;
                    //open the file
                    BinaryReader br = new BinaryReader(stream);

                    //reading the file
                    for (int i = 0; (i < conf.MaxResultList) && (br.BaseStream.Position < word.EndPositionInvertedFile); i++)
                    {
                        //for debug 
                        Debug.WriteLine("WordID: " + word.WordID + " Start: " + word.StartPositionInvertedFile + " End: " + word.EndPositionInvertedFile);

                        int tempDocumentHashOne = br.ReadInt32();
                        int hitsCount = br.ReadInt32();

                        Debug.WriteLine("WordID: " + word.WordID + " docHash: " + tempDocumentHashOne + " hitsCount: " + hitsCount);

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
                        node.Doc = this.repDocumentFinal.Read(tempDocumentHashOne);

                        result.Add(node);
                    }

                    stream.Close();
                    br.Close();
                }
                
                return result;
            }
            catch (IOException e)
            {
                throw e;
            }
            finally
            {
                
            }
        }

        private void IndexFile(Document docItem, Block tmpBlock)
        {
            IRepositoryLog repLog = FactoryRepositoryLog.GetRepositoryLog();

            try
            {
                Hashtable postingList = docItem.GetPostingListClass();
                IDictionaryEnumerator iDicE = postingList.GetEnumerator();

                while (iDicE.MoveNext())
                {
                    //get posting list and add hits
                    //never reindex the same document 2 times.

                    DictionaryEntry dicEntry = (DictionaryEntry)iDicE.Current;
                    WordOccurrenceNode occurrence = dicEntry.Value as WordOccurrenceNode;
                    lock (_lock)
                    {
                        //Do not has any word
                        if (!tmpBlock.Lexicon.HasWord(ref occurrence.Word.WordID))
                        {
                            //tmp lexicon
                            occurrence.Word.FirstOccurrence = occurrence;
                            occurrence.Word.LastOccurrence = occurrence;
                            occurrence.Word.QuantityHits = occurrence.Hits.Count;
                            tmpBlock.Lexicon.AddNewWord(occurrence.Word);
                            occurrence.Word.QuantityDocFrequency++;

                            //final lexicon
                            Word finalWord = new Word();
                            finalWord.WordID = occurrence.Word.WordID;
                            finalWord.Text = occurrence.Word.Text;
                            finalWord.QuantityDocFrequency = occurrence.Word.QuantityDocFrequency;
                            finalWord.QuantityHits = occurrence.Word.QuantityHits;
                            this.finalLexicon.AddNewWord(finalWord);
                        }
                        else
                        {
                            //to do: memory allocation alert!! remove ref!! use newnode.Word.text or something, don't pass WordID as a parameter!! Performance is poor.
                            occurrence.Word = tmpBlock.Lexicon.GetWord(ref occurrence.Word.WordID);

                            occurrence.PreviousOccurrence = occurrence.Word.LastOccurrence;
                            occurrence.Word.LastOccurrence.NextOccurrence = occurrence;
                            occurrence.Word.LastOccurrence = occurrence;
                            occurrence.Word.QuantityDocFrequency++;
                            occurrence.Word.QuantityHits += occurrence.Hits.Count;

                            //final lexicon
                            Word finalWord = this.finalLexicon.GetWord(ref occurrence.Word.WordID);
                            finalWord.QuantityDocFrequency++;
                            finalWord.QuantityHits += occurrence.Hits.Count;
                        }

                        totalWordQuantity += occurrence.Hits.Count;
                    }
                }

                lock (_lock)
                {
                    this.totalIndexedDocuments--;
                    Debug.WriteLine("#Number: " + this.totalIndexedDocuments.ToString() + "#File: " + docItem.File);
                }

                GC.ReRegisterForFinalize(postingList);
                GC.ReRegisterForFinalize(iDicE);
                GC.Collect();
            }
            catch (Exception e)
            {
                Log entry = new Log();
                entry.TaskDescription = "Read file error";
                entry.LogParameters = new List<string>();
                entry.LogParameters.Add("FileName: " + docItem.File);
                entry.LogParameters.Add("Error: " + e.Message);

                repLog.Write(entry);
            }
        }

        private void WriteBlockToDisk(Block block)
        {
            BinaryWriter bw;
            FileStream fileStream = new FileStream(block.BlockFileName, FileMode.Create);
            bw = new BinaryWriter(fileStream);

            try
            {
                SortedDictionary<int, Word> dictionary = ((LexiconDisk)block.Lexicon).GetDictionary();

                foreach (KeyValuePair<int, Word> entry in dictionary)
                {
                    Word word = (Word)entry.Value;

                    word.StartPositionInvertedFile = bw.BaseStream.Position;

                    WordOccurrenceNode tmpOccur = word.FirstOccurrence;
                    
                    do
                    {
                        bw.Write(tmpOccur.Doc.DocID);
                        bw.Write(tmpOccur.QuantityHits);

                        foreach (WordHit hit in tmpOccur.Hits)
                        {
                            bw.Write(hit.Position);
                        }
                        
                    } while ((tmpOccur = tmpOccur.NextOccurrence) != null) ;

                    word.EndPositionInvertedFile = bw.BaseStream.Position;
                    word.FirstOccurrence = null;//free memory
                    word.LastOccurrence = null;//free memory 
                    //wait for gc
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

        private void ClearTmpBlockFiles(List<Block> listOfBlocks)
        {
            foreach (Block item in listOfBlocks)
            {
                File.Delete(item.BlockFileName);
            }
        }
        private void Merge(List<Block> listOfBlocks)
        {
            try
            {
                //final lexicon
                LexiconDisk lexicon = (LexiconDisk)this.finalLexicon;
                //final index stream
                FileStream fileStreamFinalIndex = new FileStream(this.strFinalIndexFile, FileMode.Create);
                fileStreamFinalIndex.SetLength(this.initialFileSize);
                BinaryWriter bwFinalIndex = new BinaryWriter(fileStreamFinalIndex);
                int bufferSize = (512 * 2); //twice disk sector
                int intSizeBytes = sizeof(int);

                SortedDictionary<int, Word> dictionary = lexicon.GetDictionary();

                foreach (KeyValuePair<int, Word> entry in dictionary)
                {
                    Word word = (Word)entry.Value;
                    word.StartPositionInvertedFile = bwFinalIndex.BaseStream.Position;

                    foreach (Block block in listOfBlocks)
                    {
                        using (var stream = File.Open(block.BlockFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            BinaryReader br = new BinaryReader(stream);

                            if (block.Lexicon.HasWord(ref word.WordID))
                            {
                                Word wordInBlock = block.Lexicon.GetWord(ref word.WordID);
                                long startWordPositionActualBlock = wordInBlock.StartPositionInvertedFile;
                                long finalWordPositionActualBlock = wordInBlock.EndPositionInvertedFile;

                                br.BaseStream.Position = startWordPositionActualBlock;

                                int bytesToRead = (int)(finalWordPositionActualBlock - startWordPositionActualBlock);

                                byte[] buffer = new byte[bufferSize];
                                int read;
                                while (bytesToRead > 0 &&
                                       (read = br.Read(buffer, 0, Math.Min(buffer.Length, bytesToRead))) > 0)
                                {
                                    bwFinalIndex.Write(buffer, 0, read);
                                    bytesToRead -= read;
                                }
                            }
                        }
                    }
                    word.EndPositionInvertedFile = bwFinalIndex.BaseStream.Position;
                }
                //to do: delele block files.
                lexicon.WriteToStorage();

            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
            }
        }

        private void WaitThreadFinishDoc()
        {
            while (docIndexQueue.Count > 0)
            {
                foreach (Thread th in docIndexQueue)
                {
                    if (!th.IsAlive)
                    {
                        docIndexQueue.Remove(th);
                        tmpQuantThreadAlive--;
                        break;
                    }
                }
            }
        }

        private void WaitThreadBlockWrite()
        {
            while (indexBlockWriterQueue.Count > 0)
            {
                foreach (Thread th in indexBlockWriterQueue)
                {
                    if (!th.IsAlive)
                    {
                        indexBlockWriterQueue.Remove(th);
                        tmpQuantThreadAliveBlock--;
                        break;
                    }
                }
            }
        }

        private bool ItsDone()
        {
            lock (_lock)
            {
                if (this.totalIndexedDocuments == 0)
                    return true;
                else
                    return false;
            }
        }

        public void Index(List<Document> listOfDocs)
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            this.finalLexicon.LoadFromStorage();
            this.totalDocumentQuantity = this.repDocumentFinal.GetTotalQuantity();
            this.totalWordQuantity = this.finalLexicon.Quantity;

            if (!File.Exists(this.strFinalIndexFile))
            {
                throw new Exception("Index Not Found: " + strFinalIndexFile);
            }
        }

        public void ReIndexing()
        {
            Index();
        }

        public Word Search(int wordID)
        {
            if (this.finalLexicon.HasWord(ref wordID))
                return this.finalLexicon.GetWord(ref wordID);
            else
                return null;
        }
    }
}
