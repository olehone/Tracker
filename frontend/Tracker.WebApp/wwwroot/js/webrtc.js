let dotNetInstance = null;
let myUserId = null;

function registerDotNetInstance(instance) {
    dotNetInstance = instance;
    log("DotNet instance registered");
}

// -------------------------------------------------------------------------
// Media constraints / selected devices
// -------------------------------------------------------------------------

let selectedAudioDeviceId = null;
let selectedVideoDeviceId = null;

function buildMediaConstraints() {
    return {
        audio: selectedAudioDeviceId ? { deviceId: { exact: selectedAudioDeviceId } } : true,
        video: selectedVideoDeviceId
            ? { deviceId: { exact: selectedVideoDeviceId }, aspectRatio: { ideal: 1.333333 } }
            : { aspectRatio: { ideal: 1.333333 } },
    };
}

// -------------------------------------------------------------------------
// Peer state
// -------------------------------------------------------------------------

const peerConnections = new Map();
const dataChannels = new Map();
const makingOffer = new Map();
const pendingCandidates = new Map();
const remoteStreams = new Map();
const remoteScreenStreams = new Map();
const closingConnections = new Set();
const dataChannelScreenIds = new Map();

let webcamStream = null;
let screenStream = null;
let screenStreamId = null;
let isMuted = false;
let isVideoEnabled = true;
let isSharingScreen = false;

// -------------------------------------------------------------------------
// Logging
// -------------------------------------------------------------------------

function log(text) {
    console.log("[" + new Date().toLocaleTimeString() + "] " + text);
}

function log_error(text) {
    console.trace("[" + new Date().toLocaleTimeString() + "] " + text);
}

function reportError(errMessage) {
    log_error("Error " + errMessage.name + ": " + errMessage.message);
}

// -------------------------------------------------------------------------
// Polite peer logic
// -------------------------------------------------------------------------

function isPolite(remoteUserId) {
    return myUserId > remoteUserId;
}

// -------------------------------------------------------------------------
// Data channel / state messages
// -------------------------------------------------------------------------

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
            log("Sent state to " + userId);
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

    dc.onclose = () => log("Data channel closed with " + userId);
    dc.onerror = err => log_error("Data channel error with " + userId + ": " + err);

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

// -------------------------------------------------------------------------
// Local stream
// -------------------------------------------------------------------------

async function getLocalStream() {
    if (webcamStream) {
        log("Reusing existing local stream");
        return webcamStream;
    }

    log("Requesting local media");
    webcamStream = await navigator.mediaDevices.getUserMedia(buildMediaConstraints());
    log("Local stream acquired (" + webcamStream.getTracks().length + " tracks)");
    return webcamStream;
}

// Re-attach the local stream to #local_video — called from OnAfterRenderAsync
// so it survives DOM replacement when switching preview <-> in-call UI.
function attachLocalStream() {
    const video = document.getElementById("local_video");
    if (!video || !webcamStream) return;

    if (video.srcObject !== webcamStream) {
        log("Attaching local stream to #local_video");
        video.srcObject = webcamStream;
    }
}

// -------------------------------------------------------------------------
// Device enumeration
// -------------------------------------------------------------------------

async function enumerateDevices() {
    if (!webcamStream) {
        try {
            const probe = await navigator.mediaDevices.getUserMedia({ audio: true, video: true });
            probe.getTracks().forEach(t => t.stop());
        } catch (_) {
            return { audioDevices: [], videoDevices: [] };
        }
    }

    const devices = await navigator.mediaDevices.enumerateDevices();

    const audioDevices = devices
        .filter(d => d.kind === "audioinput")
        .map(d => ({ deviceId: d.deviceId, label: d.label || "Microphone " + d.deviceId.slice(0, 4) }));

    const videoDevices = devices
        .filter(d => d.kind === "videoinput")
        .map(d => ({ deviceId: d.deviceId, label: d.label || "Camera " + d.deviceId.slice(0, 4) }));

    return { audioDevices, videoDevices };
}

// -------------------------------------------------------------------------
// Preview (lobby)
// -------------------------------------------------------------------------

