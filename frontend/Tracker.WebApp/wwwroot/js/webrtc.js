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
const dataChannels = new Map();
const makingOffer = new Map();
const pendingCandidates = new Map();
const remoteStreams = new Map();
const remoteScreenStreams = new Map();
const closingConnections = new Set();

let webcamStream = null;
let screenStream = null;
let screenStreamId = null;
let isMuted = false;
let isVideoEnabled = true;
let isSharingScreen = false;

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

function buildStateMessage() {
    return JSON.stringify({
        type: "state",
        audio: !isMuted,
        video: isVideoEnabled,
        screen: isSharingScreen,
        screenStreamId: screenStreamId,
    });
}

function broadcastLocalState() {
    const msg = buildStateMessage();
    dataChannels.forEach((dc, userId) => {
        if (dc.readyState === "open") {
            dc.send(msg);
            log("Sent state to " + userId + ": audio=" + !isMuted + " video=" + isVideoEnabled + " screen=" + isSharingScreen);
        }
    });
}

function sendLocalStateTo(userId) {
    const dc = dataChannels.get(userId);
    if (dc && dc.readyState === "open") {
        dc.send(buildStateMessage());
        log("Sent initial state to " + userId);
    }
}

function setupDataChannel(userId, dc) {
    dataChannels.set(userId, dc);

    dc.onopen = () => {
        log("Data channel open with " + userId);
        sendLocalStateTo(userId);
    };

    dc.onclose = () => {
        log("Data channel closed with " + userId);
    };

    dc.onerror = err => {
        log_error("Data channel error with " + userId + ": " + err);
    };

    dc.onmessage = e => {
        try {
            const msg = JSON.parse(e.data);
            if (msg.type === "state") {
                log("Received state from " + userId + ": audio=" + msg.audio + " video=" + msg.video + " screen=" + msg.screen);
                dataChannelScreenIds.set(userId, msg.screenStreamId || null);
                dotNetInstance.invokeMethodAsync("OnPeerStateChanged", userId, msg.audio, msg.video, msg.screen)
                    .catch(err => log_error("OnPeerStateChanged failed: " + err));
            }
        } catch (err) {
            log_error("Failed to parse data channel message from " + userId + ": " + err);
        }
    };
}

const dataChannelScreenIds = new Map();

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

    const dc = pc.createDataChannel("state", { ordered: true });
    setupDataChannel(userId, dc);
    log("Created data channel for " + userId);

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

    pc.ondatachannel = e => {
        log("Received data channel from " + userId);
        setupDataChannel(userId, e.channel);
    };

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

        if (pc.connectionState === "connected") {
            if (isSharingScreen && screenStream) {
                const screenTrack = screenStream.getVideoTracks()[0];
                const alreadySending = pc.getSenders().some(s => s.track && s.track === screenTrack);
                if (!alreadySending) {
                    log("Deferred: adding screen track to newly connected peer " + userId);
                    pc.addTrack(screenTrack, screenStream);
                }
            }
        }

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

        if (!e.streams || !e.streams[0]) {
            log_error("Track event from " + userId + " had no streams");
            return;
        }

        const stream = e.streams[0];
        const knownScreenId = dataChannelScreenIds.get(userId);

        if (e.track.kind === "video" && knownScreenId && stream.id === knownScreenId) {
            log("Identified screen stream from " + userId + " (stream id=" + stream.id + ")");
            remoteScreenStreams.set(userId, stream);
            dotNetInstance.invokeMethodAsync("OnRemoteScreenTrack", userId)
                .catch(err => log_error("OnRemoteScreenTrack failed: " + err));
        } else if (e.track.kind !== "video" || stream.id !== (remoteScreenStreams.get(userId) || {}).id) {
            remoteStreams.set(userId, stream);
            dotNetInstance.invokeMethodAsync("OnRemoteTrack", userId)
                .catch(err => log_error("OnRemoteTrack failed: " + err));
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

    const dc = dataChannels.get(userId);
    if (dc) {
        dc.onopen = null;
        dc.onclose = null;
        dc.onerror = null;
        dc.onmessage = null;
        if (dc.readyState === "open") {
            dc.close();
        }
        dataChannels.delete(userId);
    }

    pc.ontrack = null;
    pc.ondatachannel = null;
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
    remoteScreenStreams.delete(userId);
    dataChannelScreenIds.delete(userId);

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

    if (screenStream) {
        screenStream.getTracks().forEach(t => t.stop());
        screenStream = null;
        screenStreamId = null;
    }

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
    isMuted = false;
    isVideoEnabled = true;
    isSharingScreen = false;
    closingConnections.clear();

    log("All connections closed");
}

async function setMuted(muted) {
    if (!webcamStream) {
        return;
    }

    isMuted = muted;
    webcamStream.getAudioTracks().forEach(t => {
        t.enabled = !muted;
    });
    log("Audio " + (muted ? "muted" : "unmuted"));
    broadcastLocalState();
}

async function setVideoEnabled(enabled) {
    if (!webcamStream) {
        return;
    }

    isVideoEnabled = enabled;
    webcamStream.getVideoTracks().forEach(t => {
        t.enabled = enabled;
    });
    log("Video " + (enabled ? "enabled" : "disabled"));
    broadcastLocalState();
}

async function startScreenShare() {
    if (isSharingScreen) {
        log("Already sharing screen");
        return false;
    }

    log("Starting screen share");

    try {
        screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true, audio: false });
    } catch (err) {
        log("Screen share cancelled or failed: " + err.message);
        return false;
    }

    screenStreamId = screenStream.id;
    log("Screen stream id: " + screenStreamId);

    const screenTrack = screenStream.getVideoTracks()[0];

    peerConnections.forEach((pc, userId) => {
        log("Adding screen track to connection with " + userId);
        pc.addTrack(screenTrack, screenStream);
    });

    isSharingScreen = true;
    broadcastLocalState();

    screenTrack.onended = () => {
        log("Screen share ended by browser");
        stopScreenShare();
    };

    return true;
}

async function stopScreenShare() {
    if (!isSharingScreen) {
        return;
    }

    log("Stopping screen share");

    const screenTrack = screenStream && screenStream.getVideoTracks()[0];

    peerConnections.forEach((pc, userId) => {
        const sender = pc.getSenders().find(s => s.track && s.track === screenTrack);
        if (sender) {
            log("Nulling screen sender track for " + userId + " (keeping transceiver)");
            sender.replaceTrack(null).catch(err => log_error("replaceTrack null failed for " + userId + ": " + err));
        }
    });

    if (screenStream) {
        screenStream.getTracks().forEach(t => t.stop());
        screenStream = null;
        screenStreamId = null;
    }

    isSharingScreen = false;
    broadcastLocalState();

    dotNetInstance.invokeMethodAsync("OnLocalScreenStopped")
        .catch(err => log_error("OnLocalScreenStopped failed: " + err));
}

function attachRemoteStream(userId) {
    const stream = remoteStreams.get(userId);
    const video = document.getElementById("video-" + userId);

    if (video && stream) {
        log("Attaching remote cam stream for " + userId);
        video.srcObject = stream;
    } else if (!video) {
        log_error("Cam video element not found for " + userId);
    }
}

function attachRemoteScreenStream(userId) {
    const stream = remoteScreenStreams.get(userId);
    const video = document.getElementById("screen-" + userId);

    if (video && stream) {
        log("Attaching remote screen stream for " + userId);
        video.srcObject = stream;
    } else if (!video) {
        log_error("Screen video element not found for " + userId);
    }
}

window.attachRemoteStream = attachRemoteStream;
window.attachRemoteScreenStream = attachRemoteScreenStream;
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
