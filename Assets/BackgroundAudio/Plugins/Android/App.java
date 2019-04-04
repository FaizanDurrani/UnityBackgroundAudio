package com.Faizan.Github.BackgroundAudio;

import android.app.Application;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.os.Build;
import android.util.Log;

public class App extends Application {

    public static final String TAG = "Application";
    public static final String NOTIF_CHANNEL_ID = "audioNotifChannel";

    @Override
    public void onCreate() {
        super.onCreate();

        Log.d(TAG, "onCreate() was called!");

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.O) return;

        NotificationChannel channel = new NotificationChannel(
                NOTIF_CHANNEL_ID,
                "Audio Notification",
                NotificationManager.IMPORTANCE_LOW
        );

        NotificationManager manager = getSystemService(NotificationManager.class);
        if (manager != null) {
            manager.createNotificationChannel(channel);
        }
    }
}
