import { CSSProperties } from "react";

export const useAudioPageStyles = () => {
    const featureContainerStyle: CSSProperties = {
        position: 'absolute',
        height: '100%',
        width: '100%',
    };

    const featureContentStyle: CSSProperties = {
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignContent: 'center',
        height: '100%',
        margin: '0 auto',
    }

    const libraryContentStyle: CSSProperties = {
        height: '80px',
        marginBottom: '8px',
        display: 'flex'
    }

    const playerStyle: CSSProperties = {
        position: 'absolute',
        bottom: 24,
        background: 'grey'
    }

    const buttonStyle: React.CSSProperties = {
        width: "42%",
        marginLeft: "5px",
        marginRight: "5px",
    }

    const inputStyle: React.CSSProperties = {
        width: "59%",
        margin: "0 auto",
        minWidth: "230px"
    }

    const streamPlayerContainerStyle: CSSProperties = {
        minWidth: '120px',
        minHeight: '120px',
        marginRight: '10px',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignContent: 'center',
        background: '#fecaca'
    };

    return {
        featureContainerStyle,
        featureContentStyle,
        buttonStyle,
        playerStyle,
        inputStyle,
        libraryContentStyle,
        streamPlayerContainerStyle,
    }
}