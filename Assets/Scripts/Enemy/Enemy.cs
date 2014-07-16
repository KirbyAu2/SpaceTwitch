using UnityEngine;
using System.Collections;

public enum PowerUps 
{
    Clone,
    Rapid,
    Multi,
    None
}

public abstract class Enemy : MonoBehaviour {
    private const float PERCENTAGE_DROP = 20f; //Twenty percent of enemies drop items
    protected bool _alive = false;
    protected Lane _currentLane;
    protected int _score = 0;
    protected bool _invulnerable = false;
    protected AudioClip _deathSound;
    protected AudioClip _shootSound;
    protected PowerUps _powerUp = PowerUps.None;
    
    public bool Alive {
        get {
            return _alive;
        }
    }

    public bool Invulernable
    {
        get
        {
            return _invulnerable;
        }
    }

    public Lane CurrentLane {
        get {
            return _currentLane;
        }
    }

    void Start () {

    }

    void Update () {

    }

    /*
     * toggles in vulnerability 
     */
    public void setVulnerability(bool value) {
        _invulnerable = !value;
    }

    /*
     * Spawns enemy projectiles w/sfx
     */
    protected void spawnProjectile() {
        if (!_shootSound) {
            _shootSound = (AudioClip)Resources.Load("Sound/EnemyShoot");
        }
        AudioSource.PlayClipAtPoint(_shootSound, transform.position);
        GameObject p = (GameObject)Instantiate(EnemyManager.Instance.enemyProjectilePrefab);
        p.GetComponent<EnemyProjectile>().startLocation = gameObject.transform.position;
        p.GetComponent<EnemyProjectile>().spawn(_currentLane);
    }

    /**
     * Implement this function. This will be called when an enemy spawns!
     */
    abstract public void spawn(Lane spawnLane);

    /*
     * Calls explode() when enemy hit by PlayerProjectile
     */
    void OnCollisionEnter(Collision collision) {
        if (_invulnerable)
        {
            return;
        }
        if(collision.gameObject.tag == "PlayerProjectile") {
            explode();
        }
    }

    /*
     * When enemy dies, it will make the explosion and sound
     * Destroy game object and increases score and multiplier 
     */
    public void explode() {
        ParticleManager.Instance.initParticleSystem(ParticleManager.Instance.enemyDeath, gameObject.transform.position);
        _deathSound = (AudioClip)Resources.Load("Sound/EnemyExplode");
        AudioSource.PlayClipAtPoint(_deathSound, transform.position);
        dropPowerup();
        _alive = false;
        EnemyManager.Instance.removeEnemy(this);
        Destroy(gameObject);
        Score.CurrentScore += _score * (Score.CurrentMultiplier + 1);
        Score.BuildUp++;
    }

    /*
     * Holds cases for all three power ups
     * Prints Power up message and calls to activate power up
     */
    protected void dropPowerup()
    {
        if (GameManager.Instance.CurrentPlayerShips.Count < 1) {
            return;
        }
        GUIStyle tempStyle = new GUIStyle (GUIManager.Instance.defaultStyle);
        tempStyle.alignment = TextAnchor.MiddleCenter;
        tempStyle.normal.textColor = Color.green;
        switch (_powerUp)
        {
            case PowerUps.Clone:
                if (GameManager.Instance.CurrentPlayerShips[0].isCloneActivated == false)
                {
                    tempStyle.alignment = TextAnchor.MiddleCenter;
                    GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, ScreenUtil.getPixels(200), "Clone!", tempStyle, 2));
                }
                GameManager.Instance.CurrentPlayerShips[0].ActivateClone();
                break;

            case PowerUps.Multi:
                GameManager.Instance.CurrentPlayerShips[0].ActivateMulti();
                tempStyle.alignment = TextAnchor.MiddleCenter;
                GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, ScreenUtil.getPixels(100), "Multi-Shot!", tempStyle, 2));
                break;

            case PowerUps.Rapid:
                GameManager.Instance.CurrentPlayerShips[0].ActivateRapid();
                tempStyle.alignment = TextAnchor.MiddleCenter;
                GUIManager.Instance.addGUIItem(new GUIItem(Screen.width / 2, ScreenUtil.getPixels(300), "Rapid Shot!", tempStyle, 2));
                break;
        }
    }

    /*
     * Function that when called, chooses a random power up to activate out of the three
     */
    void randomPower() 
    {
        float powerNumber = Random.Range(0f, 3f);
        if (powerNumber > 2f)
        {
            _powerUp = PowerUps.Clone;
        }
        else if (powerNumber > 1f)
        {
            _powerUp = PowerUps.Multi;
        }
        else
        {
            _powerUp = PowerUps.Rapid;
        }
    }

    /*
     * Function that will choose random enemies to hold the power-up drops
     */
    protected void randomEnemyDrop() 
    {
        
        float temp = Random.Range(0f, 100f);
        if (temp < PERCENTAGE_DROP)
        {
            randomPower();
            GameObject p = (GameObject)Instantiate(EnemyManager.Instance.powerupPrefab);
            p.transform.position = gameObject.transform.position;
            p.transform.parent = gameObject.transform;
        }

    }

    /*
     * Function that draws to help debugging 
     */
    void OnDrawGizmos() { 
        if (_invulnerable) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(gameObject.transform.position, .5f);
        }
    }
}
