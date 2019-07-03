using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;


namespace Example.Site
{
    internal static class TraceUtil
    {
        public enum TraceMode
        {
            /// <summary>
            /// Trace spento
            /// </summary>
            OFF,
            /// <summary>
            /// Trace acceso per ingresso/uscita
            /// </summary>
            ON,
            /// <summary>
            /// Trace acceso per ingresso/uscita e dati in input/output
            /// </summary>
            DATA
        }

        public static TraceMode TraceIsEnabled()
        {
            try
            {
                return (TraceMode)Enum.Parse(typeof(TraceMode), ConfigurationManager.AppSettings["TraceRequests"]);
            }
            catch
            {
                return TraceMode.OFF;
            }
        }

        public class OutputFilterStream : Stream
        {
            private readonly Stream InnerStream;
            private readonly MemoryStream CopyStream;
            private readonly object thisLock = new object();
            internal string GuidRequest;

            public OutputFilterStream(Stream inner)
            {
                this.InnerStream = inner;
                this.CopyStream = new MemoryStream();
                GuidRequest = Guid.NewGuid().ToString("N");
            }

            public string ReadStream()
            {
                lock (this.thisLock)
                {
                    if (this.CopyStream.Length <= 0L || !this.CopyStream.CanRead || !this.CopyStream.CanSeek)
                    {
                        return String.Empty;
                    }

                    long pos = this.CopyStream.Position;
                    this.CopyStream.Position = 0L;
                    try
                    {
                        return new StreamReader(this.CopyStream).ReadToEnd();
                    }
                    finally
                    {
                        try
                        {
                            this.CopyStream.Position = pos;
                        }
                        catch
                        {
                            //Error on copy is not handled
                        }
                    }
                }
            }


            public override bool CanRead
            {
                get { return this.InnerStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return this.InnerStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return this.InnerStream.CanWrite; }
            }

            public override void Flush()
            {
                this.InnerStream.Flush();
            }

            public override long Length
            {
                get { return this.InnerStream.Length; }
            }

            public long CopyStreamLength
            {
                get
                {
                    return this.CopyStream.Length;
                }
            }

            public override long Position
            {
                get { return this.InnerStream.Position; }
                set { this.CopyStream.Position = this.InnerStream.Position = value; }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.InnerStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                this.CopyStream.Seek(offset, origin);
                return this.InnerStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                this.CopyStream.SetLength(value);
                this.InnerStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.CopyStream.Write(buffer, offset, count);
                this.InnerStream.Write(buffer, offset, count);
            }
        }
    }
}