using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    [Header("How much damage the player can take.")]
    public int startingHealth = 100;

    [Header("After damge, how many seconds invincible.")]
    public float invincibleLength = 0.75f;

    [Header("Speed in units per second")]
    public float playerSpeed = 2;

    [HideInInspector]
    public float currentHealth = 0;

    private bool isInvincible = false;
    
    private MeshRenderer meshRenderer = null;
    private Color meshColorCache;

    private Rigidbody rb = null;
    private Collider coll = null;
    private Vector3 input = Vector3.zero;

    void Awake () 
	{
        currentHealth = startingHealth;
        meshRenderer = GetComponent<MeshRenderer>();
        meshColorCache = meshRenderer.material.color;

        //Add a rigidbody
        AddPhysics();

    }//Awake

    private void AddPhysics()
    {
        rb = GetComponent<Rigidbody>();
        if(rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }//if
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.mass = 100;

        coll = GetComponent<Collider>();
        if (coll == null || coll.isTrigger || !coll.enabled)
        {
            Debug.LogWarningFormat("{0}: No enabled non-trigger collider specified on {1}, adding a sphere collider.", GetType(), gameObject.name);
            coll = gameObject.AddComponent<SphereCollider>();
        }//if
    }//AddPhysics

    void Update () 
	{
        Controls();
	}//Update  

    private void Controls()
    {
        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        input = new Vector3(horz, 0, vert);
    }//Controls

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + input * Time.deltaTime * playerSpeed * 2);
    }//FixedUpdate

    public void PushPlayer(GameObject source)
    {
        //print("TODO: Push player away from the damage source?");
    }//PushPlayer

    public void FlashRed()
    {
        StartCoroutine(DoFlashRed());
    }//FlashRed

    private IEnumerator DoFlashRed()
    {
        int numFlashes = 5;
        //On and off pulse length
        float flashLength = invincibleLength / numFlashes / 2;

        Color flashColor = Color.red;
        flashColor.a = 0.1f;
        //Flash the player red so we know it took damage
        if (meshRenderer != null)
        {
            for(int i=0; i < numFlashes; i++)
            {
                //Turn red
                meshRenderer.material.color = flashColor;
                yield return new WaitForSeconds(flashLength);
                //Turn back
                meshRenderer.material.color = meshColorCache;
                yield return new WaitForSeconds(flashLength);
            }//for
        }//if
        
        yield return new WaitForEndOfFrame();
    }//FlashRed

    public void InvincibilityTimer()
    {
        StartCoroutine(DoInvincibilityTimer());
    }//InvincibilityTimer

    private IEnumerator DoInvincibilityTimer()
    {
        //MAKE PLAYER INVINCIBLE FOR invincibleLength TO AVOID JUGGLING
        isInvincible = true;
        yield return new WaitForSeconds(invincibleLength);
        isInvincible = false;
    }//DoInvincibilityTimer

    public void DoDamage(float amount, GameObject damager)
    {
        //Cant damage player while invincible, ignore it
        if (isInvincible)
            return;

        FlashRed();
        InvincibilityTimer();
        PushPlayer(damager);
        Debug.LogFormat("{0} deals {1} damage to player.", damager.name, amount);
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }//DoDamage

    public void Die()
    {
        print("PLAYER HAS DIED!");
        Destroy(gameObject);
    }//Die
}
