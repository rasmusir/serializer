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
                    ml.Add(m);
                    Console.Clear();
                }
                else if (choice == 'r')
                {
                    ml.List();
                }
                else if (choice == 'd')
                {
                    Console.WriteLine("Select member to kill:");
                    ml.List();
                    int id = 0;
                    if (int.TryParse(Console.ReadLine(),out id))
                    {
                        if (id<ml.members.Count && id >= 0)
                        {
                            ml.members.RemoveAt(id);
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

        public Member(string name, string ssn)
        {
            Name = name;
            SSN = ssn;
            uid = Guid.NewGuid().ToString();
            Console.WriteLine("GUID: {0}", uid);
        }
    }

    [Serializable]
    class MemberList
    {
        public List<Member> members = new List<Member>();

        public void FetchData(string file)
        {
            members.Clear();
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read);

                members.AddRange( ((Member[])formatter.Deserialize(stream)) );

                stream.Close();
            }
            catch(Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("No file found, or broken format. Using blank list.");
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        public void Add(Member member)
        {
            members.Add(member);
            
        }

        public void SaveToFile(string file)
        {
            IFormatter formatter = new BinaryFormatter();

            Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, members.ToArray());
            stream.Close();
        }

        public void List()
        {
            int pos = 0;
            foreach (Member m in members)
            {
                Console.WriteLine("{3}: {0} {1} {2}", m.Name, m.SSN, m.uid, pos);
                pos++;
            }
        }
    }
}
