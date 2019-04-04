//
//  AudioInstance.swift
//  Unity-iPhone
//
//  Created by Home on 31/03/2019.
//

import AVFoundation;
import Foundation;

class AudioInstance: AVAudioPlayer, AVAudioPlayerDelegate {
    
    private var _instanceId : Int?;
    
    convenience init(contentsOfURL url: URL, withId instanceId: Int) throws {
        do{
            try self.init(contentsOf: url);
            _instanceId = instanceId;
            delegate = self;
        } catch {
            throw error;
        }
    }
    
    func audioPlayerDidFinishPlaying(_ player: AVAudioPlayer, successfully flag: Bool) {
        
        guard let instanceId = _instanceId else {
            print("\(#function) called on an audioplayer with no InstanceId");
            return;
        }
        
        UnityBackgroundAudio.dispose(instanceId: instanceId);
    }
}