async function startLocalPreview() {
    log("Starting local preview");
    try {
        await getLocalStream();
        attachLocalStream();
        log("Local preview started");
    } catch (err) {
        log_error("Failed to start local preview: " + err);
    }
}

async function stopLocalPreview() {
    log("Stopping local preview");
    if (webcamStream) {
        webcamStream.getTracks().forEach(t => t.stop());
        webcamStream = null;
    }
    const localVideo = document.getElementById("local_video");
    if (localVideo) localVideo.srcObject = null;
    log("Local preview stopped");
}

// -------------------------------------------------------------------------
// Device switching
//
// KEY FIX: stop the old track on webcamStream BEFORE calling getUserMedia
// with the new deviceId. Some OSes (Windows + certain cameras) treat the
// device as exclusively locked until every MediaStreamTrack using it is
// stopped — opening it again while the old track is still live throws
// "Device in use" / NotReadableError.
// -------------------------------------------------------------------------

async function switchAudioDevice(deviceId) {
    selectedAudioDeviceId = deviceId;
    log("Switching audio device to " + deviceId);

    if (!webcamStream) return;

    // 1. Stop and remove old audio tracks first to release the device
    webcamStream.getAudioTracks().forEach(t => {
        t.stop();
        webcamStream.removeTrack(t);
    });

    // 2. Now acquire the new device — it's free
    const newStream = await navigator.mediaDevices.getUserMedia({
        audio: { deviceId: { exact: deviceId } },
        video: false,
    });
    const newAudioTrack = newStream.getAudioTracks()[0];

    // 3. Swap into webcamStream and apply current mute state
    webcamStream.addTrack(newAudioTrack);
    newAudioTrack.enabled = !isMuted;

    // 4. Replace in all active peer connections (no renegotiation needed)
    for (const [userId, pc] of peerConnections) {
        const sender = pc.getSenders().find(s => s.track && s.track.kind === "audio");
        if (sender) {
            await sender.replaceTrack(newAudioTrack).catch(err =>
                log_error("replaceTrack audio failed for " + userId + ": " + err));
        }
    }

    log("Audio device switched");
}

async function switchVideoDevice(deviceId) {
    selectedVideoDeviceId = deviceId;
    log("Switching video device to " + deviceId);

    if (!webcamStream) return;

    // 1. Stop and remove old video tracks first to release the device
    webcamStream.getVideoTracks().forEach(t => {
        t.stop();
        webcamStream.removeTrack(t);
    });

    // 2. Now acquire the new device
    const newStream = await navigator.mediaDevices.getUserMedia({
        audio: false,
        video: { deviceId: { exact: deviceId }, aspectRatio: { ideal: 1.333333 } },
    });
    const newVideoTrack = newStream.getVideoTracks()[0];

    // 3. Swap into webcamStream and apply current video-enabled state
    webcamStream.addTrack(newVideoTrack);
    newVideoTrack.enabled = isVideoEnabled;

    // 4. Replace in all active peer connections, skipping the screen sender
    const screenTrack = screenStream && screenStream.getVideoTracks()[0];
    for (const [userId, pc] of peerConnections) {
        const sender = pc.getSenders().find(s =>
            s.track && s.track.kind === "video" && s.track !== screenTrack);
        if (sender) {
            await sender.replaceTrack(newVideoTrack).catch(err =>
                log_error("replaceTrack video failed for " + userId + ": " + err));
        }
    }

    // 5. Re-attach to #local_video so the preview reflects the new camera
    attachLocalStream();

    log("Video device switched");
}

// -------------------------------------------------------------------------
// In-call peer connection management
// -------------------------------------------------------------------------

