import React from 'react'
import {AudioMetadataWithId} from '../../models/AudioMetadata';
import {StreamPlayer} from './streamPlayer';
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faPlay, faPause} from "@fortawesome/free-solid-svg-icons";

interface IProps {
    metadata: AudioMetadataWithId | null;
    hasNext: boolean;
    hasPrev: boolean;
}

export type TPlaybackStatus = 'stopped' | 'paused' | 'playing';

export const Player = (props: IProps) => {
    const [downloadProgress, setdownloadProgress] = React.useState<number>(0);
    const [playerProgress, setPlayerProgress] = React.useState<number>(0);
    const [status, setStatus] = React.useState<TPlaybackStatus>('stopped');
    
    React.useEffect(() => {
        setdownloadProgress(0);
        setPlayerProgress(0);
        setStatus('stopped');
    }, []);

    const setDownloadProgress = (p: number) => {
        if (p > 100) setdownloadProgress(100);
        else setdownloadProgress(p)
    }
    const setPlaybackProgress = (p: number) => {
        // possible with AudioContext since it does not stop playback after 
        // all the chunks played.
        if (p > 100) setPlayerProgress(100);
        else setPlayerProgress(p);
    }

    const setPlaybackStatus = (p: TPlaybackStatus) => {
        setStatus(p);
    }

    const onPause = () => {
        if (props.metadata !== null && status === 'playing') setStatus('paused');
    }

    const onResume = () => {
        if (props.metadata !== null && status === 'paused') setStatus('playing');
    }


    return (
        <div style={{
            width: '100%',
            position: 'absolute',
            bottom: 24,
            display: 'flex',
            flexDirection: 'column',
            height: 60,
            backgroundColor: 'white'
        }}>

            <div style={{
                width: '100%', flex: 'auto', display: 'flex', flexDirection: 'row', justifyContent: 'center',
                alignContent: 'center',
            }}>
                {
                    status === 'playing' ? <FontAwesomeIcon icon={faPause} onClick={onPause}/>
                        : <FontAwesomeIcon icon={faPlay} onClick={onResume}/>
                }
                <span>{props.metadata?.performers} - {props.metadata?.title}</span>
            </div>
            <StreamPlayer
                metadata={props.metadata}
                status={status}
                onDownloadProgress={setDownloadProgress}
                onPlaybackProgress={setPlaybackProgress}
                onPlaybackStatus={setPlaybackStatus}
            />
            <div style={{height: 10, width: `${playerProgress}%`, backgroundColor: '#f87171'}}/>
            <div style={{height: 10, width: `${downloadProgress}%`, backgroundColor: '#9ca3af'}}/>
        </div>
    )
}
