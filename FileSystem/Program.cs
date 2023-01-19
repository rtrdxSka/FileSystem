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
        static FileStreamLinkedListNode<FileContent> TemporaryFolder = null;
        static void Main(string[] args)
        {
            File.Delete("storage.bin");
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

                while (true)
                {
                    UserInput(fsll, ref newNode);
                }
            
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
                            newNode.LocalPrev = prevNode.Position;
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
                            prevNode.LocalNext = newNode.Position;
                            prevNode.LocalHead = newNode.Position;
                            CurrentFolder = prevNode;
                            fsll.SaveNode(prevNode);
                            var dummyNode = new FileStreamLinkedListNode<FileContent>
                            {
                                Value = null,
                                Name = "..",
                                IsFolder = true,
                                LocalNext = -1,
                                LocalHead= -1,

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
                        if (currNode != null)
                        {
                            while (true)
                            {
                                if (currNode.Name == folderName && currNode.IsFolder)
                                {
                                    if (currNode.LocalHead == -1)
                                    {
                                        //CASE 1 - tuk sme na parviq element, koyto nqma sledvast
                                        if (currNode.LocalPrev == CurrentFolder.Position && currNode.LocalNext == -1)
                                        {

                                            CurrentFolder.LocalHead = -1;

                                            fsll.SaveNode(CurrentFolder);
                                            fsll.Remove(currNode);

                                        }//CASE 2- tuk sme na purviq element, koyto ima sledvasht
                                        else if (currNode.LocalPrev == CurrentFolder.Position)
                                        {
                                            var currNextNode = fsll.LoadNodeByPositon(currNode.LocalNext);

                                            CurrentFolder.LocalHead = currNextNode.Position;
                                            currNextNode.LocalPrev = CurrentFolder.Position;

                                            fsll.SaveNode(currNextNode);
                                            fsll.SaveNode(CurrentFolder);
                                            fsll.Remove(currNode);
                                        }//CASE 3 - tuks sme na posledniq element
                                        else if (currNode.LocalNext == -1)
                                        {
                                            var currPrevNode = fsll.LoadNodeByPositon(currNode.LocalPrev);
                                            currPrevNode.LocalNext = -1;
                                            fsll.Remove(currNode);
                                            fsll.SaveNode(currPrevNode);

                                        }
                                        else
                                        {
                                            var currNextNode = fsll.LoadNodeByPositon(currNode.LocalNext);
                                            var currPrevNode = fsll.LoadNodeByPositon(currNode.LocalPrev);

                                            currPrevNode.LocalNext = currNode.LocalNext;
                                            currNextNode.LocalPrev = currNode.LocalPrev;

                                            fsll.SaveNode(currPrevNode);
                                            fsll.SaveNode(currNextNode);
                                            fsll.Remove(currNode);

                                        }

                                    }
                                    else
                                    {
                                        Console.WriteLine("The selected folder is not empty!");
                                    }

                                    break;
                                }
                                if (currNode.LocalNext != -1)
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
                        else
                        {
                            Console.WriteLine("Current Directory is Empty!");
                        }
                    }
                    // Iztrivane na prazna direktoriq
                    break;
                case "ls":
                    {
                        
                       

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
                                Console.WriteLine(CurrentFolder.Name);
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
                        var parentFolder = fsll.LoadNodeByPositon(dummy.LocalPrev);
                        CurrentFolder = parentFolder;
                        Console.WriteLine(CurrentFolder.Name);
                    }
                    break;
                case "cp": // Kopirane na fail
                    break;
                case "rm": // Iztrivane na fail
                    break;
                case "cat": // Izvejdane na sadarjanie na fail na ekrana
                    break;
                case "write":
                    {
                    }// Sazdavane na prazen fail 
                    break;
                case "write+append":
                    break;
                case "import":
                    break;
                case "export":
                    break;
                default:
                    Console.WriteLine("Invalid command!");
                    break;
            }
        
        }

    }
}
