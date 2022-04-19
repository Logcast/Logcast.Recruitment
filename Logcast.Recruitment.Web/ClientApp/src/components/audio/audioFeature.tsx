import React from 'react'
import {Col, Row} from 'reactstrap';
import {TFeature} from './audio';
import {useAudioPageStyles} from './audioPage.styles'

export const AudioFeature = (props: React.PropsWithChildren<{ feature: TFeature, currentFeature: TFeature }>) => {
    const {featureContainerStyle} = useAudioPageStyles();
    const relevantClassname = props.feature === props.currentFeature ? 'featureVisible' : 'featureHiddenBottom';
    return (
        <div className={relevantClassname} style={featureContainerStyle}>
            <div style={{position: 'relative', width: '100%', height: '100%'}}>
                <Row className="justify-content-md-center align-items-top" style={{height: "100%", padding: "0 15px"}}>
                    <Col style={{margin: '0 auto'}} md="5" className="text-center">
                        {props.children}
                    </Col>
                </Row>
            </div>
        </div>
    )
}
