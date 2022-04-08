import * as React from "react";
import {Col, Container, Row} from 'reactstrap';
import {Button} from "../common/button/Button";
import {useHomeStyles} from "../home/home.styles";
import {useSurveyPageStyles} from "../signUp/surveyPage.styles";
import {WebApiClient} from "../../common/webApiClient";
import {AddAudioRequest} from "../../models/newAudioRequest";
import {useState} from "react";
import {Input} from "reactstrap";
import { FileInformation } from "../../models/fileInformation";
import * as util from "util";
import * as path from 'path';
import { ReactComponent as Play } from '../../Assets/play.svg';
import { ReactComponent as Pause } from  '../../Assets/pause.svg';

export const Player = ( props: {metadata: FileInformation | null, emergencyStop: boolean | null} ) => {
    const apiClient = WebApiClient();
    const [isPlaying, setIsPlaying] = useState(false);
    const [redrawingPlayer, setRedrawingPlayer] = useState(false);
    const ref = React.createRef<HTMLAudioElement>();
    const reff = React.useRef<HTMLAudioElement | null>(null);

    const onPlayPauseClick = (e: boolean) => {
        setRedrawingPlayer(true);
        setIsPlaying(e);
    };

    React.useEffect(() => {
        if (!props.emergencyStop){
            if (!redrawingPlayer){
                if (props.metadata != null 
                    && props.metadata.id != null
                    && props.metadata.id != undefined
                    && props.metadata.id !== 0){
                    setIsPlaying(true);
                    fetch('api/audio/stream/'+props.metadata.id)
                    .then(r => r.blob())
                    .then(blob => {
                        const url = URL.createObjectURL(blob);
                        if (reff && reff.current){
                            
                            reff.current!.src = url;
                            return reff.current!.play();                        
                        }
                    })
                    .then(_ => {
                        // Video playback started ;)
                    })
                    .catch(e => { }); 
                }
            }
            else {
                if (reff && reff.current && isPlaying) {
                    reff.current!.play();
                } else {
                    reff.current!.pause();
                }
                setRedrawingPlayer(false);
            }
        } else {
            if (reff && reff.current && isPlaying) {
                reff.current!.pause();
            }
        }
    }, [props.metadata, props.emergencyStop, isPlaying]);   

    return (
        <div className="track-info">
            <h2 className="title">{props.metadata!.trackTitle}</h2>
            <h3 className="artist">{props.metadata!.artist}</h3>

            <div className="audio-controls justify-content-md-center align-items-center">            
                            
                <audio ref={reff}></audio>
                {isPlaying ? (
                <button
                    type="button"
                    className="pause"
                    onClick={() => onPlayPauseClick(false)}
                    aria-label="Pause"
                >
                    <Pause style={{ minWidth: "80px", minHeight: "80px" }} />
                </button>
                ) : (
                <button
                    type="button"
                    className="play"
                    onClick={() => onPlayPauseClick(true)}
                    aria-label="Play"
                >
                    <Play style={{ minWidth: "80px", minHeight: "80px" }} />
                </button>
                )}

            </div>
        </div>
    );
};
