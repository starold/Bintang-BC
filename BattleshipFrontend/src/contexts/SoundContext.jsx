import { createContext, useContext, useRef, useCallback, useEffect } from 'react';
import { useSettings } from './SettingsContext';

// Import all audio files
import naroSawitSound from '../../asset/audio/naro sawit.mp3';
import naroPohonSound from '../../asset/audio/naro pohon.mp3';
import sawitNembakSound from '../../asset/audio/sawit nembak.mp3';
import hutanNembakSound from '../../asset/audio/hutan nembak.mp3';
import menangSound from '../../asset/audio/sound effect menang.mp3';
import homepageSoundtrack from '../../asset/audio/soundtrack homepage.mp3';
import perang1Soundtrack from '../../asset/audio/soundtrack perang 1.mp3';
import perang2Soundtrack from '../../asset/audio/soundtrack perang 2.mp3';
import perang3Soundtrack from '../../asset/audio/soundtrack perang 3.mp3';

const SoundContext = createContext(null);

const SOUND_EFFECTS = {
    naroSawit: naroSawitSound,
    naroPohon: naroPohonSound,
    sawitNembak: sawitNembakSound,
    hutanNembak: hutanNembakSound,
    menang: menangSound,
};

const SOUNDTRACKS = {
    homepage: homepageSoundtrack,
    perang1: perang1Soundtrack,
    perang2: perang2Soundtrack,
    perang3: perang3Soundtrack,
};

export function SoundProvider({ children }) {
    const { settings } = useSettings();
    const soundEffectsRef = useRef({});
    const soundtracksRef = useRef({});
    const currentSoundtrackRef = useRef(null);

    // Preload all sounds
    useEffect(() => {
        Object.entries(SOUND_EFFECTS).forEach(([key, src]) => {
            const audio = new Audio(src);
            audio.preload = 'auto';
            soundEffectsRef.current[key] = audio;
        });

        Object.entries(SOUNDTRACKS).forEach(([key, src]) => {
            const audio = new Audio(src);
            audio.preload = 'auto';
            audio.loop = true;
            soundtracksRef.current[key] = audio;
        });

        return () => {
            Object.values(soundEffectsRef.current).forEach(audio => {
                audio.pause();
                audio.src = '';
            });
            Object.values(soundtracksRef.current).forEach(audio => {
                audio.pause();
                audio.src = '';
            });
        };
    }, []);

    // Update volumes when settings change
    useEffect(() => {
        const masterVol = settings.masterVolume / 100;
        const sfxVol = settings.sfxVolume / 100;
        const musicVol = settings.musicVolume / 100;

        Object.values(soundEffectsRef.current).forEach(audio => {
            audio.volume = masterVol * sfxVol;
        });

        Object.values(soundtracksRef.current).forEach(audio => {
            audio.volume = masterVol * musicVol;
        });
    }, [settings.masterVolume, settings.sfxVolume, settings.musicVolume]);

    const playSound = useCallback((soundName) => {
        const audio = soundEffectsRef.current[soundName];
        if (audio) {
            audio.currentTime = 0;
            audio.play().catch(e => console.warn('Sound play failed:', e));
        }
    }, []);

    const playSoundtrack = useCallback((trackName) => {
        // Stop current soundtrack
        if (currentSoundtrackRef.current) {
            currentSoundtrackRef.current.pause();
            currentSoundtrackRef.current.currentTime = 0;
        }

        const audio = soundtracksRef.current[trackName];
        if (audio) {
            currentSoundtrackRef.current = audio;
            audio.play().catch(e => console.warn('Soundtrack play failed:', e));
        }
    }, []);

    const stopSoundtrack = useCallback(() => {
        if (currentSoundtrackRef.current) {
            currentSoundtrackRef.current.pause();
            currentSoundtrackRef.current.currentTime = 0;
            currentSoundtrackRef.current = null;
        }
    }, []);

    return (
        <SoundContext.Provider value={{
            playSound,
            playSoundtrack,
            stopSoundtrack,
            availableSoundtracks: Object.keys(SOUNDTRACKS).filter(k => k !== 'homepage')
        }}>
            {children}
        </SoundContext.Provider>
    );
}

export function useSound() {
    const context = useContext(SoundContext);
    if (!context) {
        throw new Error('useSound must be used within SoundProvider');
    }
    return context;
}
