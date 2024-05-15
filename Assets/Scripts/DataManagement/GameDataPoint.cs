using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Specialized;

[System.Serializable]
public class GameDataPoint  // defines the daata to be recorded each frame/time step
{
    public Vector2 playerPosition;
    public Vector2[] ghostsPositions;
    public bool pacmanAttack; // true if pacman is attacking

    public int[] ghostStates;   // 0 = home, 1 = scatter, 2 = chase, 3 = frightened , 4 = eaten
    public int score;
    public int powerPelletsRemaining;
    public int pelletsRemaining;
    public int livesRemaining;
    public float timeElapsed;
    // public string ToCsvString()
    // {
    //     // Example for formatting; adjust based on your actual fields
    //     return $"{playerPosition.x};{playerPosition.y};{string.Join(";", ghostsPositions.Select(gp => gp.ToString()))};{score};{livesRemaining};{timeElapsed}";
    // }
}

[System.Serializable]
public class GameDataContainer  // contains all the data to be sent after finalizing a game
{
    public List<GameDataPoint> dataPoints;
    public int user_id;
    public string start_time;
    public double game_duration;
    public int session_number;
    public int game_in_session;
    public string source;
}