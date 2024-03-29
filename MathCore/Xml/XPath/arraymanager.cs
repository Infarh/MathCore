﻿//------------------------------------------------------------------------------
// <copyright file="ArrayManager.cs" company="Microsoft">
//     
//      Copyright (c) 2002 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.
//     
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

internal class ArrayManager
{
    #region Types

    internal class Buffer(char[] buffer, int offset, int size)
    {
        #region Fields

        public int _offset = offset;

        public char[] CharBuffer = buffer;
        public int Size = size;

        #endregion

        #region Constructors

        #endregion
    }

    #endregion

    #region Fields

    private Queue _BufferQueue;
    private Buffer _CurrentBuffer;

    #endregion

    #region Properties

    private Queue BufferQueue
    {
        get => _BufferQueue ??= new();
        set => _BufferQueue = value;
    }

    private int Offset { get; set; }

    internal char[] CurrentBuffer => _CurrentBuffer?.CharBuffer;

    internal int CurrentBufferOffset => _CurrentBuffer?._offset ?? 0;

    internal int CurrentBufferLength => _CurrentBuffer?.Size ?? 0;

    internal int Length
    {
        get
        {
            var len = 0;
            if(_CurrentBuffer != null)
                len += _CurrentBuffer.Size - _CurrentBuffer._offset;
            var enumerator = BufferQueue.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var element = (Buffer)enumerator.Current;
                len += element.Size - element._offset;
            }
            return len;
        }
    }

    internal char this[int index]
    {
        get
        {
            var ch = '\0';
            if(_CurrentBuffer is null)
            {
                if(BufferQueue.Count > 0)
                    _CurrentBuffer = (Buffer)BufferQueue.Dequeue();
                else
                    return ch;
            }

            if(!(_CurrentBuffer._offset + index - Offset < _CurrentBuffer.Size))
            {
                Offset         = index;
                _CurrentBuffer = BufferQueue.Count > 0 ? (Buffer)BufferQueue.Dequeue() : null;
            }

            if(_CurrentBuffer != null)
                ch = _CurrentBuffer.CharBuffer[_CurrentBuffer._offset + (index - Offset)];
            return ch;
        }
    }

    #endregion

    #region Constructors

    internal ArrayManager()
    {
        BufferQueue    = null;
        Offset         = 0;
        _CurrentBuffer = null;
    }

    #endregion

    #region Methods

    internal void Append(char[] buffer, int offset, int size) => BufferQueue.Enqueue(new Buffer(buffer, offset, size));

    internal void CleanUp(int InternalBufferOffset)
    {
        if(_CurrentBuffer is null) return;
        _CurrentBuffer._offset += InternalBufferOffset - Offset;
        Offset                 =  0;
    }

    internal void Refresh()
    {
        BufferQueue    = new();
        _CurrentBuffer = null;
        Offset         = 0;
    }

    #endregion
}