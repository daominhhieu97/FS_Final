using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Classes
{
    class SeasonProcessing
    {
        public List<Node> clubRecord = new List<Node>();
        private bool isEmpty = true;

        public SeasonProcessing()
        {

        }
        private Node CreateNewRecord(string str, int index)
        {
            Node newNode = new Node();
            newNode.Data = str;
            newNode.Index = index;
            return newNode;
        }

        private int getIndex(string str)
        {
            string _str = null;
            int length = str.Length;
            for (int i = 0; i < length; i++)
            {
                if (char.IsDigit(str[i]) || str[i] == '-')
                    _str += str[i];
            }
            return int.Parse(_str);
        }

        public string AddRecord(string str)
        {
            if (isEmpty)
            {
                clubRecord.Add(CreateNewRecord(str, -1));
                isEmpty = false;
            }
            else
            {
                if (clubRecord[0].Index == -1)
                    clubRecord.Add(CreateNewRecord(str, 0));
                else
                {
                    string _str = clubRecord[clubRecord[0].Index].Data;
                    clubRecord[clubRecord[0].Index] = CreateNewRecord(str, 0);
                    clubRecord[0].Index = getIndex(_str);
                }
            }
            return display();
        }

        private string GetPositionDelted(string str, int index)
        {
            string _str = string.Copy(str.Remove(0, 2));
            return _str.Insert(0, '*' + Convert.ToString(index));
        }

        public string DeleteRecord(string str)
        {
            if (clubRecord.Count == 0)
            {
                isEmpty = true;
            }
            for (int i = 0; i < clubRecord.Count; i++)
            {
                if (clubRecord[i].Data == str)
                {
                    clubRecord[i].Data = GetPositionDelted(str, clubRecord[0].Index);
                    clubRecord[0].Index = i;
                }
            }

            return display();
        }

        private string display()
        {
            string result = ""; 
            foreach(Node node in clubRecord)
            {
                result += node.Data;
            }
            return result;
        }

        public void Defragment()
        {
            if (clubRecord[0].Index == -1)
                return;
            for (int i = 0; i < clubRecord.Count; i++)
            {
                if (clubRecord[i].Data[0] == '*')
                    clubRecord.RemoveAt(i);
            }
            clubRecord[0].Index = -1;
        }
    }
}
