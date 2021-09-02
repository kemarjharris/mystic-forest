using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputViewSection : MonoBehaviour
{
    public GameObject arrow;
    public GameObject lightButton;
    public GameObject heavyButton;
    public GameObject jumpButton;
    public Text frameText;
    private int currentFrame;
    public bool count;

    public State state;

    private void Awake()
    {
        state = new State();
        count = true;
    }


    // Update is called once per frame
    public void UpdateState(State state)
    {

        if (!count) return;
        float angle = state.dir;
        if (angle < 0)
        {
            arrow.SetActive(false);
        }
        else
        {
            arrow.SetActive(true);
            angle = state.dir;
            arrow.transform.eulerAngles = new Vector3(0, 0, angle);
        }

        lightButton.SetActive(state.light);
        heavyButton.SetActive(state.heavy);
        jumpButton.SetActive(state.jump);

        if (currentFrame < 999)
        {
            currentFrame++;
            frameText.text = currentFrame.ToString();
        }
    }

    public class State
    {
        private float frame;
        public float dir = -1;
        public bool light;
        public bool heavy;
        public bool jump;

        public State(float dir, bool light, bool heavy, bool jump)
        {
            frame = Time.frameCount;
            this.dir = dir;
            this.light = light;
            this.heavy = heavy;
            this.jump = jump;
        }

        public State() : this(-1, false, false, false) { }

        public State UpdateKey(KeyCode lightKey, KeyCode heavyKey, KeyCode jumpKey)
        {
            if (frame == Time.frameCount) return this;
            dir = DirectionalInput.Angle();
            if (dir > -1)
            {
                dir = 45 * Mathf.FloorToInt(dir / 45);
            }
            
            light = Input.GetKey(lightKey);
            heavy = Input.GetKey(heavyKey);
            jump = Input.GetKey(jumpKey);
            frame = Time.frameCount;

            return this;
        }

        public override bool Equals(object obj)
        {
            try
            {
                State state = (State) obj;
                return state.dir == dir && light == state.light && heavy == state.heavy && state.jump == jump;

            } catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("jump: {0}, dir: {1}, light: {2}, heavy: {3}", jump, dir, light, heavy);
        }
    }
}
