export interface AudioMetadata {
    audioBitrate: number;
    duration: number;
    title: string;
    album: string;
    performers: string;
    mimeType: string;
    genres: string;
}
export interface AudioMetadataWithId extends AudioMetadata {
   audioId : string
}