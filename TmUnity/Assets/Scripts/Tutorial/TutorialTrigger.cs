using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TmUnity
{
    class TutorialTrigger : MonoBehaviour
    {
        public void ToggleTutorial()
        {
            var currentState = gameObject.activeSelf;
            gameObject.SetActive(!currentState);
        }
    }

}
