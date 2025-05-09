using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject epicWin;
    [SerializeField] GameObject enemyParent;
    [SerializeField] GameObject continueCanvas;
    [SerializeField] bool useParasut;


    [SerializeField] GameObject parasut;
    [SerializeField] GameObject cameraFollowTarget;
    
    [SerializeField] Transform finishTarget;
    [SerializeField] Transform finishTarget2;


    public int _enemyCount;
    public bool levelFinished=false;


    NavMeshAgent _agent;
    Doors _door;
    GunTargetPosition _gunTarget;
    AnimationController _animationController;
    BoneRenderer _boneRenderer;
    RigBuilder _rigBuilder;
    stopListMover _mover;
    // Start is called before the first frame update
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyCount = enemyParent.transform.childCount;
        _door = FindObjectOfType<Doors>();
        _gunTarget = FindObjectOfType<GunTargetPosition>();
       // _boneRenderer = GetComponentInChildren<BoneRenderer>();
       // _rigBuilder = GetComponentInChildren<RigBuilder>();
        _animationController = GetComponent<AnimationController>();

        //_enemyCount = _gunTarget._enemies.Length;
        _mover = GetComponent<stopListMover>();
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if (_enemyCount > 0) return;


        if (_enemyCount==0 && !levelFinished)
        {
            levelFinished = true;
            GameManager.Instance.levelFinished = true;
            Invoke("MoveToFinish", 0.8f);
           // _door.doorLeft.gameObject.transform.DOMoveX(0.18f,0.5f);
            //_door.doorRight.gameObject.transform.DOMoveX(_door.doorRight.gameObject.transform.localPosition.x - 0.12f, 0.5f);

        }

    }


    void MoveToFinish()
    {
        float door1x = _door.doorLeft.gameObject.transform.localPosition.x;
        float door2x = _door.doorRight.gameObject.transform.localPosition.x;
        // _boneRenderer.enabled = false;
        //_rigBuilder.enabled = false;
        _animationController.FinalRunAnimation(true);
        transform.LookAt(finishTarget);
        transform.DOMove(transform.position+new Vector3(0.01f,0.01f,0.01f), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            epicWin.gameObject.SetActive(true);
            _door.doorRight.gameObject.transform.DOMove(new Vector3(door2x, _door.doorRight.gameObject.transform.position.y, _door.doorRight.gameObject.transform.position.z), 0.5f);
            _door.doorLeft.gameObject.transform.DOMove(new Vector3(door1x, _door.doorLeft.gameObject.transform.position.y, _door.doorLeft.gameObject.transform.position.z), 0.5f);

            
            continueCanvas.gameObject.SetActive(true);
            // transform.LookAt(finishTarget2);
            _animationController.FinalRunAnimation(false);
            _animationController.AttackAnimation(false);
            _animationController.FinalRunAnimation(true);
            //transform.DOMove(finishTarget2.position, 7f);
            _agent.speed = 20f;
            _agent.destination = finishTarget.position;

            
        });
        
        _door.doorLeft.gameObject.transform.DOMoveX(_door.doorLeft.gameObject.transform.localPosition.x + 0.12f, 0.5f);
        _door.doorRight.gameObject.transform.DOMoveX(_door.doorRight.gameObject.transform.localPosition.x - 0.12f, 0.5f);
        cameraFollowTarget.gameObject.transform.parent = null;
    }
   /* public void DisableBone()
    {
        _boneRenderer.enabled = false;
        _rigBuilder.enabled = false;
    }
    public void EnableBone()
    {
        _boneRenderer.enabled = true;
        _rigBuilder.enabled = true;
    }*/
}
 