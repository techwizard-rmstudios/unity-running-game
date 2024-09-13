using UnityEngine;

public class CyborgController : MonoBehaviour
{
    [SerializeField]
    private int _forwardAcceleration;
    [SerializeField]
    private float _forwardMaxSpeed;
    [SerializeField]
    private float _sideSpeed;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private float _gravityForce;
    [SerializeField]
    private LayerMask _groundMask;
    [Header("References")]
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private Animator _animator;
    private bool _isGrounded;

    private void Update()
    {
        this.UpdateMovement();
        this.CheckGrounded();
        this._animator.SetFloat("Magnitude", this._rigidbody.velocity.sqrMagnitude);
    }

    private void UpdateMovement()
    {
        this._rigidbody.AddForce(Vector3.forward * (float)((double)this._forwardAcceleration * (double)Time.deltaTime * 100.0));
        this._rigidbody.AddForce(Vector3.down * (float)((double)this._gravityForce * (double)Time.deltaTime * 100.0));
        this._rigidbody.velocity = Vector3.ClampMagnitude(this._rigidbody.velocity, this._forwardMaxSpeed * 10f);
        this.transform.Translate(this._sideSpeed * Input.GetAxis("Horizontal") * Time.deltaTime * Vector3.right);
        if (!this._isGrounded || !Input.GetKeyDown(KeyCode.Space))
            return;
        this._rigidbody.AddForce(Vector3.up * this._jumpForce, ForceMode.Impulse);
        this._isGrounded = false;
    }

    private void CheckGrounded() => this._isGrounded = Physics.Raycast(this.transform.position, Vector3.down, 0.3f, (int)this._groundMask);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Collectible component = other.GetComponent<Collectible>();
            if (!((Object)component != (Object)null))
                return;
            UiManager.Instance.UpdateScore(component.OnPickUp());
        }
        else
        {
            if (!other.CompareTag("Finish"))
                return;
            Debug.Log("Finish");
            UiManager.Instance.ShowGameOverScreen();
            this._rigidbody.velocity = Vector3.zero;
            this._forwardAcceleration = 0;
            this._forwardMaxSpeed = 0.0f;
            this._sideSpeed = 0.0f;
        }
    }
}