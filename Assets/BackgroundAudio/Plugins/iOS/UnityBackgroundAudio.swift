//
//  UnityBackgroundAudio.swift
//  UnityBackgroundAudio
//
//  Created on 30/03/2019.
//  Copyright © 2019 Faizan Durrani. All rights reserved.
//

import UIKit
import AVFoundation

@objc public class UnityBackgroundAudio: NSObject {
    
    private static var _players = [Int: AudioInstance]();
    
    @objc public static func initialize(_ instanceId: Int) {
        do{
            let audioSession = AVAudioSession.sharedInstance();
            try audioSession.setCategory(AVAudioSession.Category.playback);
            try audioSession.setActive(true);
        } catch {
            print (error);
        }
        print("Successfully initialized player with Id \(instanceId) \n");
    }
    
    @objc public static func play (onInstance instanceId: Int, fromUrl path: String) {
        print("Trying to play from path: \(path)\n");
        do{
            _players.updateValue(try AudioInstance.init(contentsOfURL: URL(fileURLWithPath: path), withId: instanceId), forKey: instanceId);
            let player = _players[instanceId]!;
            player.enableRate=true;
            if player.prepareToPlay(){
                player.play();
                print("Playing instance \(instanceId)");
            } else {
                print("Unable to prepare the instance \(instanceId) for playing audio");
                dispose(instanceId: instanceId);
            }
        } catch {
            print(error);
            dispose(instanceId: instanceId);
        }
    }
    
    @objc public static func dispose (instanceId: Int){
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return;
        }
        player.stop();
        _players.removeValue(forKey: instanceId);
    }
    
    @objc public static func pause(instanceId: Int){
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return;
        }
        
        if !isPlaybackOver(onPlayer: player) && player.isPlaying{
            player.pause();
        }
    }
    
    @objc public static func resume(instanceId: Int){
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return;
        }
        
        if !isPlaybackOver(onPlayer: player) {
            player.play();
        }
    }
    
    @objc public static func seek(instanceId: Int, forSeconds time: Float){
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return;
        }
        
        let seekTo = player.currentTime.advanced(by: Double(time));
        
        if seekTo < 0 {
            player.currentTime = 0;
        }
        else if seekTo > player.duration {
            player.currentTime = player.duration;
        }
        else {
            player.currentTime = seekTo;
        }
    }
    
    @objc public static func getDuration(forInstanceId instanceId: Int)->Float{
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return 0;
        }
        return Float(player.duration);
    }
    
    @objc public static func getCurrentPosition(forInstanceId instanceId: Int)->Float{
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return 0;
        }
        return Float(player.currentTime);
    }
    
    @objc public static func setVolume(forInstanceId instanceId: Int, to volume: Float){
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return;
        }
        player.volume = volume;
    }
    
    @objc public static func setSpeed(forInstanceId instanceId: Int, to speed: Float){
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return;
        }
        player.rate = speed;
        //player.prepareToPlay();
    }
    
    @objc public static func setLoop(forInstanceId instanceId: Int, to value: Bool){
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return;
        }
        player.numberOfLoops = value == true ? -1 : 0;
    }
    
    @objc public static func isPlaying(onInstanceId instanceId: Int) -> Bool {
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return false;
        }
        return !isPlaybackOver(onPlayer: player);
    }
    
    @objc public static func isLooping(onInstanceId instanceId: Int) -> Bool {
        guard let player = _players[instanceId] else {
            print("[\(#function)] Could not find instance \(instanceId)");
            return false;
        }
        return (player.numberOfLoops != 0);
    }
    
    private static func isPlaybackOver(onPlayer player: AudioInstance) -> Bool{
        return !player.isPlaying && (player.currentTime == player.duration && player.duration > 0 && player.numberOfLoops == 0);
    }
}
