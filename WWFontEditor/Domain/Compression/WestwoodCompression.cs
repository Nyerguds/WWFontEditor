using System;
using System.Collections.Generic;

// Disabled for now. Check the project properties for this file, and set them back to "Compile" to enable.
namespace Nyerguds.FileData.Westwood
{
    public class WestwoodCompression
    {

        public static Byte[] LzwDecode(Byte[] buffer, Int32 decompressedSize)
        {
            //Header h = GetHeader(c.data);
            byte[] data = new byte[decompressedSize];
            int a = 0;
            int b = 0;
            bool low = false;
            byte[] tribbles = new byte[(buffer.Length - (a + 1)) * 2 / 3 * 2];
            //if (buffer[buffer.Length - 1] != 0x00) return null;
            while (true)
            {
                if (a >= buffer.Length - 1)
                    return null;
                byte[] nibbles = new byte[3];
                for (int n = 0; n != 3; ++n)
                {
                    if (low == false)
                    {
                        nibbles[n] = (byte)(buffer[a] >> 4);
                        low = true;
                    }
                    else
                    {
                        nibbles[n] = (byte)(buffer[a] & 0x0f);
                        low = false;
                        a++;
                    }
                }
                if (b + 1 >= tribbles.Length)
                    return null;
                tribbles[b] = nibbles[0];
                tribbles[b + 1] = (byte)((nibbles[1] << 4) | nibbles[2]);
                if (tribbles[b] == 0x0f && tribbles[b + 1] == 0xff)
                    break;
                b += 2;
            }
            if (a + 1 != buffer.Length && a + 2 != buffer.Length && b + 2 != tribbles.Length)
                return null;
            for (a = 0, b = 0; a != tribbles.Length - 2; a += 2)
            {
                int offset = a;
                int count = 0;
                while (tribbles[offset] != 0x00)
                {
                    offset = (((tribbles[offset] - 1) << 8) | tribbles[offset + 1]) * 2;
                    if (offset >= a)
                        return null;
                    count++;
                }
                if (b == data.Length)
                    return null;
                data[b] = tribbles[offset + 1];
                b++;
                if (count == 0)
                    continue;
                while (count != 0)
                {
                    offset += 2;
                    int o = offset;
                    while (tribbles[offset] != 0x00)
                    {
                        offset = (((tribbles[offset] - 1) << 8) | tribbles[offset + 1]) * 2;
                        if (offset >= o)
                            return null;
                    }
                    if (b == data.Length)
                        return null;
                    data[b] = tribbles[offset + 1];
                    b++;
                    count--;
                    offset = a;
                    int counter = 0;
                    while (counter != count)
                    {
                        offset = (((tribbles[offset] - 1) << 8) | tribbles[offset + 1]) * 2;
                        if (offset >= a)
                            return null;
                        counter++;
                    }
                }
            }
            return data;
        }

        // Westwoods_Eye_of_the_Beholder_Tool.Compression
        public static Byte[] LzwEncode(Byte[] data)
        {
            LinkedList<ValueList> linkedList = new LinkedList<ValueList>();
            Int32 num = 0;
            while (num != data.Length)
            {
                Int32[] array = new int[2];
                array[0] = 0;
                for (LinkedListNode<ValueList> linkedListNode = linkedList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
                {
                    if (linkedListNode.Value.Offset != -1 || linkedListNode.Value.Value != data[num])
                        continue;
                    array[0] = 1;
                    array = C12Recursion(linkedListNode, data, num, array);
                    if (array[0] != 1)
                        break;
                }
                ValueList valueList = new ValueList();
                if (linkedList.Count == 0)
                    valueList.Index = 0;
                else
                    valueList.Index = linkedList.Last.Value.Index + 1;
                if (array[0] < 2)
                {
                    valueList.Value = data[num];
                    num++;
                }
                else
                {
                    valueList.Offset = array[1];
                    LinkedListNode<ValueList> linkedListNode2 = linkedList.First;
                    while (linkedListNode2.Value.Index != array[1])
                        linkedListNode2 = linkedListNode2.Next;
                    valueList.Link = linkedListNode2;
                    num += array[0];
                }
                linkedList.AddLast(valueList);
            }
            linkedList.AddLast(new ValueList { Offset = 3839 });
            Byte[] array2 = new byte[(Int32)(linkedList.Count * 1.5) + 3];
            Boolean flag = false;
            Int32 num2 = 0;
            for (LinkedListNode<ValueList> linkedListNode3 = linkedList.First; linkedListNode3 != null; linkedListNode3 = linkedListNode3.Next)
            {
                if (!flag)
                {
                    if (linkedListNode3.Value.Offset == -1)
                        array2[num2] = (byte)(linkedListNode3.Value.Value >> 4);
                    else
                        array2[num2] = (byte)(linkedListNode3.Value.Offset + 256 >> 4);
                    num2++;
                    if (linkedListNode3.Value.Offset == -1)
                        array2[num2] = (byte)((linkedListNode3.Value.Value & 15) << 4);
                    else
                        array2[num2] = (byte)((linkedListNode3.Value.Offset + 256 & 15) << 4);
                    flag = true;
                }
                else
                {
                    if (linkedListNode3.Value.Offset == -1)
                        array2[num2] = array2[num2];
                    else
                        array2[num2] = (byte)((int)array2[num2] | linkedListNode3.Value.Offset + 256 >> 8);
                    num2++;
                    if (linkedListNode3.Value.Offset == -1)
                        array2[num2] = linkedListNode3.Value.Value;
                    else
                        array2[num2] = (byte)(linkedListNode3.Value.Offset + 256 & 255);
                    num2++;
                    flag = false;
                }
            }
            if (flag)
                num2++;
            if (num2 + 1 >= array2.Length)
                return null;
            array2[num2] = 0;
            num2++;
            Byte[] array3 = new Byte[num2];
            Array.Copy(array2, 0, array3, 0, num2);
            return array3;
        }

        // Westwoods_Eye_of_the_Beholder_Tool.Compression
        private static Int32[] C12Recursion(LinkedListNode<ValueList> cNode, Byte[] data, Int32 dataPos, Int32[] retLow)
        {
            if (cNode.Value.Index >= 3839)
            {
                return retLow;
            }
            if (cNode.Next == null || dataPos + retLow[0] == data.Length)
            {
                if (dataPos + retLow[0] != data.Length && data[dataPos] == data[dataPos + retLow[0]])
                {
                    retLow[0]++;
                    retLow[1] = cNode.Value.Index;
                }
                return retLow;
            }
            LinkedListNode<ValueList> linkedListNode = cNode.Next;
            while (linkedListNode.Value.Offset != -1)
            {
                linkedListNode = linkedListNode.Value.Link;
            }
            if (linkedListNode.Value.Value != data[dataPos + retLow[0]])
            {
                return retLow;
            }
            retLow[0]++;
            retLow[1] = cNode.Value.Index;
            Int32[] array = new int[] { retLow[0], cNode.Value.Index };
            for (linkedListNode = cNode.Next; linkedListNode != null; linkedListNode = linkedListNode.Next)
            {
                if (linkedListNode.Value.Offset == cNode.Value.Index)
                {
                    Int32[] array2 = new Int32[2];
                    array2 = C12Recursion(linkedListNode, data, dataPos, retLow);
                    if (array2[0] > array[0])
                    {
                        array[0] = array2[0];
                        array[1] = array2[1];
                        break;
                    }
                }
            }
            return array;
        }

        private class ValueList
        {
            public Int32 Index;
            public Int32 Offset = -1;
            public Byte Value;
            public LinkedListNode<ValueList> Link;
        }

    }
}