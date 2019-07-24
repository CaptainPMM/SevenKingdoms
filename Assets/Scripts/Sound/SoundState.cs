public class SoundState {
    private Sound sound;

    private float transitionNextVal;
    private SoundState nextSoundState;

    private float transitionBackVal;
    private SoundState backSoundState;

    /**
        Transition to next state if transitionNextVal >= a value provieded in TransitionPossible.
        Transition back to parent state if transitionBackVal < a value provieded in TransitionPossible.
     */
    public SoundState(Sound sound, float transitionNextVal, SoundState nextSoundState, float transitionBackVal, SoundState backSoundState) {
        this.sound = sound;
        sound.source.loop = false; // Disable loops for SoundState management

        this.transitionNextVal = transitionNextVal;
        this.nextSoundState = nextSoundState;

        this.transitionBackVal = transitionBackVal;
        this.backSoundState = backSoundState;
    }

    /**
        Transition to next state if transitionNextVal >= a value provieded in TransitionPossible.
        Transition back to parent state if transitionBackVal < a value provieded in TransitionPossible.
        Set next sound state later...
     */
    public SoundState(Sound sound, float transitionNextVal, float transitionBackVal, SoundState backSoundState) {
        this.sound = sound;
        sound.source.loop = false; // Disable loops for SoundState management

        this.transitionNextVal = transitionNextVal;
        // this.nextSoundState = nextSoundState; later...

        this.transitionBackVal = transitionBackVal;
        this.backSoundState = backSoundState;
    }

    public void SetNextSoundState(SoundState st) {
        nextSoundState = st;
    }

    /**
        Check if this state can be transitioned into another state.
        Returns:
            0 -> no transition
            1 -> transition to next sound state
            -1-> transition to back/parent sound state
     */
    public int TransitionPossible(float currVal) {
        if (!sound.source.isPlaying) {
            if (currVal >= transitionNextVal) {
                return 1;
            } else if (currVal < transitionBackVal) {
                return -1;
            } else {
                sound.source.Play();
            }
        }
        return 0;
    }

    public SoundState NextState() {
        sound.source.Stop();
        sound.source.loop = sound.loop;
        nextSoundState.Play();
        return nextSoundState;
    }

    public SoundState BackState() {
        sound.source.Stop();
        sound.source.loop = sound.loop;
        backSoundState.Play();
        return backSoundState;
    }

    public void Play() {
        sound.source.Play();
    }

    public void Stop() {
        sound.source.Stop();
    }
}