using GameCore;
using System;
using System.Collections.Generic;
using UserNamespace;
namespace UNO__ {
    public enum msgType { userlist, user, initusercards, card_with_id };
    [Serializable]
    public class Communication {
        public List<User> users = null;
        public User user = null;
        public card_pile[] usercards;
        public Card card = null;
        public int cardid = 0;
        public string color = null;
        public int userid = 0;
        public Communication(card_pile[] user_cards) {
            usercards = user_cards;
            MsgType = msgType.initusercards;
        }
        public Communication(List<User> usrs) {
            users = usrs;
            MsgType = msgType.userlist;
        }
        public Communication(User usr) {
            user = usr;
            MsgType = msgType.user;
        }
        public Communication(Card crd, int id, string clor = "", int usrid = 0) {
            card = crd;
            cardid = id;
            color = clor;
            userid = usrid;
            MsgType = msgType.card_with_id;
        }
        public msgType MsgType { get; set; } = msgType.user;
    }
}
