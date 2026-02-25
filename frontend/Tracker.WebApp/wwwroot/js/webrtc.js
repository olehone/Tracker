let dotNetInstance = null;
let myUserId = null;

let webcamStream = null;
let screenStream = null;

const consoleOutput = true;

const peerConnections = new Map();
const dataChannels = new Map();
const makingOffer = new Map();
const pendingCandidates = new Map();
const remoteStreams = new Map();
const remoteScreenStreams = new Map();
const closingConnections = new Set();
const remoteScreenStreamIds = new Map();

function registerDotNetInstance(instance) {
    dotNetInstance = instance;
    log("DotNet instance registered");
}

function log(text) {
    if (consoleOutput) {
        console.log("[" + new Date().toLocaleTimeString() + "] " + text);
    }
}

function log_error(text) {
    if (consoleOutput) {
        console.trace("[" + new Date().toLocaleTimeString() + "] " + text);
    }
}

function reportError(err) {
    if (consoleOutput) {
        log_error("Error " + err.name + ": " + err.message);
    }
}


async function getLocalStream() {
    if (webcamStream) return webcamStream;
    log("Requesting webcam/mic");
    try {
        webcamStream = await navigator.mediaDevices.getUserMedia({
            audio: true,
            video: { aspectRatio: { ideal: 1.777777 } },
        });
    } catch (err) {
        log_error("getUserMedia denied: " + err.message);
        dotNetInstance.invokeMethodAsync("OnMediaDeviceDenied", "webcam")
            .catch(e => log_error("OnMediaDeviceDenied failed: " + e));
        return null;
    }
    log("Webcam stream acquired (" + webcamStream.getTracks().length + " tracks)");
    return webcamStream;
}

function attachStream(elementId, streamType, userId) {
    const video = document.getElementById(elementId);
    if (!video) { log_error("Element not found: #" + elementId); return; }

    let stream = null;
    if (streamType === "webcam") {
        stream = webcamStream;
    } else if (streamType === "screen") {
        stream = screenStream;
    } else if (streamType === "remote-cam") {
        stream = remoteStreams.get(userId);
    } else if (streamType === "remote-screen") {
        stream = remoteScreenStreams.get(userId);
    }

    if (!stream) { log("No stream yet for " + streamType + " / " + (userId ?? "")); return; }
    if (video.srcObject !== stream) {
        log("Attaching " + streamType + " to #" + elementId);
        video.srcObject = stream;
    }
}

function isPolite(remoteUserId) {
    return myUserId > remoteUserId;
}

function setupDataChannel(userId, dc) {
    dataChannels.set(userId, dc);

    dc.onopen = () => {
        log("Data channel open with " + userId);
        dotNetInstance.invokeMethodAsync("GetLocalState")
            .then(({ audio, video, screen, screenStreamId }) => {
                if (dc.readyState === "open")
                    dc.send(JSON.stringify({ type: "state", audio, video, screen, screenStreamId: screenStreamId ?? null }));
            })
            .catch(err => log_error("GetLocalState failed: " + err));
    };

    dc.onclose = () => log("Data channel closed with " + userId);
    dc.onerror = err => log_error("Data channel error with " + userId + ": " + err);

    dc.onmessage = e => {
        try {
            const msg = JSON.parse(e.data);
            if (msg.type === "state") {
                log("State from " + userId + ": audio=" + msg.audio + " video=" + msg.video + " screen=" + msg.screen);
                remoteScreenStreamIds.set(userId, msg.screenStreamId || null);
                dotNetInstance.invokeMethodAsync("OnPeerStateChanged", userId, msg.audio, msg.video, msg.screen)
                    .catch(err => log_error("OnPeerStateChanged failed: " + err));
            }
        } catch (err) {
            log_error("Failed to parse data channel message from " + userId + ": " + err);
        }
    };
}

function broadcastState(audio, video, screen, screenStreamId) {
    const msg = JSON.stringify({ type: "state", audio, video, screen, screenStreamId: screenStreamId ?? null });
    dataChannels.forEach((dc, userId) => {
        if (dc.readyState === "open") {
            dc.send(msg);
            log("Broadcast state to " + userId);
        }
    });
}

