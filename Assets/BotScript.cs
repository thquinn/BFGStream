using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.Api;
using System.Text.RegularExpressions;
using System;

public class BotScript : MonoBehaviour {
    static readonly string TWITCH_CHANNEL_TO_JOIN = GameScript.CONFIG["channel"];
    static readonly string TWITCH_ACCESS_TOKEN_BOT = GameScript.CONFIG["channel_oauth"];
    static readonly string TWITCH_BOT_USERNAME = GameScript.CONFIG["bot_username"];
    static readonly string TWITCH_ACCESS_TOKEN_CHANNEL = GameScript.CONFIG["bot_oauth"];
    static readonly string TWITCH_APPLICATION_ID = GameScript.CONFIG["application_id"];
    static readonly HashSet<string> TWITCH_BOT_ADMINS = new HashSet<string>(GameScript.CONFIG["bot_admins"].Split(','));

    public ToastsScript toastsScript;
    
    TwitchClient client;
    TwitchPubSub pubsub;
    TwitchAPI api;

    public Dictionary<string, string> words;
    public Dictionary<string, string> adminCommands, viewerCommands;
    public List<Event> events;
    bool lastChatted;
    int frames, lastChattedFrames;
    public HashSet<string> subscribers;

    // Use this for initialization
    void Awake() {
        ConnectionCredentials credentials = new ConnectionCredentials(TWITCH_BOT_USERNAME, TWITCH_ACCESS_TOKEN_CHANNEL);
        client = new TwitchClient();
        client.Initialize(credentials, TWITCH_CHANNEL_TO_JOIN);
        client.OnConnected += Client_OnConnected;
        client.OnConnectionError += Client_OnConnectionError;
        client.OnError += Client_OnError;
        client.OnIncorrectLogin += Client_OnIncorrectLogin;
        client.OnJoinedChannel += Client_OnJoinedChannel;
        client.OnMessageReceived += Client_OnMessageReceived;
        client.OnWhisperReceived += Client_OnWhisperReceived;
        client.OnWhisperThrottled += Client_OnWhisperThrottled;
        //client.OnBeingHosted += Client_OnBeingHosted;

        client.Connect();

        // TODO: The Twitch pubsub API doesn't seem to support follows... so why does this have an OnFollow event?
        pubsub = new TwitchPubSub();
        pubsub.OnPubSubServiceConnected += Pubsub_OnPubSubServiceConnected;
        pubsub.OnListenResponse += Pubsub_OnListenResponse;
        pubsub.OnPubSubServiceError += PubSub_OnPubSubServiceError;
        pubsub.OnFollow += Pubsub_OnFollow;
        pubsub.OnChannelSubscription += PubSub_OnChannelSubscription;
        pubsub.OnBitsReceived += Pubsub_OnBitsReceived;
        pubsub.OnRewardRedeemed += Pubsub_OnRewardRedeemed;
        pubsub.Connect();

        words = new Dictionary<string, string>();
        adminCommands = new Dictionary<string, string>();
        viewerCommands = new Dictionary<string, string>();
        events = new List<Event>();
        lastChattedFrames = -99999;

        TwitchAPI api = new TwitchAPI();
        api.Settings.ClientId = TWITCH_APPLICATION_ID;
        api.Settings.AccessToken = TWITCH_ACCESS_TOKEN_BOT;
        var task = api.V5.Channels.GetChannelSubscribersAsync(GameScript.CONFIG["channel_id"]);
        task.Wait();
        subscribers = new HashSet<string>(task.Result.Subscriptions.Select(i => i.User.Name));
    }
    void Update() {
        frames++;
    }

