using UnityEngine;

namespace Assets.Scripts.Player
{
    public static class AnimationHashes
    {
        public static int POLE_BACK = Animator.StringToHash("Pole_Back_Blend Tree");
        public static int CASTING_FISHING = Animator.StringToHash("Casting_Blend Tree");
        public static int WAIT_IDLE_FISHING = Animator.StringToHash("Wait_Idle_Fishing_Blend Tree");
        public static int CAPTURE_NOFISH = Animator.StringToHash("Capture_NoFish_Blend Tree");
        public static int IDLE = Animator.StringToHash("Idle_Blend Tree");
        public static int ROLL = Animator.StringToHash("Roll_Blend Tree");
        public static int FISH_HOOK_BLEND_TREE = Animator.StringToHash("Fish_Hook_Blend Tree");
        public static int BOBBER_FISH = Animator.StringToHash("BobberFish");
    }
}