async function initiateCall(userId, selfId) {
    if (peerConnections.has(userId)) { log("Already connected to " + userId); return; }
    if (userId === selfId) { log("Skipping self"); return; }

    myUserId = selfId;
    log("Initiating call with " + userId);
    peerConnections.set(userId, null);

    const stream = await getLocalStream();

    if (!stream) {
        log("No local stream for " + userId + " — aborting call initiation");
        peerConnections.delete(userId); // clear sentinel
        return;
    }

    if (peerConnections.get(userId) !== null) {
        log("Offer arrived while awaiting stream for " + userId + " — skipping initiator role");
        return;
    }

    const pc = createPeerConnection(userId);

    const dc = pc.createDataChannel("state", { ordered: true });
    setupDataChannel(userId, dc);

    stream.getTracks().forEach(t => {
        log("Adding " + t.kind + " track to " + userId);
        pc.addTrack(t, stream);
    });
}

function createPeerConnection(userId) {
    log("Creating RTCPeerConnection with " + userId);

    closingConnections.delete(userId);
    makingOffer.set(userId, false);
    pendingCandidates.set(userId, []);

    const pc = new RTCPeerConnection({
        iceServers: [{ urls: "stun:stun.l.google.com:19302" }],
    });
    peerConnections.set(userId, pc);

    pc.ondatachannel = e => {
        log("Incoming data channel from " + userId);
        setupDataChannel(userId, e.channel);
    };

    pc.onicecandidate = e => {
        if (e.candidate) {
            dotNetInstance.invokeMethodAsync("SendIceCandidate", userId, JSON.stringify(e.candidate));
        } else {
            log("ICE gathering complete for " + userId);
        }
    };

    pc.onicegatheringstatechange = () => log("ICE gathering:   " + userId + " → " + pc.iceGatheringState);
    pc.onsignalingstatechange = () => log("Signaling state: " + userId + " → " + pc.signalingState);

    pc.oniceconnectionstatechange = () => {
        log("ICE connection: " + userId + " → " + pc.iceConnectionState);
        if (["closed", "failed", "disconnected"].includes(pc.iceConnectionState))
            handlePeerLeaved(userId);
    };

    pc.onconnectionstatechange = () => {
        log("Connection state: " + userId + " → " + pc.connectionState);

        if (pc.connectionState === "connected" && screenStream) {
            const screenTrack = screenStream.getVideoTracks()[0];
            const alreadySending = pc.getSenders().some(s => s.track === screenTrack);
            if (!alreadySending) {
                log("Deferred: adding screen track to " + userId);
                pc.addTrack(screenTrack, screenStream);
            }
        }

        if (pc.connectionState === "failed")
            handlePeerLeaved(userId);
    };

    pc.onnegotiationneeded = async () => {
        if (pc.signalingState !== "stable" || makingOffer.get(userId)) {
            log("Negotiation needed with " + userId + " — skipping (state=" + pc.signalingState + ")");
            return;
        }
        log("Negotiation needed with " + userId);
        try {
            makingOffer.set(userId, true);
            await pc.setLocalDescription();
            dotNetInstance.invokeMethodAsync("SendVideoOffer", userId, JSON.stringify(pc.localDescription));
        } catch (err) {
            reportError(err);
        } finally {
            makingOffer.set(userId, false);
        }
    };

    pc.ontrack = e => {
        if (!e.streams?.[0]) { log_error("Track from " + userId + " had no stream"); return; }

        const stream = e.streams[0];
        const knownScreenId = remoteScreenStreamIds.get(userId);

        if (e.track.kind === "video" && knownScreenId && stream.id === knownScreenId) {
            log("Screen track from " + userId);
            remoteScreenStreams.set(userId, stream);
            dotNetInstance.invokeMethodAsync("OnRemoteScreenTrack", userId)
                .catch(err => log_error("OnRemoteScreenTrack failed: " + err));
        } else {
            remoteStreams.set(userId, stream);
            dotNetInstance.invokeMethodAsync("OnRemoteTrack", userId)
                .catch(err => log_error("OnRemoteTrack failed: " + err));
        }
    };

    return pc;
}

async function flushPendingCandidates(userId) {
    const pc = peerConnections.get(userId);
    const queued = pendingCandidates.get(userId) || [];
    if (!queued.length) return;
    log("Flushing " + queued.length + " queued candidates for " + userId);
    pendingCandidates.set(userId, []);
    for (const c of queued) await pc.addIceCandidate(c).catch(reportError);
}