async function initiateCall(userId, selfId) {
    if (peerConnections.has(userId)) {
        log("Already have connection with " + userId + ", skipping");
        return;
    }
    if (userId === selfId) {
        log("Skipping self: " + userId);
        return;
    }

    myUserId = selfId;
    log("Initiating call with " + userId + " (polite=" + isPolite(userId) + ")");

    const stream = await getLocalStream();
    attachLocalStream();
    const pc = createPeerConnection(userId);

    const dc = pc.createDataChannel("state", { ordered: true });
    setupDataChannel(userId, dc);

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

    pc.onicegatheringstatechange = () =>
        log("ICE gathering state with " + userId + ": " + pc.iceGatheringState);

    pc.oniceconnectionstatechange = () => {
        log("ICE connection state with " + userId + ": " + pc.iceConnectionState);
        if (["closed", "failed", "disconnected"].includes(pc.iceConnectionState)) {
            handlePeerGone(userId);
        }
    };

    pc.onconnectionstatechange = () => {
        log("Connection state with " + userId + ": " + pc.connectionState);

        if (pc.connectionState === "connected" && isSharingScreen && screenStream) {
            const screenTrack = screenStream.getVideoTracks()[0];
            const alreadySending = pc.getSenders().some(s => s.track && s.track === screenTrack);
            if (!alreadySending) {
                log("Deferred: adding screen track to newly connected peer " + userId);
                pc.addTrack(screenTrack, screenStream);
            }
        }

        if (pc.connectionState === "failed") {
            handlePeerGone(userId);
        }
    };

    pc.onsignalingstatechange = () =>
        log("Signaling state with " + userId + ": " + pc.signalingState);

    pc.onnegotiationneeded = async () => {
        log("Negotiation needed with " + userId);
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
        log("Track event from " + userId + " (kind=" + e.track.kind + ")");

        if (!e.streams || !e.streams[0]) {
            log_error("Track event from " + userId + " had no streams");
            return;
        }

        const stream = e.streams[0];
        const knownScreenId = dataChannelScreenIds.get(userId);

        if (e.track.kind === "video" && knownScreenId && stream.id === knownScreenId) {
            log("Identified screen stream from " + userId);
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

async function flushPendingCandidates(userId) {
    const pc = peerConnections.get(userId);
    const queued = pendingCandidates.get(userId) || [];
    if (queued.length === 0) return;

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
        attachLocalStream();
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

    await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(sdpJson)));
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
    if (!pc) { log_error("No peer connection for answer from " + fromUserId); return; }

    if (pc.signalingState !== "have-local-offer") {
        log("Ignoring answer from " + fromUserId + " in state " + pc.signalingState);
        return;
    }

    await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(sdpJson))).catch(reportError);
    await flushPendingCandidates(fromUserId);
}

