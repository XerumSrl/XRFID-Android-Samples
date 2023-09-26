using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Server.Helpers;

internal static class ModuleParser
{
    /// <summary>
    /// Method to parse the first <paramref name="bitmaskSize"/> bits (from Least Significant Bit) of <paramref name="value"/>
    /// </summary>
    /// <remarks>
    /// array is ordered from LSB to MSB
    /// </remarks>
    /// <param name="value"></param>
    /// <param name="bitmaskSize">defaults to 64</param>
    /// <param name="strict">if true throw an exception if there are bits of value 1 above the <paramref name="bitmaskSize"/></param>
    /// <returns> a <see cref="bool"/> array ordered from LSB</returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public static bool[] Parse(ulong value, int bitmaskSize = 64, bool strict = true)
    {
        ulong validityBitMask = ulong.MaxValue << bitmaskSize; // this will be used to check if there is anything in unused bits

        if ((value & validityBitMask) != 0 && strict)
        {
            throw new IndexOutOfRangeException();
        }

        bool[] resultingArray = new bool[bitmaskSize];
        for (int i = 0; i < bitmaskSize; i++)
        {
            ulong mask = 1ul << i;//move the mask on the current working bit
            resultingArray[i] = (value & mask) == mask ? true : false; // if the result of the binary AND operation between the value and the mask is equal to the mask (aka the bit is 1) put true in the array otherwhise false
        }
        return resultingArray;
    }

    /// <summary>
    /// Method to parse the first n bits (from LSB) of <paramref name="value"/>
    /// </summary>
    /// <remarks>
    /// The value of n is calculated using the current list of available modules, so this might break when using a config from a newer vesrion of Backend
    /// </remarks>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool[] ParseModules(ulong value, bool strict = true)
    {
        return Parse(value, Enum.GetValues(typeof(ServerStateMachineModuleValue)).Length, strict);//intentionally not handling exceptions as I want to crash the software
    }

    /// <summary>
    /// Method to parse the first n bits (from LSB) of <paramref name="value"/>
    /// </summary>
    /// <remarks>
    /// The value of n is calculated using the current list of available modes, so this might break when using a config from a newer vesrion of Backend
    /// </remarks>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool[] ParseModes(ulong value, bool strict = true)
    {
        return Parse(value, Enum.GetValues(typeof(ServerModeValue)).Length, strict);//intentionally not handling exceptions as I want to crash the software
    }

    /// <summary>
    /// Method  that generates a bitfield with the values in <paramref name="values"/>
    /// </summary>
    /// <remarks>
    /// this is Little endian, values[0] corresponds to the LSB
    /// </remarks>
    /// <param name="values"></param>
    /// <param name="truncate"></param>
    /// <returns>a 64 bit long bitfield contained in a <see cref="ulong"/> </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ulong GenerateBitField(bool[] values, bool truncate = true)
    {
        if (values.Length > 64 && !truncate)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(values.Length));
        }

        ulong bitfield = 0;
        for (byte i = 0; i < values.Length && i < 64; i++)
        {
            bitfield |= values[i] ? 1ul << i : 0ul;
        }
        return bitfield;
    }
}
