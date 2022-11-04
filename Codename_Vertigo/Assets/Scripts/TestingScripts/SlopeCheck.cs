using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeCheck : MonoBehaviour
{
    [SerializeField] GenericInputController input = null;
    [SerializeField] float _slopeCheckDistance;
    [SerializeField] LayerMask _whatIsGround;
    Vector2 _colliderSize;
    public Vector2 _slopeNormalPerp { get; private set; }
    float _slopeDownAngle;
    float _slopeDownAngleOld;

    public bool _onSlope;



    Rigidbody2D _rb2d;
    Jump _jump;
    Move _move;
    CollisionDataCheck _collisionDataCheck;

    [SerializeField] PhysicsMaterial2D noFriction, fullFriction;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _jump = GetComponent<Jump>();
        _collisionDataCheck = GetComponent<CollisionDataCheck>();
        _rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _colliderSize = GetComponent<CapsuleCollider2D>().size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_onSlope && input.GetMoveInput() == 0 && !_jump._jumping)
        {
            _rb2d.sharedMaterial = fullFriction;
        }
        else
        {
            _rb2d.sharedMaterial = noFriction;
        }

        SlopeChecking();
    }

    public void CheckForSlope(Collision2D collisionSurface)
    {
        for(int i = 0; i < collisionSurface.contactCount; i++)
        {
            Vector2 contactNormal = collisionSurface.GetContact(i).normal;

            Debug.Log(contactNormal.y);

            if(contactNormal.y < 1 && contactNormal.y > .6f)
            {
                _onSlope = true;
            }
            else
            {
                _onSlope = false;
            }
        }
    }

    public void SlopeChecking()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, _colliderSize.y / 2);
        SlopeCheckVert(checkPos);
    }

    public void SlopeCheckHor(Vector2 checkPos)
    {
        
    }

    public void SlopeCheckVert(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, _slopeCheckDistance, _whatIsGround);

        if (hit)
        {
            _slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            _slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(_slopeDownAngle != _slopeDownAngleOld)
            {

            }

            Debug.DrawRay(hit.point, _slopeNormalPerp, Color.red);

            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckForSlope(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckForSlope(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _onSlope = false;
    }
}
