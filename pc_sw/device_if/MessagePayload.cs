using System;
namespace pc_sw.device_if
{

    public class MessagePayload
    {

        public virtual Byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        public virtual String ToString()
        {
            throw new NotImplementedException();
        }
    }
}
