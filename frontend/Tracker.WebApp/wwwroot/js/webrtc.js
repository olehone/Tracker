let dotNetInstance = null;
let myUserId = null;

function registerDotNetInstance(instance) {
    dotNetInstance = instance;
    log("DotNet instance registered");
}

const mediaConstraints = {
    audio: true,
    video: {
        aspectRatio: {
            ideal: 1.333333,
        },
    },
};

const peerConnections = new Map();
const makingOffer = new Map();
const pendingCandidates = new Map();
const remoteStreams = new Map();
const closingConnections = new Set();
let webcamStream = null;

function log(text) {
    let time = new Date();
    console.log("[" + time.toLocaleTimeString() + "] " + text);
}

function log_error(text) {
    let time = new Date();
    console.trace("[" + time.toLocaleTimeString() + "] " + text);
}

function reportError(errMessage) {
    log_error("Error " + errMessage.name + ": " + errMessage.message);
}

function isPolite(remoteUserId) {
    return myUserId > remoteUserId;
}

async function initiateCall(userId, selfId) {
    if (peerConnections.has(userId)) {
        log("Already have connection with " + userId + ", skipping initiation");
        return;
    }

    if (userId === selfId) {
        log("Skipping self: " + userId);
        return;
    }

    myUserId = selfId;

    log("Initiating call with " + userId + " (polite=" + isPolite(userId) + ")");
    const stream = await getLocalStream();
    const pc = createPeerConnection(userId);
    stream.getTracks().forEach(track => {
        log("Adding local track " + track.kind + " to connection with " + userId);
        pc.addTrack(track, stream);
    });
}

function createPeerConnection(userId) {
    log("Setting up RTCPeerConnection with " + userId);

    closingConnections.delete(userId);
    makingOffer.set(userId, false);
    pendingCandidates.set(userId, []);

    const pc = new RTCPeerConnection({
        iceServers: [{ urls: "stun:stun.l.google.com:19302" }]
    });

    peerConnections.set(userId, pc);

    pc.onicecandidate = e => {
        if (e.candidate) {
            log("Outgoing ICE candidate to " + userId + " (" + e.candidate.type + ")");
            dotNetInstance.invokeMethodAsync("SendIceCandidate", userId, JSON.stringify(e.candidate));
        } else {
            log("ICE gathering complete for " + userId);
        }
    };

    pc.onicegatheringstatechange = () => {
        log("ICE gathering state with " + userId + ": " + pc.iceGatheringState);
    };

    pc.oniceconnectionstatechange = () => {
        log("ICE connection state with " + userId + ": " + pc.iceConnectionState);

        if (["closed", "failed", "disconnected"].includes(pc.iceConnectionState)) {
            log("ICE connection lost with " + userId + " (" + pc.iceConnectionState + ")");
            handlePeerGone(userId);
        }
    };

    pc.onconnectionstatechange = () => {
        log("Connection state with " + userId + ": " + pc.connectionState);

        if (pc.connectionState === "failed") {
            log("Connection failed with " + userId);
            handlePeerGone(userId);
        }
    };

    pc.onsignalingstatechange = () => {
        log("Signaling state with " + userId + ": " + pc.signalingState);
    };

    pc.onnegotiationneeded = async () => {
        log("Negotiation needed with " + userId + " (polite=" + isPolite(userId) + ")");
        try {
            makingOffer.set(userId, true);
            await pc.setLocalDescription();
            log("Sending offer to " + userId);
            dotNetInstance.invokeMethodAsync("SendVideoOffer", userId, JSON.stringify(pc.localDescription));
        } catch (err) {
            reportError(err);
        } finally {
            makingOffer.set(userId, false);
        }
    };

    pc.ontrack = e => {
        log("Track event from " + userId + " (kind=" + e.track.kind + ", streams=" + e.streams.length + ")");

        if (e.streams && e.streams[0]) {
            remoteStreams.set(userId, e.streams[0]);
            dotNetInstance.invokeMethodAsync("OnRemoteTrack", userId)
                .catch(err => log_error("OnRemoteTrack failed: " + err));
        } else {
            log_error("Track event from " + userId + " had no streams");
        }
    };

    return pc;
}

