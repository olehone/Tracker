let dotNetInstance = null;

function registerDotNetInstance(instance) {
    dotNetInstance = instance;
}

let mediaConstraints = {
    audio: true,
    video: {
        aspectRatio: {
            ideal: 1.333333,
        },
    },
};

const peerConnections = new Map();
const pendingCandidates = new Map();
let myUserId = null;
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
    log_error(`Error ${errMessage.name}: ${errMessage.message}`);
}

// Called by Blazor when user list arrives from hub
async function handleUserList(userIds, myId) {
    myUserId = myId;
    log("User list received: " + userIds.join(", "));

    const iAmNewest = userIds[userIds.length - 1] === myUserId;
    if (iAmNewest) {
        for (const userId of userIds) {
            if (userId !== myUserId) {
                log("Auto-connecting to " + userId);
                await initiateCall(userId);
            }
        }
    }
}

async function initiateCall(userId) {
    if (peerConnections.has(userId) || userId === myUserId) return;

    log("Initiating call with " + userId);
    const pc = await createPeerConnection(userId);
    const stream = await getLocalStream();
    stream.getTracks().forEach(track => pc.addTrack(track, stream));
}

async function createPeerConnection(userId) {
    log("Setting up connection with " + userId);

    const pc = new RTCPeerConnection({
        iceServers: [{ urls: "stun:stun.l.google.com:19302" }]
    });

    peerConnections.set(userId, pc);
    pendingCandidates.set(userId, []);

    pc.onicecandidate = e => {
        if (e.candidate) {
            log("*** Outgoing ICE candidate to " + userId);
            dotNetInstance.invokeMethodAsync("SendIceCandidate", userId, JSON.stringify(e.candidate));
        }
    };

    pc.oniceconnectionstatechange = () => {
        log("*** ICE state with " + userId + ": " + pc.iceConnectionState);
        if (["closed", "failed", "disconnected"].includes(pc.iceConnectionState)) {
            closePeerConnection(userId);
        }
    };

    pc.onsignalingstatechange = () => {
        log("*** Signaling state with " + userId + ": " + pc.signalingState);
        if (pc.signalingState === "closed") closePeerConnection(userId);
    };

    pc.onnegotiationneeded = async () => {
        log("*** Negotiation needed with " + userId);
        try {
            const offer = await pc.createOffer();
            if (pc.signalingState !== "stable") {
                log("     -- Not stable, postponing");
                return;
            }
            await pc.setLocalDescription(offer);
            log("---> Sending offer to " + userId);
            dotNetInstance.invokeMethodAsync("SendVideoOffer", userId, JSON.stringify(pc.localDescription));
        } catch (err) { reportError(err); }
    };

    pc.ontrack = e => {
        log("*** Track event from " + userId);
        const stream = e.streams[0];
        if (!dotNetInstance) {
            log_error("dotNetInstance is null, cannot notify Blazor");
            return;
        }
        dotNetInstance.invokeMethodAsync("OnRemoteTrack", userId)
            .then(() => {
                requestAnimationFrame(() => {
                    const video = document.getElementById("video-" + userId);
                    if (video) {
                        video.srcObject = stream;
                    } else {
                        setTimeout(() => {
                            const v = document.getElementById("video-" + userId);
                            if (v) v.srcObject = stream;
                        }, 100);
                    }
                });
            })
            .catch(err => log_error("OnRemoteTrack failed: " + err));
    };

    return pc;
}

async function getLocalStream() {
    if (webcamStream) return webcamStream;
    webcamStream = await navigator.mediaDevices.getUserMedia(mediaConstraints);
    document.getElementById("local_video").srcObject = webcamStream;
    return webcamStream;
}

// Incoming signaling — called by Blazor from service events

