using System;
using System.Runtime.InteropServices;

//   There are 1 #define that have an impact on performance
//      UNSAFE_BYTEBUFFER
//          This will use unsafe code to manipulate the underlying byte array.

namespace Blake3
{
    /// <summary>
    /// Blake3 Hasher.
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

        /// <summary>
        /// Create Hasher.
        /// </summary>
        public static Hasher Create()
        {
            return new Hasher();
        }

        /// <summary>
        /// calculate inputs' hash into output.
        /// output buffer size must be 32.
        /// </summary>
        public static void Calc(byte[] input, byte[] output /*Length must be 32*/)
        {
            ThrowBlake3ExceptionUnless(input != null, "input buffer is null");
            ThrowBlake3ExceptionUnless(output != null, "output buffer is null");
            ThrowBlake3ExceptionUnless(output.Length == OUTPUT_SIZE,
                "output buffer size must be " + OUTPUT_SIZE.ToString());

            Bindings.calculate_blake3(input.Length, input, output.Length, output);
        }

        /// <summary>
        /// calculate input's hash.
        /// </summary>
        public void Update(byte[] input)
        {
            ThrowBlake3ExceptionUnless(!_finalized, "hasher has been already finalized.");
            Bindings.update_blake3(_ptr, input.Length, input);
        }

        /// <summary>
        /// finalize calculating input's hash, copy hash into output.
        /// output buffer size must be 32.
        /// </summary>
        public void Finalize(byte[] output /*Length must be 32*/)
        {
            ThrowBlake3ExceptionUnless(!_finalized, "hasher has been already finalized.");
            ThrowBlake3ExceptionUnless(output != null, "output buffer is null");
            ThrowBlake3ExceptionUnless(output.Length == OUTPUT_SIZE,
                "output buffer size must be " + OUTPUT_SIZE.ToString());

            Bindings.finalize_blake3(_ptr, output.Length, output);
        }
#if UNSAFE_BYTEBUFFER
        /// <summary>
        /// calculate inputs' hash into output.
        /// output buffer size must be 32.
        /// </summary>
        public static void Calc(IntPtr input, int input_size, IntPtr output, int output_size)
        {
            ThrowBlake3ExceptionUnless(output_size == OUTPUT_SIZE,
                "output buffer size must be " + OUTPUT_SIZE.ToString());

            Bindings.calculate_unsafe_blake3(input_size, input, output_size, output);
        }

        /// <summary>
        /// calculate input's hash.
        /// </summary>
        public void Update(int input_size, IntPtr input)
        {
            ThrowBlake3ExceptionUnless(!_finalized, "hasher has been already finalized.");
            Bindings.update_unsafe_blake3(_ptr, input_size, input);
        }

        /// <summary>
        /// finalize calculating input's hash, copy hash into output.
        /// output buffer size must be 32.
        /// </summary>
        public void Finalize(int output_size, IntPtr output)
        {
            ThrowBlake3ExceptionUnless(!_finalized, "hasher has been already finalized.");
            ThrowBlake3ExceptionUnless(output_size == OUTPUT_SIZE,
                "output buffer size must be " + OUTPUT_SIZE.ToString());
            Bindings.finalize_unsafe_blake3(_ptr, output_size, output);
        }
#endif

        readonly IntPtr _ptr; //!< Hasher's pointer allocated in rust. must call free.
        bool _finalized;      //!< set true when Dispos.

        /// <summary>
        /// constructor.
        /// </summary>
        Hasher()
        {
            _ptr = Bindings.create_blake3();
            _finalized = false;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Bindings.delete_blake3(_ptr);
            _finalized = true;
        }

        #endregion

        /// <summary>
        /// throw Blake3Exception when cond is false.
        /// </summary>
        static void ThrowBlake3ExceptionUnless(bool cond, string message)
        {
            if (!cond)
            {
                throw new Blake3Exception(message);
            }
        }
    }

    public class Blake3Exception : System.Exception
    {
        public Blake3Exception()
        {
        }

        public Blake3Exception(string message) : base(message)
        {
        }

        public Blake3Exception(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected Blake3Exception(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
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

#if UNSAFE_BYTEBUFFER
        [DllImport(BLAKE3DLL)]
        public static extern void calculate_unsafe_blake3(int input_length, IntPtr input, int output_length,
            IntPtr output);

        [DllImport(BLAKE3DLL)]
        public static extern void update_unsafe_blake3(IntPtr hasher, int input_length, IntPtr input);

        [DllImport(BLAKE3DLL)]
        public static extern void finalize_unsafe_blake3(IntPtr hasher, int output_length, IntPtr output);
#endif
    }
}