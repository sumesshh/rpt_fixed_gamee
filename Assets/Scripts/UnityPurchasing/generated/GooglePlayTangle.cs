// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("jCx0bKZUZmgu3ERUi3CnfdxeXp7tfozQ9dWKNQ/hFdia2mPkghhkPNE4/lvlFpKTHFNug0agHUCGvRGqsNARwsXbI70nOyQ0zWtqD50+iq0wN4ixq/Ja2ZCBVyPa1WjVEBzM2tZVW1Rk1lVeVtZVVVS3odDDl2ATZNZVdmRZUl1+0hzSo1lVVVVRVFdvnU7k/px4t6I6ZEE6b2I5FZVPO2aTLpHQYakhDrv1Lb7VYAO8C8pxgsP9UTA1ZSFIm3cVOi5FoonlkPk1tRzryAoqPXHfJ5UrP+ftMmBDFagUchFdF16+cTrsnO/YF9w3nAPdpxuLHwAnwcGsk0oJW7CgQKt57mL/ShhuzouJcNkRfxokvNczoc0dbQ4epQUXwJsnbVZXVVRV");
        private static int[] order = new int[] { 11,13,9,5,5,13,11,13,12,12,13,12,13,13,14 };
        private static int key = 84;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
