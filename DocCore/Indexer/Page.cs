using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DocCore
{
    public class Page
    {
        public static readonly int pageSize = 8192;
        public static readonly int intSizeBytes = (sizeof(int));

        public IntPtr bufferData;

        private int lastUsedBytePosition;
        public int LastUsedBytePosition
        {
            get { return lastUsedBytePosition; }
            set { lastUsedBytePosition = value; }
        }

        Page nextPage;

        public Page NextPage
        {
            get { return nextPage; }
            set { nextPage = value; }
        }
        Page lastPage;

        public Page LastPage
        {
            get { return lastPage; }
            set { lastPage = value; }
        }

        public Page()
        {
            bufferData = Marshal.AllocHGlobal(pageSize);
            this.nextPage = null;
            this.lastUsedBytePosition = 0;
        }
        public Page(int size)
        {
            bufferData = Marshal.AllocHGlobal(size);
            this.nextPage = null;
            this.lastUsedBytePosition = 0;
        }
        
        ~Page()
        {
            Marshal.FreeHGlobal(this.bufferData);
        }

        public bool IsFull()
        {
            if ((pageSize - lastUsedBytePosition) > intSizeBytes)
            {
                return false;
            }

            return true;
        }

        public bool IsFullForOccurrence(WordOccurrenceNode wordOccur)
        {
            int occurSize = wordOccur.GetSizeBytes();

            if ((pageSize - lastUsedBytePosition) > occurSize)
            {
                return false;
            }

            return true;
        }

        public bool HasNext()
        {
            if (this.nextPage != null)
                return true;

            return false;
        }

        public void Write(WordOccurrenceNode wordOccur)
        {
            this.Write(wordOccur.Doc.DocID, wordOccur);
            this.Write(wordOccur.QuantityHits, wordOccur);

            foreach (WordHit hit in wordOccur.Hits)
            {
                this.Write(hit.Position, wordOccur);
            }
        }

        public List<WordOccurrenceNode> GetWordOccurrencies(Word word)
        {
            List<WordOccurrenceNode> result = new List<WordOccurrenceNode>();

            for (int i = 0; i < this.lastUsedBytePosition; )
            {
                WordOccurrenceNode node = new WordOccurrenceNode();

                int tempDocumentHashOne = ReadInt32(i);
                i += intSizeBytes;
                int hitsCount = ReadInt32(i);
                i += intSizeBytes;

                node.Hits = new List<WordHit>();

                for (int y = 0; y < hitsCount; y++)
                {
                    WordHit hit = new WordHit();
                    hit.Position = ReadInt32(i);
                    node.Hits.Add(hit);
                    i += intSizeBytes;
                }

                node.Word = word;
                node.QuantityHits = hitsCount;
                node.DocID = tempDocumentHashOne;
                result.Add(node);
            }

            return result;
        }


        private void Write(int data, WordOccurrenceNode occur)
        {
            if ((occur.GetSizeBytes() > pageSize))
            {
                throw new Exception("O tamanho da total de uma página é muito pequeno para esta ocorrencia: " + occur.Doc.File + "_:_" + occur.Word.Text + "_:_" + occur.Hits.Count);
            }
            
            CopyBytes(data);
        }

        public static unsafe byte[] GetBytes(int value)
        {
            byte[] numArray = new byte[intSizeBytes];
            
            fixed (byte* numPtr = numArray)
                *(int*)numPtr = value;
            
            return numArray;
        }

        private unsafe void CopyBytes(int value)
        {
            Marshal.WriteInt32(this.bufferData + this.lastUsedBytePosition, value);

            this.lastUsedBytePosition += intSizeBytes;
        }

        private int ReadInt32(int position)
        {
            return Marshal.ReadInt32(this.bufferData, position);
        }

        private byte ReadByte(int position)
        {
            return Marshal.ReadByte(this.bufferData, position);

        }

        private unsafe void callback(IntPtr buffer, int length, string filename)
        {
            try
            {
                FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write);
                UnmanagedMemoryStream ustream = new UnmanagedMemoryStream((byte*)buffer, length);
                ustream.CopyTo(file);
                file.Close();
                ustream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void IncreaseSizePage(int bytes)
        {
            if (bufferData != IntPtr.Zero)
            { // memory has already been allocated
                bufferData = Marshal.ReAllocHGlobal(bufferData, (IntPtr)bytes);
            }
            else
            { // this is a brand new data buffer
                bufferData = Marshal.AllocHGlobal(bytes);
            }
        }
    }
}
