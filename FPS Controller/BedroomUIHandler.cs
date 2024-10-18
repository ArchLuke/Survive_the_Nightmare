using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BedroomUIHandler : MonoBehaviour
{
    [Header("Load")]
    [SerializeField] private GameObject _ui;
    [SerializeField] private Thunder _light;
    [SerializeField] private GameObject _rain;
    [SerializeField] private GameObject _bar;
    [SerializeField] private GameObject _loadText;
    [SerializeField] private GameObject _loadFade;
    
    
    [SerializeField] private GameObject 		_fade =	null;
    [SerializeField] private float _fadeTime = 1.5f;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color _pressedCol;
    [SerializeField] private GameObject _carrot;

    private IEnumerator _coroutine = null;
    private Image _screenFade = null; 
    private Color _col = new Color();
    private Color _originalCol;
    public void Start()
    {
        _originalCol = _text.color;
        _screenFade = _fade.GetComponent<Image>();
        if (_screenFade != null)
            _col = _screenFade.color;
    }

    public void Hover()
    {
        _carrot.SetActive(true);
    }
    public void UnHover()
    {
        _carrot.SetActive(false);
    }
    public void FadeOut ()
    {
        _fade.SetActive(true);
        _coroutine = FadeInternal(1f);
        StartCoroutine(_coroutine);
    }


    IEnumerator FadeInternal(float targetFade )
    {
        
        _text.color = _pressedCol;

        if (!_screenFade) yield break;
        
        float timer = 0;
        float _currentFadeLevel = 0f;

        while (timer<_fadeTime)
        {
            if (timer > 0.128f)
                _text.color = _originalCol;
            timer+=Time.deltaTime;
            _currentFadeLevel = Mathf.Lerp( 0, targetFade, timer/_fadeTime );
            _col.a = _currentFadeLevel;
            _screenFade.color = _col;
            yield return null;
        }
        gameObject.SetActive(false);
        if (ApplicationManager.instance) ApplicationManager.instance.LoadGame(_ui, _light,  _rain,  _bar,  _loadText,  _loadFade);
        
    }
    
}