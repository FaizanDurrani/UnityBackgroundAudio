package com.Faizan.Github.BackgroundAudio;

import android.app.Notification;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.media.MediaPlayer;
import android.net.Uri;
import android.os.Build;
import android.os.IBinder;
import android.support.v4.app.NotificationCompat;
import android.util.Log;

import java.io.Console;
import java.util.Date;
import java.util.HashMap;
import java.util.Objects;

import com.Faizan.Github.BackgroundAudio.MediaWrapper;
import com.Faizan.Github.BackgroundAudio.BackgroundAudioServiceCallback;

import static com.Faizan.Github.BackgroundAudio.App.NOTIF_CHANNEL_ID;

public class BackgroundAudioService extends Service {

    private static final String NOTIFICATION_ICON_NAME = "ba_notification_icon";

    private static final String EXTRA_UNITY_PACKAGE_ID = "unityPackageId";
    private static final String EXTRA_PATH = "audioPath";
    private static final String EXTRA_PENDING_INTENT = "mainActivityPendingIntent";
    private static final String EXTRA_TIME = "runningTime";
    private static final String EXTRA_CALLBACK = "callback";
    private static final String EXTRA_ICON = "notificationIcon";
    private static final String EXTRA_SEEK_TIME = "seekTime";
    private static final String EXTRA_INSTANCE_ID = "instanceId";
    private static final String EXTRA_VOLUME = "volume";
    private static final String EXTRA_LOOP = "loop";

    private static final String ACTION_SET_VOLUME = "volume";
    private static final String ACTION_SET_LOOP = "loop";
    private static final String ACTION_DISPOSE = "dispose";
    private static final String ACTION_START = "play";
    private static final String ACTION_STOP = "stop";
    private static final String ACTION_PAUSE = "pause";
    private static final String ACTION_RESUME = "resume";
    private static final String ACTION_SEEK = "seek";
    private static final String ACTION_STOP_SERVICE = "stopService";

