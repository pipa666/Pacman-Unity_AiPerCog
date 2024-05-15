using UnityEngine;
using UnityEngine.UI;
// TODO: Implement game identifiersss
public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; } // I dont understand this one and the Awake start
    public Ghost[] ghosts;
    
    public Pacman pacman;
    
    public Transform pellets;

    public DataCollector gameDatacollector;

    public Text Gameover;
    public Text ScoreText;
    public Text livesText;

    public Text restartKey;

    public int ghostMultiplier { get; private set; } = 1;

    public int score {get ; private set; }

    public int remainingPellets {get ; private set; }

    public int remainingPills {get ; private set; }
    
    public int lives {get ; private set; }


    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    
    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (this.lives <= 0 && Input.anyKeyDown){
            Invoke(nameof(NewGame), 2.0f); // Start a new round after 1 second, buffer time for saving data
        }
    }
    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        remainingPellets = CountRemainingPellets();
        remainingPills = CountRemainingPowerPellets();
        NewRound();

    }

    private void NewRound()
    {
        MainManager.Instance.games_in_session++; // Increase the count of games in a session
        gameDatacollector.Startdatacollection();
        Gameover.enabled = false;
        restartKey.enabled = false;
        foreach (Transform pellet in this.pellets) // reset all pellets
        {
            pellet.gameObject.SetActive(true);
        }
        
        for (int i = 0; i < this.ghosts.Length; i++) { // reset all ghosts
            this.ghosts[i].ResetState();
        }
        this.pacman.ResetState(); // reset pacman
    }

    private void ResetState()  // If pacman dies, resets ghots and pacman but not pellet
    {
       ResetGhostMultiplier();

       for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState(); 
    }

    private void GameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false); 
        // Game over screen
        Gameover.enabled = true;
        restartKey.enabled = true;
        gameDatacollector.SaveData();
    }

    private void SetScore(int score)
    {
        this.score = score;
        ScoreText.text = score.ToString().PadLeft(2, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        
        this.pacman.DeathSequence();


        SetLives(this.lives - 1);

        if (this.lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f); // If pacman dies, resets ghots and pacman but not pellet (3 seconds delay)
        }
        else
        {
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore (this.score + pellet.points);
        remainingPellets = CountRemainingPellets();
        remainingPills = CountRemainingPowerPellets();
        if (remainingPellets == 0){
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
            gameDatacollector.SaveData(); // TODO: change this to a game win screen
        }
    }

    public void PowerPelletEaten (PowerPellet pellet)
    {
        for (int i = 0; i < this.ghosts.Length; i++){
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(); // If you take more than one powerpellet, cancel the first invoke timer and start it again
        PacmanAttack();
        Invoke(nameof(PacmanAttackEnd), pellet.duration);
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);        
    }

    // change pacman state to attack for the duration of the power pellet
    public void PacmanAttack(){
        pacman.pacmanAttack = true;
    }
    public void PacmanAttackEnd(){
        this.pacman.pacmanAttack = false;
    }

    private int CountRemainingPellets()
    {
        int count = 0;
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf && pellet.GetComponent<Pellet>() != null)
            {
                count++;
            }
        }
        return count;
    }

    private int CountRemainingPowerPellets()
    {
        int count = 0;
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf && pellet.GetComponent<PowerPellet>() != null)
            {
                count++;
            }
        }
        return count;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }

}