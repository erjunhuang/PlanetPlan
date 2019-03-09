using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InputMethod
{
    KeyboardInput,
    MouseInput,
    TouchInput
}
public class PlayerInputManager : MonoBehaviour {
    private float inputHorizontal, inputVertical;
    private Vector3 dir;

    public bool isSelfDrive = false;
    public InputMethod inputType = InputMethod.KeyboardInput;

    private PlayerController currentPlayer ;
    private void Start()
    {
    }
    // Use this for initialization
    void Update () {

        if (inputType == InputMethod.KeyboardInput)
            KeyboardInput();
        else if (inputType == InputMethod.MouseInput)
            MouseInput();
        else if (inputType == InputMethod.TouchInput)
            TouchInput();
    }

    #region KEYBOARD
    private void KeyboardInput()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        dir = new Vector3(0, 0, inputVertical).normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
         
        if (currentPlayer && !currentPlayer.isAI)
        {
            if (!isSelfDrive)
            {
                dir = Vector3.forward;
            }
            currentPlayer.move(dir, Time.fixedDeltaTime);
            currentPlayer.rotation(inputHorizontal, Time.fixedDeltaTime);
        }
    }
    #endregion

    bool isLongMove;
    Vector2 _startPressPosition;
    Vector2 _endPressPosition;
    float _buttonDownPhaseStart;
    Vector2 _currentSwipe;
    public float tapInterval;
    public int OneScreenMove = 6;

    #region TOUCH
    private void TouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _startPressPosition = touch.position;
                _endPressPosition = touch.position;
                _buttonDownPhaseStart = Time.time;
                isLongMove = false;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                isLongMove = true;

                _buttonDownPhaseStart = Time.time;
                //save ended touch 2d point
                _endPressPosition = new Vector2(touch.position.x, touch.position.y);

                //create vector from the two points
                _currentSwipe = new Vector2(_endPressPosition.x - _startPressPosition.x, _endPressPosition.y - _startPressPosition.y);

                //模拟键盘 Input.GetAxisRaw("Horizontal");
                _currentSwipe.Normalize();
                if (_currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                {
                    _currentSwipe.x = 1;
                }
                if (_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                {
                    _currentSwipe.x = -1;
                }
                if (_currentSwipe.x == 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                {
                    _currentSwipe.x = inputHorizontal;
                }
                _startPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                inputHorizontal = _currentSwipe.x;

                ////长按滑动屏幕的距离大于 1/OneScreenMove
                //if (Mathf.Abs(_currentSwipe.x) > Screen.width / OneScreenMove)
                //{
                     
                //    _startPressPosition = touch.position;

                //    //normalize the 2d vector
                //    _currentSwipe.Normalize();

                //    //swipe left
                //    if (_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                //    {
                //        print("左移");
                //    }
                //    //swipe right
                //    if (_currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                //    {
                //        print("右移");
                //    }
                //}
            }
            if (touch.phase == TouchPhase.Ended)
            {
                inputHorizontal = 0;
                if (Time.time - _buttonDownPhaseStart > 0 && !isLongMove)
                {
                    //save ended touch 2d point
                    _endPressPosition = new Vector2(touch.position.x, touch.position.y);

                    //create vector from the two points
                    _currentSwipe = new Vector2(_endPressPosition.x - _startPressPosition.x, _endPressPosition.y - _startPressPosition.y);

                    //normalize the 2d vector
                    _currentSwipe.Normalize();

                    //swipe left
                    if (_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                    {
                        print("左滑");
                    }
                    //swipe right
                    if (_currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                    {
                        print("右滑");
                    }

                    //swipe up
                    if (_currentSwipe.y > 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
                    {
                        print("上滑");
                    }

                    //swipe down
                    if (_currentSwipe.y < 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
                    {
                        print("下滑");
                    }

                    if (_currentSwipe.x == 0 && _currentSwipe.y == 0)
                    {
                        print("点击了一下");
                    }
                }
            }
        }
    }
    #endregion

    #region MOUSE
    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
             _startPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _buttonDownPhaseStart = Time.time;
        }
        if (Input.GetMouseButton(0)) {
            isLongMove = true;
            _buttonDownPhaseStart = Time.time;
            //save ended touch 2d point
            _endPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            _currentSwipe = new Vector2(_endPressPosition.x - _startPressPosition.x, _endPressPosition.y - _startPressPosition.y);

            //模拟键盘 Input.GetAxisRaw("Horizontal");
            _currentSwipe.Normalize();
            if (_currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
            {
                _currentSwipe.x = 1;
            }
            if (_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
            {
                _currentSwipe.x = -1;
            }
            if (_currentSwipe.x == 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
            { 
                _currentSwipe.x = inputHorizontal;
            }
            _startPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            inputHorizontal = _currentSwipe.x;

            ////长按滑动屏幕的距离大于 1/OneScreenMove
            //if (Mathf.Abs(_currentSwipe.x) > Screen.width / OneScreenMove)
            //{
            //    _startPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //    //normalize the 2d vector
            //    _currentSwipe.Normalize();

            //    //swipe left
            //    if (_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
            //    {
            //        print("左移");
            //    }
            //    //swipe right
            //    if (_currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
            //    {
            //        print("右移");
            //    }
            //}
        }
        if (Input.GetMouseButtonUp(0))
        {
            inputHorizontal = 0;
            if (Time.time - _buttonDownPhaseStart > tapInterval && !isLongMove)
            {   
                //save ended touch 2d point
                _endPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                //create vector from the two points
                _currentSwipe = new Vector2(_endPressPosition.x - _startPressPosition.x, _endPressPosition.y - _startPressPosition.y);

                //normalize the 2d vector
                _currentSwipe.Normalize();
               
                //swipe left
                if (_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                {
                    print("左滑");
                }
                //swipe right
                if (_currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                {
                    print("右滑");
                }
                //swipe up
                if (_currentSwipe.y > 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
                {
                    print("上滑");
                }
                //swipe down
                if (_currentSwipe.y < 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
                {
                    print("下滑");
                }
                if (_currentSwipe.x == 0 && _currentSwipe.y == 0)
                {
                    print("点击了一下");
                }
            }
        }
    }
    #endregion
 
}
