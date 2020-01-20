#if UNSAFE_BYTEBUFFER
using System;
#endif
using UnityEngine;
using Blake3;

public class Scene : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        test();
#if UNSAFE_BYTEBUFFER
        test2();
#endif
        test3();
#if UNSAFE_BYTEBUFFER
        test4();
#endif
    }

    void test()
    {
        byte[] input = new byte[] {1, 2, 3, 4, 5};
        byte[] output = new byte[Hasher.OUTPUT_SIZE];

        Hasher.Calc(input, output);

        Debug.Log(string.Format("in : {0}, out:{1}", BytesToString(input), BytesToString(output)));
    }

#if UNSAFE_BYTEBUFFER
    unsafe void test2()
    {
        byte[] input = new byte[] {1, 2, 3, 4, 5};
        byte[] output = new byte[Hasher.OUTPUT_SIZE];

        fixed (byte* in_ptr = input)
        {
            fixed (byte* out_ptr = output)
            {
                Hasher.Calc((IntPtr) in_ptr, input.Length, (IntPtr) out_ptr, output.Length);
            }
        }

        Hasher.Calc(input, output);

        Debug.Log(string.Format("in : {0}, out:{1}", BytesToString(input), BytesToString(output)));
    }
#endif

    void test3()
    {
        using (var hasher = Hasher.Create())
        {
            var buf = new byte[1];
            for (int i = 1; i <= 5; i++)
            {
                buf[0] = (byte) i;
                hasher.Update(buf);
            }

            var output = new byte[Hasher.OUTPUT_SIZE];
            hasher.Finalize(output);

            Debug.Log(string.Format("in : {0}, out:{1}", BytesToString(new byte[] {1, 2, 3, 4, 5}),
                BytesToString(output)));
        }
    }

#if UNSAFE_BYTEBUFFER
    unsafe void test4()
    {
        using (var hasher = Hasher.Create())
        {
            for (byte i = 1; i <= 5; i++)
            {
                hasher.Update(sizeof(byte), (IntPtr)(&i));
            }

            var output = new byte[Hasher.OUTPUT_SIZE];
            fixed (byte* ptr = output)
            {
                hasher.Finalize(output.Length, (IntPtr)ptr);
            }
            Debug.Log(string.Format("in : {0}, out:{1}", BytesToString(new byte[] {1, 2, 3, 4, 5}),
                BytesToString(output)));
        }
    }
#endif

    string BytesToString(byte[] input)
    {
        var builder = new System.Text.StringBuilder();
        foreach (var v in input)
        {
            builder.Append(v.ToString("X"));
        }

        return builder.ToString();
    }
}