async function getLocalStream() {
    if (webcamStream) {
        log("Reusing existing local stream");
        return webcamStream;
    }

    log("Requesting local media (audio + video)");
    webcamStream = await navigator.mediaDevices.getUserMedia(mediaConstraints);
    document.getElementById("local_video").srcObject = webcamStream;
    log("Local stream acquired (" + webcamStream.getTracks().length + " tracks)");
    return webcamStream;
}

async function flushPendingCandidates(userId) {
    const pc = peerConnections.get(userId);
    const queued = pendingCandidates.get(userId) || [];
    if (queued.length === 0) {
        return;
    }

    log("Flushing " + queued.length + " queued ICE candidates for " + userId);
    pendingCandidates.set(userId, []);
    for (const candidate of queued) {
        await pc.addIceCandidate(candidate).catch(reportError);
    }
}

async function receiveVideoOffer(fromUserId, sdpJson) {
    log("Received offer from " + fromUserId + " (polite=" + isPolite(fromUserId) + ")");

    let pc = peerConnections.get(fromUserId);
    if (!pc) {
        log("No existing connection for " + fromUserId + ", creating one");
        const stream = await getLocalStream();
        pc = createPeerConnection(fromUserId);
        stream.getTracks().forEach(track => {
            log("Adding local track " + track.kind + " to incoming connection with " + fromUserId);
            pc.addTrack(track, stream);
        });
    }

    const offerCollision = makingOffer.get(fromUserId) || pc.signalingState !== "stable";
    const imPolite = !isPolite(fromUserId);

    if (offerCollision) {
        if (imPolite) {
            log("Offer collision with " + fromUserId + ": impolite peer ignoring offer");
            return;
        }
        log("Offer collision with " + fromUserId + ": polite peer rolling back");
    }

    const desc = new RTCSessionDescription(JSON.parse(sdpJson));
    await pc.setRemoteDescription(desc);
    await flushPendingCandidates(fromUserId);

    if (pc.signalingState === "have-remote-offer") {
        await pc.setLocalDescription();
        log("Sending answer to " + fromUserId);
        dotNetInstance.invokeMethodAsync("SendVideoAnswer", fromUserId, JSON.stringify(pc.localDescription));
    }
}

async function receiveVideoAnswer(fromUserId, sdpJson) {
    log("Received answer from " + fromUserId);

    const pc = peerConnections.get(fromUserId);
    if (!pc) {
        log_error("No peer connection for answer from " + fromUserId);
        return;
    }

    if (pc.signalingState !== "have-local-offer") {
        log("Ignoring answer from " + fromUserId + " in state " + pc.signalingState + " (already settled)");
        return;
    }

    await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(sdpJson))).catch(reportError);
    await flushPendingCandidates(fromUserId);
}

async function receiveIceCandidate(fromUserId, candidateJson) {
    const pc = peerConnections.get(fromUserId);
    const candidate = new RTCIceCandidate(JSON.parse(candidateJson));

    if (!pc) {
        log("Dropping ICE candidate from " + fromUserId + " (no peer connection)");
        return;
    }

    if (!pc.remoteDescription || pc.remoteDescription.type === "") {
        log("Queuing ICE candidate from " + fromUserId + " (no remote description yet)");
        const queue = pendingCandidates.get(fromUserId);
        if (queue) {
            queue.push(candidate);
        }
        return;
    }

    await pc.addIceCandidate(candidate).catch(err => {
        if (!isPolite(fromUserId) && makingOffer.get(fromUserId)) {
            log("Ignoring ICE candidate error from " + fromUserId + " during offer collision");
            return;
        }
        reportError(err);
    });
    log("Added ICE candidate from " + fromUserId);
}

function receiveHangUp(fromUserId) {
    log("Received hang up from " + fromUserId);
    closePeerConnection(fromUserId);
    dotNetInstance.invokeMethodAsync("OnPeerDisconnected", fromUserId)
        .catch(err => log_error("OnPeerDisconnected failed: " + err));
}

function handlePeerGone(userId) {
    if (closingConnections.has(userId)) {
        return;
    }

    closingConnections.add(userId);
    log("Peer gone: " + userId);
    closePeerConnection(userId);
    dotNetInstance.invokeMethodAsync("OnPeerDisconnected", userId)
        .catch(err => log_error("OnPeerDisconnected failed: " + err));
}

