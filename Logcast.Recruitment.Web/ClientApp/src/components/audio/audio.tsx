import * as React from "react";
import {Container, Input} from 'reactstrap';
import {Button} from "../common/button/Button";
import {AudioFeature} from "./audioFeature";
import {AudioMetadata, AudioMetadataWithId} from "../../models/AudioMetadata";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {faBinoculars} from "@fortawesome/free-solid-svg-icons";
import {useAudioPageStyles} from "./audioPage.styles";
import './audioPage.animations.css';
import {Player} from "./player";

export type TFeature = 'loading' | 'emptyLibrary' | 'library' | 'upload-file' | 'upload-metadata';

interface FileUploadResult {
    audioId: string;
    suggestedMetadataModel: AudioMetadata;
}

export const Audio = () => {
    const acceptExtensions = '.mp3'
    const [feature, setFeature] = React.useState<TFeature>("loading");
    const [library, setLibrary] = React.useState<AudioMetadataWithId[]>([]);
    const [metadata, setMetadata] = React.useState<AudioMetadataWithId | null>(null);
    const [fileSubmitError, setFileSubmitError] = React.useState<string | null>(null);
    const {buttonStyle, inputStyle, libraryContentStyle} = useAudioPageStyles();
    const [file, setFile] = React.useState<File | null>(null);
    const [current, setCurrent] = React.useState<AudioMetadataWithId | null>(null);
    const [prevNext, setPrevNext] = React.useState<{ hasPrev: boolean, hasNext: boolean }>({
        hasPrev: false,
        hasNext: false
    });

    const formatDuration = (millis: number) => {
        let minutes = Math.floor(millis / 60000);
        let seconds = parseInt(((millis % 60000) / 1000).toFixed(0));
        return (
            seconds === 60 ?
                (minutes + 1) + ":00" :
                minutes + ":" + (seconds < 10 ? "0" : "") + seconds
        );
    }
    React.useEffect(() => {
        fetch('api/audio', {method: 'get'})
            .then(response => response.json())
            .then((lib: AudioMetadataWithId[]) => {
                if (lib.length === 0) {
                    setFeature('emptyLibrary');
                } else {
                    setLibrary(lib);
                    setFeature('library');
                }
            })
    }, []);
    const handleMetadataChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    }
    const onMetadataSubmit = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (metadata === null) return;
        fetch('/api/audio', {
            method: 'post',
            body: JSON.stringify(metadata),
            headers: {'Content-Type': 'application/json'}
        })
            .then(response => {
                if (response.ok) {
                    setLibrary([...library, {...metadata}]);
                    setFeature('library');
                }
                else{
                    console.log("failed to set metadata")
                }
            })
    }

    const onTrackSelected = (e: React.SyntheticEvent) => {
        e.preventDefault();
        e.stopPropagation();

        const trackId = e.currentTarget.getAttribute('track-id');
        if (trackId === null) return;
        const idx = library.findIndex(x => x.audioId === trackId);

        if (idx === -1) return;
        if (library.length === 1) {
            setPrevNext({hasPrev: false, hasNext: false});
        }
        setCurrent(library[idx]);
    }
    const onFileSelected = (e: React.SyntheticEvent) => {
        let target = e.target as HTMLInputElement;
        if (target.files === null) {
            setFile(null);
            setFileSubmitError('Please, select a file!');
        } else {
            setFile(target.files[0]);
            setFileSubmitError(null);
        }
    }
    const onUploadRequest = () => {
        if (file === null) {
            setFileSubmitError('Please, select a file!');
        } else {
            const data = new FormData();
            data.append('audioFile', file, file.name);
            fetch('/api/audio/audio-file', {method: 'post', body: data})
                .then(response => response.json())
                .then((uploadResult: FileUploadResult) => {
                    setMetadata({...uploadResult.suggestedMetadataModel, audioId: uploadResult.audioId});
                    setFeature('upload-metadata');
                })
                .catch(() => {
                    setFileSubmitError('Sorry, but this file is not recognized by server.')
                })
        }
    }

    const features = [
        <AudioFeature key='loading' feature="loading" currentFeature={feature}>
            <p style={{color: "gray"}}>Loading library...</p>
        </AudioFeature>,
        <AudioFeature key='emptyLibrary' feature="emptyLibrary" currentFeature={feature}>
            <div>
                <p>Oops, looks like library is empty!</p>
                <p style={{color: "gray"}}>You can be first to upload a track</p>
                <Button style={{...buttonStyle, ...{marginTop: '18px'}}}
                        onClick={() => setFeature('upload-file')}>Upload</Button>
            </div>
        </AudioFeature>,
        <AudioFeature key='library' feature="library" currentFeature={feature}>
            <div style={{maxHeight: '600px', overflowY: 'auto'}}>
                {
                    library.map(track => (
                        <div key={track.audioId} style={libraryContentStyle}>
                            <div track-id={track.audioId} onClick={onTrackSelected}
                                 style={{height: '80px', width: '80px', backgroundColor: '#f4144a'}}>
                                <FontAwesomeIcon icon={faBinoculars}/>
                            </div>
                            <div style={{flex: 'auto',}}>
                                <p><b>{track.title}</b> - {track.performers}</p>
                                <p>{formatDuration(track.duration)}</p>
                            </div>
                        </div>
                    ))
                }
                <Button style={{...buttonStyle, ...{marginTop: '18px'}}}
                        onClick={() => setFeature('upload-file')}>Upload</Button>
            </div>
        </AudioFeature>,
        <AudioFeature key='uplaod-file' feature="upload-file" currentFeature={feature}>
            <div>
                <p>Please, select a file to upload.</p>
                <Input style={inputStyle} type={'file'} name={'file'} onChange={onFileSelected}
                       accept={acceptExtensions}/>
                <p style={{color: 'red'}}>{fileSubmitError}</p>
                <Button style={{...buttonStyle, ...{marginTop: '18px'}}} onClick={onUploadRequest}>Upload</Button>
            </div>
        </AudioFeature>,
        <AudioFeature key='uppload-metadata' feature="upload-metadata" currentFeature={feature}>
            <div>
                <p>This is metadata server parsed.</p>
                <p>You can change it, if you want.</p>
                <p>Title</p>
                <Input style={inputStyle} type={'text'} name={'title'} value={metadata?.title}
                       onChange={handleMetadataChange}/>
                <p>Artist(s)</p>
                <Input style={inputStyle} type={'text'} name={'performers'} value={metadata?.performers}
                       onChange={handleMetadataChange}/>
                <p>Album</p>
                <Input style={inputStyle} type={'text'} name={'album'} value={metadata?.album}
                       onChange={handleMetadataChange}/>
                <p>MimeType: {metadata?.mimeType}, Bitrate: {metadata?.audioBitrate}</p>
                <p>Duration: {formatDuration(metadata?.duration ?? 0)}</p>
                <Button onClick={onMetadataSubmit}>Submit</Button>
            </div>
        </AudioFeature>
    ];


    return (
        <Container>
            <div style={{height: '100%', position: 'relative'}}>
                {features}
                {feature === 'library' ?
                    <Player metadata={current} hasNext={prevNext.hasNext} hasPrev={prevNext.hasPrev}/>
                    : null}
            </div>
        </Container>
    );
};