// Video Player JavaScript Functions with Enhanced Error Handling

window.playVideo = (videoElement) => {
    try {
        if (videoElement && videoElement.play) {
            videoElement.play();
        }
    } catch (error) {
        console.error('Error in playVideo:', error);
    }
};

window.pauseVideo = (videoElement) => {
    try {
        if (videoElement && videoElement.pause) {
            videoElement.pause();
        }
    } catch (error) {
        console.error('Error in pauseVideo:', error);
    }
};

window.seekVideo = (videoElement, time) => {
    try {
        if (videoElement) {
            videoElement.currentTime = time;
        }
    } catch (error) {
        console.error('Error in seekVideo:', error);
    }
};

window.muteVideo = (videoElement, mute) => {
    try {
        if (videoElement) {
            videoElement.muted = mute;
        }
    } catch (error) {
        console.error('Error in muteVideo:', error);
    }
};

window.toggleFullscreen = (videoElement) => {
    try {
        if (videoElement) {
            if (videoElement.requestFullscreen) {
                videoElement.requestFullscreen();
            } else if (videoElement.webkitRequestFullscreen) {
                videoElement.webkitRequestFullscreen();
            } else if (videoElement.msRequestFullscreen) {
                videoElement.msRequestFullscreen();
            }
        }
    } catch (error) {
        console.error('Error in toggleFullscreen:', error);
    }
};

window.initializeVideoPlayer = (videoElement) => {
    try {
        if (videoElement) {
            // Add event listeners for video events
            videoElement.addEventListener('loadedmetadata', function() {
                console.log('Video metadata loaded');
            });
            
            videoElement.addEventListener('timeupdate', function() {
                // Video time updated - silent to avoid spam
            });
            
            videoElement.addEventListener('ended', function() {
                console.log('Video ended');
            });

            videoElement.addEventListener('error', function(e) {
                console.error('Video player error:', e);
            });
        }
    } catch (error) {
        console.error('Error in initializeVideoPlayer:', error);
    }
};

// Enhanced Error Monitoring
window.monitorBlazorErrors = () => {
    // Monitor for Blazor error UI visibility
    const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.type === 'attributes' && mutation.attributeName === 'style') {
                const errorUI = document.getElementById('blazor-error-ui');
                if (errorUI && !errorUI.classList.contains('hidden') && errorUI.style.display !== 'none') {
                    console.error('?? Blazor Error UI became visible!');
                    console.error('Current page:', window.location.href);
                    console.error('Time:', new Date().toISOString());
                    
                    // Try to get more context
                    const activeElement = document.activeElement;
                    if (activeElement) {
                        console.error('Active element when error occurred:', activeElement);
                    }
                }
            }
        });
    });

    const errorUI = document.getElementById('blazor-error-ui');
    if (errorUI) {
        observer.observe(errorUI, { 
            attributes: true, 
            attributeFilter: ['style', 'class'] 
        });
    }

    // Also monitor for class changes
    const classObserver = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                const errorUI = document.getElementById('blazor-error-ui');
                if (errorUI && !errorUI.classList.contains('hidden')) {
                    console.error('?? Blazor Error UI class changed - error may have occurred!');
                }
            }
        });
    });

    if (errorUI) {
        classObserver.observe(errorUI, { 
            attributes: true, 
            attributeFilter: ['class'] 
        });
    }
};

// Start monitoring when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', window.monitorBlazorErrors);
} else {
    window.monitorBlazorErrors();
}