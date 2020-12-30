using System;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace GameCore
{
    public static class Core
    {
        public static bool SinglePlayerMode = false;
        
        public static Card[,] user = new Card[9, 20];//用户最多8人，手牌上限为16； 
        public static Card last_card = new Card(-2);

        public static card_pile[] get_pile = new card_pile[109];//抽牌堆
        public static card_pile[] aba_pile = new card_pile[109];//弃牌堆
        public static int[] user_card_sum = new int[9];//玩家手牌总数量
        public static string[] win_or_not = new string[9];
        public static string top_color;//记录牌堆顶牌的颜色
        public static bool skip = false;//记录是否有禁
        public static int plus = 0;//记录加牌顺序
        public static int order = 1;//记录出牌顺序
        public static int direct = 1;//记录出牌顺序
        public static int n;//n为游戏总人数
        public static int win_man = 0;//胜利人数
        public static int[] win_rand = new int[9];//记录胜利次序
        public static int outcard = 0;//记录出的牌的序号

        public static void Write_card()
        {
            for (int i = 1; i <= n; i++)
            {
                Console.WriteLine("{0}号玩家：", i);
                if (win_or_not[i] == "win")
                {
                    Console.WriteLine("win");
                    continue;
                }
                for (int j = 1; j <= user_card_sum[i]; j++)
                {
                    Console.WriteLine("{0} {1} {2}", user[i, j].num, user[i, j].attribute, user[i, j].color);
                }
                Console.WriteLine();
            }
            Console.WriteLine();

        }
        public static bool judge_all(int user_num)//对玩家所有手牌进行判定 odk
        {
            for (int i = 1; i <= user_card_sum[user_num]; i++)
                if (Judge(user[user_num, i])) return true;//枚举每一张手牌
            return false;
        }
        public static void step_user(int step)//step表示步数 odk
        {
            while (step != 0)
            {
                step--;
                do
                {
                    order = (order + direct) % n;
                    if (order == 0) order = n;
                } while (win_or_not[order] == "win");//找到没赢的下step个人
            }
        }
        public static void handle_card()//处理牌堆顶对下一玩家的作用 odk
        {
            top_color = last_card.color;//记录弃牌堆顶的牌颜色
            if (last_card.attribute == "ban" && skip == false) skip = true;
            if (last_card.attribute == "reverse") direct *= (-1);
            if (last_card.attribute == "plus_2") plus += 2;
            if (last_card.attribute == "plus_4") plus += 4;
        }
        public static void Delete(int user_num, int out_cardnum)//在user_num号玩家的手牌中删除第out_cardnum张牌 odk
        {
            for (int i = out_cardnum; i < user_card_sum[user_num]; i++)
            {
                user[user_num, i].number = user[user_num, i + 1].number;
                user[user_num, i].reset_card();
            }
            user_card_sum[user_num]--;
            if (user_card_sum[user_num] == 0)//第user_num号玩家胜利
            {
                win_or_not[user_num] = "win";
                win_man++;
                win_rand[win_man] = user_num;
            }
        }
        public static string get_card(int user_num, int get_card_sum)//编号为user_num的玩家摸get_card_sum张牌 odk
        {
            if (get_card_sum >= get_pile[0].card_sum)
            {
                return "need to wash";//牌堆已满，需要主机进行洗牌操作并发送牌堆//*
            }//牌不够，洗牌
             // Console.WriteLine(get_pile[0].card_sum);
            for (int i = get_pile[0].card_sum; i >= get_pile[0].card_sum - get_card_sum + 1; i--)
            {
                user_card_sum[user_num]++;
                // Console.WriteLine(i);
                user[user_num, user_card_sum[user_num]].number = get_pile[i].card_num;
                user[user_num, user_card_sum[user_num]].reset_card();
                aba_pile[0].card_sum++;
                aba_pile[aba_pile[0].card_sum].card_num = get_pile[i].card_num;
            }
            get_pile[0].card_sum -= get_card_sum;
            aba_pile[0].card_sum += get_card_sum;
            sort(user_num);
            return "nothing";
        }
        public static void sort(int user_num)//整理玩家手牌，按照牌编号number大小排序，选择排序 odk
        {
            Card t = new Card();
            int Min;//一共108张牌，number最大为108
            int min_num = 0;//记录最小编号牌
            for (int i = 1; i < user_card_sum[user_num]; i++)
            {
                Min = 109;
                for (int j = i; j <= user_card_sum[user_num]; j++)
                {
                    if (user[user_num, j].number <= Min)
                    {
                        Min = user[user_num, j].number;
                        min_num = j;
                    }
                }
                t = user[user_num, i]; user[user_num, i] = user[user_num, min_num]; user[user_num, min_num] = t;
            }
        }
        public static bool Judge(Card owe_card)//true 可出,false 不可出 odk
        {
            if (last_card.num == -2) return true;
            if (plus != 0)//存在牌的叠加
            {
                if (last_card.attribute == "plus_4")
                {
                    if (owe_card.attribute == "plus_4") return true;
                    return false;
                }
                if (last_card.attribute == "plus_2")
                {
                    if (owe_card.attribute == "plus_4") return true;
                    if (owe_card.attribute == "plus_2") return true;
                    return false;
                }
            }
            if (owe_card.num != -1)//欲出的牌为 数字牌
            {
                if (owe_card.num == last_card.num) return true;//同数字
                if (owe_card.color == top_color) return true;//同颜色
                return false;
            }
            if (owe_card.attribute == "plus_2")//欲出的牌为 +2
            {
                if (owe_card.color == top_color) return true;//同颜色
                if (owe_card.attribute == last_card.attribute) return true;//同属性
                return false;
            }
            if (owe_card.attribute == "reverse")//欲出的牌为 转
            {
                if (owe_card.color == top_color) return true;//同颜色
                if (owe_card.attribute == last_card.attribute) return true;//同属性
                return false;
            }
            if (owe_card.attribute == "ban")//欲出的牌为 禁
            {
                if (owe_card.color == top_color) return true;//同颜色
                if (owe_card.attribute == last_card.attribute) return true;//同属性
                return false;
            }
            if (owe_card.attribute == "power_change") return true;//欲出的牌为 万能牌
            if (owe_card.attribute == "plus_4") return true;//欲出的牌为 +4（王牌）
            return false;//使路径闭合
        }
        static void wash_pile()//洗牌(对a数组操作) odk
        {
            for (int i = get_pile[0].card_sum + 1; i <= get_pile[0].card_sum + aba_pile[0].card_sum; i++)
                get_pile[i].card_num = aba_pile[i - get_pile[0].card_sum].card_num;
            get_pile[0].card_sum += aba_pile[0].card_sum;
            aba_pile[0].card_sum = 0;
            int[] random_cards = new int[109];
            int Mod = get_pile[0].card_sum;
            int T = 40;//洗牌次数
            while (T != 0)
            {
                T--;
                byte[] randomBytes = new byte[109];
                RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
                rngServiceProvider.GetBytes(randomBytes);
                Int32 result = BitConverter.ToInt32(randomBytes, 0);
                random_cards[1] = randomBytes[1];
                random_cards[1] = random_cards[1] % Mod + 1;
                for (int i = 1; i < get_pile[0].card_sum; i++)
                {
                    random_cards[i + 1] = randomBytes[i + 1];
                    random_cards[i + 1] = random_cards[i + 1] % Mod + 1;
                    card_pile y;
                    y = get_pile[random_cards[i]]; get_pile[random_cards[i]] = get_pile[random_cards[i + 1]]; get_pile[random_cards[i + 1]] = y;
                }
            }
        }
        public static void prepare() //初始的准备 odk
        {
            for (int i = 1; i <= 8; i++) win_or_not[i] = "not win";
          //  for(int j=1;j<=4;j++)
                for (int i = 1; i <= 108; i++) 
                    get_pile[i].card_num = i;//初始化抽牌堆
            get_pile[0].card_sum = 108;
            aba_pile[0].card_sum = 0;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 20; j++)
                    user[i, j] = new Card();
        }
        public static void InitGame(int n)
        {
            prepare();
            wash_pile();
        }
        public static string out_change(string str)
        {
            if (str == "figure") return "数字牌";
            if (str == "ban") return "禁牌";
            if (str == "plus_2") return "+2牌";
            if (str == "plus_4") return "王牌";
            if (str == "reverse") return "转向牌";
            if (str == "power_change") return "万能牌";
            if (str == "red") return "红色";
            if (str == "green") return "绿色";
            if (str == "blue") return "蓝色";
            if (str == "yellow") return "黄色";
            if (str == "any_color") return "任何颜色";
            return str;
        }
        public static bool FindNextUser()
        {
            int T = 100;
            while (Convert.ToBoolean(T--))
            {
                if (win_man == 1)
                {
                    return true;
                }
                if (skip)//为禁牌
                {
                    step_user(2);
                    skip = false;
                }
                else step_user(1);//确定应该👉order号出牌
                if (plus != 0)//存在加牌的叠加
                {
                    if (!judge_all(order))//若无牌可出则只能被加牌
                    {
                        if (get_card(order, plus) == "need to wash")
                        {
                            wash_pile();//需要主机进行洗牌后发送；发送牌堆结构体即可//*
                            aba_pile[0].card_sum = 0;//将弃牌堆清空
                            get_card(order, plus);
                        }//如果发现牌堆不足，则返回洗牌后再发；否则直接发牌
                        plus = 0;
                        continue;
                    }
                }
                if (!judge_all(order))//若无牌可出，则摸一张并跳过出牌
                {
                    if (get_card(order, 1) == "need to wash")
                    {
                        wash_pile();//需要主机进行洗牌后发送；发送牌堆结构体即可//*
                        aba_pile[0].card_sum = 0;//将弃牌堆清空
                        get_card(order, 1);
                    }//如果发现牌堆不足，则返回洗牌后再发；否则直接发牌
                    continue;
                }
                break;
            }
            return false;
        }

    }
}
