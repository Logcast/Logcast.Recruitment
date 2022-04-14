import * as React from "react";
import {Col, Container, Row, Label, ListGroup, ListGroupItem} from 'reactstrap';
import {Button} from "../common/button/Button";
import {useHomeStyles} from "../home/home.styles";
import {useSurveyPageStyles} from "../signUp/surveyPage.styles";
import {WebApiClient} from "../../common/webApiClient";
import {AddAudioRequest} from "../../models/newAudioRequest";
import {AudioMetadataRequest} from "../../models/audioMetadataRequest";
import {useState,useEffect } from "react";
import {Input} from "reactstrap";
import * as util from "util";
import * as path from 'path';
import {QuestionnaireMetadata} from "./QuestionnaireMetadata";
import {Player} from "./Player";
import { FileInformation } from "../../models/fileInformation";
import { PlayerData } from "../../models/playerInformation";

export const Audio = () => {

    const apiClient = WebApiClient();
    const { buttonStyle } = useHomeStyles();
    const {inputStyle} = useSurveyPageStyles();
    const [formAudioUpload, setFormData] = useState({audio: {} as File} as AddAudioRequest);
    let [audioMetadata, setAudioMetadata] = useState<AudioMetadataRequest | null>(null);
    const [files, setFilesData] = useState([] as Array<FileInformation>);    
    const [step, setStep] = useState(0);
    const [redrawing, setRedrawing] = useState(true);
    const [playerIntermediateData, setPlayerIntermediateData] = useState({ showPlayer: false, selectedId: 0} as PlayerData);
    const [metadataPlayer, setMetadataPlayer] = useState({} as FileInformation);   
    const [emergencyStop, setEmergencyStop] = useState(false);

    React.useEffect(() => {
        if (redrawing){
            apiClient.get('api/audio/')
            .then((res) => {
                const response = res as Array<FileInformation>;
                setFilesData(response);
            }).catch(e => { 
                console.log(e);
            }); 
            setRedrawing(false);
        }      
        if (playerIntermediateData.selectedId !== 0){
            apiClient.get('api/audio/'+playerIntermediateData.selectedId)
            .then((res) => {
                const response = res as FileInformation;
                setEmergencyStop(false);
                setMetadataPlayer(response);
            }).catch(e => {
                console.log(e);
            }); 
        } else {
            setEmergencyStop(true);
        }
    }, [audioMetadata, step, redrawing, playerIntermediateData]);

    const onSubmitFileUpload = () => {
        const formAudioUploadData: FormData = new FormData();
        formAudioUploadData.append('audioFile', formAudioUpload.audio);

        apiClient.postAxios('api/audio/audio-file', formAudioUploadData)
            .then((res) => {
                const response = res as AudioMetadataRequest;
                setAudioMetadata(response);
                setRedrawing(true);
                setStep(1);
            }).catch(e => {
                console.log(e);
            });
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files != null){
            setFormData({ ...formAudioUpload, [e.target.name]: e.target.files[0] });
        }
    };

    const handleListChange = (e: any) => {
        let valueId = 0;
        if (e.target.value !== null){
            valueId = parseInt(e.target.value);
        }
        setPlayerIntermediateData({ showPlayer: true, selectedId: valueId});
        
    };

    const validateSoundFile = () => {
        const types = /(\.|\/)(mp3|opus|flac|webm|weba|wav|ogg|m4a|mp3|oga|mid|amr|aiff|wma|au|aac)$/i;
        return (formAudioUpload.audio && (types.test(formAudioUpload.audio.type) || types.test(formAudioUpload.audio.name)));
    }

    const closePlayer = () => {
        setPlayerIntermediateData({ showPlayer: false, selectedId: 0});
    }

    return (
        <Container>
            <Row className="justify-content-md-center align-items-center" style={{ height: '10%' }}>
                <Col style={{ background: 'white' }} md="15" className="text-center">
                    Records List
                    <ListGroup>
                        {files.map((item: FileInformation) => (
                            <ListGroupItem key={item.id} style={{padding: "0 10px"}} value={item.id} onClick={handleListChange} >{item.artist} - {item.trackTitle}</ListGroupItem>
                        ))}
                    </ListGroup>
                </Col>
            </Row>
            <Row hidden={step != 0 || playerIntermediateData.showPlayer} className="justify-content-md-center align-items-center" style={{ height: '90%' }}>
                <Col md="5" className="text-center">
                    <Input style={inputStyle} accept="audio/*" type={"file"} name={"audio"} onChange={handleChange}/>
                    <p><Button style={{...{marginTop: "18px"}, ...buttonStyle}} disabled={!formAudioUpload.audio || !formAudioUpload.audio.size} onClick={() => {if(validateSoundFile()) onSubmitFileUpload()}}>Upload</Button> </p>
                </Col>
            </Row>

            <Row hidden={step != 1} className="justify-content-md-center align-items-center" style={{ height: '90%' }}>
                <Col md="15" className="text-center">
                    <QuestionnaireMetadata audioMetadataRequest={audioMetadata} />
                </Col>
            </Row>
            <Row hidden={!playerIntermediateData.showPlayer} className="justify-content-md-center align-items-center" style={{ height: '90%' }}>
                <Col md="15" className="text-center">
                    <div style={{position: "absolute", bottom: "52px", left: "50%"}}>
                        <Button style={{position: "relative", left: "-50%", width: "125px"}} color={"gray"} onClick={() => closePlayer()}>Close</Button>
                    </div>
                    <Player metadata={metadataPlayer} emergencyStop={emergencyStop} />
                </Col>
            </Row>
        </Container>
    );
};
