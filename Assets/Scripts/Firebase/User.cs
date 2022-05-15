using System;

[Serializable]
public class User
{
    public string nickname;
    public string gameID;
    public User(string _nickname,string _gameID){
        nickname=_nickname;
        gameID=_gameID;
    }
}