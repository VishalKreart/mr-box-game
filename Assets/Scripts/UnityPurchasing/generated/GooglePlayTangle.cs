// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("droMajEn1JZEah0T0HAojaN78P7TULFBu80KeFYDBEPsmP4Ojc3zUiNGvqShzjdcFjO7oGTwD411aZrBjwwCDT2PDAcPjwwMDbPs6rkHWW7x06CDZSj8NrL7cTc3poDp384frfmWqPUl6rTPmFqE3jNZu1nJwDwuYhbACjAeLCv9GIzgsbLs4BncWCYvTgCexvlbjHjRHGeemRA7267NsD2PDC89AAsEJ4tFi/oADAwMCA0O3vnL8P4s1gEnlmv6c0uD4NSxI6pk8AC/3f8Il+h+3CaURjpWJPEc06SOw5wLRCZXClV5kPT3YdDwzEIuwFHNeXvQANT89mIuAplXYf+hdS19K1ETmc34hICl3k17h4oGVPS+zwqtwH9nBBZ+vA8ODA0M");
        private static int[] order = new int[] { 12,6,11,6,9,13,8,13,12,11,13,13,13,13,14 };
        private static int key = 13;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