async function receiveVideoOffer(fromUserId, sdpJson) {
    log("Offer from " + fromUserId);
    let pc = peerConnections.get(fromUserId);

    if (pc === null) {
        log("Clearing sentinel for " + fromUserId + " — taking answerer role");
        peerConnections.delete(fromUserId);
        pc = null;
    }

    if (!pc) {
        const stream = await getLocalStream();
        if (!stream) { log("No local stream — cannot answer offer from " + fromUserId); return; }
        pc = createPeerConnection(fromUserId);
        stream.getTracks().forEach(t => pc.addTrack(t, stream));
    }

    const offerCollision = makingOffer.get(fromUserId) || pc.signalingState !== "stable";
    if (offerCollision) {
        if (!isPolite(fromUserId)) {
            log("Impolite: ignoring colliding offer from " + fromUserId);
            return;
        }
        log("Polite: rolling back local offer for " + fromUserId);
        await pc.setLocalDescription({ type: "rollback" });
        pendingCandidates.set(fromUserId, []);
    }

    await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(sdpJson)));
    await flushPendingCandidates(fromUserId);

    if (pc.signalingState === "have-remote-offer") {
        await pc.setLocalDescription();
        dotNetInstance.invokeMethodAsync("SendVideoAnswer", fromUserId, JSON.stringify(pc.localDescription));
    }
}

async function receiveVideoAnswer(fromUserId, sdpJson) {
    log("Answer from " + fromUserId);
    const pc = peerConnections.get(fromUserId);
    if (!pc) { log_error("No PC for answer from " + fromUserId); return; }
    if (pc.signalingState !== "have-local-offer") {
        log("Ignoring answer in state " + pc.signalingState);
        return;
    }
    await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(sdpJson))).catch(reportError);
    await flushPendingCandidates(fromUserId);
}

async function receiveIceCandidate(fromUserId, candidateJson) {
    const pc = peerConnections.get(fromUserId);
    const candidate = new RTCIceCandidate(JSON.parse(candidateJson));

    if (!pc) { log("Dropping ICE from " + fromUserId + " (no PC)"); return; }

    if (!pc.remoteDescription?.type) {
        log("Queuing ICE from " + fromUserId);
        pendingCandidates.get(fromUserId)?.push(candidate);
        return;
    }

    await pc.addIceCandidate(candidate).catch(err => {
        if (!isPolite(fromUserId) && makingOffer.get(fromUserId)) return; // glare — safe to ignore
        reportError(err);
    });
    log("Added ICE from " + fromUserId);
}

function handleLeaved(userId) {
    if (closingConnections.has(userId)) return;
    closingConnections.add(userId);
    log("Peer gone: " + userId);
    closePeerConnection(userId);
    dotNetInstance.invokeMethodAsync("OnPeerDisconnected", userId)
        .catch(err => log_error("OnPeerDisconnected failed: " + err));
}

function closePeerConnection(userId) {
    const pc = peerConnections.get(userId);
    if (!pc) return;

    const dc = dataChannels.get(userId);
    if (dc) {
        dc.onopen = dc.onclose = dc.onerror = dc.onmessage = null;
        if (dc.readyState === "open") dc.close();
        dataChannels.delete(userId);
    }

    pc.ontrack = pc.ondatachannel = pc.onicecandidate = null;
    pc.onicegatheringstatechange = pc.oniceconnectionstatechange = null;
    pc.onconnectionstatechange = pc.onsignalingstatechange = null;
    pc.onnegotiationneeded = null;

    pc.getSenders().forEach(s => { if (s.track) s.replaceTrack(null).catch(() => { }); });
    pc.close();

    peerConnections.delete(userId);
    makingOffer.delete(userId);
    pendingCandidates.delete(userId);
    remoteStreams.delete(userId);
    remoteScreenStreams.delete(userId);
    remoteScreenStreamIds.delete(userId);

    log("Closed connection with " + userId);
}

async function closeStreams({ keepLocalStream = false } = {}) {
    log("Closing streams");
    if (screenStream) {
        screenStream.getTracks().forEach(t => t.stop());
        screenStream = null;
    }

    if (!keepLocalStream && webcamStream) {
        webcamStream.getTracks().forEach(t => t.stop());
        webcamStream = null;
        const el = document.getElementById("local_video");
        if (el) el.srcObject = null;
    }

    myUserId = null;
    closingConnections.clear();
    log("All streams closed");
}