    void OnApplicationQuit() {
        client.SendMessage(TWITCH_CHANNEL_TO_JOIN, TWITCH_BOT_USERNAME + " is leaving the channel.");
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e) {
        Debug.Log("Client connected.");
    }
    private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e) {
        Debug.Log("Client connection error: " + e.Error.Message);
    }
    private void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e) {
        Debug.Log("Client log: " + e.Data);
    }
    private void Client_OnError(object sender, OnErrorEventArgs e) {
        Debug.Log("Client error: " + e.Exception.Message);
    }
    private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e) {
        Debug.Log("Client incorrect login: " + e.Exception.Message);
    }
    private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e) {
        Debug.Log("Client joined channel " + e.Channel + ".");
        Spam();
    }
    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e) {
        try {
            string user = e.ChatMessage.Username.ToLower();
            if (user == TWITCH_BOT_USERNAME) {
                return;
            }
            lastChatted = false;
        } catch (Exception exc) {
            Debug.Log(string.Format("OnMessageReceived exception: {0}", exc));
        }
    }
    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e) {
        try {
            Debug.Log("Received whisper from " + e.WhisperMessage.Username + ".");
            string user = e.WhisperMessage.Username.ToLower();
            string whisper = e.WhisperMessage.Message.Trim().ToLower();
            if (whisper.StartsWith("!")) { // Admin commands.
                if (TWITCH_BOT_ADMINS.Contains(user)) {
                    lock (adminCommands) {
                        adminCommands[user] = whisper;
                    }
                } else {
                    lock (viewerCommands) {
                        viewerCommands[user] = whisper;
                    }
                }
            } else if (whisper.Contains(" ")) {
                client.SendWhisper(e.WhisperMessage.Username, "One word only. No spaces allowed!");
            } else {
                whisper = Util.SanitizeWord(whisper);
                if (whisper.Length == 0) {
                    client.SendWhisper(e.WhisperMessage.Username, "Your word must contain alphabetic characters. (That means letters.)");
                }
                lock (words) {
                    words[user] = whisper;
                }
            }
        } catch (Exception exc) {
            Debug.Log(string.Format("OnWhisperReceived exception: {0}", exc));
        }
    }
    private void Client_OnWhisperThrottled(object sender, OnWhisperThrottledEventArgs e) {
        Debug.Log("Whisper throttled: " + e.Message);
    }
    private void Client_OnBeingHosted(object sender, OnBeingHostedArgs e) {
        // TODO: Temporarily disabled. Only the broadcaster's account can receive this event.
        toastsScript.Toast(ToastType.HOST, string.Format("{0} is hosting the channel for {1} {2}!", e.BeingHostedNotification.HostedByChannel, e.BeingHostedNotification.Viewers, e.BeingHostedNotification.Viewers == 1 ? "viewer" : "viewers"));
    }

    private void Pubsub_OnPubSubServiceConnected(object sender, System.EventArgs e) {
        Debug.Log("Pubsub connected.");
        pubsub.ListenToFollows(GameScript.CONFIG["channel_id"]);
        pubsub.ListenToSubscriptions(GameScript.CONFIG["channel_id"]);
        pubsub.ListenToBitsEvents(GameScript.CONFIG["channel_id"]);
        pubsub.ListenToRewards(GameScript.CONFIG["channel_id"]);
        pubsub.SendTopics(TWITCH_ACCESS_TOKEN_BOT);
    }
    private void Pubsub_OnListenResponse(object sender, TwitchLib.PubSub.Events.OnListenResponseArgs e) {
        if (e.Successful)
            Console.WriteLine($"Successfully verified listening to topic: {e.Topic}");
        else
            Console.WriteLine($"Failed to listen! Error: {e.Response.Error}");
    }
    private void PubSub_OnPubSubServiceError(object sender, TwitchLib.PubSub.Events.OnPubSubServiceErrorArgs e) {
        Debug.Log(e.Exception);
    }
    private void Pubsub_OnFollow(object sender, OnFollowArgs e) {
        lock (events) {
            events.Add(new Event(EventType.FOLLOW, e.Username));
        }
    }
    private void PubSub_OnChannelSubscription(object sender, OnChannelSubscriptionArgs e) {
        // TODO: the "months" key in the json response has been replaced: https://dev.twitch.tv/docs/pubsub/#receiving-messages
        // TODO: Replace the twitchlib dll with a project and replace months with cumulative-months and streak-months
        // https://github.com/TwitchLib/TwitchLib.PubSub/blob/512a7ed7bb77d508437a6739797deaa9fc15873d/TwitchLib.PubSub/Models/Responses/Messages/ChannelSubscription.cs#L90
        lock (events) {
            string cumulative = (e.Subscription.CumulativeMonths == null || e.Subscription.CumulativeMonths < 2) ? null : e.Subscription.CumulativeMonths.ToString();
            string streak = (e.Subscription.StreakMonths == null || e.Subscription.StreakMonths < 2) ? null : e.Subscription.StreakMonths.ToString();
            events.Add(new Event(EventType.SUBSCRIPTION, e.Subscription.Username, e.Subscription.RecipientName, cumulative, streak));
        }
    }
    private void Pubsub_OnBitsReceived(object sender, OnBitsReceivedArgs e) {
        lock (events) {
            events.Add(new Event(EventType.BITS, e.Username, e.BitsUsed.ToString(), e.ChatMessage));
        }
    }
    private void Pubsub_OnRewardRedeemed(object sender, OnRewardRedeemedArgs e) {
        if (e.RewardTitle == "Double Up!") {
            lock (events) {
                events.Add(new Event(EventType.DOUBLE_UP, e.Login));
            }
        } else if (e.RewardTitle == "Dock a Host's Points") {
            lock (events) {
                events.Add(new Event(EventType.PUNISH, e.Login, e.Message, e.RewardPrompt.Contains('%').ToString()));
            }
        } else if (e.RewardTitle == "Demand a Recount") {
            lock (events) {
                events.Add(new Event(EventType.RECOUNT, e.Login));
            }
        } else if (e.RewardTitle == "Think Twice") {
            lock (events) {
                events.Add(new Event(EventType.ADDITIONAL_WORD, e.Login));
            }
        }
    }

    public void Chat(string message, bool spamControl) {
        try {
            if (spamControl) {
                int sinceLastChat = frames - lastChattedFrames;
                if (sinceLastChat < 30 * 60) {
                    return;
                }
                if (lastChatted && sinceLastChat < 120 * 60) {
                    return;
                }
            }
            client.SendMessage(TWITCH_CHANNEL_TO_JOIN, message);
            lastChatted = true;
            if (spamControl) {
                lastChattedFrames = frames;
            }
        } catch (Exception exc) {
            Debug.Log(string.Format("Bot chat exception: {0}", exc));
        }
    }
    public void Spam() {
        Chat("Play along with us! Whisper this bot, or check the stream descriptions for directions.", true);
    }
    public void Whisper(string username, string message) {
        client.SendWhisper(username, message);
    }
    public void WhisperAdmin(string message) {
        Whisper(TWITCH_BOT_ADMINS.First(), message);
    }
}

public class Event {
    public EventType type;
    public string[] info;

    public Event(EventType type, params string[] info) {
        this.type = type;
        this.info = info;
    }
}

public enum EventType {
    ADDITIONAL_WORD, BITS, DOUBLE_UP, FOLLOW, PUNISH, RECOUNT, SUBSCRIPTION
}