    private static final String TAG = "BackgroundAudio";
    private volatile static HashMap<Integer, MediaWrapper> usedMediaPlayers = new HashMap<>();
    private volatile static HashMap<Integer, BackgroundAudioServiceCallback> baCallbacks = new HashMap<>();
    private static boolean serviceActive;

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        try {

            String action = intent.getAction();
            final int instanceId = intent.getIntExtra(EXTRA_INSTANCE_ID, -1);

            int seekDuration;
            final int currPosition;

            switch (action != null ? action : "") {
                case ACTION_START: {
                    final String path = intent.getStringExtra(EXTRA_PATH);

                    // Stop service pending intent
                    Intent stopServiceIntent = new Intent(this, BackgroundAudioService.class);
                    stopServiceIntent.setAction(ACTION_STOP_SERVICE);
                    PendingIntent pendingIntent = PendingIntent.getService(this, 0, stopServiceIntent, 0);

                    int icon = intent.getIntExtra(EXTRA_ICON, 0);

                    MediaWrapper wrapper;
                    MediaPlayer mediaPlayer;
                    if (usedMediaPlayers.containsKey(instanceId)) {
                        wrapper = usedMediaPlayers.get(instanceId);
                        wrapper.Reset();
                    } else {
                        wrapper = new MediaWrapper(instanceId);
                        usedMediaPlayers.put(instanceId, wrapper);
                    }

                    Uri myUri = Uri.parse(path);
                    mediaPlayer = wrapper.player;
                    mediaPlayer.setDataSource(this, myUri);
                    mediaPlayer.setOnCompletionListener((mp) -> {
                        if (isLooping(instanceId)) return;
                        disposeInstance(this, instanceId);
                    });
                    mediaPlayer.prepare();
                    mediaPlayer.start();

                    wrapper.playing = true;
                    wrapper.paused = false;

                    Notification notification;
                    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.O) {
                        notification = new NotificationCompat.Builder(this)
                                .setContentTitle("Audio Playing")
                                .setContentText("Tap to stop audio")
                                .setSmallIcon(icon)
                                .setContentIntent(pendingIntent)
                                .build();
                    } else {
                        notification = new Notification.Builder(this, NOTIF_CHANNEL_ID)
                                .setContentTitle("Audio Playing")
                                .setContentText("Tap to stop audio")
                                .setSmallIcon(icon)
                                .setContentIntent(pendingIntent)
                                .build();
                    }

                    serviceActive = true;
                    baCallbacks.get(instanceId).BackgroundAudioStarted();
                    startForeground(1, notification);
                    break;
                }
                case ACTION_PAUSE: {

                    if (!usedMediaPlayers.containsKey(instanceId)) break;
                    MediaWrapper wrapper = usedMediaPlayers.get(instanceId);

                    if (wrapper.player == null || !wrapper.player.isPlaying()) break;

                    wrapper.player.pause();
                    wrapper.playing = false;
                    wrapper.paused = true;

                    baCallbacks.get(instanceId).BackgroundAudioPaused();
                    break;
                }
                case ACTION_RESUME: {
                    if (!usedMediaPlayers.containsKey(instanceId)) break;
                    MediaWrapper wrapper = usedMediaPlayers.get(instanceId);

                    if (wrapper.player == null || wrapper.player.isPlaying()) break;

                    wrapper.player.start();
                    wrapper.playing = true;
                    wrapper.paused = false;

                    baCallbacks.get(instanceId).BackgroundAudioResumed();
                    break;
                }
                case ACTION_SEEK: {
                    if (!usedMediaPlayers.containsKey(instanceId)) break;
                    MediaWrapper wrapper = usedMediaPlayers.get(instanceId);

                    if (!wrapper.playing && !wrapper.paused) break;

                    seekDuration = intent.getIntExtra(EXTRA_SEEK_TIME, 15000);
                    currPosition = getCurrentPosition(instanceId);
                    int maxDuration = getDuration(instanceId);
                    int seekFwd = currPosition + seekDuration;

                    seekFwd = seekFwd < 0 ? 0 : seekFwd;
                    seekFwd = seekFwd > maxDuration ? maxDuration : seekFwd;

                    wrapper.player.seekTo(seekFwd);
                    break;
                }
                case ACTION_STOP: {
                    if (!usedMediaPlayers.containsKey(instanceId)) break;
                    MediaWrapper wrapper = usedMediaPlayers.get(instanceId);

                    wrapper.player.stop();
                    wrapper.player.reset();

                    baCallbacks.get(instanceId).BackgroundAudioStopped();
                    break;
                }
                case ACTION_DISPOSE: {
                    if (!usedMediaPlayers.containsKey(instanceId)) break;
                    MediaWrapper wrapper = usedMediaPlayers.get(instanceId);

                    try {
                        wrapper.playing = false;
                        wrapper.paused = false;
                        wrapper.player.release();

                        usedMediaPlayers.remove(instanceId);

                        baCallbacks.get(instanceId).BackgroundAudioStopped();
                        if (usedMediaPlayers.size() <= 0) {
                            serviceActive = false;
                            stopSelf();
                        }

                    } catch (Exception ignored) {
                        Log.e(TAG, "EXCEPTION OCCURRED WHILE DISPOSING", ignored);
                    }
                    break;
                }
                case ACTION_SET_VOLUME: {
                    if (!usedMediaPlayers.containsKey(instanceId)) break;
                    MediaWrapper wrapper = usedMediaPlayers.get(instanceId);

                    float volume = intent.getFloatExtra(EXTRA_VOLUME, 1f);

                    try {
                        wrapper.player.setVolume(volume, volume);
                    } catch (Exception e) {
                        Log.d(TAG, e.getMessage());
                        throw e;
                    }

                    break;
                }
                case ACTION_SET_LOOP: {
                    if (!usedMediaPlayers.containsKey(instanceId)) break;
                    MediaWrapper wrapper = usedMediaPlayers.get(instanceId);

                    boolean loop = intent.getBooleanExtra(EXTRA_LOOP, false);

                    try {
                        wrapper.player.setLooping(loop);
                    } catch (Exception e) {
                        Log.d(TAG, e.getMessage());
                        throw e;
                    }

                    break;
                }
                case ACTION_STOP_SERVICE: {

                    for (MediaWrapper wrapper : usedMediaPlayers.values()) {
                        if (wrapper.player.isPlaying()) {
                            wrapper.player.stop();
                            wrapper.player.release();
                            baCallbacks.get(wrapper.instanceId).BackgroundAudioStopped();
                        }
                    }

                    usedMediaPlayers.clear();
                    serviceActive = false;
                    stopSelf();

                    break;
                }
            }


        } catch (Exception e) {
            e.printStackTrace();
        }

        return START_NOT_STICKY;
    }

    @Override
    public void onDestroy() {
        super.onDestroy();

        for (MediaWrapper wrapper :
                usedMediaPlayers.values()) {
            try {
                wrapper.player.stop();
                wrapper.player.reset();
            } catch (Exception ignored) {
            }


            wrapper.playing = false;
            wrapper.paused = false;
        }

        usedMediaPlayers.clear();
    }

    public static void initialize(int id, BackgroundAudioServiceCallback callback) {
        baCallbacks.put(id, callback);
    }

    public static void uninitialize(int id) {
        baCallbacks.remove(id);
    }

    public static void play(Context context, int id, String path, String packageId) {
        Intent intent = new Intent(context, BackgroundAudioService.class);

        intent.putExtra(EXTRA_INSTANCE_ID, id);
        intent.setAction(ACTION_START);
        intent.putExtra(EXTRA_UNITY_PACKAGE_ID, packageId);
        intent.putExtra(EXTRA_PATH, path);

        Resources res = context.getResources();
        int icon = res.getIdentifier(NOTIFICATION_ICON_NAME, "drawable", packageId);

        intent.putExtra(EXTRA_ICON, icon);

        serviceActive = true;

        context.startService(intent);
    }

    public static void pause(Context context, int id) {
        Intent intent = new Intent(context, BackgroundAudioService.class);
        intent.putExtra(EXTRA_INSTANCE_ID, id);
        intent.setAction(ACTION_PAUSE);

        context.startService(intent);
    }

    public static void resume(Context context, int id) {
        Intent intent = new Intent(context, BackgroundAudioService.class);
        intent.putExtra(EXTRA_INSTANCE_ID, id);
        intent.setAction(ACTION_RESUME);

        context.startService(intent);
    }

    public static void disposeInstance(Context context, int id) {
        try {
            Log.d(TAG, "disposeInstance: CALLED");
            Intent intent = new Intent(context, BackgroundAudioService.class);
            intent.putExtra(EXTRA_INSTANCE_ID, id);
            intent.setAction(ACTION_DISPOSE);

            context.startService(intent);
        } catch (Exception e) {
            Log.e(TAG, "disposeInstance: ", e);
        }
    }

    public static void stopService(Context context) {
        Intent intent = new Intent(context, BackgroundAudioService.class);
        intent.setAction(ACTION_STOP_SERVICE);

        context.startService(intent);
    }

    public static boolean isPlaying(int id) {
        return usedMediaPlayers.containsKey(id) && usedMediaPlayers.get(id).playing;
    }

    public static boolean isPaused(int id) {
        return usedMediaPlayers.containsKey(id) && usedMediaPlayers.get(id).paused;
    }

    public static boolean isLooping(int id) {
        return usedMediaPlayers.containsKey(id) && usedMediaPlayers.get(id).player.isLooping();
    }

    public static int getCurrentPosition(int id) {
        if (!usedMediaPlayers.containsKey(id)) return 0;

        MediaWrapper wrapper = usedMediaPlayers.get(id);
        MediaPlayer mediaPlayer = wrapper.player;

        if (mediaPlayer != null) {
            if (!wrapper.playing && !wrapper.paused) {
                return 0;
            }
        } else {
            return 0;
        }

        return mediaPlayer.getCurrentPosition();
    }

    public static int getDuration(int id) {
        if (!usedMediaPlayers.containsKey(id)) return -1;
        android.util.Log.d(TAG, "getDuration: KEY EXISTS");

        MediaWrapper wrapper = usedMediaPlayers.get(id);
        MediaPlayer mediaPlayer = wrapper.player;

        if (mediaPlayer != null) {
            if (!wrapper.playing && !wrapper.paused) {
                return -1;
            }
        } else {
            return -1;
        }

        return mediaPlayer.getDuration() - 1;
    }

    public static void seek(Context context, int id, int milliseconds) {
        Intent intent = new Intent(context, BackgroundAudioService.class);
        intent.putExtra(EXTRA_INSTANCE_ID, id);
        intent.setAction(ACTION_SEEK);
        intent.putExtra(EXTRA_SEEK_TIME, milliseconds);

        context.startService(intent);
    }

    public static void setVolume(Context context, int id, float vol) {
        Intent intent = new Intent(context, BackgroundAudioService.class);
        intent.putExtra(EXTRA_INSTANCE_ID, id);
        intent.setAction(ACTION_SET_VOLUME);
        intent.putExtra(EXTRA_VOLUME, vol);

        context.startService(intent);
    }

    public static void setLoop(Context context, int id, boolean value) {
        Intent intent = new Intent(context, BackgroundAudioService.class);
        intent.putExtra(EXTRA_INSTANCE_ID, id);
        intent.setAction(ACTION_SET_LOOP);
        intent.putExtra(EXTRA_LOOP, value);

        context.startService(intent);
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }
}