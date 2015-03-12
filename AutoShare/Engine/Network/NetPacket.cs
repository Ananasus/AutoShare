using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AutoShare.Engine.Network
{
    public class NetPacket
    {
        #region Helper Classes
        public enum PacketType
        {
            PT_PUBLIC_WITH_HASH, //Packet's integrity is secured with MD5 checksum
            PT_PUBLIC_WITHOUT_HASH, //Packet's integrity is not guaranteed
            //PT_PUBLIC_STREAMING, //Packet's integrity is not checked as it is a public packet. Special structure for continous packets
            PT_PRIVATE, //Packet Integrity is 
            //PT_PRIVATE_STREAMING //Packet's integrity is guaranteed through streaming encryption with ECB mode. Special structure for streaming packets.
        }
        public abstract class PacketSignatureRule
        {
            #region Helper Classes
            public enum PacketSignatureAlignement {
                PS_ALIGN_BEGIN,
                PS_ALIGN_END
            }
            #endregion
            #region Public Static Members
            public static int MaxSignatureSize = 80;
            #endregion
            #region Public Members
            public readonly int SignatureSize;
            public readonly int SignatureVersion;
            public readonly PacketSignatureAlignement Alignement;
            public readonly bool DataEncrypted;
            public readonly bool MD5HashPresent;
            public readonly string SignatureName;
            public readonly string SignatureDetectionHelper;
            #endregion
            #region Constructors
            public PacketSignatureRule( 
                                        int SignatureSize, int SignatureVersion, PacketSignatureAlignement Alignement,
                                        bool DataEncrypted, bool MD5HashPresent,
                                        string SignatureName, string SignatureDetectionHelper
                                      )
            {
                this.SignatureSize = SignatureSize;
                if (MaxSignatureSize < this.SignatureSize)
                    throw new TypeInitializationException("PacketSignatureRule", 
                        new Exception("SignatureSize value is more than MaxSignatureSize. Either change the PacketSignatureRule.MaxSignatureSize or lower the passed SignatureSize value."+System.Environment.NewLine+
                                      "Ignoring this message may change the behaviour of the application when accepting packets over network using MaxSignatureSize as the buffer size."));
                this.SignatureName = SignatureName;
                this.SignatureVersion = SignatureVersion;
                this.Alignement = Alignement;
                this.DataEncrypted = DataEncrypted;
                this.MD5HashPresent = MD5HashPresent;
                this.SignatureDetectionHelper = SignatureDetectionHelper;
                
            }
            #endregion

            #region API Calls
            public abstract PacketType GetPacketType();
            #endregion
        }

        public class PublicNodePacket:PacketSignatureRule
        {
            #region Static Members
            private static readonly int SizeFieldSize = 8; //Size of the field in the signature, that tells us the size of the actual packet
            private static readonly int PacketVersion = 1;
            private static readonly string SignatureHelper = "{PUBLIC}";

            public static readonly PacketType Type = PacketType.PT_PUBLIC_WITH_HASH; 
            #endregion
            public PublicNodePacket():
                base(16+SizeFieldSize+SignatureHelper.Length, PacketVersion, PacketSignatureAlignement.PS_ALIGN_BEGIN, false, true, "PublicNodePacket", SignatureHelper)
            {
                
            }

            public override PacketType GetPacketType()
            {
                return Type;
            }
        }
        public class PublicUnsafePacket : PacketSignatureRule
        {
            #region Static Members
            private static readonly int SizeFieldSize = 8; //Size of the field in the signature, that tells us the size of the actual packet
            private static readonly int PacketVersion = 1;
            private static readonly string SignatureHelper = "{UNSAFE}";

            public static readonly PacketType Type = PacketType.PT_PUBLIC_WITHOUT_HASH;
            #endregion
            public PublicUnsafePacket()
                : base(SizeFieldSize + SignatureHelper.Length, PacketVersion, PacketSignatureAlignement.PS_ALIGN_BEGIN, false, false, "PublicUnsafePacket", SignatureHelper)
            {

            }

            public override PacketType GetPacketType()
            {
                return Type;
            }
        }
        #endregion
        #region Static Members
        public static readonly PacketSignatureRule[] SignatureRules = new PacketSignatureRule[2]{ new PublicNodePacket(), new PublicUnsafePacket() };
        #endregion
        #region Static Methods
        #endregion

    }
}
