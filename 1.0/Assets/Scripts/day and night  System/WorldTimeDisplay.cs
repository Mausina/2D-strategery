using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WorldTime
{
    [RequireComponent(typeof(TMP_Text))]
    public class WorldTimeDisplay : MonoBehaviour
    {
        [SerializeField]
        private WorldTime _worldTime;
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _worldTime.WorldTimeChanged += OnWorldTimeChangedl;
        }

        private void OnDestroy()
        {
            _worldTime.WorldTimeChanged -= OnWorldTimeChangedl;
        }

        private void OnWorldTimeChangedl(object sender, TimeSpan newTime) 
        { 
           _text.SetText(newTime.ToString(@"hh/:mm"));
        }
    }
}