async function setMuted(muted) {
    if (!webcamStream) return;

    if (muted) {
        webcamStream.getAudioTracks().forEach(t => { t.stop(); webcamStream.removeTrack(t); });
        log("Microphone released");
    } else {
        let fresh;
        try { fresh = await navigator.mediaDevices.getUserMedia({ audio: true, video: false }); }
        catch (err) { log_error("Mic re-acquire failed: " + err); return; }

        const track = fresh.getAudioTracks()[0];
        webcamStream.addTrack(track);

        await Promise.allSettled(
            Array.from(peerConnections.entries()).map(([userId, pc]) => {
                const sender = pc.getSenders().find(s => s.track?.kind === "audio");
                if (sender && sender.track != track) {
                    log("Replacing audio for " + userId);
                    return sender.replaceTrack(track);
                }
            })
        );
        log("Microphone re-acquired");
    }
}

async function setVideoEnabled(enabled) {
    if (!webcamStream) return;

    if (!enabled) {
        webcamStream.getVideoTracks().forEach(t => { t.stop(); webcamStream.removeTrack(t); });

        await Promise.allSettled(
            Array.from(peerConnections.entries()).map(([userId, pc]) => {
                const sender = pc.getSenders().find(s => s.track?.kind === "video");
                if (sender) { log("Clearing video for " + userId); return sender.replaceTrack(null); }
            })
        );

        const el = document.getElementById("local_video");
        if (el) el.srcObject = null;
        log("Camera released");
    } else {
        let fresh;
        try {
            fresh = await navigator.mediaDevices.getUserMedia({
                audio: false,
                video: { aspectRatio: { ideal: 1.777777 } },
            });
        } catch (err) { log_error("Camera re-acquire failed: " + err); return; }

        const track = fresh.getVideoTracks()[0];
        webcamStream.addTrack(track);

        await Promise.allSettled(
            Array.from(peerConnections.entries()).map(([userId, pc]) => {
                const sender = pc.getSenders().find(s => s.track === null || s.track?.kind === "video");
                if (sender && sender.track != track) {
                    log("Replacing video for " + userId);
                    return sender.replaceTrack(track);
                }
            })
        );

        const localVideo = document.getElementById("local_video");
        if (localVideo) localVideo.srcObject = webcamStream;
        log("Camera re-acquired");
    }
}


async function startScreenShare() {
    if (screenStream) return screenStream.id;
    log("Starting screen share");

    try {
        screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true, audio: false });
    } catch (err) {
        log("Screen share cancelled: " + err.message);
        return null;
    }

    const track = screenStream.getVideoTracks()[0];

    peerConnections.forEach((pc, userId) => {
        log("Adding screen track to " + userId);
        pc.addTrack(track, screenStream);
    });

    track.onended = () => {
        log("Screen share ended by browser");
        stopScreenShare();
    };

    return screenStream.id;
}

async function stopScreenShare() {
    if (!screenStream) return;
    log("Stopping screen share");

    const track = screenStream.getVideoTracks()[0];

    peerConnections.forEach((pc, userId) => {
        const sender = pc.getSenders().find(s => s.track === track);
        if (sender) sender.replaceTrack(null).catch(err =>
            log_error("replaceTrack null failed for " + userId + ": " + err));
    });

    screenStream.getTracks().forEach(t => t.stop());
    screenStream = null;

    dotNetInstance.invokeMethodAsync("OnLocalScreenStopped")
        .catch(err => log_error("OnLocalScreenStopped failed: " + err));
}

window.registerDotNetInstance = registerDotNetInstance;
window.getLocalStream = getLocalStream;
window.attachStream = attachStream;
window.broadcastState = broadcastState;
window.initiateCall = initiateCall;

window.receiveVideoOffer = receiveVideoOffer;
window.receiveVideoAnswer = receiveVideoAnswer;
window.receiveIceCandidate = receiveIceCandidate;
window.handleLeaved = handleLeaved;

window.closeStreams = closeStreams;

window.setMuted = setMuted;
window.setVideoEnabled = setVideoEnabled;
window.startScreenShare = startScreenShare;
window.stopScreenShare = stopScreenShare;