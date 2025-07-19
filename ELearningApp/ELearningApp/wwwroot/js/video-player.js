// Video Player JavaScript Functions

window.playVideo = (videoElement) => {
    if (videoElement && videoElement.play) {
        videoElement.play();
    }
};

window.pauseVideo = (videoElement) => {
    if (videoElement && videoElement.pause) {
        videoElement.pause();
    }
};

window.seekVideo = (videoElement, time) => {
    if (videoElement) {
        videoElement.currentTime = time;
    }
};

window.muteVideo = (videoElement, mute) => {
    if (videoElement) {
        videoElement.muted = mute;
    }
};

window.toggleFullscreen = (videoElement) => {
    if (videoElement) {
        if (videoElement.requestFullscreen) {
            videoElement.requestFullscreen();
        } else if (videoElement.webkitRequestFullscreen) {
            videoElement.webkitRequestFullscreen();
        } else if (videoElement.msRequestFullscreen) {
            videoElement.msRequestFullscreen();
        }
    }
};

window.initializeVideoPlayer = (videoElement) => {
    if (videoElement) {
        // Add event listeners for video events
        videoElement.addEventListener('loadedmetadata', function() {
            // Video metadata loaded
        });
        
        videoElement.addEventListener('timeupdate', function() {
            // Video time updated
        });
        
        videoElement.addEventListener('ended', function() {
            // Video ended
        });
    }
}; 