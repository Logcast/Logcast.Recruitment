import * as React from "react";
import {Col, Container, Row, Form, FormGroup, Label, Input} from 'reactstrap';
import {Button} from "../common/button/Button";
import {useHomeStyles} from "../home/home.styles";
import {useSurveyPageStyles} from "../signUp/surveyPage.styles";
import {WebApiClient} from "../../common/webApiClient";
import {AddAudioRequest} from "../../models/newAudioRequest";
import {useState} from "react";
import {AudioMetadataRequest} from "../../models/audioMetadataRequest";

export const QuestionnaireMetadata = ( props: React.PropsWithChildren<{audioMetadataRequest: AudioMetadataRequest | null}>) => {

    const apiClient = WebApiClient();
    const {buttonStyle, inputStyle} = useSurveyPageStyles();
    const [formData, setFormData] = useState(props.audioMetadataRequest);
    const [step, setStep] = useState(1);

    React.useEffect(() => {
        if (props.audioMetadataRequest !== null){
            setFormData(props.audioMetadataRequest);
        }
    }, [props.audioMetadataRequest]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        //@ts-ignore
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const onSubmitMetadataUpdate = () => {
        apiClient.post('api/audio/', formData)
            .then((res) => {
                setStep(0);
            }).catch(e => { });
    };
    
    const skip = () => {
        setStep(0);
    };

    return (
        <Form>
            <FormGroup>
                <Label for="artist">
                    Artist
                </Label>
                <Input style={inputStyle} type={"text"} id="artist" name={"artist"} onChange={handleChange} value={formData?.artist} />
            </FormGroup>
            <FormGroup>
                <Label for="album">
                    Album
                </Label>
                <Input style={inputStyle} type={"text"} id="album" name={"album"} onChange={handleChange} value={formData?.album} />
            </FormGroup>
            <FormGroup>
                <Label for="trackTitle">
                    Track title
                </Label>
                <Input style={inputStyle} type={"text"} id="trackTitle" name={"trackTitle"} onChange={handleChange} value={formData?.trackTitle} />
            </FormGroup>
            <FormGroup>
                <Label for="genre">
                Genre
                </Label>
                <Input type={"text"} id="genre" name={"genre"} onChange={handleChange} value={formData?.genre} />
            </FormGroup>
            <FormGroup>
                <Label for="trackNumber">
                    Track number
                </Label>
                <Input style={inputStyle} type={"text"} id="trackNumber" name={"trackNumber"} onChange={handleChange} value={formData?.trackNumber} />
            </FormGroup>
            
            <p><Button style={{...{marginTop: "18px"}, ...buttonStyle}} onClick={() => onSubmitMetadataUpdate()}>Update</Button> </p>
            <p><Button style={{...{marginTop: "18px"}, ...buttonStyle}} onClick={() => skip()}>Skip Set Up</Button> </p>
        </Form>
    );
};