function closePeerConnection(userId) {
    const pc = peerConnections.get(userId);
    if (!pc) {
        return;
    }

    log("Closing RTCPeerConnection with " + userId);

    pc.ontrack = null;
    pc.onicecandidate = null;
    pc.onicegatheringstatechange = null;
    pc.oniceconnectionstatechange = null;
    pc.onconnectionstatechange = null;
    pc.onsignalingstatechange = null;
    pc.onnegotiationneeded = null;

    pc.getSenders().forEach(sender => {
        if (sender.track) {
            log("Detaching sender track " + sender.track.kind + " from connection with " + userId);
            sender.replaceTrack(null).catch(() => {});
        }
    });

    pc.close();

    peerConnections.delete(userId);
    makingOffer.delete(userId);
    pendingCandidates.delete(userId);
    remoteStreams.delete(userId);

    log("Closed connection with " + userId);
}

async function hangUpAll() {
    log("Hanging up all connections (" + peerConnections.size + " peers)");

    const hangUpPromises = [];
    peerConnections.forEach((pc, userId) => {
        log("Sending hang up to " + userId);
        hangUpPromises.push(
            dotNetInstance.invokeMethodAsync("SendHangUp", userId)
                .catch(err => log_error("SendHangUp failed for " + userId + ": " + err))
        );
    });

    await Promise.allSettled(hangUpPromises);

    Array.from(peerConnections.keys()).forEach(userId => {
        closePeerConnection(userId);
    });

    const localVideo = document.getElementById("local_video");
    if (localVideo && localVideo.srcObject) {
        localVideo.pause();
        localVideo.srcObject.getTracks().forEach(t => {
            log("Stopping local track: " + t.kind);
            t.stop();
        });
        localVideo.srcObject = null;
    }

    webcamStream = null;
    myUserId = null;
    closingConnections.clear();

    log("All connections closed");
}

async function setMuted(muted) {
    if (!webcamStream) {
        return;
    }

    webcamStream.getAudioTracks().forEach(t => {
        t.enabled = !muted;
        log("Audio track " + (muted ? "muted" : "unmuted"));
    });
}

async function setVideoEnabled(enabled) {
    if (!webcamStream) {
        return;
    }

    webcamStream.getVideoTracks().forEach(t => {
        t.enabled = enabled;
        log("Video track " + (enabled ? "enabled" : "disabled"));
    });
}

async function startScreenShare() {
    log("Starting screen share");
    const screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
    const screenTrack = screenStream.getVideoTracks()[0];

    peerConnections.forEach((pc, userId) => {
        const sender = pc.getSenders().find(s => s.track && s.track.kind === "video");
        if (sender) {
            log("Replacing video track with screen for " + userId);
            sender.replaceTrack(screenTrack);
        }
    });

    screenTrack.onended = () => {
        log("Screen share ended");
        stopScreenShare();
    };
}

async function stopScreenShare() {
    log("Stopping screen share, reverting to camera");
    const camTrack = webcamStream && webcamStream.getVideoTracks()[0];

    if (!camTrack) {
        log_error("No camera track to revert to");
        return;
    }

    peerConnections.forEach((pc, userId) => {
        const sender = pc.getSenders().find(s => s.track && s.track.kind === "video");
        if (sender) {
            log("Reverting video track to camera for " + userId);
            sender.replaceTrack(camTrack);
        }
    });
}

function attachRemoteStream(userId) {
    const stream = remoteStreams.get(userId);
    const video = document.getElementById("video-" + userId);

    if (!video) {
        log_error("Video element not found for " + userId);
        return;
    }

    if (!stream) {
        log_error("No remote stream available for " + userId);
        return;
    }

    log("Attaching remote stream for " + userId);
    video.srcObject = stream;
}

window.attachRemoteStream = attachRemoteStream;
window.registerDotNetInstance = registerDotNetInstance;
window.initiateCall = initiateCall;
window.receiveVideoOffer = receiveVideoOffer;
window.receiveVideoAnswer = receiveVideoAnswer;
window.receiveIceCandidate = receiveIceCandidate;
window.receiveHangUp = receiveHangUp;
window.hangUpAll = hangUpAll;
window.setMuted = setMuted;
window.setVideoEnabled = setVideoEnabled;
window.startScreenShare = startScreenShare;
window.stopScreenShare = stopScreenShare;