async function receiveIceCandidate(fromUserId, candidateJson) {
    const pc = peerConnections.get(fromUserId);
    const candidate = new RTCIceCandidate(JSON.parse(candidateJson));

    if (!pc) { log("Dropping ICE candidate from " + fromUserId + " (no peer connection)"); return; }

    if (!pc.remoteDescription || pc.remoteDescription.type === "") {
        log("Queuing ICE candidate from " + fromUserId);
        const queue = pendingCandidates.get(fromUserId);
        if (queue) queue.push(candidate);
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

    log("Closing RTCPeerConnection with " + userId);

    const dc = dataChannels.get(userId);
    if (dc) {
        dc.onopen = null; dc.onclose = null; dc.onerror = null; dc.onmessage = null;
        if (dc.readyState === "open") dc.close();
        dataChannels.delete(userId);
    }

    pc.ontrack = null; pc.ondatachannel = null; pc.onicecandidate = null;
    pc.onicegatheringstatechange = null; pc.oniceconnectionstatechange = null;
    pc.onconnectionstatechange = null; pc.onsignalingstatechange = null;
    pc.onnegotiationneeded = null;

    pc.getSenders().forEach(sender => {
        if (sender.track) sender.replaceTrack(null).catch(() => {});
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

async function hangUpAll({ keepLocalStream = false } = {}) {
    log("Hanging up all (" + peerConnections.size + " peers), keepLocalStream=" + keepLocalStream);

    const hangUpPromises = [];
    peerConnections.forEach((_, userId) => {
        hangUpPromises.push(
            dotNetInstance.invokeMethodAsync("SendHangUp", userId)
                .catch(err => log_error("SendHangUp failed for " + userId + ": " + err))
        );
    });

    await Promise.allSettled(hangUpPromises);
    Array.from(peerConnections.keys()).forEach(userId => closePeerConnection(userId));

    if (screenStream) {
        screenStream.getTracks().forEach(t => t.stop());
        screenStream = null;
        screenStreamId = null;
    }

    if (!keepLocalStream) {
        if (webcamStream) {
            webcamStream.getTracks().forEach(t => t.stop());
            webcamStream = null;
        }
        const localVideo = document.getElementById("local_video");
        if (localVideo) localVideo.srcObject = null;
    }

    myUserId = null;
    isMuted = false;
    isVideoEnabled = true;
    isSharingScreen = false;
    closingConnections.clear();

    log("All connections closed");
}

// -------------------------------------------------------------------------
// Media controls
// -------------------------------------------------------------------------

async function setMuted(muted) {
    if (!webcamStream) return;
    isMuted = muted;
    webcamStream.getAudioTracks().forEach(t => t.enabled = !muted);
    log("Audio " + (muted ? "muted" : "unmuted"));
    broadcastLocalState();
}

async function setVideoEnabled(enabled) {
    if (!webcamStream) return;
    isVideoEnabled = enabled;
    webcamStream.getVideoTracks().forEach(t => t.enabled = enabled);
    log("Video " + (enabled ? "enabled" : "disabled"));
    broadcastLocalState();
}

async function startScreenShare() {
    if (isSharingScreen) return false;
    log("Starting screen share");

    try {
        screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true, audio: false });
    } catch (err) {
        log("Screen share cancelled or failed: " + err.message);
        return false;
    }

    screenStreamId = screenStream.id;
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
    if (!isSharingScreen) return;
    log("Stopping screen share");

    const screenTrack = screenStream && screenStream.getVideoTracks()[0];

    peerConnections.forEach((pc, userId) => {
        const sender = pc.getSenders().find(s => s.track && s.track === screenTrack);
        if (sender) sender.replaceTrack(null).catch(err =>
            log_error("replaceTrack null failed for " + userId + ": " + err));
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

// -------------------------------------------------------------------------
// Attach helpers (called from Blazor OnAfterRenderAsync)
// -------------------------------------------------------------------------

function attachRemoteStream(userId) {
    const stream = remoteStreams.get(userId);
    const video = document.getElementById("video-" + userId);

    if (video && stream) {
        if (video.srcObject !== stream) {
            log("Attaching remote cam stream for " + userId);
            video.srcObject = stream;
        }
    } else if (!video) {
        log_error("Cam video element not found for " + userId);
    }
}

function attachRemoteScreenStream(userId) {
    const stream = remoteScreenStreams.get(userId);
    const video = document.getElementById("screen-" + userId);

    if (video && stream) {
        if (video.srcObject !== stream) {
            log("Attaching remote screen stream for " + userId);
            video.srcObject = stream;
        }
    } else if (!video) {
        log_error("Screen video element not found for " + userId);
    }
}

// Attaches own screen share stream to the local screen preview element.
function attachLocalScreenStream() {
    const video = document.getElementById("local_screen");
    if (!video || !screenStream) return;
    if (video.srcObject !== screenStream) {
        log("Attaching local screen stream to #local_screen");
        video.srcObject = screenStream;
    }
}

// Generic attach by element id — used by the participant drawer thumbnails.
// userId = "local" uses webcamStream, otherwise looks up remoteStreams.
function attachStreamToElement(elementId, userId) {
    const video = document.getElementById(elementId);
    if (!video) return;

    const stream = userId === "local"
        ? webcamStream
        : remoteStreams.get(userId);

    if (stream && video.srcObject !== stream) {
        log("Attaching stream to #" + elementId + " (userId=" + userId + ")");
        video.srcObject = stream;
    }
}

// -------------------------------------------------------------------------
// Exports
// -------------------------------------------------------------------------

window.startLocalPreview = startLocalPreview;
window.stopLocalPreview = stopLocalPreview;
window.attachLocalStream = attachLocalStream;
window.attachLocalScreenStream = attachLocalScreenStream;
window.attachStreamToElement = attachStreamToElement;
window.enumerateDevices = enumerateDevices;
window.switchAudioDevice = switchAudioDevice;
window.switchVideoDevice = switchVideoDevice;
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
