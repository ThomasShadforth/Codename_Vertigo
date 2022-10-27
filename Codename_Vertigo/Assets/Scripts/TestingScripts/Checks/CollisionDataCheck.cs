using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDataCheck : MonoBehaviour
{
    public bool _onGround { get; private set; }
    public bool _onWall { get; private set; }
    public float _friction { get; private set; }
    public bool _onSlope { get; private set; }
    public Vector2 _contactNormal { get; private set; }

    public Vector2 _slopeNormalPerp { get; private set; }

    [SerializeField] float _slopeDownAngle;
    float _slopeDownAngleOld;
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] float _checkDistance;
    [SerializeField] [Range(0, 100f)] float _maxGroundAngle = 45f;
    float minGroundDotProd;
    Vector2 colliderSize;

    private PhysicsMaterial2D _material;

    private void Awake()
    {
        
    }

    private void Start()
    {
        colliderSize = GetComponent<CapsuleCollider2D>().size;
    }

    private void Update()
    {
        
    }

    private void OnValidate()
    {
        minGroundDotProd = Mathf.Cos(_maxGroundAngle * Mathf.Deg2Rad);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _onGround = false;
        
        _friction = 0;
        _onWall = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EvaluateCollision(other);
        RetrieveFriction(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EvaluateCollision(other);
        RetrieveFriction(other);
        
    }

    public void EvaluateCollision(Collision2D collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
            _contactNormal = collision.GetContact(i).normal;
            _onGround |= _contactNormal.y >= minGroundDotProd;

            

            _onWall = Mathf.Abs(_contactNormal.x) >= .9f && collision.gameObject.layer == LayerMask.NameToLayer("WallClimbObj");
        }
    }

    

    void RetrieveFriction(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>())
        {
            _material = collision.rigidbody.sharedMaterial;
        }
        else
        {
            _material = null;
        }

        _friction = 0f;

        if(_material != null)
        {
            _friction = _material.friction;
        }

    }
}
