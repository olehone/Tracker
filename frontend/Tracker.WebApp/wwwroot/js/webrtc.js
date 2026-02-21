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
let myUsername = null;
let webcamStream = null;

function log(text) {
    let time = new Date();
    console.log("[" + time.toLocaleTimeString() + "] " + text);
}

function log_error(text) {
    let time = new Date();
    console.trace("[" + time.toLocaleTimeString() + "] " + text);
}

async function sendToServer(msg) {
    let msgJSON = JSON.stringify(msg);
    log("Sending '" + msg.type + "' message: " + msgJSON);
    dotNetInstance.invokeMethodAsync("SendToServer", msgJSON);
}

function handleReceiveData(data, username) {
    myUsername = username;

    log("Message received: ");
    log(data);

    let msg;
    try {
        msg = JSON.parse(data);
    } catch (e) {
        log_error("Failed to parse message: " + data);
        return;
    }

    switch (msg.type) {
        case "userlist":
            handleUserlistMsg(msg);
            break;
        case "video-offer":
            handleVideoOfferMsg(msg);
            break;
        case "video-answer":
            handleVideoAnswerMsg(msg);
            break;
        case "new-ice-candidate":
            handleNewICECandidateMsg(msg);
            break;
        case "hang-up":
            handleHangUpMsg(msg);
            break;
        default:
            log_error("Unknown message received:");
            log_error(msg);
    }
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
            sendToServer({ type: "new-ice-candidate", target: userId, candidate: e.candidate });
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
                log("     -- Not stable yet, postponing");
                return;
            }
            await pc.setLocalDescription(offer);
            log("---> Sending offer to " + userId);
            sendToServer({ name: myUsername, target: userId, type: "video-offer", sdp: pc.localDescription });
        } catch (err) { reportError(err); }
    };
    pc.ontrack = e => {
        log("*** Track event from " + userId);
        dotNetInstance.invokeMethodAsync("OnRemoteTrack", userId).then(() => {
            let video = document.getElementById("video-" + userId);
            if (video) video.srcObject = e.streams[0];
        });
    };

    return pc;
}

async function getLocalStream() {
    if (webcamStream) return webcamStream;
    webcamStream = await navigator.mediaDevices.getUserMedia(mediaConstraints);
    document.getElementById("local_video").srcObject = webcamStream;
    return webcamStream;
}

async function inviteUser(userId) {
    if (peerConnections.has(userId) || userId === myUsername) return;

    log("Inviting user " + userId);
    const pc = await createPeerConnection(userId);
    const stream = await getLocalStream();
    stream.getTracks().forEach(track => pc.addTrack(track, stream));
}

async function handleUserlistMsg(msg) {
    if (msg.users.length >= 2) {
        const iAmNewest = msg.users[msg.users.length - 1] === myUsername;
        if (iAmNewest) {
            for (const userId of msg.users) {
                if (userId !== myUsername) {
                    log("Auto-connecting to " + userId);
                    await inviteUser(userId);
                }
            }
        }
    }
}

async function handleVideoOfferMsg(msg) {
    const callerId = msg.name;
    log("Received video offer from " + callerId);

    let pc = peerConnections.get(callerId);
    if (!pc) pc = await createPeerConnection(callerId);

    const stream = await getLocalStream();
    if (pc.getSenders().length === 0) {
        stream.getTracks().forEach(track => pc.addTrack(track, stream));
    }

    const desc = new RTCSessionDescription(msg.sdp);
    if (pc.signalingState !== "stable") {
        log("  - Signaling not stable, triggering rollback");
        await Promise.all([
            pc.setLocalDescription({ type: "rollback" }),
            pc.setRemoteDescription(desc),
        ]);
    } else {
        await pc.setRemoteDescription(desc);
    }

    for (const c of (pendingCandidates.get(callerId) || [])) {
        await pc.addIceCandidate(c).catch(reportError);
    }
    pendingCandidates.set(callerId, []);

    await pc.setLocalDescription(await pc.createAnswer());
    log("---> Sending answer to " + callerId);
    sendToServer({ name: myUsername, target: callerId, type: "video-answer", sdp: pc.localDescription });
}

async function handleVideoAnswerMsg(msg) {
    log("*** Call accepted by " + msg.name);
    const pc = peerConnections.get(msg.name);
    if (!pc) return;

    await pc.setRemoteDescription(new RTCSessionDescription(msg.sdp)).catch(reportError);

    for (const c of (pendingCandidates.get(msg.name) || [])) {
        await pc.addIceCandidate(c).catch(reportError);
    }
    pendingCandidates.set(msg.name, []);
}

async function handleNewICECandidateMsg(msg) {
    const candidate = new RTCIceCandidate(msg.candidate);
    const pc = peerConnections.get(msg.name);

    log("*** Incoming ICE candidate from " + msg.name);

    if (!pc || !pc.remoteDescription) {
        log("*** Queuing ICE candidate from " + msg.name);
        if (!pendingCandidates.has(msg.name)) pendingCandidates.set(msg.name, []);
        pendingCandidates.get(msg.name).push(candidate);
        return;
    }
    await pc.addIceCandidate(candidate).catch(reportError);
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
    dotNetInstance.invokeMethodAsync("OnPeerDisconnected", userId);
}

function handleHangUpMsg(msg) {
    log("*** Hang up from " + msg.name);
    closePeerConnection(msg.name);
}

function hangUpCall() {
    log("Hanging up call");

    peerConnections.forEach((pc, userId) => {
        sendToServer({ name: myUsername, target: userId, type: "hang-up" });
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

function handleGetUserMediaError(e) {
    log_error(e);
    switch (e.name) {
        case "NotFoundError":
            alert("Unable to open your call because no camera and/or microphone were found.");
            break;
        case "SecurityError":
        case "PermissionDeniedError":
            break;
        default:
            alert("Error opening your camera and/or microphone: " + e.message);
            break;
    }
}

function reportError(errMessage) {
    log_error(`Error ${errMessage.name}: ${errMessage.message}`);
}

window.hangUpCall = hangUpCall;
window.handleReceiveData = handleReceiveData;
window.registerDotNetInstance = registerDotNetInstance;