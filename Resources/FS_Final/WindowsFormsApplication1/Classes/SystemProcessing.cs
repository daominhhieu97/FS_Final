using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Classes
{
    class ClubProcessing
    {
        List<Player> lst = new List<Player>(); //khoi tao danh sach cau thu cua club qua tung mua giai
        Player head = new Player(); // bien head = "-1"
        public ClubProcessing()
        {
            Player head = new Player();
            head.Name = "-1";
            lst.Insert(0, head);
        }

        internal string doFirstFitRecording(Player player)
        {
            Player player1 = player.Clone(); // deep copy 

            if (addCuoi(player1) == true) // kiem tra add cuoi 
            {
                lst.Add(player1); // add vao cuoi
            }
            else // neu sai 
            {
                addFirstFit(player1); // add kieu first fit
            }
            return (display()); // tra ve mang player trong club
        }


        private void addFirstFit(Player player1)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Vitrixoa != 0 && (lst[i].Name.Length >= player1.Name.Length)) // da tim thay vi tri thich hop
                {
                    //them vao 
                    lst[0].Name = lst[i].Vitrixoa.ToString(); //doi cho vi tri xoa da luu voi head
                    lst[i].Name = player1.Name + sodaucham(lst[i].Name.Length, player1.Name.Length); // gan player cu bang player moi 
                    lst[i].Vitrixoa = 0; // gan lai vi tri xoa = 0
                    break;
                }
            }
        }

        private string sodaucham(int length1, int length2)
        {
            string daucham = "";
            for (int i = 0; i < Math.Abs(length1 - length2); i++) // chenh lech hai do dai
            {
                daucham += ".";
            }
            return daucham;
        }

        private string display()
        {
            string result = "";
            for (int i = 0; i < lst.Count; i++)
            {
                if (i == 0)
                {
                    result+= lst[i].Name + " ";
                }
                else
                {
                 result +=lst[i].Name + lst[i].Vitrixoa + " ";
                }
            }
            return result;
        }


        private bool addCuoi(Player player1)
        {
            //case 1: chi co node head 
            bool isOnlyHead = checkOnlyHead(); 
            //case 2: toan ko 
            bool checkAllZero = checkOnlyZero();
            //case 3: ko tim duoc vi tri thich hop tu cac node da xoa 
            bool checkPosition = checkFitPosition(player1);

            if (isOnlyHead == true || checkAllZero == true || checkPosition == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool checkFitPosition(Player player1)
        {
            bool flag = false;
            foreach (Player tmp in lst)
            {
                if (tmp.Vitrixoa != 0 && tmp.Name.Length >= player1.Name.Length) // neu co vi tri thich hop
                {
                    flag = true; // flag = true
                    return flag;
                }
            }
            return flag;
        }

        private bool checkOnlyZero()
        {
            bool flag = true;
            foreach (Player tmp in lst)
            {
                if (tmp.Vitrixoa != 0)
                    flag = false;
            }
            return flag;
        }

        private bool checkOnlyHead()
        {
            bool flag = false;
            if (lst.Count == 1)
            {
                flag = true;
            }
            return flag;
        }

        internal  string doDeleteFirstFit(Player player)
        {
            Player player1 = player.Clone(); // deep copy
            Player tmp = lst.FirstOrDefault(x => x.Name == player1.Name); // tim cau thu player can xoa trong list
            tmp.Vitrixoa = int.Parse(lst[0].Name); // doi vi tri xoa cua Node = head.name
            lst[0].Name = timvitri(player1); // gan head.name = vi tri xoa
            return (display());
        }

        private string timvitri(Player player1)
        {
            int vitrinodecanxoatheobit = 0;
            for (int i = 0; i < lst.Count; i++)
            {
                if (i == 0) // neu la node dau 
                {
                    vitrinodecanxoatheobit++;
                }
                else
                {
                    if (lst[i].Name == player1.Name) // neu la node can tim
                    {
                        vitrinodecanxoatheobit++; // tang 1 don vi cho bit
                        break;
                    }
                    else
                    {
                        vitrinodecanxoatheobit += lst[i].Name.Length + 1; // vitrinodecanxoatheobit + do dai chuoi  +1 
                    }
                }
            }
            return vitrinodecanxoatheobit.ToString();
        }

        internal string doDefragment()
        {
            foreach (Player player in lst.ToList())
            {
                if (player.Vitrixoa != 0) // nhung thang da bi xoa
                {
                    lst.Remove(player); //remove nhung node da bi xoa
                }
            }
            lst[0].Name = "-1"; // head.name = "-1"

            return (display());
        }

        internal string doAddBestFit(Player player)
        {
            Player player6 = player.Clone();
            if (addCuoi(player6) == true)
            {
                lst.Add(player6);
            }
            else
            {
                addBestFit(player6);
            }
            return display();
        }

        private void addBestFit(Player player6)
        {
            //tim index thich hop 
            int index = timindexvoitenplayernhonhat(player6); // tim index thich hop truoc khi chen
            //chen cau thu
            addPlayerBestFit(player6, index);
        }

        private void addPlayerBestFit(Player player6, int index)
        {
            lst[0].Name = lst[index].Vitrixoa.ToString();
            lst[index].Name = player6.Name + sodaucham(lst[index].Name.Length, player6.Name.Length);
            lst[index].Vitrixoa = 0;
        }

        private int timindexvoitenplayernhonhat(Player player6)
        {
            int index = 0;
            int min = 1000;
            for (int i = 1; i < lst.Count; i++)
            {
                if (lst[i].Vitrixoa != 0)
                {
                    if (Math.Abs(lst[i].Name.Length - player6.Name.Length) <= min)
                    {
                        index = i;
                        min = Math.Abs(lst[i].Name.Length - player6.Name.Length);
                    }
                }
            }
            return index;
        }

        internal string doDeleteBestFit(Player player)
        {
            Player player6 = player.Clone();
            doDeleteFirstFit(player6);
            return display();
        }
    }
    }

