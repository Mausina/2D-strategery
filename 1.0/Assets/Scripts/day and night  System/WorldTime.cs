using System;
using System.Collections;
using UnityEngine;

namespace WorldTimeSystem
{
    public class WorldTime : MonoBehaviour
    {
        // Define the event using the generic EventHandler delegate with your custom EventArgs
        public event EventHandler<TimeSpan> WorldTimeChanged;

        [SerializeField]
        private float _dayLength; // 150 _dayLength

        private TimeSpan _currentTime;
        private float _minuteLength => _dayLength / WorlTimeConstans.MinutesInDay; // Make sure the constant is correctly named and accessible

        private void Start()
        {
            StartCoroutine(AddMinute());
        }

        private IEnumerator AddMinute()
        {
            while (true) // Using a loop to continuously add minutes without recursive StartCoroutine calls
            {
                _currentTime = _currentTime.Add(TimeSpan.FromMinutes(1));
                // Invoke the event with the custom EventArgs
               // Debug.Log($"Current Time: {_currentTime.ToString()}");
                WorldTimeChanged?.Invoke(this, _currentTime);

                yield return new WaitForSeconds(_minuteLength);
            }
        }
    }

    // Custom EventArgs class to hold the TimeSpan data
    
}
