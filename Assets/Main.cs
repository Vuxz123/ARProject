using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Main : MonoBehaviour
{
    public enum ChoiceState
    {
        None,
        Do1,
        Do2,
        Guilotine,
    }
    
    public ARRaycastManager raycastManager;
    
    public ARTrackedImageManager trackedImageManager;
    
    public UIDocument uiDocument;
    
    public GameObject do_1;
    public GameObject do_2;
    public GameObject guilotine;
    
    private bool _isChanging = false;
    private GameObject _currentObject;
    
    private ChoiceState _choiceState = ChoiceState.None;

    private void Start()
    {
        uiDocument.rootVisualElement.Q<Button>("do1").clicked += () => OnChoice(ChoiceState.Do1);
        uiDocument.rootVisualElement.Q<Button>("do2").clicked += () => OnChoice(ChoiceState.Do2);
        uiDocument.rootVisualElement.Q<Button>("guilotine").clicked += () => OnChoice(ChoiceState.Guilotine);
        uiDocument.rootVisualElement.Q<Button>("start").clicked += StartPlay;
    }

    private void Update()
    {
        if(trackedImageManager.trackables.count > 0)
        {
            foreach (var im in trackedImageManager.trackables)
            {
                if (_isChanging)
                {
                    var prefab = GetPrefab();
        
                    if(prefab == null) return;
                    
                    Debug.Log("Changing");
                    Destroy(_currentObject);
                    _currentObject = Instantiate(prefab, im.transform.position, prefab.transform.rotation);
                    _isChanging = false;
                }
                break;
            }
        }
    }
    
    void ListAllImages()
    {
        Debug.Log(
            $"There are {trackedImageManager.trackables.count} images being tracked.");

        foreach (var trackedImage in trackedImageManager.trackables)
        {
            Debug.Log($"Image: {trackedImage.referenceImage.name} is at " +
                      $"{trackedImage.transform.position}");
        }
    }

    private void OnEnable()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        //trackedImageManager.trackedImagesChanged += OnChange;
    }

    private void OnDisable()
    {
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
        Touch.onFingerDown -= OnFingerDown;
        //trackedImageManager.trackedImagesChanged -= OnChange;
    }

    private void StartPlay()
    {
        if (_currentObject == null) return;
        _currentObject.SendMessage("StartThis");
    }

    private void OnChange(ARTrackedImagesChangedEventArgs arTrackedImagesChangedEventArgs)
    {
        if(arTrackedImagesChangedEventArgs.updated.Count == 0) return;
        var im = arTrackedImagesChangedEventArgs.updated[0];
        
        var prefab = GetPrefab();
        
        if(prefab == null) return;

        if (_isChanging)
        {
            Debug.Log("Changing");
            Destroy(_currentObject);
            _currentObject = Instantiate(prefab, im.transform.position, prefab.transform.rotation);
            _isChanging = false;
        }
        
    }

    private void OnChoice(ChoiceState choiceState = ChoiceState.None)
    {
        if (_choiceState != choiceState)
        {
            Debug.Log($"Changing to {choiceState}");
            _isChanging = true;
        }
        _choiceState = choiceState;
    }

    private void OnFingerDown(Finger finger)
    {
        if(_choiceState == ChoiceState.None) return;
        if(finger.index != 0) return;

        if (!_isChanging) return;
        
        var hits = new List<ARRaycastHit>();
        
        if(raycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            var prefab = GetPrefab();
            Destroy(_currentObject);
            _currentObject = Instantiate(prefab, hitPose.position, prefab.transform.rotation);
        }
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        
        if(_choiceState == ChoiceState.None) return;
        if (!_isChanging) return;
        
        var hits = new List<ARRaycastHit>();
        
        if(raycastManager.Raycast(Mouse.current.position.ReadValue(), hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            var prefab = GetPrefab();
            Destroy(_currentObject);
            _currentObject = Instantiate(prefab, hitPose.position, prefab.transform.rotation);
        }
    }
    
    private GameObject GetPrefab()
    {
        return _choiceState switch
        {
            ChoiceState.Do1 => do_1,
            ChoiceState.Do2 => do_2,
            ChoiceState.Guilotine => guilotine,
            _ => null
        };
    }
}
