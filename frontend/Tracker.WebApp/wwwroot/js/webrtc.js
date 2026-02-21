let dotNetInstance = null;

function registerDotNetInstance(instance) {
    dotNetInstance = instance;
}

let clientID = 0;

let mediaConstraints = {
    audio: true,
    video: {
        aspectRatio: {
            ideal: 1.333333,
        },
    },
};

let myUsername = null;        // module-level so all functions can see it
let targetUsername = null;
let myPeerConnection = null;
let transceiver = null;
let webcamStream = null;
let pendingCandidates = [];   // queue for ICE candidates that arrive before remote desc is set

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
    // Set myUsername from the second argument passed by Blazor (AppState.MyId)
    myUsername = username;

    let chatBox = document.querySelector(".chatbox");
    let text = "";
    log("Message received: ");
    log(data);

    let msg;
    try {
        msg = JSON.parse(data);
    } catch (e) {
        log_error("Failed to parse message: " + data);
        return;
    }

    let time = new Date(msg.date);
    let timeStr = time.toLocaleTimeString();

    switch (msg.type) {
        case "userlist":
            handleUserlistMsg(msg);
            break;
        //case "video-offer":
        //    handleVideoOfferMsg(msg);
        //    break;
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

    if (text.length) {
        chatBox.innerHTML += text;
        chatBox.scrollTop = chatBox.scrollHeight - chatBox.clientHeight;
    }
}

async function createPeerConnection() {
    log("Setting up a connection...");

    myPeerConnection = new RTCPeerConnection({
        iceServers: [
            {
                urls: "stun:stun.l.google.com:19302",
            },
        ],
    });

    myPeerConnection.onicecandidate = handleICECandidateEvent;
    myPeerConnection.oniceconnectionstatechange = handleICEConnectionStateChangeEvent;
    myPeerConnection.onicegatheringstatechange = handleICEGatheringStateChangeEvent;
    myPeerConnection.onsignalingstatechange = handleSignalingStateChangeEvent;
    myPeerConnection.onnegotiationneeded = handleNegotiationNeededEvent;
    myPeerConnection.ontrack = handleTrackEvent;
}

async function handleNegotiationNeededEvent() {
    log("*** Negotiation needed");
    try {
        log("---> Creating offer");
        const offer = await myPeerConnection.createOffer();

        if (myPeerConnection.signalingState != "stable") {
            log("     -- The connection isn't stable yet; postponing...");
            return;
        }

        log("---> Setting local description to the offer");
        await myPeerConnection.setLocalDescription(offer);

        log("---> Sending the offer to the remote peer");
        sendToServer({
            name: myUsername,
            target: targetUsername,
            type: "video-offer",
            sdp: myPeerConnection.localDescription,
        });
    } catch (err) {
        log("*** The following error occurred while handling the negotiationneeded event:");
        reportError(err);
    }
}

function handleTrackEvent(event) {
    log("*** Track event");
    document.getElementById("received_video").srcObject = event.streams[0];
    document.getElementById("hangup-button").disabled = false;
}

function handleICECandidateEvent(event) {
    if (event.candidate) {
        log("*** Outgoing ICE candidate: " + event.candidate.candidate);

        sendToServer({
            type: "new-ice-candidate",
            target: targetUsername,
            candidate: event.candidate,
        });
    }
}

function handleICEConnectionStateChangeEvent(event) {
    log("*** ICE connection state changed to " + myPeerConnection.iceConnectionState);

    switch (myPeerConnection.iceConnectionState) {
        case "closed":
        case "failed":
        case "disconnected":
            closeVideoCall();
            break;
    }
}

function handleSignalingStateChangeEvent(event) {
    log("*** WebRTC signaling state changed to: " + myPeerConnection.signalingState);
    switch (myPeerConnection.signalingState) {
        case "closed":
            closeVideoCall();
            break;
    }
}

function handleICEGatheringStateChangeEvent(event) {
    log("*** ICE gathering state changed to: " + myPeerConnection.iceGatheringState);
}

// Rebuild the user list UI and auto-connect if I'm the newest joiner
async function handleUserlistMsg(msg) {
    let listElem = document.querySelector(".userlistbox");

    while (listElem.firstChild) {
        listElem.removeChild(listElem.firstChild);
    }

    msg.users.forEach(function (username) {
        let item = document.createElement("li");
        item.appendChild(document.createTextNode(username));
        item.addEventListener("click", invite, false);
        listElem.appendChild(item);
    });

    // Auto-connect: if there are 2+ users and I'm the last one (newest joiner), I call
    if (msg.users.length >= 2 && !myPeerConnection) {
        const iAmNewest = msg.users[msg.users.length - 1] === myUsername;
        if (iAmNewest) {
            // Call the first other user in the list
            const otherUser = msg.users.find(u => u !== myUsername);
            if (otherUser) {
                log("Auto-connecting to " + otherUser);
                await inviteUser(otherUser);
            }
        }
    }
}

