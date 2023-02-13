
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;


public class CreateExpressionLogic : EditorWindow
{
    // Constants
    string leftGestureParameter = "GestureLeft";
    string rightGestureParameter = "GestureRight";
    string leftGestureWeightParameter = "GestureLeftWeight";
    string rightGestureWeightParameter = "GestureRightWeight";

    // UI
    AnimatorController targetController;
    string targetLayerName = "FaceGestures";
    AnimationClip defaultAnimation;
    bool ambidextrousLogic = true;
    bool onlyTransitions = false;
    float transitionDuration = 0.25f;

    // Internal
    AnimatorStateMachine targetStateMachine;

    [MenuItem("Tools/Sidega/Create Expression Logic")]
    public static void OpenWindow()
    {
        CreateExpressionLogic window = GetWindow<CreateExpressionLogic>();
        window.titleContent = new GUIContent("Create Expression Logic");
    }

    private void OnGUI()
    /* 
     *  Draws the editor GUI
     */
    {        
        targetController = EditorGUILayout.ObjectField(
            "Animator Controller", 
            targetController, 
            typeof(AnimatorController), 
            false
        ) as AnimatorController;

        targetLayerName = EditorGUILayout.TextField("Target Layer", targetLayerName);
        
        defaultAnimation = EditorGUILayout.ObjectField(
            "Default Animation Clip", 
            defaultAnimation, 
            typeof(AnimationClip), 
            false
        ) as AnimationClip;

        ambidextrousLogic = EditorGUILayout.Toggle("Ambidextrous Logic", ambidextrousLogic);

        onlyTransitions = EditorGUILayout.Toggle("Only Transitions", onlyTransitions);

        transitionDuration = EditorGUILayout.FloatField("Transition Duration", transitionDuration);
        
        if (GUILayout.Button("Generate"))
            GenerateLogic();
    }

    private void GenerateLogic()
    /*
     *  Main entry point for operation.
     */
    {

        if (targetController == null) {
            ShowNotification(new GUIContent("Animator Controller not set"));
            return;
        }

        if (onlyTransitions) {
            UpdateTransitions();
        } else {           
            CreateControllerLayer();
            if (ambidextrousLogic)
                AmbidextrousLogic();
            else
                NonAmbidextrousLogic();
        }     
    }

    private void CreateControllerLayer()
    /* 
     *  Creates/recreates layer in the target controller to contain all face gesture logic.
     */
    {
        for (int i = 0; i < targetController.layers.Length; i++) {
            if (targetController.layers[i].name == targetLayerName) {
                targetController.RemoveLayer(i);
                break;
            }
        }        
        
        targetController.AddLayer(targetLayerName);
        targetStateMachine = targetController.layers[targetController.layers.Length - 1].stateMachine;
    }

    private void UpdateTransitions()
    /* 
     *  Updates all transition settings found in the face gesture layer.
     */
    {   
        foreach (AnimatorControllerLayer layer in targetController.layers) {
            if (layer.name == targetLayerName) {
                targetStateMachine = layer.stateMachine;
                break;
            }
        }

        foreach (AnimatorStateTransition transition in targetStateMachine.anyStateTransitions) {
            transition.canTransitionToSelf = false;
            transition.duration = transitionDuration;   
            transition.destinationState.writeDefaultValues = false;
        }
    }

    private void AmbidextrousLogic()
    /* 
     *  Generates ambidextrous logic.
     *  To perform a specific facial gesture the correct gesture indexes must be produced by either hand.
     *  
     *  (8 gestures indexes / 2) * (8 gestures indexes + 1) = 36 states.
     */
    {        

        for (int leftIndex = 0; leftIndex < 8; leftIndex++) {
            for (int rightIndex = 0; rightIndex < 8; rightIndex++) {

                if (leftIndex > rightIndex) 
                    continue;
                
                Vector3 location = new Vector3(
                    300 + leftIndex * 210, 
                    rightIndex * 50,
                    0.0f
                );
                                        
                AnimatorState state = targetStateMachine.AddState(
                    leftIndex.ToString() + rightIndex.ToString(),
                    location
                );   
                state.motion = defaultAnimation;
                state.writeDefaultValues = false;

                AnimatorStateTransition transitionA = targetStateMachine.AddAnyStateTransition(state);
                transitionA.canTransitionToSelf = false;
                transitionA.duration = transitionDuration;   
                transitionA.AddCondition(AnimatorConditionMode.Equals, leftIndex, leftGestureParameter);
                transitionA.AddCondition(AnimatorConditionMode.Equals, rightIndex, rightGestureParameter);   
                
                if (leftIndex != rightIndex) {
                    AnimatorStateTransition transitionB = targetStateMachine.AddAnyStateTransition(state);
                    transitionB.canTransitionToSelf = false;
                    transitionB.duration = transitionDuration;   
                    transitionB.AddCondition(AnimatorConditionMode.Equals, leftIndex, rightGestureParameter);
                    transitionB.AddCondition(AnimatorConditionMode.Equals, rightIndex, leftGestureParameter);
                }           
            }
        }
    }

    private void NonAmbidextrousLogic()
    /* 
     *  Generates non-ambidextrous logic. 
     *  To perform a specific facial gesture each hand must produce the correct gesture index.
     *  
     *  8 gestures indexes ^ 2 = 64 states.
     */
    {         
        for (int leftIndex = 0; leftIndex < 8; leftIndex++) {
            for (int rightIndex = 0; rightIndex < 8; rightIndex++) {
                
                Vector3 location = new Vector3(
                    300 + leftIndex * 210, 
                    rightIndex * 50,
                    0.0f
                );

                AnimatorState state = targetStateMachine.AddState(
                    leftIndex.ToString() + rightIndex.ToString(),                
                    location
                );   
                state.motion = defaultAnimation;
                state.writeDefaultValues = false;

                AnimatorStateTransition transition = targetStateMachine.AddAnyStateTransition(state);
                transition.canTransitionToSelf = false;
                transition.duration = transitionDuration;   
                transition.AddCondition(AnimatorConditionMode.Equals, leftIndex, leftGestureParameter);
                transition.AddCondition(AnimatorConditionMode.Equals, rightIndex, rightGestureParameter);      
            }
        }
    }
}
