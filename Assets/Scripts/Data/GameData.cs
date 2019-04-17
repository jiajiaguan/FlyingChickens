using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {
    private static GameData _instance;
    public static GameData Instance{
        set{
            _instance = value;
        }
        get{
            if (_instance == null)
                _instance = new GameData();
            return _instance;
        }
    }
    public void InitData(){

    }
}
