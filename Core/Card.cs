using System;

namespace GameCore {
    [Serializable]
    public struct card_pile//弃牌堆中aba_pile数组，抽牌堆为get_pile
    {
        public int card_num;//对应牌编号，方便计算属性
        public int card_sum;//对应牌堆牌数,存在[0].card_sum中
    }
    [Serializable]
    public class Card {
        public int number;//牌总编号
        public string attribute;//属性
        public string color; //颜色
        public int num;//牌上数字
                       // public int N;
        public int Number {
            get { return number; }//调用类中的静态函数确定牌的总编号
            set { number = value; }
        }
        public string Attribute {
            get { return attribute; }//调用类中的静态函数确定牌的属性
            set { attribute = value; }
        }
        public string Color {
            get { return color; }//调用类中的静态函数确定牌的属性
            set { color = value; }
        }
        public int Num {
            get { return num; }//调用类中的静态函数确定牌的数字
            set { num = value; }
        }
        public Card() {
        }

        public Card(int num) => this.num = num;
        public string get_attribute() {
            if (number <= 76) return "figure";
            if (number >= 77 && number <= 84) return "plus_2";
            if (number >= 85 && number <= 92) return "reverse";
            if (number >= 93 && number <= 100) return "ban";
            if (number >= 101 && number <= 104) return "power_change";
            if (number >= 105 && number <= 108) return "plus_4";
            return "fuck";//闭合路径
        }
        public int get_num() {
            if (number > 76) return -1;
            if (number <= 10) return (number - 1);
            if (number >= 11 && number <= 19) return (number - 10);
            if (number >= 20 && number <= 29) return (number - 20);
            if (number >= 30 && number <= 38) return (number - 29);
            if (number >= 39 && number <= 48) return (number - 39);
            if (number >= 49 && number <= 57) return (number - 48);
            if (number >= 58 && number <= 67) return (number - 58);
            if (number >= 68 && number <= 76) return (number - 67);
            return -2;//闭合路径
        }
        public string get_color() {
            if (number <= 19) return "red";
            if (number >= 20 && number <= 38) return "yellow";
            if (number >= 39 && number <= 57) return "blue";
            if (number >= 58 && number <= 76) return "green";
            if ((number >= 77 && number <= 78) || (number >= 85 && number <= 86) || (number >= 93 && number <= 94)) return "red";
            if ((number >= 79 && number <= 80) || (number >= 87 && number <= 88) || (number >= 95 && number <= 96)) return "yellow";
            if ((number >= 81 && number <= 82) || (number >= 89 && number <= 90) || (number >= 97 && number <= 98)) return "blue";
            if ((number >= 83 && number <= 84) || (number >= 91 && number <= 92) || (number >= 99 && number <= 100)) return "green";
            if (number > 100) return "any_color";
            return "fuck";//闭合路径
        }
        public void reset_card()//对牌的全部属性进行更新
        {
            attribute = get_attribute();
            num = get_num();
            color = get_color();
        }
    }
}
