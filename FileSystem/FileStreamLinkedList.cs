using System;
using System.IO;

namespace FileSystem
{
    public class FileStreamLinkedListNode<FileContent>
    {
        public long Position;
        public long Prev;
        public long Next;
        public long LocalHead;
        public long LocalPrev;
        public long LocalNext;
        public bool IsFolder;
        public string Name;

        public FileContent Value;
    }
    public class FileStreamLinkedList<FileContent> : IDisposable
    /*where T : IStreamable, new()*/
    where FileContent : IStreamable, new()
    {

        public Stream _stream;
        private BinaryWriter _bw;
        private BinaryReader _br;
        private long _head;
        private long _tail;
        public long Size;

        public long GetStreamLength()
        {
            return _stream.Length;
        }
        public FileStreamLinkedListNode<FileContent> Head
        {
            get
            {
                var node1 = new FileStreamLinkedListNode<FileContent>() { Position = _head };

                LoadNode(node1);

                return node1;

            }

        }
        public FileStreamLinkedListNode<FileContent> Tail
        {
            get
            {
                var node2 = new FileStreamLinkedListNode<FileContent>() { Position = _tail };

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
                Size = 100;
                _head = -1;
                _tail = -1;

                SaveMetaData();
            }
            /*else
            {
                
                LoadMetaData();
            }*/
        }
        /* public FileStreamLinkedListNode<FileContent> LoadContainer()
         {
             var node = ContainerExist();
             return node;
         }*/
        //testovo

        void SaveMetaData()
        {
            _stream.Position = 0;
            _bw.Write(Size);
            _bw.Write(_head);
            _bw.Write(_tail);
        }
        void LoadMetaData()
        {
            _stream.Position = 0;
            Size = _br.ReadInt64();
            _head = _br.ReadInt64();
            _tail = _br.ReadInt64();
        }

        //testovo
        public FileStreamLinkedListNode<FileContent> ContainerExist()
        {

            LoadMetaData();
            var node = new FileStreamLinkedListNode<FileContent>();
            node.Position = _br.ReadInt64();
            node.Prev = _br.ReadInt64();
            node.Next = _br.ReadInt64();
            node.LocalHead = _br.ReadInt64();
            node.LocalPrev = _br.ReadInt64();
            node.LocalNext = _br.ReadInt64();
            node.IsFolder = _br.ReadBoolean();
            node.Name = _br.ReadString();
            return node;

        }


        void SaveContentNode(FileStreamLinkedListNode<FileContent> node)
        {
            if (!node.IsFolder)
                node.Value.SaveToStream(_stream);

        }
        public void SaveNode(FileStreamLinkedListNode<FileContent> node)
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

        public void WriteAppend(FileStreamLinkedListNode<FileContent> nodeToCopyFrom, FileStreamLinkedListNode<FileContent> nodeNew, byte[] toAddContent)
        {
            LoadNode(nodeToCopyFrom);
            FileContent FileCreator = new FileContent();
            long valuePosition = _stream.Position;



            long size = 0;



            var nextNode = LoadNodeByPositon(nodeToCopyFrom.Next);
            size = nextNode.Position - valuePosition;




            int counter = 0;

            var buffCounter = Math.Ceiling(((double)size / 10000));
            _stream.Position = valuePosition;
            if (size < 5000)
            {
                byte[] buffer = FileCreator.LoadFormStream(_stream, size);
                SaveContentNode(nodeNew);
                FileCreator.Content = toAddContent;
                nodeNew.Value = FileCreator;
                SaveContentNode(nodeNew);


            }
            else
            {
                if (buffCounter < 1)
                    buffCounter = 1;


                while (counter < buffCounter)
                {
                    if (counter == buffCounter - 1)
                    {
                        long bufferSize = size - (10000 * ((long)buffCounter - 1));
                        byte[] buffer = FileCreator.LoadFormStream(_stream, bufferSize);
                        SaveContentNode(nodeNew);
                        break;


                    }
                    else
                    {
                        byte[] buffer = FileCreator.LoadFormStream(_stream, 10000);
                        SaveContentNode(nodeNew);
                        counter++;

                    }

                }
            }



        }
        public void ShowContent(FileStreamLinkedListNode<FileContent> node)
        {
            LoadNode(node);
            FileContent FileCreator = new FileContent();
            long valuePosition = _stream.Position;
            var nextNode = LoadNodeByPositon(node.Next);


            long size = 0;


            if (node.Next == 1)
            {
                size = _stream.Length - valuePosition;
            }
            else
            {
                size = nextNode.Prev - valuePosition;
            }



            int counter = 0;

            var buffCounter = Math.Ceiling(((double)size / 10000));
            _stream.Position = valuePosition;
            if (size < 5000)
            {
                byte[] buffer = FileCreator.LoadFormStream(_stream, size);
                //var content = System.Text.Encoding.Default.GetString(buffer);
                var str = System.Text.Encoding.Default.GetString(buffer);
                Console.Write(str);

            }
            else
            {
                if (buffCounter < 1)
                    buffCounter = 1;


                while (counter < buffCounter)
                {
                    if (counter == buffCounter - 1)
                    {
                        long bufferSize = size - (10000 * ((long)buffCounter - 1));
                        byte[] buffer = FileCreator.LoadFormStream(_stream, bufferSize);
                        var str = System.Text.Encoding.Default.GetString(buffer);
                        Console.Write(str);
                        break;


                    }
                    else
                    {
                        byte[] buffer = FileCreator.LoadFormStream(_stream, 10000);
                        var str = System.Text.Encoding.Default.GetString(buffer);
                        Console.Write(str);
                        counter++;

                    }



                }
                
            }
        }
        public void LoadExport(FileStreamLinkedListNode<FileContent> node, string pathString)
        {
            LoadNode(node);
            FileContent FileCreator = new FileContent();
            long valuePosition = _stream.Position;
            var nextNode = LoadNodeByPositon(node.Next);


            long size = 0;


            if (node.Next == 1)
            {
                size = _stream.Length - valuePosition;
            }
            else
            {
                size = nextNode.Prev - valuePosition;
            }



            int counter = 0;

            var buffCounter = Math.Ceiling(((double)size / 10000));
            _stream.Position = valuePosition;
            if (size < 5000)
            {
                byte[] buffer = FileCreator.LoadFormStream(_stream, size);
                //var content = System.Text.Encoding.Default.GetString(buffer);
                if (!System.IO.File.Exists($@"{pathString}"))
                {
                    using (System.IO.FileStream fs = System.IO.File.Create($@"{pathString}"))
                    {
                        fs.Write(buffer, 0, (int)size);
                    }
                }
                else
                {
                    Console.WriteLine("File  already exists.");
                    return;
                }

            }
            else
            {
                if (buffCounter < 1)
                    buffCounter = 1;

                if (!System.IO.File.Exists($@"{pathString}"))
                {
                    using (System.IO.FileStream fs = System.IO.File.Create($@"{pathString}"))
                    {
                        while (counter < buffCounter)
                        {
                            if (counter == buffCounter - 1)
                            {
                                long bufferSize = size - (10000 * ((long)buffCounter - 1));
                                byte[] buffer = FileCreator.LoadFormStream(_stream, bufferSize);
                                fs.Write(buffer, 0, (int)bufferSize);
                                break;


                            }
                            else
                            {
                                byte[] buffer = FileCreator.LoadFormStream(_stream, 10000);
                                fs.Write(buffer, 0, 10000);
                                counter++;

                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("File already exists.");
                    return;
                }

            }
        }
        void LoadContentNode(FileStreamLinkedListNode<FileContent> node)
        {

            LoadNode(node);
            node.Value = new FileContent();
            if (node.Next == 1)
                node.Value.LoadFromStream(_stream, _stream.Length - _stream.Position);
            else
                node.Value.LoadFromStream(_stream, node.Next - _stream.Position);
        }
        public void LoadNode(FileStreamLinkedListNode<FileContent> node)
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

            if (position == 1)
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


            return node;

        }

        public void ImportInsertAppend(FileStreamLinkedListNode<FileContent> prev, FileStreamLinkedListNode<FileContent> node, string filePath, byte[] content)
        {
            node.Position = _stream.Length;
            node.Prev = _tail;
            node.Next = 1;
            SaveNode(node);

            using (var ofs = new FileStream($@"{filePath}", FileMode.Open, FileAccess.Read))
            {
                FileContent FileCreator = new FileContent();

                int counter = 0;

                var buffCounter = Math.Ceiling(((double)ofs.Length / 10000));
                if (ofs.Length < 5000)
                {
                    byte[] buffer = new byte[ofs.Length];
                    ofs.Read(buffer, 0, buffer.Length);
                    FileCreator.Content = buffer;
                    node.Value = FileCreator;
                    SaveContentNode(node);
                    FileCreator.Content = content;
                    node.Value = FileCreator;
                    SaveContentNode(node);
                }
                else
                {
                    if (buffCounter < 1)
                        buffCounter = 1;

                    while (counter < buffCounter)
                    {
                        if (counter == buffCounter - 1)
                        {
                            byte[] buffer = new byte[ofs.Length - (10000 * ((long)buffCounter - 1))];
                            ofs.Read(buffer, 0, buffer.Length);
                            FileCreator.Content = buffer;
                            node.Value = FileCreator;
                            SaveContentNode(node);
                            counter++;
                        }
                        else
                        {
                            byte[] buffer = new byte[10000];
                            ofs.Read(buffer, 0, buffer.Length);
                            FileCreator.Content = buffer;
                            node.Value = FileCreator;
                            SaveContentNode(node);
                            counter++;
                        }

                    }
                    FileCreator.Content = content;
                    node.Value = FileCreator;
                    SaveContentNode(node);
                }
            }


            prev.Next = node.Position;
            SaveNode(prev);

            _tail = node.Position;
            SaveMetaData();
        }
        public void ImportInsert(FileStreamLinkedListNode<FileContent> prev, FileStreamLinkedListNode<FileContent> node, string filePath)
        {

            node.Position = _stream.Length;
            node.Prev = _tail;
            node.Next = 1;
            SaveNode(node);

            using (var ofs = new FileStream($@"{filePath}", FileMode.Open, FileAccess.Read))
            {
                FileContent FileCreator = new FileContent();

                int counter = 0;

                var buffCounter = Math.Ceiling(((double)ofs.Length / 10000));
                if (ofs.Length < 5000)
                {
                    byte[] buffer = new byte[ofs.Length];
                    ofs.Read(buffer, 0, buffer.Length);
                    FileCreator.Content = buffer;
                    node.Value = FileCreator;
                    SaveContentNode(node);
                }
                else
                {
                    if (buffCounter < 1)
                        buffCounter = 1;

                    while (counter < buffCounter)
                    {
                        if (counter == buffCounter - 1)
                        {
                            byte[] buffer = new byte[ofs.Length - (10000 * ((long)buffCounter - 1))];
                            ofs.Read(buffer, 0, buffer.Length);
                            FileCreator.Content = buffer;
                            node.Value = FileCreator;
                            SaveContentNode(node);
                            counter++;
                        }
                        else
                        {
                            byte[] buffer = new byte[10000];
                            ofs.Read(buffer, 0, buffer.Length);
                            FileCreator.Content = buffer;
                            node.Value = FileCreator;
                            SaveContentNode(node);
                            counter++;
                        }
                    }
                }
            }


            prev.Next = node.Position;
            SaveNode(prev);

            _tail = node.Position;
            SaveMetaData();

        }
        public void Insert(FileStreamLinkedListNode<FileContent> prev, FileStreamLinkedListNode<FileContent> node)
        {
            node.Position = _stream.Length;
            if (prev == null && _head == -1) // situaciq nqma nishto v spisaka
            {

                node.Prev = -1;
                node.Next = 1;
                SaveNode(node);
                SaveContentNode(node);
                long position = node.Position;
                _head = position;
                _tail = position;

                SaveMetaData();
            }
            else if (prev != null && prev.Next == 1) // posledniq element, koito se dobavq
            {
                node.Prev = _tail;
                node.Next = 1;
                SaveNode(node);
                SaveContentNode(node);

                prev.Next = node.Position;
                SaveNode(prev);

                _tail = node.Position;
                SaveMetaData();


            }
            else if (prev != null && prev.Next != 1) // dobavqne element v sredata
            {
                node.Prev = prev.Position;
                node.Next = prev.Next;
                SaveNode(node);
                SaveContentNode(node);
                prev.Next = node.Position;
                SaveNode(prev);

                var next = new FileStreamLinkedListNode<FileContent>() { Position = node.Next };
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
                var next = new FileStreamLinkedListNode<FileContent>() { Position = _head };
                LoadNode(next);
                next.Prev = node.Position;
                SaveNode(next);

                _head = node.Position;
                SaveMetaData();


            }
        }

        public FileStreamLinkedListNode<FileContent> NextInside(FileStreamLinkedListNode<FileContent> node)
        {
            var currNode = Tail;
            while (currNode.Prev == node.Position)
            {
                currNode = LoadNodeByPositon(currNode.Prev);
            }


            return currNode;
        }
        public FileStreamLinkedListNode<FileContent> Next(FileStreamLinkedListNode<FileContent> prev)
        {
            var node = new FileStreamLinkedListNode<FileContent>() { Position = prev.Next };

            LoadNode(node);

            return node;
        }
        public FileStreamLinkedListNode<FileContent> Prev(FileStreamLinkedListNode<FileContent> node)
        {
            var prev = new FileStreamLinkedListNode<FileContent>() { Position = node.Prev };

            LoadNode(prev);
            return prev;
        }
        public void Remove(FileStreamLinkedListNode<FileContent> node)
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

