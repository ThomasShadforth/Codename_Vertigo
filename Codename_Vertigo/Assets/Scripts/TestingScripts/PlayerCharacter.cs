using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IDamageInterface
{
    [SerializeField] int _maxHealth;
    [SerializeField] float _knockTime;
    [SerializeField] Vector2 _knockForce;

    HealthSystem _healthSystem;
    [SerializeField] HealthBar _PlayerHealthBar;

    bool _isKnocked;
    Move _move;
    Jump _jump;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _jump = GetComponent<Jump>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _healthSystem = new HealthSystem(_maxHealth);
        _healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    public void Damage(float damage, Transform attackerPos)
    {
        _healthSystem.Damage((int)damage);
        Vector2 knockDir = transform.position - attackerPos.position;
        GetComponent<Animator>().Play("Player_Hit");
        knockDir = knockDir.normalized;
        knockDir.y = .3f;
        StartCoroutine(KnockbackCo(knockDir));
    }

    public void CheckForHealth()
    {

    }


    IEnumerator KnockbackCo(Vector2 knockDir)
    {
        float knockCounter = _knockTime;

        while(knockCounter > 0)
        {
            if (!_isKnocked)
            {
                _isKnocked = true;
                _move._isKnocked = true;
                _jump._isKnocked = true;
                GetComponent<Rigidbody2D>().velocity = new Vector2(knockDir.x * _knockForce.x, knockDir.y * _knockForce.y);
            }

            
            knockCounter -= 1 * Time.deltaTime;

            yield return null;
        }

        _isKnocked = false;
        _move._isKnocked = false;
        _jump._isKnocked = false;

    }

    void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        Debug.Log("DAMAGED, HEALTH IS NOW: " + _healthSystem.GetHealth());

        if (_PlayerHealthBar != null)
        {
            _PlayerHealthBar.SetHealthFill(_healthSystem.GetHealthPercent());
        }

        if (_healthSystem.CheckIsDead())
        {
            //Restart from the checkpoint
        }
    }
}
