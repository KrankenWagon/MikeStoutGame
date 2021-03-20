using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UIPlayerHealth : MonoBehaviour 
{
    public Player player = null;
    private Text textObj = null;

	void Awake () 
	{
        if (player == null)
            player = GameObject.FindObjectOfType<Player>();

        if (textObj == null)
            textObj = gameObject.GetComponent<Text>();
	}//Awake
	
	void Update () 
	{
        if (textObj != null && player != null)
            textObj.text = string.Format("Health: {0}", player.currentHealth);
        else
        {
            if (textObj == null)
                Debug.LogFormat("{0}: {1} script must be placed on an object with a 'Text' component attached", gameObject.name, GetType());

            if (player == null)
                textObj.text = "Player has died.";
        }//else
	}//Update
}
