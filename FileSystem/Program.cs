using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FileSystem
{

    internal class Program
    {
        static FileStreamLinkedListNode<FileContent> CurrentFolder = null;
        static FileContent FileCreator = new FileContent();
        

        static void Main(string[] args)
        {
            /*File.Delete("storage.bin");*/
            var _new = File.Exists("storage.bin");
            if (!_new)
            {
                using (var fsll = new FileStreamLinkedList<FileContent>("storage.bin"))
                {
                    /*var fileContent = new FileContent() { Content = new byte[] { 12, 31, 45, 65, 123, 77, 9 } };*/
                    var newNode = new FileStreamLinkedListNode<FileContent>();
                    newNode.Name = "ROOT";
                    newNode.IsFolder = true;
                    newNode.LocalHead = -1;
                    newNode.LocalNext = -1;
                    newNode.LocalPrev = -1;
                    fsll.Insert(null, newNode);
                    CurrentFolder = newNode;
                    Console.WriteLine("Enter size of your FileSystem");
                    CheckSize(fsll);
                    while (true)
                    {
                        UserInput(fsll, ref newNode);
                    }

                }
            }else
            {
                using (var fsll = new FileStreamLinkedList<FileContent>("storage.bin"))
                {
                    /*var fileContent = new FileContent() { Content = new byte[] { 12, 31, 45, 65, 123, 77, 9 } };*/
                    CurrentFolder = fsll.ContainerExist();
                    var newNode = CurrentFolder;
                    
                    while (true)
                    {
                        UserInput(fsll, ref newNode);
                    }

                }
            }

        
    }
        
        public static byte[] ToBytes(string input)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            return bytes;
        }
        public static long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
                if (drive.IsReady && drive.Name == driveName)
                    return drive.TotalFreeSpace;

            return -1;
        }
        public static long GBtoByte(float gb)
        {
            return (long)Math.Floor(gb * 1073741824);
        }
        public static long BytetoGB(long bytes)
        {
            return bytes / 1073741824;
        }
        public static void CheckSize (FileStreamLinkedList<FileContent> fsll)
        {
            long freeSpace = GetTotalFreeSpace("D:\\");

            Console.WriteLine($"Free space of your partition : {freeSpace} bytes, {BytetoGB(freeSpace)} GB");
            Console.WriteLine("Hello, please choose size of your file system in GB: ");
            float size = 0;
            bool isSizeValid = false;
            long fsSizeinBytes = 0;
            while (!isSizeValid)
            {
                if (float.TryParse(Console.ReadLine(), out size))
                {
                    fsSizeinBytes = GBtoByte(size);

                    if (fsSizeinBytes < freeSpace + GBtoByte(20))
                    { // 20 svobodni gb ako iska da e s maks size filesystema
                        isSizeValid = true;
                        fsll.Size = fsSizeinBytes;
                    }
                    else
                        Console.WriteLine("Nqmate tolkova prostranstvo.");
                }
                else
                    Console.WriteLine("vavedete chislo");
            }
        }
        private static void UserInput(FileStreamLinkedList<FileContent> fsll,ref FileStreamLinkedListNode<FileContent> newNode)
        {

            string command = Console.ReadLine();
            string[] arguments = utilityClass.mySplit(command, ' ');

            FileStreamLinkedListNode<FileContent> prevNode = newNode;
            //prevNode.Value.Content = null; // NQMASHE GO

            switch (arguments[0])
            {
                case "mkdir":
                    {

                        var folderName = arguments[1];

                        newNode = new FileStreamLinkedListNode<FileContent>
                        {
                            Value = null,
                            Name = folderName,
                            IsFolder = true,
                            LocalPrev = -1,
                            LocalNext = -1
                        };
                        if(fsll.GetStreamLength() + 500>=fsll.Size)
                        {
                            Console.WriteLine("Not enough space for folder");
                            break;
                        }
                        
                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        if(currNode != null)
                        {
                            
                            while (true)
                            {
                                if (currNode.LocalNext != -1)
                                    currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                                else
                                {
                                    newNode.LocalNext = -1;
                                    fsll.Insert(prevNode, newNode);
                                    currNode.LocalNext = newNode.Position;
                                    newNode.LocalPrev = currNode.Position;
                                    fsll.SaveNode(currNode);
                                    fsll.SaveNode(newNode);
                                    

                                    //tova sled tyk ne e sigyrno 

                                    /*TemporaryFolder = CurrentFolder;
                                    CurrentFolder = newNode;*/
                                    var dummyNode = new FileStreamLinkedListNode<FileContent>
                                    {
                                        Value = null,
                                        Name = "..",
                                        IsFolder = true,
                                        LocalNext = -1,
                                       

                                    };
                                    if (CurrentFolder.Name == "ROOT")
                                    {
                                        dummyNode.LocalPrev = CurrentFolder.Position;
                                        dummyNode.LocalHead = newNode.Position;
                                        fsll.Insert(newNode, dummyNode);
                                        newNode.LocalHead = dummyNode.Position;
                                        fsll.SaveNode(newNode);
                                       

                                    }
                                    else
                                    {
                                        dummyNode.LocalPrev = CurrentFolder.LocalHead;
                                        dummyNode.LocalHead = newNode.Position;
                                        fsll.Insert(newNode, dummyNode);
                                        newNode.LocalHead = dummyNode.Position;
                                        fsll.SaveNode(newNode);
                                    }
                                    /* CurrentFolder = TemporaryFolder;
                                     TemporaryFolder = null;*/
                                    newNode = dummyNode;
                                    break;
                                }    
                            }
                        }
                        else
                        {   
                            // da se opravi
                           /* var dummyNode = new FileStreamLinkedListNode<FileContent>
                            {
                            Value = null,
                            Name = "..",
                            IsFolder = true,
                            LocalPrev = newNode.LocalPrev,
                            LocalNext = -1

                            };*/
                            newNode.LocalPrev = CurrentFolder.Position;
                            newNode.LocalNext = -1;
                            fsll.Insert(prevNode, newNode);
                            /* if (prevNode.LocalHead == -1)
                             {  
                                     prevNode.LocalNext = newNode.Position;
                                     CurrentFolder = prevNode;
                                     fsll.SaveNode(prevNode);  
                             }*/

                            /* TemporaryFolder = CurrentFolder;
                             CurrentFolder = newNode;*/
                            CurrentFolder.LocalNext = newNode.Position;
                            CurrentFolder.LocalHead = newNode.Position;
                            /*CurrentFolder = prevNode;*/
                            fsll.SaveNode(CurrentFolder);
                            var dummyNode = new FileStreamLinkedListNode<FileContent>
                            {
                                Value = null,
                                Name = "..",
                                IsFolder = true,
                                LocalNext = -1,
                                

                            };
                            
                                dummyNode.LocalPrev = CurrentFolder.Position;
                                dummyNode.LocalHead = newNode.Position;
                                fsll.Insert(newNode, dummyNode);
                                newNode.LocalHead = dummyNode.Position;
                                fsll.SaveNode(newNode);
                                newNode = dummyNode;
                            /*CurrentFolder = TemporaryFolder;
                            TemporaryFolder = null;*/
                        }

                        // makeDirectory(pathSplited);
                        
                    }// mkdir FolderName
                    
                    break;
                case "rmdir":
                    {
                        var folderName = arguments[1];
                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                            //currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                        while (true)
                        {
                            if (currNode.Name == folderName && currNode.IsFolder)
                            {
                                
                                var dummy = fsll.LoadNodeByPositon(currNode.LocalHead);

                                if (dummy.LocalNext == -1)
                                {
                                    //Premahvame dummyto ot globalnata pamet i go razkachame

                                    dummy.LocalPrev = -1;
                                    dummy.LocalHead = -1;
                                    /*fsll.Remove(dummy);*/

                                    //CASE 1 - tuk sme na parviq element, koyto nqma sledvast
                                    if (currNode.LocalPrev == CurrentFolder.Position && currNode.LocalNext == -1)
                                    {

                                        CurrentFolder.LocalNext = -1;
                                        if (CurrentFolder.Name == "ROOT")
                                            CurrentFolder.LocalHead = -1;

                                        /*fsll.Remove(currNode);*/
                                        fsll.SaveNode(CurrentFolder);

                                    }//CASE 2- tuk sme na purviq element, koyto ima sledvasht
                                    else if (currNode.LocalPrev == CurrentFolder.Position)
                                    {
                                        var currNextNode = fsll.LoadNodeByPositon(currNode.LocalNext);

                                        CurrentFolder.LocalNext = currNextNode.Position;
                                        if (CurrentFolder.Name == "ROOT")
                                            CurrentFolder.LocalHead = currNextNode.Position;

                                        currNextNode.LocalPrev = CurrentFolder.Position;

                                        /*fsll.Remove(currNode);*/
                                        fsll.SaveNode(currNextNode);
                                        fsll.SaveNode(CurrentFolder);
                                    }//CASE 3 - tuks sme na posledniq element
                                    else if (currNode.LocalNext == -1)
                                    {
                                        var currPrevNode = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                        currPrevNode.LocalNext = -1;
                                        /*fsll.Remove(currNode);*/
                                        fsll.SaveNode(currPrevNode);

                                    }
                                    //CASE 4 - tuk sme po sredata
                                    else
                                    {
                                        var currNextNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                                        var currPrevNode = fsll.LoadNodeByPositon(currNode.LocalPrev);

                                        currPrevNode.LocalNext = currNode.LocalNext;
                                        currNextNode.LocalPrev = currNode.LocalPrev;

                                        fsll.SaveNode(currPrevNode);
                                        fsll.SaveNode(currNextNode);
                                        /*fsll.Remove(currNode);*/

                                    }

                                }
                                else
                                    Console.WriteLine("The selected folder is not empty!");

                                break;
                            }
                            else if (currNode.LocalNext != -1)
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }
                            else
                            {
                                Console.WriteLine("The selected folder was not found!");
                                break;
                            }
                        }
                    }
                    // Iztrivane na prazna direktoriq
                    break;
                case "ls":
                    {
                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        if(CurrentFolder.Name!="ROOT")
                            currNode = fsll.LoadNodeByPositon((long)currNode.LocalNext);

                        while (true)
                        {
                            if (currNode != null)
                            {
                                if (currNode.IsFolder)
                                {
                                    Console.WriteLine($"DIR   {currNode.Name}");
                                }
                                else
                                {
                                    Console.WriteLine($"      {currNode.Name}");
                                }
                                currNode = fsll.LoadNodeByPositon((long)currNode.LocalNext);
                            }
                            else
                            {
                                break;
                            }
                        }

                        // Izvejdane na sadarjanieto na direktoriq
                    }
                    break;
                case "cd": // Promqna na tekushta direktoriq
                    {
                        var folderName = arguments[1];
                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        while (true)
                        {
                            if(currNode.Name == folderName && currNode.IsFolder)
                            {
                                CurrentFolder = currNode;
                                Console.WriteLine($"Current DIR    {CurrentFolder.Name}");
                                break;
                            }
                            else if(currNode.LocalNext!=-1)
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }else
                            {
                                Console.WriteLine("This folder does not exist");
                                break;
                            }
                        }

                    }
                    break;
                case "cd..":
                    {
                        var dummy = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        var parentDummy = fsll.LoadNodeByPositon(dummy.LocalPrev);
                        if(parentDummy.Name == "ROOT")
                        {
                            CurrentFolder = parentDummy;
                            Console.WriteLine($"Current DIR    {CurrentFolder.Name}");
                        }else
                        {
                            var dummyParent = fsll.LoadNodeByPositon(parentDummy.LocalHead);
                            CurrentFolder = dummyParent;
                            Console.WriteLine($"Current DIR    {CurrentFolder.Name}");
                        }
                      
                       
                    }
                    break;
                case "cp":
                    {
                        var fileName = arguments[1];
                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        while(true)
                        {
                            if(currNode.Name == fileName && !currNode.IsFolder)
                            {
                                newNode = new FileStreamLinkedListNode<FileContent>
                                {
                                    Value = null,
                                    Name = currNode.Name,
                                    IsFolder = false,
                                    LocalPrev = -1,
                                    LocalNext = -1,
                                    LocalHead = currNode.Position,
                                    
                                };
                                newNode.Name += ".CP";
                                FileCreator.Content = ToBytes("");
                                newNode.Value = FileCreator;
                                if (currNode.LocalNext == -1)
                                {
                                    fsll.Insert(currNode, newNode);
                                    currNode.LocalNext = newNode.Position;
                                    newNode.LocalPrev = currNode.Position;
                                    fsll.SaveNode(currNode);
                                    fsll.SaveNode(newNode);
                                    break;
                                }
                                else
                                {
                                    currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                                    while (true)
                                    {
                                        if(currNode.LocalNext == -1)
                                        {
                                            fsll.Insert(currNode, newNode);
                                            currNode.LocalNext = newNode.Position;
                                            newNode.LocalPrev = currNode.Position;
                                            fsll.SaveNode(currNode);
                                            fsll.SaveNode(newNode);
                                            break;
                                        }
                                        else
                                        {
                                            currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                                        }

                                    }
                                }
                                break;
                            }
                            else if (currNode.LocalNext != -1)
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }
                            else
                            {
                                Console.WriteLine("The selected file was not found!");
                                break;
                            }
                        }

                        

                    }// Kopirane na fail
                    break;
                case "rm":
                    {
                        var fileName = arguments[1];
                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                         while(true)
                        {
                            if (currNode.Name == fileName && !currNode.IsFolder)
                            {
                                //tuk sme na parviq element, koyto nqma sledvast
                                if (currNode.LocalPrev == CurrentFolder.Position && currNode.LocalNext == -1)
                                {

                                    CurrentFolder.LocalNext = -1;
                                    if (CurrentFolder.Name == "ROOT")
                                        CurrentFolder.LocalHead = -1;

                                   /* fsll.Remove(currNode);*/
                                    fsll.SaveNode(CurrentFolder);

                                }//CASE 2- tuk sme na purviq element, koyto ima sledvasht
                                else if (currNode.LocalPrev == CurrentFolder.Position)
                                {
                                    var currNextNode = fsll.LoadNodeByPositon(currNode.LocalNext);

                                    CurrentFolder.LocalNext = currNextNode.Position;
                                    if (CurrentFolder.Name == "ROOT")
                                        CurrentFolder.LocalHead = currNextNode.Position;

                                    currNextNode.LocalPrev = CurrentFolder.Position;

                                    /*fsll.Remove(currNode);*/
                                    fsll.SaveNode(currNextNode);
                                    fsll.SaveNode(CurrentFolder);
                                }//CASE 3 - tuks sme na posledniq element
                                else if (currNode.LocalNext == -1)
                                {
                                    var currPrevNode = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                    currPrevNode.LocalNext = -1;
                                    /*fsll.Remove(currNode);*/
                                    fsll.SaveNode(currPrevNode);

                                }
                                //CASE 4 - tuk sme po sredata
                                else
                                {
                                    var currNextNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                                    var currPrevNode = fsll.LoadNodeByPositon(currNode.LocalPrev);

                                    currPrevNode.LocalNext = currNode.LocalNext;
                                    currNextNode.LocalPrev = currNode.LocalPrev;

                                    fsll.SaveNode(currPrevNode);
                                    fsll.SaveNode(currNextNode);
                                    /*fsll.Remove(currNode);*/

                                }
                                break;
                            }
                            else if (currNode.LocalNext != -1)
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }
                            else
                            {
                                Console.WriteLine("The selected folder was not found!");
                                break;
                            }
                        }
                    }// Iztrivane na fail
                    break;
                case "cat": // Izvejdane na sadarjanie na fail na ekrana
                    break;
                case "write":
                    {
                        var fileName = arguments[1];
                        var fileContent = "";
                        for(int i=1;i<arguments[2].Length-1;i++)
                        {
                            fileContent += arguments[2][i];
                        }

                        if (fsll.GetStreamLength() + fileContent.Length>= fsll.Size)
                        {
                            Console.WriteLine("Not enough space for file");
                            break;
                        }

                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        if (CurrentFolder.Name != "ROOT")
                            currNode = fsll.LoadNodeByPositon(currNode.LocalNext);

                        bool found = false;
                        while (true)
                        {
                            if (currNode != null)
                            {
                                if (!currNode.IsFolder && currNode.Name==fileName)
                                {
                                    found=true;
                                    break;
                                }
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (found == true)
                        {
                            newNode = new FileStreamLinkedListNode<FileContent>
                            {
                                Name = fileName,
                                IsFolder = false,
                                LocalHead=-1,
                                LocalPrev = -1,
                                LocalNext = -1
                            };


                            //Posleden element 
                            if (currNode.LocalNext == -1)
                            {
                                //trabvaa da zapishem valueto
                                newNode.LocalPrev = currNode.LocalPrev;
                                FileCreator.Content = ToBytes(fileContent);
                                newNode.Value = FileCreator;
                                fsll.Insert(prevNode, newNode);

                                var currNodePrev = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                currNodePrev.LocalNext = newNode.Position;
                                if (currNodePrev.Name == "ROOT")
                                {
                                    currNodePrev.LocalHead = newNode.Position;
                                }
                                currNodePrev.LocalNext = newNode.Position;
                                fsll.SaveNode(currNodePrev);
                                if (currNodePrev.Name == "ROOT")
                                {
                                    CurrentFolder = currNodePrev;
                                }
                                fsll.SaveNode(currNodePrev);
                                fsll.Remove(currNode);
                                prevNode = newNode;
                            }///Sreden element
                            else if (currNode.LocalNext != -1 && currNode.LocalPrev != -1)
                            {
                                newNode.LocalPrev = currNode.LocalPrev;
                                newNode.LocalNext = currNode.LocalNext;
                                FileCreator.Content = ToBytes(fileContent);
                                newNode.Value = FileCreator;
                                fsll.Insert(prevNode, newNode);
                                var currNodePrev = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                currNodePrev.LocalNext = newNode.Position;
                                var currNodeNext = fsll.LoadNodeByPositon(currNode.LocalNext);
                                currNodeNext.LocalPrev = newNode.Position;
                                if (currNodePrev.Name == "ROOT")
                                {
                                    currNodePrev.LocalHead = newNode.Position;
                                }
                                currNodePrev.LocalNext = newNode.Position;
                                fsll.SaveNode(currNodePrev);
                                if (currNodePrev.Name == "ROOT")
                                {
                                    CurrentFolder = currNodePrev;
                                }
                                fsll.SaveNode(currNodePrev);
                                fsll.SaveNode(currNodeNext);
                                fsll.Remove(currNode);
                                prevNode=newNode;
                            }

                        }
                        else
                        {
                            newNode = new FileStreamLinkedListNode<FileContent>
                            {
                                Name = fileName,
                                IsFolder = false,
                                LocalHead = -1,
                                LocalPrev = -1,
                                LocalNext = -1
                            };

                            /*!!!!*/ //da se testva v drygi papki osven root
                            if (CurrentFolder.LocalHead == -1)
                            {
                                currNode = CurrentFolder;
                                FileCreator.Content=ToBytes(fileContent);
                                newNode.Value = FileCreator;
                                newNode.LocalPrev=CurrentFolder.Position;
                                newNode.LocalNext = -1;
                                fsll.Insert(prevNode, newNode);
                                CurrentFolder.LocalHead = newNode.Position;
                                CurrentFolder.LocalNext = newNode.Position;
                                fsll.SaveNode(CurrentFolder);

                                prevNode = newNode;
                            }
                            else
                            {
                                currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                                while(currNode.LocalNext!= -1)
                                {
                                    currNode= fsll.LoadNodeByPositon(currNode.LocalNext);
                                }
                                FileCreator.Content = ToBytes(fileContent);
                                newNode.Value = FileCreator;
                                newNode.LocalPrev = currNode.Position;
                                fsll.Insert(prevNode, newNode);
                                currNode.LocalNext = newNode.Position;
                                fsll.SaveNode(currNode);

                                prevNode = newNode;
                            }

                        }
                        



                    }// Sazdavane na prazen fail 
                    break;
                case "write+append":
                    {
                        var fileName = arguments[1];
                        var fileContent = "";
                        for (int i = 1; i < arguments[2].Length - 1; i++)
                        {
                            fileContent += arguments[2][i];
                        }

                        if (fsll.GetStreamLength() + fileContent.Length >= fsll.Size)
                        {
                            Console.WriteLine("Not enough space for file");
                            break;
                        }

                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        if (CurrentFolder.Name != "ROOT")
                            currNode = fsll.LoadNodeByPositon(currNode.LocalNext);

                        bool found = false;
                        while (true)
                        {
                            if (currNode != null)
                            {
                                if (!currNode.IsFolder && currNode.Name == fileName)
                                {
                                    found = true;
                                    break;
                                }
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (found == true)
                        {
                            newNode = new FileStreamLinkedListNode<FileContent>
                            {
                                Name = fileName,
                                IsFolder = false,
                                LocalPrev = -1,
                                LocalNext = -1
                            };
                            FileCreator.Content = ToBytes(fileContent);
                            newNode.Value = FileCreator;
                            fsll.Insert(prevNode, newNode);
                            if (currNode.LocalHead == -1)
                            {
                                currNode.LocalHead = newNode.Position;
                            }
                            else
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalHead);
                                while (currNode.LocalHead != -1)
                                {
                                    currNode = fsll.LoadNodeByPositon(currNode.LocalHead);
                                }
                                currNode.LocalHead = newNode.Position;
                            }

                            prevNode=newNode;
                        }
                    }
                    break;
                case "import":
                    {
                        string input = arguments[1];
                        string destination = arguments[2]; 
                        var pathhArray = utilityClass.mySplit(arguments[1],'\\');
                        var fileName=pathhArray[pathhArray.Length-1];
                        newNode = new FileStreamLinkedListNode<FileContent>
                        {
                            Name = destination,
                            IsFolder = false,
                            LocalHead = -1,
                            LocalPrev = -1,
                            LocalNext = -1
                        };

                       /* fsll.ImportInsert(prevNode, newNode, input);*/

                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        if (CurrentFolder.Name != "ROOT")
                            currNode = fsll.LoadNodeByPositon(currNode.LocalNext);

                        bool found = false;
                        while(true)
                        {
                            if(currNode.Name==destination && !currNode.IsFolder)
                            {
                                found = true;
                                break;
                            }else if(currNode.LocalNext!=-1)
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }else
                            {
                                break;
                            }
                            
                        }
                        if (found == true)
                        {
                            //CASE 1 Файлът е последен
                            if (currNode.LocalNext == -1)
                            {
                                newNode.LocalPrev = currNode.LocalPrev;
                                fsll.ImportInsert(prevNode, newNode, input);
                                var currNodePrev = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                if (currNodePrev.Name == "ROOT")
                                {
                                    currNodePrev.LocalHead = newNode.Position;
                                }
                                currNodePrev.LocalNext = newNode.Position;
                                fsll.SaveNode(currNodePrev);
                                if(currNodePrev.Name == "ROOT")
                                {
                                    CurrentFolder = currNodePrev;
                                }
                                fsll.Remove(currNode);
                                prevNode = newNode;
                            }
                            //CASE 2 Файлът е среден
                            else
                            {
                                newNode.LocalPrev = currNode.LocalPrev;
                                newNode.LocalNext = currNode.LocalNext;
                                fsll.ImportInsert(prevNode, newNode, input);
                                var currNodePrev = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                currNodePrev.LocalNext = newNode.Position;
                                if(currNodePrev.Name == "ROOT")
                                {
                                    currNodePrev.LocalHead = newNode.Position;
                                }
                                var currNodeNext = fsll.LoadNodeByPositon(currNode.LocalNext);
                                currNodeNext.LocalPrev = newNode.Position;
                                fsll.SaveNode(currNodePrev);
                                fsll.SaveNode(currNodeNext);
                                if (currNodePrev.Name == "ROOT")
                                {
                                    CurrentFolder = currNodePrev;
                                }
                                fsll.Remove(currNode);
                            }
                            prevNode = newNode;
                           

                        }
                        else
                        {
                            Console.WriteLine("Destination not valid!");
                        }
                        

                    }
                    break;
                case "import+append":
                    {
                        string input = arguments[1];
                        string destination = arguments[2];
                        var pathhArray = utilityClass.mySplit(arguments[1], '\\');
                        var fileName = pathhArray[pathhArray.Length - 2];
                        var fileContent = "";
                        for (int i = 1; i < arguments[3].Length - 1; i++)
                        {
                            fileContent += arguments[3][i];
                        }
                        newNode = new FileStreamLinkedListNode<FileContent>
                        {
                            Name = destination,
                            IsFolder = false,
                            LocalHead = -1,
                            LocalPrev = -1,
                            LocalNext = -1
                        };

                        /* fsll.ImportInsert(prevNode, newNode, input);*/

                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);
                        if (CurrentFolder.Name != "ROOT")
                            currNode = fsll.LoadNodeByPositon(currNode.LocalNext);

                        bool found = false;
                        while (true)
                        {
                            if (currNode.Name == destination && !currNode.IsFolder)
                            {
                                found = true;
                                break;
                            }
                            else if (currNode.LocalNext != -1)
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }
                            else
                            {
                                break;
                            }

                        }
                        if (found == true)
                        {
                            //CASE 1 Файлът е последен
                            if (currNode.LocalNext == -1)
                            {
                                newNode.LocalPrev = currNode.LocalPrev;
                                byte [] content = ToBytes(fileContent);
                                fsll.ImportInsertAppend(prevNode, newNode, input,content);
                                var currNodePrev = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                if (currNodePrev.Name == "ROOT")
                                {
                                    currNodePrev.LocalHead = newNode.Position;
                                }
                                currNodePrev.LocalNext = newNode.Position;
                                fsll.SaveNode(currNodePrev);
                                if (currNodePrev.Name == "ROOT")
                                {
                                    CurrentFolder = currNodePrev;
                                }
                                fsll.Remove(currNode);
                                prevNode = newNode;
                            }
                            //CASE 2 Файлът е среден
                            else
                            {
                                newNode.LocalPrev = currNode.LocalPrev;
                                newNode.LocalNext = currNode.LocalNext;
                                byte[] content = ToBytes(fileContent);
                                fsll.ImportInsertAppend(prevNode, newNode, input, content);
                                var currNodePrev = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                currNodePrev.LocalNext = newNode.Position;
                                if (currNodePrev.Name == "ROOT")
                                {
                                    currNodePrev.LocalHead = newNode.Position;
                                }
                                var currNodeNext = fsll.LoadNodeByPositon(currNode.LocalNext);
                                currNodeNext.LocalPrev = newNode.Position;
                                fsll.SaveNode(currNodePrev);
                                fsll.SaveNode(currNodeNext);
                                if (currNodePrev.Name == "ROOT")
                                {
                                    CurrentFolder = currNodePrev;
                                }
                                fsll.Remove(currNode);
                            }
                            prevNode = newNode;


                        }
                        else
                        {
                            Console.WriteLine("Destination not valid!");
                        }
                    }
                    break;
                case "export":
                    {
                        //export ROOT\test2 D:\export\test2.txt
                        string pathInput = arguments[1];
                        var path = utilityClass.mySplit(pathInput, '\\');
                        var currNode = fsll.LoadNodeByPositon(CurrentFolder.LocalHead);

                        int i = 1;
                        while (true)
                        {
                            if (currNode.Name == path[i] && currNode.IsFolder && i!=path.Length-1)
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalHead);
                                i++;
                            }
                            else if(currNode.Name == path[i] && !currNode.IsFolder && i == path.Length-1)
                            {
                                fsll.LoadNode(currNode);
                                fsll.LoadExport(currNode, arguments[2]);
                                break;
                            }
                            else if(currNode.LocalNext!=-1)
                            {
                                currNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                            }
                            else
                            {
                                Console.WriteLine("The given path is invalid!"); 
                                break;    
                            }
                        }
                        
                    }
                    break;
                
                default:
                    Console.WriteLine("Invalid command!");
                    break;
            }
        
        }

    }
}
