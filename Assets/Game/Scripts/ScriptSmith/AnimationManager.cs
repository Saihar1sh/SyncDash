using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Arixen.ScriptSmith
{
    public class AnimationManager
    {
        private Animator animator;

        private string currentAnimationState;

        public AnimationManager(Animator animator)
        {
            this.animator = animator;
        }

        #region String Based

        public void TransitionToAnimation(string animationName, float transitionDuration = 0.1f)
        {
            if (IsAnimationValid(animationName: animationName) == AnimationError.None)
            {
                animator.CrossFade(animationName, transitionDuration);
            }
        }

        public void SetFloat(string paramName, float value)
        {
            animator.SetFloat(paramName, value);
        }

        public void SetTrigger(string paramName)
        {
            animator.SetTrigger(paramName);
        }

        public void SetBool(string paramName, bool value)
        {
            animator.SetBool(paramName, value);
        }

        #endregion

        #region Hash Based

        public void TransitionToAnimation(int animHash, float transitionDuration = 0.1f)
        {
            if (IsAnimationValid(animHash: animHash) == AnimationError.None)
            {
                animator.CrossFade(animHash, transitionDuration);
            }
        }

        public void SetFloat(int hashValue, float value)
        {
            animator.SetFloat(hashValue, value);
        }

        public void SetTrigger(int hashValue)
        {
            animator.SetTrigger(hashValue);
        }

        public void SetBool(int hashValue, bool value)
        {
            animator.SetBool(hashValue, value);
        }

        #endregion

        private AnimationError IsAnimationValid(string animationName = "", int animHash = -1, int layer = 0)
        {
            AnimationError animationError = AnimationError.None;
            if (string.IsNullOrEmpty(animationName) || animHash == -1)
            {
                return animationError = AnimationError.AnimationNotFound;
            }

            List<AnimatorClipInfo> currentClipInfoList = new List<AnimatorClipInfo>();
            animator.GetCurrentAnimatorClipInfo(layer, currentClipInfoList);

            string currentAnimaton = string.Empty;

            if (currentClipInfoList.Any())
            {
                // Access the current Animation clip name if we are currently playing one.
                var currentClipInfo = currentClipInfoList.FirstOrDefault();

                if (currentClipInfo.clip != null)
                {
                    currentAnimaton = currentClipInfo.clip.name;
                }
            }

            // Check if we are already playing the newAnimation.
            if (string.Equals(currentAnimaton, animationName))
            {
                animationError = AnimationError.AnimationAlreadyPlaying;
            }
            // Check if the given Animator actually has the newAnimation at all.
            else if (!AnimationExists(animationName))
            {
                animationError = AnimationError.AnimationNotFound;
            }

            return animationError;
        }

        private bool AnimationExists(string newAnimation)
        {
            // Fetch all Animations from the given Animator.
            AnimationClip[] allclips = animator.runtimeAnimatorController.animationClips;

            // Check if any Animations in the Animator have the given clip name. 
            return allclips.Any(animation => string.Equals(animation.name, newAnimation));
        }

        public float GetCurrentAnimationLength(int layer = 0)
        {
            // Fetch the current Animation clips information for the base layer.
            AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(layer);

            // Access the current Animation clip length.
            return currentClipInfo[0].clip.length;
        }

        public enum AnimationError
        {
            None,
            AnimationNotFound,
            AnimationAlreadyPlaying,
        }
    }
}