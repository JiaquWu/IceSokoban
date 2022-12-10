using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;

    private IA_Main mainInputAction;
    public Direction CharacterDirection{get;private set;}
    public Vector3 CharacterPosition{get;private set;}
    private Stack<Command> characterCommands = new Stack<Command>();

    private List<Direction> moveInputPool;
    private Coroutine processMoveInputCoroutine;
    private Coroutine moveCoroutine;
    [SerializeField]
    private SpriteRenderer animalAttached;

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

        //CharacterRotateCommand(Direction.UP);
        targetPosition = transform.position;
        CharacterDirection = Direction.UP;//需要更换的
        moveInputPool=new List<Direction>();
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
        if(!Animator.GetBool("IsWalking")) Animator.SetBool("IsWalking",true);
        OnCharacterMoveInput(moveInputPool[moveInputPool.Count - 1]);
        yield return new WaitUntil(()=>Vector3.Distance(targetPosition,transform.position) < 0.01f);
        while (moveInputPool.Count > 0) {
            OnCharacterMoveInput(moveInputPool[moveInputPool.Count - 1]);
            yield return new WaitUntil(()=>Vector3.Distance(targetPosition,transform.position) < 0.01f);
        }
        Animator.SetBool("IsWalking",false);
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
        if(dir != CharacterDirection) {
            //如果是旋转,并且当前在走路的情况下,那么就先要让当前迅速到目标点,
            if(Animator.GetBool("IsWalking")) {
                if(dir.IsPerpendicular(CharacterDirection)) {//如果垂直才跳过去,反方向就停止然后继续走
                    //这里要进行一系列的操作检测
                    CharacterPushCheck();
                    CharacterMoveCommand(targetPosition);
                }
            }      
            //然后转向
            CharacterRotateCommand(dir);
        }
        //能走的情况下可以接下去执行走路
        //所以也要一个操作检测!
        CharacterPushCheck();
        
        if(Vector3.Distance(targetPosition + dir.DirectionToVector3(),transform.position) <= 1) {
            targetPosition = targetPosition + dir.DirectionToVector3();
        }
        if(moveCoroutine != null) StopCoroutine(moveCoroutine);//只要有新的,就应该停止旧的,因为target更新了
        moveCoroutine = StartCoroutine(CharacterMoveCoroutine(targetPosition));
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
            transform.position = Vector3.MoveTowards(transform.position,target,Time.deltaTime * moveSpeed);
            yield return null;
        }
        transform.position = target;
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

