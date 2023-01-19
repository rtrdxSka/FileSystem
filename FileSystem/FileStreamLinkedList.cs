using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    public class FileStreamLinkedListNode<T>
    {
        public long Position;
        public long Prev;
        public long Next;
        public long LocalHead;
        public long LocalPrev;
        public long LocalNext;
        public bool IsFolder;
        public string Name;

        public T Value;
    }
    public class FileStreamLinkedList<T> : IDisposable
    where T : IStreamable, new()
    {
        

        private Stream _stream;
        private BinaryWriter _bw;
        private BinaryReader _br;
        private long _head;
        private long _tail;
        public FileStreamLinkedListNode<T> Head
        {
            get
            {
                var node1 = new FileStreamLinkedListNode<T>() { Position = _head };
                
                LoadNode(node1);

                return node1;
               
            }
            
        }
        public FileStreamLinkedListNode<T> Tail
        {
            get
            {
                var node2 = new FileStreamLinkedListNode<T>() { Position = _tail };

                LoadNode(node2);

                return node2;
            }
        }

        public FileStreamLinkedList(string storagePath)
        {
            var _new = !File.Exists(storagePath);

            _stream = new FileStream(storagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _bw = new BinaryWriter(_stream);
            _br = new BinaryReader(_stream);

            if (_new)
            {
                _head = -1;
                _tail = -1;

                SaveMetaData();
            }
            else
            {
                LoadMetaData();
            }
        }
        void SaveMetaData()
        {
            _stream.Position = 0;
            _bw.Write(_head);
            _bw.Write(_tail);
        }
        void LoadMetaData()
        {
            _stream.Position = 0;
            _head = _br.ReadInt64();
            _tail = _br.ReadInt64();

        }
        void SaveContentNode (FileStreamLinkedListNode<T> node)
        {
            if (!node.IsFolder)
                node.Value.SaveToStream(_stream);
            
        }
       public void SaveNode(FileStreamLinkedListNode<T> node)
        {
           
            _stream.Position = node.Position;
            _bw.Write(node.Position);
            _bw.Write(node.Prev);
            _bw.Write(node.Next);
            _bw.Write(node.LocalHead);
            _bw.Write(node.LocalPrev);
            _bw.Write(node.LocalNext);
            _bw.Write(node.IsFolder);
            _bw.Write(node.Name);

        }
        void LoadContentNode(FileStreamLinkedListNode<T> node)
        {
            LoadNode(node);
            node.Value = new T();
            if (node.Next == -1)
                node.Value.LoadFromStream(_stream, _stream.Length - _stream.Position);
            else
                node.Value.LoadFromStream(_stream, node.Next - _stream.Position);
        }
        void LoadNode(FileStreamLinkedListNode<T> node)
        {
            //Head - 8
            //Tail - 16
            //prev na fail1 - 24
            //next na fail1 - 32
            //sadarjanie - 32 poziciq
            //prev na fail2 - 8

            _stream.Position = _br.BaseStream.Position = node.Position;
            node.Position = _br.ReadInt64();
            node.Prev = _br.ReadInt64();
            node.Next = _br.ReadInt64();
            node.LocalHead = _br.ReadInt64();
            node.LocalPrev = _br.ReadInt64();
            node.LocalNext = _br.ReadInt64();
            node.IsFolder = _br.ReadBoolean();
            node.Name = _br.ReadString();
            // FileContent f = new FileContent();
            // f.LoadFromStream(_stream);


        }

        public FileStreamLinkedListNode<FileContent> LoadNodeByPositon(long position)
        {
            //Head - 8
            //Tail - 16
            //prev na fail1 - 24
            //next na fail1 - 32
            //sadarjanie - 32 poziciq
            //prev na fail2 - 8
            if (position == -1)
                return null;
            _stream.Position = _br.BaseStream.Position = position;
            var node = new FileStreamLinkedListNode<FileContent>();
            node.Position = _br.ReadInt64();
            node.Prev = _br.ReadInt64();
            node.Next = _br.ReadInt64();
            node.LocalHead = _br.ReadInt64();
            node.LocalPrev = _br.ReadInt64();
            node.LocalNext = _br.ReadInt64();
            node.IsFolder = _br.ReadBoolean();
            node.Name = _br.ReadString();

            // FileContent f = new FileContent();
            // f.LoadFromStream(_stream);
            return node;

        }

        public void Insert(FileStreamLinkedListNode<T> prev, FileStreamLinkedListNode<T> node)
        {
            node.Position = _stream.Length;
            if (prev == null && _head == -1) // situaciq nqma nishto v spisaka
            {

                node.Prev = -1;
                node.Next = -1;
                SaveNode(node);
                SaveContentNode(node);
                long position = node.Position;
                _head = position;
                _tail = position;
                
                SaveMetaData();
            }
            else if (prev != null && prev.Next == -1) // posledniq element, koito se dobavq
            {
                node.Prev = _tail;
                node.Next = -1;
                SaveNode(node);
                SaveContentNode(node);

                prev.Next = node.Position;
                SaveNode(prev);

                _tail = node.Position;
                SaveMetaData();
            }
            else if (prev != null && prev.Next != -1) // dobavqne element v sredata
            {
                node.Prev = prev.Position;
                node.Next = prev.Next;
                SaveNode(node);
                SaveContentNode(node);
                prev.Next = node.Position;
                SaveNode(prev);

                var next = new FileStreamLinkedListNode<T>() { Position = node.Next };
                LoadNode(next);
                next.Prev = node.Position;
                SaveNode(next); 
            }
            else if (prev == null && _head != -1) // parvi element, v palen spisak
            {
                node.Prev = -1;
                node.Next = _head;
                SaveNode(node);
                SaveContentNode(node);
                var next = new FileStreamLinkedListNode<T>() { Position = _head };
                LoadNode(next);
                next.Prev = node.Position;
                SaveNode(next);

                _head = node.Position;
                SaveMetaData();


            }
        }
        public FileStreamLinkedListNode<T> Next(FileStreamLinkedListNode<T> prev)
        {
            var node = new FileStreamLinkedListNode<T>() { Position = prev.Next };

            LoadNode(node);

            return node;
        }
        public FileStreamLinkedListNode<T> Prev(FileStreamLinkedListNode<T> node)
        {
            var prev = new FileStreamLinkedListNode<T>() { Position = node.Prev };

            LoadNode(prev);
            return prev;
        }
        public void Remove(FileStreamLinkedListNode<T> node)
        {
           
            var nextPosition = node.Next;
            var prevPosition = node.Prev;
            if (nextPosition != -1 && prevPosition != -1)
            {
                var next = Next(node);
                var prev = Prev(node);
                next.Prev = node.Prev;
                prev.Next = node.Next;
                SaveNode(next);
                SaveNode(prev);
                //SaveNode(node);
            }
            /*else
            {
               *//* _tail = node.Prev;
                var prev = Prev(node);
                prev.Next = -1;
               
                SaveNode(prev);*//*
            }*/
            else if (nextPosition == -1 && prevPosition != -1)
            {
                var prev = Prev(node);
                //LoadNode(prev);
                prev.Next = -1;
                SaveNode(prev);
                // SaveNode(node);
                _tail = prevPosition;
            }
            else if (nextPosition != -1 && prevPosition == -1)
            {
                var next = Next(node);

                next.Prev = -1;
                SaveNode(next);
                _head = nextPosition;
            }
            else
            {
               _head = _tail = -1;
            }


            SaveMetaData();
        }
        public void Dispose()
        {
            _bw.Dispose();
            _br.Dispose();
            _stream.Dispose();
        }
    }
}

