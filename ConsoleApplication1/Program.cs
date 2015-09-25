using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            MemberList ml = new MemberList();
            ml.FetchData("members.bin");


            while (true)
            {
                Console.Write("C to create, R to read, D to delete: ");

                char choice = Console.ReadKey().KeyChar;
                Console.WriteLine("");

                if (choice == 'c')
                {

                    Console.Write("Name: ");
                    string name = Console.ReadLine();
                    Console.Write("SSN: ");
                    string ssn = Console.ReadLine();
                    var m = new Member(name, ssn);

                    Console.WriteLine("Enter boat names (enter no name to finalize):");

                    string boatname = "";
                    boatname = Console.ReadLine();
                    while (boatname.Trim() != "")
                    {
                        Boat b = new Boat(boatname);
                        m.AddBoat(b);
                        boatname = Console.ReadLine();
                    }

                    ml.Add(m);
                    Console.Clear();

                }
                else if (choice == 'r')
                {
                    Console.Clear();
                    ml.List();
                }
                else if (choice == 'd')
                {
                    Console.Clear();
                    Console.WriteLine("Select member to kill:");
                    ml.List();
                    int id = 0;
                    if (int.TryParse(Console.ReadLine(),out id))
                    {
                        if (id<ml.members.Count && id >= 0)
                        {
                            ml.members.RemoveAt(id);
                            Console.Clear();
                            Console.WriteLine("He's dead, Jim");
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine("Not in there, BRAH.");
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("Not a number bro.");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
                else
                {
                    ml.SaveToFile("members.bin");
                    break;
                }
            }
        }
    }

    [Serializable]
    class Member
    {
        public string Name;
        public string SSN;
        public string uid;

        /*
        private Boat[] _boats
        {
            get
            {
                return boats.ToArray();
            }
            set
            {
                boats.Clear();
                boats.AddRange(value);
            }
        }*/

        private List<Boat> boats = new List<Boat>();

        
        public Member(string name, string ssn)
        {
            Name = name;
            SSN = ssn;
            uid = Guid.NewGuid().ToString();
            Console.WriteLine("GUID: {0}", uid);
        }

        public string FormatedOutput()
        {
            string output = String.Format("{0} {1} {2}", Name, SSN, uid);
            foreach (Boat b in boats)
            {
                output += String.Format("\n\t{0} L:{1} W:{2} H:{3} D:{4}", b.Name, b.Length, b.Width, b.Height, b.Depth);
            }
            return output;
        }

        public void AddBoat(Boat b)
        {
            boats.Add(b);
        }
    }
    
    [Serializable]
    class Boat
    {
        public string Name;
        public float Length;
        public float Width;
        public float Depth;
        public float Height;

        public Boat(string name)
        {
            Name = name;
            Random r = new Random();
            Length = r.Next(20);
            Width = r.Next((int)Length);
            Depth = r.Next(50)/10f;
            Height = r.Next((int)Depth + 20);
        }
    }

    class MemberList
    {
        public List<Member> members = new List<Member>();

        public void FetchData(string file)
        {
            members.Clear();
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {
                object rawData = formatter.Deserialize(stream);
                members = (List<Member>)rawData;
            }
            catch
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("No file found, or broken format. Using blank list.");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            stream.Close();
        }

        public void SaveToFile(string file)
        {
            IFormatter formatter = new BinaryFormatter();

            Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, members);
            stream.Close();
        }

        public void Add(Member member)
        {
            members.Add(member);
        }

        

        public void List()
        {
            int pos = 0;
            foreach (Member m in members)
            {
                Console.WriteLine("{0}: {1}",pos, m.FormatedOutput());
                pos++;
            }
        }
    }
}
