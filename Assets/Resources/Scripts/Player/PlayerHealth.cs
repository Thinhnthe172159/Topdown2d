using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private int health;
    private bool isKill;
    public int RESURRECTION_COUNT = 3;
    public Text resurrection_count;

    [HideInInspector]
    public bool invinsible;

    private HealthBar healthBar;

    private GameObject blood1;

    private GameObject bloodHeal1;

    void Start()
    {
        health = 100;
        isKill = false;

        healthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        healthBar.SetMaxHealth(health);

        resurrection_count = GameObject.Find("CountLiveTxt").GetComponent<Text>();
        resurrection_count.text = $"x {RESURRECTION_COUNT}";

        blood1 = Resources.Load<GameObject>("Prefabs/Effects/BloodEffect1");
        bloodHeal1 = Resources.Load<GameObject>("Prefabs/Effects/HealEffect1");
    }

    void Update()
    {
        if (health == 0 && !isKill)
        {

            if (RESURRECTION_COUNT > 0)
            {
                RESURRECTION_COUNT--;
                health = 100;
            }
            else
            {
                // quang cao + health
                isKill = true;
            }
            resurrection_count.text = $"x {RESURRECTION_COUNT}";
        }
    }

    public void decreaseHealth(int damage)
    {
        if (!invinsible)
        {
            health = Math.Max(0, health - damage);
            healthBar.SetHealth(health);
            GameObject blood = Instantiate(blood1, transform.position, transform.rotation);
            blood.transform.SetParent(transform);
            SoundManager.PlaySound("Hurt");
        }
    }

    public void AddView()
    {

    }

    public void IncreaseHealth(int heal)
    {
        health = Math.Min(100, health + heal);
        healthBar.SetHealth(health);
        GameObject blood = Instantiate(bloodHeal1, transform.position, transform.rotation);
        blood.transform.SetParent(transform);
    }

    public bool IsKill
    {
        get { return isKill; }
    }
}