using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private IA_Main mainInputAction;
    public Direction CharacterDirection{get;private set;}
    public Vector3 CharacterPosition{get;private set;}
    private Stack<Command> characterCommands = new Stack<Command>();

    private List<Direction> moveInputPool;
    private Coroutine processMoveInputCoroutine;
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

    private void Awake(){

        CharacterRotateCommand(Direction.UP);
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
        //yield return new WaitForSeconds(moveInputPool.Count <= 1?0.5f:0.5f);
        yield return new WaitForSeconds(0.67f);
        while (moveInputPool.Count > 0) {
            OnCharacterMoveInput(moveInputPool[moveInputPool.Count - 1]);
            yield return new WaitForSeconds(0.67f);
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
        Debug.Log(dir);
        if(dir != CharacterDirection) {
            CharacterRotateCommand(dir);
        }
        CharacterMoveCommand(dir);
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
        //执行命令
        CharacterDirection = targetDir;
        transform.rotation = Quaternion.Euler(targetDir.DirectionToWorldRotation());
    }
    void CharacterMoveCommand(Direction dir) {
        //transform.position = (dir.DirectionToVector() + transform.position).Vector3ToVector3Int();
        // CharacterPosition = transform.position;
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