// Shared invite logic (used by auto-connect and click handler)
async function inviteUser(username) {
    if (myPeerConnection) {
        log("Already in a call, ignoring invite to " + username);
        return;
    }

    if (username === myUsername) {
        return;
    }

    targetUsername = username;
    log("Inviting user " + targetUsername);

    createPeerConnection();

    try {
        webcamStream = await navigator.mediaDevices.getUserMedia(mediaConstraints);
        document.getElementById("local_video").srcObject = webcamStream;
    } catch (err) {
        handleGetUserMediaError(err);
        return;
    }

    try {
        webcamStream.getTracks().forEach(
            (transceiver = (track) =>
                myPeerConnection.addTransceiver(track, {
                    streams: [webcamStream],
                })),
        );
    } catch (err) {
        handleGetUserMediaError(err);
    }
}

// Click handler on user list items
async function invite(evt) {
    await inviteUser(evt.target.textContent);
}

async function handleVideoOfferMsg(msg) {
    return handleVideoOffer(msg.name, msg.sdp)
}

async function handleVideoOffer(targetUsername, sdp) {

    log("Received video chat offer from " + targetUsername);
    if (!myPeerConnection) {
        createPeerConnection();
    }

    let desc = new RTCSessionDescription(sdp);

    if (myPeerConnection.signalingState != "stable") {
        log("  - But the signaling state isn't stable, so triggering rollback");
        await Promise.all([
            myPeerConnection.setLocalDescription({ type: "rollback" }),
            myPeerConnection.setRemoteDescription(desc),
        ]);
        return;
    } else {
        log("  - Setting remote description");
        await myPeerConnection.setRemoteDescription(desc);
    }

    // Flush any ICE candidates that arrived before the remote description
    for (const candidate of pendingCandidates) {
        await myPeerConnection.addIceCandidate(candidate).catch(reportError);
    }
    pendingCandidates = [];

    if (!webcamStream) {
        try {
            webcamStream = await navigator.mediaDevices.getUserMedia(mediaConstraints);
        } catch (err) {
            handleGetUserMediaError(err);
            return;
        }

        document.getElementById("local_video").srcObject = webcamStream;

        try {
            webcamStream.getTracks().forEach(
                (transceiver = (track) =>
                    myPeerConnection.addTransceiver(track, {
                        streams: [webcamStream],
                    })),
            );
        } catch (err) {
            handleGetUserMediaError(err);
        }
    }

    log("---> Creating and sending answer to caller");

    await myPeerConnection.setLocalDescription(
        await myPeerConnection.createAnswer(),
    );

    sendToServer({
        name: myUsername,
        target: targetUsername,
        type: "video-answer",
        sdp: myPeerConnection.localDescription,
    });
}

async function handleVideoAnswerMsg(msg) {
    log("*** Call recipient has accepted our call");

    let desc = new RTCSessionDescription(msg.sdp);
    await myPeerConnection.setRemoteDescription(desc).catch(reportError);

    // Flush any ICE candidates that arrived before the remote description
    for (const candidate of pendingCandidates) {
        await myPeerConnection.addIceCandidate(candidate).catch(reportError);
    }
    pendingCandidates = [];
}

async function handleNewICECandidateMsg(msg) {
    let candidate = new RTCIceCandidate(msg.candidate);

    log("*** Adding received ICE candidate: " + JSON.stringify(candidate));

    // If peer connection doesn't exist or remote description isn't set yet, queue it
    if (!myPeerConnection || !myPeerConnection.remoteDescription) {
        log("*** Queuing ICE candidate (remote description not set yet)");
        pendingCandidates.push(candidate);
        return;
    }

    try {
        await myPeerConnection.addIceCandidate(candidate);
    } catch (err) {
        reportError(err);
    }
}

function closeVideoCall() {
    let localVideo = document.getElementById("local_video");

    log("Closing the call");

    if (myPeerConnection) {
        log("--> Closing the peer connection");

        myPeerConnection.ontrack = null;
        myPeerConnection.onicecandidate = null;
        myPeerConnection.oniceconnectionstatechange = null;
        myPeerConnection.onsignalingstatechange = null;
        myPeerConnection.onicegatheringstatechange = null;
        myPeerConnection.onnegotiationneeded = null;

        myPeerConnection.getTransceivers().forEach((transceiver) => {
            transceiver.stop();
        });

        if (localVideo.srcObject) {
            localVideo.pause();
            localVideo.srcObject.getTracks().forEach((track) => {
                track.stop();
            });
        }

        myPeerConnection.close();
        myPeerConnection = null;
        webcamStream = null;
    }

    pendingCandidates = [];
    document.getElementById("hangup-button").disabled = true;
    targetUsername = null;
}

function handleHangUpMsg(msg) {
    log("*** Received hang up notification from other peer");
    closeVideoCall();
}

function hangUpCall() {
    closeVideoCall();

    sendToServer({
        name: myUsername,
        target: targetUsername,
        type: "hang-up",
    });
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

    closeVideoCall();
}

function reportError(errMessage) {
    log_error(`Error ${errMessage.name}: ${errMessage.message}`);
}

window.hangUpCall = hangUpCall;
window.handleReceiveData = handleReceiveData;
window.registerDotNetInstance = registerDotNetInstance;