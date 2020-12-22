using UnityEngine;

namespace Assets.Scripts
{
    class DeformableCloud
    {
    }

    class CloudPage
    {
        byte[,] cloud;

        public CloudPage(int size)
        {
            cloud = new byte[size, size];
        }

        // Should be inlined
        private static int getMass(byte value) => value & 0x0F;
        private static int getHard(byte value) => value & 0xF0;
    }
}
