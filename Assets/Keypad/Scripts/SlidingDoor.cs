using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NavKeypad
{
    public class SlidingDoor : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        public bool IsOpen => isOpen;
        private bool isOpen = false;

        public void ToggleDoor()
        {
            isOpen = !isOpen;
            if (anim != null) anim.SetBool("isOpen", isOpen);
        }

        public void OpenDoor()
        {
            isOpen = true;
            if (anim != null) anim.SetBool("isOpen", isOpen);
        }
        public void CloseDoor()
        {
            isOpen = false;
            if (anim != null) anim.SetBool("isOpen", isOpen);
        }
    }
}