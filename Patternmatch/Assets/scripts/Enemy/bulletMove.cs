using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletMove : MonoBehaviour
{

    public bool work=false;
    public bool automaticDestroy;
    public float destroyDistance;
    
    
    public Vector3 gunPointingDirection;
    public float bulletSpeed;
    public Vector3 generalMovementDirection=new Vector3(0,0,-1);
    public float generalSpeed;
    public float gravity=0;
    public Transform firedGun;
    

    void Start()
    {
        firedGun= findPlayer().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!work) return;
        generalSpeed = GameManager.Instance.roadSpeed;
         
        move();
    }

    private void move()
    {

        Vector3 translation = (gunPointingDirection * (bulletSpeed * Time.deltaTime)) +
                              (generalMovementDirection * (generalSpeed * Time.deltaTime))+
                              (new Vector3(0,-1,0) * (gravity * Time.deltaTime));
        transform.Translate(translation);
        //transform.Translate(translation,Space.World);

        
        
        float distance = Vector3.Distance(transform.position, firedGun.transform.position);
        if (automaticDestroy&&distance>destroyDistance)
        {
            Destroy(this.gameObject);
          //  DestroyImmediate();
        }

    }

    public GameObject findPlayer()// find player object by name 
    {
        GameObject result = findNamedObject("player");
        if (result==null)
            result = findNamedObject("Player");
        else if (result==null)
            result = findNamedObject("PLAYER");
       
        return result;
 
    } 
    public GameObject findNamedObject( string word ) // finding object by  name first if not with tag if  not with layer
    { 
        var  result = GameObject.Find(word);
        if(result==null)
            result=GameObject.FindWithTag(word);
    
        
        return result;
    }

}
