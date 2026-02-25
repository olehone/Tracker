namespace Tracker.Services.Realtime.Methods;

public static class CallRealtimeMethods
{
    public const string Peek = "Peek";
    public const string Join = "Join";
    public const string Leave = "Leave";

    public const string SendVideoOffer = "SendVideoOffer";
    public const string SendVideoAnswer = "SendVideoAnswer";
    public const string SendIceCandidate = "SendIceCandidate";
    public const string SendHangUp = "SendHangUp";

    public const string CallEnded = "CallEnded";
    public const string UserJoined = "UserJoined";
    public const string UserLeaved = "UserLeaved";
   
    public const string ReceiveVideoOffer = "ReceiveVideoOffer";
    public const string ReceiveVideoAnswer = "ReceiveVideoAnswer";
    public const string ReceiveIceCandidate = "ReceiveIceCandidate";
}