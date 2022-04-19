import React from 'react'
import { AudioMetadataWithId } from '../../models/AudioMetadata';
import { TPlaybackStatus } from './player';

interface IProps {
    metadata: AudioMetadataWithId | null;
    status: TPlaybackStatus;
    onDownloadProgress: (p: number) => void;
    onPlaybackProgress: (p: number) => void;
    onPlaybackStatus: (p: TPlaybackStatus) => void;
}

export const StreamPlayer = (props: IProps) => {
    const ctx = React.useRef<AudioContext>(new AudioContext());
    const internalStatus = React.useRef<TPlaybackStatus>(props.status);

    React.useEffect(() => {
        if (props.status === internalStatus.current) return;
        switch (props.status) {
            case 'paused':
                ctx.current.suspend();
                internalStatus.current = 'paused';
                break;
            case 'playing':
                ctx.current.resume();
                internalStatus.current = 'playing';
                break;
            case 'stopped':
                break;
            default:
                break;
        }
    }, [props.status]);

    React.useEffect(() => {
        if (props.metadata === null) return;
        const playbackTimeout = setInterval(() => {
            props.onPlaybackProgress(ctx.current.currentTime * 1000 / props.metadata!.duration * 100);
        }, 1000);
        if (ctx.current.state !== 'closed') {
            let oldContext = ctx.current;
            ctx.current = new AudioContext();
            ctx.current.suspend().then();
            oldContext.close();
        }

        fetch(`/api/audio/stream/${props.metadata.audioId}`, { method: 'get' })
            .then(response => {
                const contentLength = parseInt(response.headers.get('Content-Length') ?? '1');
                if (response.body === null) throw new Error();
                const reader = response.body.getReader();
                return new ReadableStream({
                    async start(controller) {
                        let collector = new Uint8Array(0);
                        let time = 0;
                        let consumed = 0;
                        let lastChunk = false;
                        while (true) {
                            const { done, value } = await reader.read();
                            if (done) break;
                            lastChunk = (contentLength - consumed) <= value.length;
                            const merged = new Uint8Array(collector.length + value.length);
                            merged.set(collector);
                            merged.set(value, collector.length);
                            collector = merged;
                            consumed += value.length;
                            // cb
                            props.onDownloadProgress(Math.floor(consumed / contentLength * 100));
                            if (collector.length >= 131_072) {
                                const decodedBuffer = await ctx.current.decodeAudioData(collector.buffer);
                                const bufferSource = ctx.current.createBufferSource();
                                bufferSource.buffer = decodedBuffer;
                                bufferSource.connect(ctx.current.destination);
                                bufferSource.start(time);
                                ctx.current.resume();

                                internalStatus.current = 'playing';
                                props.onPlaybackStatus('playing');

                                time += decodedBuffer.duration;

                                if (lastChunk) {
                                    bufferSource.onended = (e) => {
                                        ctx.current.suspend();
                                    }
                                }
                                collector = new Uint8Array(0);
                            }
                        }
                        controller.close();
                        reader.releaseLock();
                        props.onDownloadProgress(100);
                    }
                })
            })
            .catch(e => {
                console.error(e);
                ctx.current.suspend();
                ctx.current.close();
            })
        return () => {
            clearInterval(playbackTimeout);
            ctx.current.suspend();
        }
    }, [props.metadata]);

    return (
        <div id='stream-player' style={{ display: 'none' }} />
    )
}
