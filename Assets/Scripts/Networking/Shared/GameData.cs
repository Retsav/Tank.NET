using System;

public enum Map
{
    Default
}

public enum GameMode
{
    Default
}

public enum GameQueue
{
    Solo,
    Team
}

[Serializable]
public class GameData
{
    public string userName;
    public string userAuthId;
    public GameInfo userGamePreferences;
}

[Serializable]
public class GameInfo
{
    public Map map;
    public GameMode gameMode;
    public GameQueue GameQueue;

    public string ToMultiplayQueue()
    {
        return "";
    }
}