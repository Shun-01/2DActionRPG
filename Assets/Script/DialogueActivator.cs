using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivator : MonoBehaviour
{
    [SerializeField, Header("âÔòbï∂èÕ"), Multiline(3)]
    private string[] lines;

    private bool canActivator;
    [SerializeField]
    private bool savePoint;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1) && canActivator && !GameManager.instance.dialogueBox.activeInHierarchy)
        {
            GameManager.instance.ShowDialogue(lines);

            if (savePoint)
            {
                GameManager.instance.SaveStatus();
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canActivator = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            canActivator = false;

            GameManager.instance.ShowDialogueChange(canActivator);
        }
    }
}
