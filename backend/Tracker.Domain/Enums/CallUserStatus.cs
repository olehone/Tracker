namespace Tracker.Domain.Enums;

// Muted, Video, Screen is on webrtc data channels
// There is for user status in the hub, whether to peek or not
public enum CallUserStatus
{
    None = 0,
    Peeking = 10,
    Joined = 20,
}
