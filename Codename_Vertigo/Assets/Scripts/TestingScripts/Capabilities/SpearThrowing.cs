using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearThrowing : MonoBehaviour
{
    [SerializeField]CharacterInputController input = null;
    Move move;
    

    [SerializeField, Range(0, 50)] float _throwingStrength;
    [SerializeField] float _throwingPower;
    [SerializeField] float _distBetweenDots;
    public int numberOfDots;
    [SerializeField] GameObject _throwingDot;
    [SerializeField] Transform _throwPosition;
    [SerializeField] Transform _throwingPivot;

    [SerializeField] SpearNorm _spear;


    Vector2 _throwDirection;
    GameObject[] _throwingArcDots;
    bool _spearThrown;
    //Add reference to spear object that is part of the player


    private void Awake()
    {
        move = GetComponent<Move>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _throwingArcDots = new GameObject[numberOfDots];

        for(int i = 0; i < numberOfDots; i++)
        {
            _throwingArcDots[i] = Instantiate(_throwingDot, _throwPosition.position, Quaternion.identity);
            _throwingArcDots[i].transform.parent = transform;
            _throwingArcDots[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 startPos = _throwingPivot.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseDistFromPlayer = Mathf.Abs(transform.position.x - mousePosition.x);

        _throwDirection = mousePosition - startPos;
        _throwDirection = _throwDirection * move.GetXDirect();
        _throwingPivot.transform.right = _throwDirection;

        if (!_spearThrown)
        {
            if (input.GetSpearInput())
            {
                _throwingPower = Vector2.Distance(transform.position, mousePosition);
                _throwingPower = Mathf.Clamp(_throwingPower, 0, 5f);

                for(int i = 0; i < numberOfDots; i++)
                {
                    if (!_throwingArcDots[i].activeInHierarchy)
                    {
                        _throwingArcDots[i].SetActive(true);
                    }

                    _throwingArcDots[i].transform.position = PointPosition(i * _distBetweenDots);
                }
            } else if (input.GetSpearInputUp())
            {
                ThrowSpear();

                for (int i = 0; i < numberOfDots; i++)
                {
                    _throwingArcDots[i].SetActive(false);
                }

            }
        }
        else
        {
            if (input.GetSpearInputDown())
            {
                Invoke("TriggerRetrieveSpear", .2f);
            }
        }
    }

    void TriggerRetrieveSpear()
    {
        SpearNorm _spearToRetrieve = FindObjectOfType<SpearNorm>();

        if (!_spearToRetrieve.isPickup)
        {
            _spearToRetrieve.ReturnSpear();
            _spearThrown = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<SpearNorm>())
        {
            Destroy(other.gameObject);
            _spearThrown = false;
            //Add animator references
        }
    }

    private void ThrowSpear()
    {
        SpearNorm _newSpear = Instantiate(_spear, _throwPosition.position, _throwingPivot.rotation);
        _newSpear.GetComponent<Rigidbody2D>().velocity = _throwPosition.transform.right * (_throwingStrength * _throwingPower) * move.GetXDirect();

        _spearThrown = true;
    }

    Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)_throwPosition.position + ((_throwDirection.normalized * (_throwingStrength * _throwingPower) * move.GetXDirect()) * t) + .5f * Physics2D.gravity * (t * t);
        return position;
    }

    public bool GetSpearThrown()
    {
        return _spearThrown;
    }
}
