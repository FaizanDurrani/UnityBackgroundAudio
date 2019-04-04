package com.Faizan.Github.BackgroundAudio;

import android.media.MediaPlayer;
import java.util.Timer;

public class MediaWrapper {
    public MediaPlayer player;
    public int instanceId;
    public boolean playing, paused;

    public MediaWrapper(int instanceId) {
        this.player = new MediaPlayer();
        this.instanceId = instanceId;
    }

    public void Reset(){
        this.player.reset();
        this.playing = false;
        this.paused = false;
    }
}
