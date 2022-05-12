using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
public class PlayerController : MonoBehaviour
{
    [SerializeField] bool useParasut;


    [SerializeField] GameObject parasut;
    [SerializeField] GameObject cameraFollowTarget;
    
    [SerializeField] Transform finishTarget;
    [SerializeField] Transform finishTarget2;


    public int _enemyCount;
    public bool levelFinished=false;




    GunTargetPosition _gunTarget;
    AnimationController _animationController;
    BoneRenderer _boneRenderer;
    RigBuilder _rigBuilder;
    stopListMover _mover;
    // Start is called before the first frame update
    void Awake()
    {
        _gunTarget = FindObjectOfType<GunTargetPosition>();
        _boneRenderer = GetComponentInChildren<BoneRenderer>();
        _rigBuilder = GetComponentInChildren<RigBuilder>();
        _animationController = GetComponent<AnimationController>();

        _enemyCount = _gunTarget._enemies.Length;
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

            
        }

    }


    void MoveToFinish()
    {
        _boneRenderer.enabled = false;
        _rigBuilder.enabled = false;
        _animationController.FinalRunAnimation(true);
        transform.LookAt(finishTarget);
        transform.DOMove(finishTarget.position, 2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            LevelSystem.Instance.DidYouNextLevelPanel = true;
            if(useParasut)
            {
                parasut.gameObject.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0);
                _animationController.FlyAnimation(true);
            }
            
            // transform.LookAt(finishTarget2);
            _animationController.FinalRunAnimation(false);
            _animationController.AttackAnimation(false);
            transform.DOMove(finishTarget2.position, 7f).OnComplete(()=>
            {
                if (useParasut)
                    transform.DOScale(Vector3.zero, 3f);
            });
        });
        
        cameraFollowTarget.gameObject.transform.parent = null;
    }
    public void DisableBone()
    {
        _boneRenderer.enabled = false;
        _rigBuilder.enabled = false;
    }
    public void EnableBone()
    {
        _boneRenderer.enabled = true;
        _rigBuilder.enabled = true;
    }
}
 