async function receiveVideoOffer(fromUserId, sdpJson) {
    log("Received offer from " + fromUserId);

    let pc = peerConnections.get(fromUserId);
    if (!pc) pc = await createPeerConnection(fromUserId);

    const stream = await getLocalStream();
    if (pc.getSenders().length === 0) {
        stream.getTracks().forEach(track => pc.addTrack(track, stream));
    }

    const desc = new RTCSessionDescription(JSON.parse(sdpJson));
    if (pc.signalingState !== "stable") {
        log("  - Not stable, rolling back");
        await Promise.all([
            pc.setLocalDescription({ type: "rollback" }),
            pc.setRemoteDescription(desc),
        ]);
    } else {
        await pc.setRemoteDescription(desc);
    }

    for (const c of (pendingCandidates.get(fromUserId) || [])) {
        await pc.addIceCandidate(c).catch(reportError);
    }
    pendingCandidates.set(fromUserId, []);

    await pc.setLocalDescription(await pc.createAnswer());
    log("---> Sending answer to " + fromUserId);
    dotNetInstance.invokeMethodAsync("SendVideoAnswer", fromUserId, JSON.stringify(pc.localDescription));
}

async function receiveVideoAnswer(fromUserId, sdpJson) {
    log("*** Answer received from " + fromUserId);
    const pc = peerConnections.get(fromUserId);
    if (!pc) return;

    await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(sdpJson))).catch(reportError);

    for (const c of (pendingCandidates.get(fromUserId) || [])) {
        await pc.addIceCandidate(c).catch(reportError);
    }
    pendingCandidates.set(fromUserId, []);
}

async function receiveIceCandidate(fromUserId, candidateJson) {
    log("*** ICE candidate from " + fromUserId);
    const candidate = new RTCIceCandidate(JSON.parse(candidateJson));
    const pc = peerConnections.get(fromUserId);

    if (!pc || !pc.remoteDescription) {
        log("*** Queuing ICE candidate from " + fromUserId);
        if (!pendingCandidates.has(fromUserId)) pendingCandidates.set(fromUserId, []);
        pendingCandidates.get(fromUserId).push(candidate);
        return;
    }
    await pc.addIceCandidate(candidate).catch(reportError);
}

function receiveHangUp(fromUserId) {
    log("*** Hang up from " + fromUserId);
    closePeerConnection(fromUserId);
    dotNetInstance.invokeMethodAsync("OnPeerDisconnected", fromUserId);
}

function closePeerConnection(userId) {
    const pc = peerConnections.get(userId);
    if (!pc) return;

    log("Closing connection with " + userId);
    pc.ontrack = null;
    pc.onicecandidate = null;
    pc.oniceconnectionstatechange = null;
    pc.onsignalingstatechange = null;
    pc.onnegotiationneeded = null;
    pc.getTransceivers().forEach(t => t.stop());
    pc.close();

    peerConnections.delete(userId);
    pendingCandidates.delete(userId);
}

function hangUpAll() {
    log("Hanging up all connections");

    peerConnections.forEach((pc, userId) => {
        dotNetInstance.invokeMethodAsync("SendHangUp", userId);
        closePeerConnection(userId);
    });

    const localVideo = document.getElementById("local_video");
    if (localVideo?.srcObject) {
        localVideo.pause();
        localVideo.srcObject.getTracks().forEach(t => t.stop());
        localVideo.srcObject = null;
    }
    webcamStream = null;
}

async function setMuted(muted) {
    webcamStream?.getAudioTracks().forEach(t => t.enabled = !muted);
}

async function setVideoEnabled(enabled) {
    webcamStream?.getVideoTracks().forEach(t => t.enabled = enabled);
}

async function startScreenShare() {
    const screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
    const screenTrack = screenStream.getVideoTracks()[0];

    peerConnections.forEach(pc => {
        const sender = pc.getSenders().find(s => s.track?.kind === "video");
        sender?.replaceTrack(screenTrack);
    });

    screenTrack.onended = () => stopScreenShare();
}

async function stopScreenShare() {
    const camTrack = webcamStream?.getVideoTracks()[0];
    peerConnections.forEach(pc => {
        const sender = pc.getSenders().find(s => s.track?.kind === "video");
        sender?.replaceTrack(camTrack);
    });
}

window.registerDotNetInstance = registerDotNetInstance;
window.handleUserList = handleUserList;
window.receiveVideoOffer = receiveVideoOffer;
window.receiveVideoAnswer = receiveVideoAnswer;
window.receiveIceCandidate = receiveIceCandidate;
window.receiveHangUp = receiveHangUp;
window.hangUpAll = hangUpAll;
window.setMuted = setMuted;
window.setVideoEnabled = setVideoEnabled;
window.startScreenShare = startScreenShare;
window.stopScreenShare = stopScreenShare;
