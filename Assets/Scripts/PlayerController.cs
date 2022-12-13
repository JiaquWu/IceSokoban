using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;
    private bool isPushing;
    private IA_Main mainInputAction;
    public Direction CharacterDirection{get;private set;}
    public Vector3 CharacterPosition{get;private set;}
    private Stack<Command> characterCommands = new Stack<Command>();

    private List<Direction> moveInputPool;
    private Coroutine processMoveInputCoroutine;
    private Coroutine moveCoroutine;

    private Animator animator;
    public Animator Animator {
        get{
            if(animator == null) {
                animator = GetComponent<Animator>();
                Debug.Assert(animator != null);
            }
            return animator;
        }
    }

    private Vector3 targetPosition;
    private void Awake(){

        CharacterRotateCommand(Direction.UP);
        targetPosition = transform.position;
        CharacterDirection = Direction.UP;//需要更换的
        moveInputPool = new List<Direction>();
        //if(animalAttached != null) animalAttached.enabled = false;     
    }
    private void OnEnable() {
        mainInputAction = new IA_Main();
        mainInputAction.Enable();
        
        
        mainInputAction.Gameplay.MoveUp.performed += OnMoveUpPerformed;
        mainInputAction.Gameplay.MoveLeft.performed += OnMoveLeftPerformed;
        mainInputAction.Gameplay.MoveDown.performed += OnMoveDownPerformed;
        mainInputAction.Gameplay.MoveRight.performed += OnMoveRightPerformed;

        //mainInputAction.Gameplay.Interact.performed += OnInteractPerformed;
        mainInputAction.Gameplay.Undo.performed += OnUndoPerformed;
        mainInputAction.Gameplay.Restart.performed += OnRestartPerformed;
        // LevelManager.OnPlayerDead += OnPlayerDead;
        // LevelManager.OnlevelFinish += OnLevelFinish;
    }
    private void OnDisable() {
        
        
        mainInputAction.Gameplay.MoveUp.performed -= OnMoveUpPerformed;
        mainInputAction.Gameplay.MoveLeft.performed -= OnMoveLeftPerformed;
        mainInputAction.Gameplay.MoveDown.performed -= OnMoveDownPerformed;
        mainInputAction.Gameplay.MoveRight.performed -= OnMoveRightPerformed;

        //mainInputAction.Gameplay.Interact.performed -= OnInteractPerformed;
        mainInputAction.Gameplay.Undo.performed -= OnUndoPerformed;
        mainInputAction.Gameplay.Restart.performed -= OnRestartPerformed;

        mainInputAction.Disable();
        // LevelManager.OnPlayerDead -= OnPlayerDead;
        // LevelManager.OnlevelFinish -= OnLevelFinish;
    }
    
    
    IEnumerator ProcessMoveInput() {
        //应该先判断是不是推,再判断走路这件事情
        while(moveInputPool.Count > 0) {
            //如果在推东西就不让继续输入了
            if(Animator.GetBool("IsPushing")) yield return new WaitUntil(()=>!Animator.GetBool("IsPushing"));
            if(moveInputPool.Count <= 0) yield break; 
            Direction dir = moveInputPool[moveInputPool.Count - 1];
            if(dir != CharacterDirection) {
                //如果是旋转,并且当前在走路的情况下,那么就先要让当前迅速到目标点,
                if(Animator.GetBool("IsWalking")) {
                    if(Vector3.Distance(transform.position,targetPosition) < Extensions.UNIT_DISTANCE * 0.99f) {
                        //LevelManager.GetGroundOn(targetPosition) != null && LevelManager.GetGroundOn(targetPosition).IsWalkable()
                        if(dir.IsPerpendicular(CharacterDirection)) {//如果垂直才跳过去,反方向就停止然后继续走
                            CharacterMoveCommand(targetPosition);
                        }
                    }else {
                        //目标太远了,怎么办? 直接跳过去?
                        CharacterMoveCommand(targetPosition);
                        //原地撞墙的情况下转向,那应该换一个targetpos
                        //要换,但是现在的player位置不一定是整数,所以要想办法
                        //targetPosition = transform.position;
                        //targetPosition = targetPosition + dir.DirectionToVector3();
                    }
                }      
                //然后转向
                CharacterRotateCommand(dir);
            }
             //先假设一下
            targetPosition = targetPosition + dir.DirectionToVector3();
            //这里比如说如果太远的话,直接让玩家瞬移
            if(Vector3.Distance(transform.position,targetPosition) > Extensions.UNIT_DISTANCE * 1.5f) {
                CharacterMoveCommand(targetPosition - dir.DirectionToVector3());
                CharacterRotateCommand(dir);
                
            }
            
            //所以这里就是要检测targetposition能不能推
            SokobanObject obj = LevelManager.GetObjectOn(targetPosition);
            if(obj != null) {
                if(obj.IsPushable()) {
                    //用命令模式
                    //如果没有到跟前,就瞬移一下
                    if(Vector3.Distance(transform.position,targetPosition) > Extensions.UNIT_DISTANCE * 1f) {
                        CharacterMoveCommand(targetPosition - dir.DirectionToVector3());
                        CharacterRotateCommand(dir);
                    }
                    if(obj.IsPushed(dir)) {
                        Animator.SetBool("IsPushing",true);
                        Animator.SetBool("IsWalking",false);
                        //有东西推的情况下,移动,直接取消后面的
                        if(moveCoroutine != null) StopCoroutine(moveCoroutine);//只要有新的,就应该停止旧的,因为target更新了
                        moveCoroutine = StartCoroutine(CharacterMoveCoroutine(targetPosition));
                        //yield return null;
                    }else {
                        //有东西,但是不能推
                        //所以这里应该播放一个特别的动画
                        targetPosition -= dir.DirectionToVector3();
                        if(Animator.GetBool("IsWalking")) Animator.SetBool("IsWalking",false);
                        Animator.SetTrigger("CannotPush");
                        yield break;
                        //return;
                    }
                }else {
                    //有东西,但是不能推
                    //所以这里应该播放一个特别的动画
                    targetPosition -= dir.DirectionToVector3();
                    if(Animator.GetBool("IsWalking")) Animator.SetBool("IsWalking",false);
                    Animator.SetTrigger("CannotPush");
                    yield break;

                }
                
                
                // //那么推了之后就应该还原
                // targetPosition = transform.position;
                // //那么后面就不执行了,
                // return;
            }else {
                //没有东西,那就再判断能不能走?
                //targetPosition = transform.position;
                //然后再看能不能走?代码到这里说明没东西推,那就直接走路
                //但是在走路之前要去判断
                SokobanGround ground = LevelManager.GetGroundOn(targetPosition);
                if(ground != null && ground.IsWalkable()) {
                    //命令模式
                    //if(Vector3.Distance(targetPosition + dir.DirectionToVector3(),transform.position) <= 1) {
            
                    //}
                    if(moveCoroutine != null) StopCoroutine(moveCoroutine);//只要有新的,就应该停止旧的,因为target更新了
                    moveCoroutine = StartCoroutine(CharacterMoveCoroutine(targetPosition));
                }else {
                    //说明没路或者不能走
                    targetPosition -= dir.DirectionToVector3();
                }
                // if(Animator.GetBool("IsPushing")) {
                //     Animator.SetBool("IsWalking",false);
                //     yield break;
                // }
                if(!Animator.GetBool("IsWalking")) Animator.SetBool("IsWalking",true);
                //执行到这里,该推开始推,该走开始走,下面的代码干嘛呢?
                //玩家可能会按着不动,不能每一帧都检测,因此要隔一段时间检测一次
                //隔多少时间呢?
                //
                //OnCharacterMoveInput(moveInputPool[moveInputPool.Count - 1]);
                yield return new WaitUntil(()=>Vector3.Distance(targetPosition,transform.position) < 0.01f); 
                //while (moveInputPool.Count > 0) {
                //    //OnCharacterMoveInput(moveInputPool[moveInputPool.Count - 1]);
                //    yield return new WaitUntil(()=>Vector3.Distance(targetPosition,transform.position) < 0.01f);
                //}
                //到这里说明没有可以等的了,所以不走了
                Animator.SetBool("IsWalking",false);
            }
            yield return new WaitUntil(()=>Vector3.Distance(targetPosition,transform.position) < 0.01f);
        }
    }
    void OnMoveUpPerformed(InputAction.CallbackContext context) {
        //if(LevelManager.Instance.IsPlayerDead || LevelManager.Instance.IsLevelFinished) return;
        if (context.ReadValueAsButton()) {
            if (!moveInputPool.Contains(Direction.UP)) {
                moveInputPool.Add(Direction.UP);
                if(processMoveInputCoroutine != null) StopCoroutine(processMoveInputCoroutine);
                processMoveInputCoroutine = StartCoroutine(ProcessMoveInput());
            }
        }else {
            moveInputPool.Remove(Direction.UP);
        }
    }
    void OnMoveLeftPerformed(InputAction.CallbackContext context) {
        //if(LevelManager.Instance.IsPlayerDead || LevelManager.Instance.IsLevelFinished) return; 
        if (context.ReadValueAsButton()) {
            if (!moveInputPool.Contains(Direction.LEFT)) {
                moveInputPool.Add(Direction.LEFT);
                if(processMoveInputCoroutine != null) StopCoroutine(processMoveInputCoroutine);
                processMoveInputCoroutine = StartCoroutine(ProcessMoveInput());
            }
        }else {
            moveInputPool.Remove(Direction.LEFT);
        }    
    }
    void OnMoveDownPerformed(InputAction.CallbackContext context) {
        //if(LevelManager.Instance.IsPlayerDead || LevelManager.Instance.IsLevelFinished) return;    
        if (context.ReadValueAsButton()) {
            if (!moveInputPool.Contains(Direction.DOWN)) {
                moveInputPool.Add(Direction.DOWN);
                if(processMoveInputCoroutine != null) StopCoroutine(processMoveInputCoroutine);
                processMoveInputCoroutine = StartCoroutine(ProcessMoveInput());
            }
        } 
        else {
            moveInputPool.Remove(Direction.DOWN);
        }    
    }
    void OnMoveRightPerformed(InputAction.CallbackContext context) {
        //if(LevelManager.Instance.IsPlayerDead || LevelManager.Instance.IsLevelFinished) return;         
        if (context.ReadValueAsButton()) {
            if (!moveInputPool.Contains(Direction.RIGHT)) {
                moveInputPool.Add(Direction.RIGHT);
                if(processMoveInputCoroutine != null) StopCoroutine(processMoveInputCoroutine);
                processMoveInputCoroutine = StartCoroutine(ProcessMoveInput());
            }
        }else {
            moveInputPool.Remove(Direction.RIGHT);
        }  
    }
    void OnInteractPerformed(InputAction.CallbackContext context) {
        //if(LevelManager.Instance.IsPlayerDead) return;
        //OnCharacterInteractInput();
    }
    void OnUndoPerformed(InputAction.CallbackContext context) {
        // if(LevelManager.Instance.IsLevelFinished) return;
        // LevelManager.Instance.UndoCheckPlayerDead();
        CharacterUndoCommand();          
    }
    void OnRestartPerformed(InputAction.CallbackContext context) {
        Debug.Log("restart game");
        //重启当前关卡
        GameObject light = FindObjectOfType<Light>().gameObject;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void OnCharacterMoveInput(Direction dir) {
        // if(dir == CharacterDirection) {
        //     if(!IsPassable(dir)) return;
        //     List<IPassable> objects = LevelManager.GetInterfaceOn<IPassable>(Utilities.DirectionToVector(dir) + transform.position);
        //     Command command = new Command();
        //     command.executeAction += ()=>CharacterMoveCommand(dir);
        //     command.undoAction += ()=>CharacterMoveCommand(Utilities.ReverseDirection(dir));
        //     foreach (var item in objects) {
        //        item.OnPlayerEnter(gameObject,ref command);
        //     }
        //     LevelManager.Instance.commandHandler.AddCommand(command); 
        // }else {
        //     Direction temp = CharacterDirection;
        //     Command command = new Command(()=>CharacterRotateCommand(dir),()=>CharacterRotateCommand(temp));
        //     LevelManager.Instance.commandHandler.AddCommand(command);         
        // }
        //执行某一个方向的输入
        //首先要判断和当前方向是否一样,如果不一样要先旋转一次
        
        
        //先更新目标,再判断目标是否可行
        //if(Vector3.Distance(targetPosition + dir.DirectionToVector3(),transform.position) <= 1) {
            
        //}
        

        
    }
    void OnCharacterInteractInput() { 
        // InteractionType interaction = InteractionType.NONE;
        // Command command = new Command();
        // if(InteractableCharacterHold != null) {
        //     if(IsPlaceable(out IPlaceable placeable)) {  
        //         IInteractable temp = InteractableCharacterHold;
        //         interaction = InteractionType.PUT_DOWN_ANIMALS;
        //         command.executeAction += ()=>CharacterInteractCommand(interaction,temp);
        //         command.undoAction += ()=>CharacterInteractCommand(Utilities.ReverseInteractionType(interaction),temp);
        //         InteractableCharacterHold.OnPlayerInteract(interaction,placeable,gameObject,ref command);
        //         List<IPlaceable> placeables = LevelManager.GetInterfaceOn<IPlaceable>((Utilities.DirectionToVector(CharacterDirection)) + transform.position);
        //         foreach (var item in placeables) {
        //             item.OnPlayerPlace(temp,ref command);
        //         }
        //         LevelManager.Instance.commandHandler.AddCommand(command);     
        //     }
        // }else {
        //     if(!IsInteractable()) return;
        //     interaction = InteractionType.PICK_UP_ANIMALS;
        //     List<IPlaceable> placeables = LevelManager.GetInterfaceOn<IPlaceable>((Utilities.DirectionToVector(CharacterDirection)) + transform.position);
        //     IPlaceable temp = placeables[0];
        //     List<IInteractable> objects = LevelManager.GetInterfaceOn<IInteractable>((Utilities.DirectionToVector(CharacterDirection)) + transform.position);
        //     foreach (var item in objects) {
        //         item.OnPlayerInteract(interaction,temp,gameObject,ref command);             
        //     }
        //     command.executeAction += ()=>CharacterInteractCommand(interaction,objects[0]);
        //     command.undoAction += ()=>CharacterInteractCommand(Utilities.ReverseInteractionType(interaction),objects[0]);
        //     List<Ground> grounds = LevelManager.GetInterfaceOn<Ground>(transform.position);
        //     foreach (var item in grounds) {
        //        command.executeAction += ()=>item.OnBreakingGround(true);
        //        command.undoAction += ()=>item.OnBreakingGround(false);
        //     }
        //     LevelManager.Instance.commandHandler.AddCommand(command);
        // }  
    }
    void CharacterRotateCommand(Direction targetDir) {
        //执行命令,强制改方向
        CharacterDirection = targetDir;
        transform.rotation = Quaternion.Euler(targetDir.DirectionToWorldRotation());
    }
    
    void CharacterMoveCommand(Vector3 targetPos) {
        //强行给玩家位置赋值
        transform.position = targetPos;
        CharacterPosition = transform.position;
    }
    IEnumerator CharacterMoveCoroutine(Vector3 target) {
        while(Vector3.Distance(transform.position, target) > 0.001f) {
            float speed = Animator.GetBool("IsPushing")? moveSpeed / 2.5f : moveSpeed;
            transform.position = Vector3.MoveTowards(transform.position,target,Time.deltaTime * speed);
            yield return null;
        }
        transform.position = target;
        if(Animator.GetBool("IsPushing")) Animator.SetBool("IsPushing", false);
        //if(Animator.GetBool("IsWalking")) Animator.SetBool("IsWalking",false);
    }
    void CharacterPushCheck() {

    }
    void CharacterMoveUndo() {

    }
    // void CharacterInteractCommand(InteractionType interaction,IInteractable interactableItem) {
    //     switch (interaction) {
    //         case InteractionType.PICK_UP_ANIMALS:
    //         InteractableCharacterHold = interactableItem;
    //         animalAttached.enabled = true;
    //         break;
    //         case InteractionType.PUT_DOWN_ANIMALS:
    //         InteractableCharacterHold = null;
    //         animalAttached.enabled = false;
    //         break;
    //     }
    // }
    void CharacterUndoCommand() {
    //    if(!LevelManager.Instance.commandHandler.Undo()) {
           
    //    }
        Debug.Log("undo stuff");
    }
    void OnLevelFinish() {
        //animalAttached.sprite = happyGoldenSprite;
    }
    void OnPlayerDead() {
        //moveInputPool.Clear();
    }
    // bool IsPassable(Direction dir) {
    //     List<GameObject> objects = LevelManager.GetObjectsOn(Utilities.DirectionToVector(dir) + transform.position);
    //     if(objects.Count == 0) return false;
    //     foreach (var item in objects) {
    //        if(!item.TryGetComponent<IPassable>(out IPassable passable)) return false;
    //        if(!passable.IsPassable(dir)) return false;
    //     }
    //     return true;
    // }
    // bool IsPlaceable(out IPlaceable placeable) {
    //     IPlaceable temp = null;
    //     List<GameObject> objects = LevelManager.GetObjectsOn(Utilities.DirectionToVector(CharacterDirection) + transform.position);
    //     if(objects.Count == 0) {
    //         placeable = null;
    //         return false;
    //     }
    //     foreach (var item in objects) {
    //         if(!item.TryGetComponent<IPlaceable>(out IPlaceable place)) {
    //             placeable = null;
    //             return false;
    //         }else if(place.IsPlaceable()){
    //             temp = place;
    //         }else {
    //             placeable = null;
    //             return false;
    //         }
    //     }
    //     placeable = temp;
    //     return true;
    // }

    // bool IsInteractable() { 
    //     List<IInteractable> objects = LevelManager.GetInterfaceOn<IInteractable>(Utilities.DirectionToVector(CharacterDirection) + transform.position);
    //     if(objects.Count == 0) return false;
    //     foreach (var item in objects) {
    //         if(item.IsInteractable(gameObject)) return true;
    //     }
    //     return false;       
    // }

}

