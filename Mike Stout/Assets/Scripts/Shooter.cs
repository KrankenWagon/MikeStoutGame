using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour 
{
    [Header("Shot damage to player")]
    public float damage = 10.0f;

    [Header("How fast the shot goes")]
    public float shotSpeed = 2.0f;

    [Header("One shot every x seconds")]
    public float shotTimer = 1.0f;

    [Header("Delay before first shot")]
    public float shotDelay = 0f;

    [Header("What color the bullet is")]
    public Color shotColor = Color.red;

    private bool isRunning = false;
    private float maxShotDistance = 100;
    public float shotSize = 0.5f;

    void Awake () 
	{
        isRunning = true;
        StartCoroutine(FiringLoop());
	}//Awake
	
	private IEnumerator FiringLoop()
    {
        if (shotDelay > 0)
            yield return new WaitForSeconds(shotDelay);

        while(gameObject.activeSelf)
        {       
            Shoot();

            if (shotTimer < 0.05f)
                shotTimer = 0.05f;

            yield return new WaitForSeconds(shotTimer);
        }//while

        yield return new WaitForEndOfFrame();
    }//FiringLoop

    public void Shoot()
    {
        Bullet bull = Bullet.Spawn(transform.position + transform.forward, transform.forward, shotSpeed, shotColor, maxShotDistance, shotSize);
        bull.damage = damage;
        bull.shooter = gameObject;
        bull.Fire();
    }//Shoot

    void OnDrawGizmos()
    {
        //Draw the debug gizmos in scene view unless the game has started
        if (isRunning)
            return;

        Vector3 lineEnd = transform.position + transform.forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lineEnd, 0.1f);
        Gizmos.DrawLine(transform.position, lineEnd);
    }//OnDrawGizmos

    public class Bullet : MonoBehaviour
    {
        public float speed = 1;
        public float maxDist = 0;
        public float damage = 1.0f;
        public Vector3 dir = Vector3.forward;
        public GameObject shooter = null;

        private Rigidbody rb = null;
        private Vector3 startPoint;

        public void Fire()
        {
            transform.position = startPoint;
            gameObject.SetActive(true);
        }//Fire

        void Update()
        {
            if (Vector3.Distance(transform.position, startPoint) > maxDist)
                Explode();
        }//Update

        void FixedUpdate()
        {
            rb.MovePosition(transform.position + dir * Time.deltaTime * speed * 2);
        }//FixedUpdate

        void OnCollisionEnter(Collision collision)
        {
            //Dont collide with the shooter of the bullet
            if (shooter != null && collision.gameObject == shooter)
            {
                return;
            }//if

            //Dont collide with other bullets
            if(collision.gameObject.GetComponent<Bullet>() != null)
            {
                return;
            }//if

            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                //print("PLAYER IS HIT!!");
                player.DoDamage(damage, gameObject);
            }//if
            
            Explode();

        }//OnCollisionEnter

        private void Explode()
        {
            Destroy(gameObject);
        }//Explode

        private void AddPhysics()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }//if
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = false;
            
        }//AddPhysics

        public static Bullet Spawn(Vector3 startPos, Vector3 dir, float speed, Color color, float maxDist, float size)
        {
            GameObject bulletObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulletObj.name = "Bullet";
            bulletObj.transform.position = startPos;
            bulletObj.transform.localScale = bulletObj.transform.localScale * size;

            MeshRenderer mr = bulletObj.GetComponent<MeshRenderer>();
            mr.material.color = color;

            Bullet bullet = bulletObj.AddComponent<Bullet>();
            bullet.AddPhysics();
            bullet.speed = speed;
            bullet.dir = dir;
            bullet.maxDist = maxDist;
            bullet.startPoint = startPos;

            bulletObj.SetActive(false);

            return bullet;
        }//Spawn
    }//Bullet
}
