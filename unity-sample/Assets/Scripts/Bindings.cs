using System;
using System.Runtime.InteropServices;

namespace Blake3
{
    /// <summary>
    /// Blak3 Hasher.
    /// </summary>
    /// <example>
    /// // simple
    /// byte[] output = new byte[Hasher.OUTPUT_SIZE];
    /// Hasher.Calc(input_bytes, output);
    /// 
    /// // stream
    /// using(var hasher = Hasher.Create()){
    ///   hasher.Update(input_bytes1);
    ///   hasher.Update(input_bytes2);
    ///   hasher.Finalize(output);
    /// }
    /// </example>
    public class Hasher : IDisposable
    {
        public const int OUTPUT_SIZE = 32;
        readonly IntPtr ptr;
        bool finalized;

        public static void Calc(byte[] input, byte[] output/*Length must be 32*/)
        {
            ThrowBlake3ExceptionUnless(input != null, "input buffer is null");
            ThrowBlake3ExceptionUnless(output != null, "output buffer is null");
            ThrowBlake3ExceptionUnless(output.Length == OUTPUT_SIZE, "output buffer size must be " +OUTPUT_SIZE.ToString());

            Bindings.calculate_blake3(input.Length, input, output.Length, output);
        }

        public static Hasher Create()
        {
            return new Hasher();
        }

        public void Update(byte[] input)
        {
            ThrowBlake3ExceptionUnless(!finalized, "hasher has been already finalized.");
            Bindings.update_blake3(ptr, input.Length, input);
        }

        public void Finalize(byte[] output/*Length must be 32*/)
        {
            ThrowBlake3ExceptionUnless(!finalized, "hasher has been already finalized.");
            ThrowBlake3ExceptionUnless(output != null, "output buffer is null");
            ThrowBlake3ExceptionUnless(output.Length == OUTPUT_SIZE, "output buffer size must be " + OUTPUT_SIZE.ToString());

            Bindings.finalize_blake3(ptr, output.Length, output);
        }

        Hasher()
        {
            ptr = Bindings.create_blake3();
            finalized = false;
        }

#region IDisposable implementation
        public void Dispose()
        {
            Bindings.delete_blake3(ptr);
        }
#endregion

        static void ThrowBlake3ExceptionUnless(bool cond, string message)
        {
            if(!cond)
            {
                throw new Blake3Exception(message);
            }
        }
    }

    public class Blake3Exception : System.Exception
    {
        public Blake3Exception() { }
        public Blake3Exception(string message) : base(message) { }
        public Blake3Exception(string message, System.Exception inner) : base(message, inner) { }
        protected Blake3Exception(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    static class Bindings
    {
#if UNITY_EDITOR
    #if UNITY_EDITOR_WIN
        const string BLAKE3DLL = "blake3unity"; //for win
    #else
        const string BLAKE3DLL = "blake3unity"; //for macOS
    #endif
#else
    #if UNITY_IPHONE
        const string BLAKE3DLL = "__Internal";  //for iOS
    #else
        const string BLAKE3DLL = "blake3unity"; //for android
    #endif
#endif

        [DllImport(BLAKE3DLL)]
        public static extern void calculate_blake3(int input_length, byte[] input, int output_length, byte[] output);

        [DllImport(BLAKE3DLL)]
        public static extern IntPtr create_blake3();

        [DllImport(BLAKE3DLL)]
        public static extern void delete_blake3(IntPtr hasher);

        [DllImport(BLAKE3DLL)]
        public static extern void update_blake3(IntPtr hasher, int input_length, byte[] input);

        [DllImport(BLAKE3DLL)]
        public static extern void finalize_blake3(IntPtr hasher, int output_length, byte[] output);
    }
}