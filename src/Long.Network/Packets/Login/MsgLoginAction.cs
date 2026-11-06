using Long.Network.Sockets;
using ProtoBuf;

namespace Long.Network.Packets.Login
{
    public abstract class MsgLoginAction<TActor>
        : MsgProtoBufBase<TActor, MsgLoginAction<TActor>.LoginActionPB>
        where TActor : TcpServerActor
    {
        public enum LoginActionEnum
        {
            None,
            First = 15000,
            RequestMasterRealm,
            RequestChildServers,
            RequestCredentials,
            RequestRealmData,
            Ping,
            UserFee,
            QueryFee
        }

        /// <summary>
        ///     The types of MsgFee 1200 of MsgServer.exe that the servers use here. 2 is the point card, 3 the
        ///     heartbeat of a user, 6 the busy message port. There is nothing for them in this source.
        /// </summary>
        public enum LoginFeeType : uint
        {
            Begin = 0,
            End = 1,
            Kickout = 4,
            Offline = 5,
            ServerFull = 7,
            ServerClose = 8
        }

        /// <summary>
        ///     QueryFee 1201 with the players online, every 30 seconds (0x140065a4f).
        /// </summary>
        public const uint QUERY_FEE_ONLINE = 0x67;

        /// <summary>
        ///     The reason of a kick in Command. 0 is another login.
        /// </summary>
        public const uint KICK_CHANGE_PASSWORD = 1;

        protected MsgLoginAction()
            : base(PacketType.MsgLoginAction)
        {
            serializeWithHeaders = true;
        }

        [ProtoContract]
        public struct LoginActionPB
        {
            [ProtoMember(1)]
            public uint ServerID { get; set; }
            [ProtoMember(2)]
            public string ServerUUID { get; set; }
            [ProtoMember(3)]
            public uint Action { get; set; }
            [ProtoMember(4)]
            public uint Data { get; set; }
            [ProtoMember(5)]
            public uint Command { get; set; }
            [ProtoMember(6)]
            public uint Param { get; set; }
            [ProtoMember(7)]
            public ulong Data64 { get; set; }
            [ProtoMember(8)]
            public List<string> Strings { get; set; }
        }
    }
}
