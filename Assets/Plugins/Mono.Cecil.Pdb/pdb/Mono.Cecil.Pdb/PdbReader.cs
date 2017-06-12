//
// PdbReader.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2011 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Cci.Pdb;

using Mono.Cecil.Cil;

namespace Mono.Cecil.Pdb
{

    public class PdbReader : ISymbolReader
    {

        int age;
        Guid guid;

        readonly Stream pdb_file;
        readonly Dictionary<string, Document> documents = new Dictionary<string, Document>();
        readonly Dictionary<uint, PdbFunction> functions = new Dictionary<uint, PdbFunction>(new uintCompare());

        internal PdbReader(Stream file)
        {
            this.pdb_file = file;
        }

        /*
        uint Magic = 0x53445352;
        Guid Signature;
        uint Age;
        string FileName;
         */

        public bool ProcessDebugHeader(ImageDebugDirectory directory, byte[] header)
        {
            if (header.Length < 24)
                return false;

            var magic = ReadInt32(header, 0);
            if (magic != 0x53445352)
                return false;

            var guid_bytes = new byte[16];
            Buffer.BlockCopy(header, 4, guid_bytes, 0, 16);

            this.guid = new Guid(guid_bytes);
            this.age = ReadInt32(header, 20);

            return PopulateFunctions();
        }

        static int ReadInt32(byte[] bytes, int start)
        {
            return (bytes[start]
                | (bytes[start + 1] << 8)
                | (bytes[start + 2] << 16)
                | (bytes[start + 3] << 24));
        }

        bool PopulateFunctions()
        {
            using (pdb_file)
            {
                int age;
                Guid guid;
                var funcs = PdbFile.LoadFunctions(pdb_file, true, out age, out guid);

                if (this.age != 0 && this.guid != guid)
                    return false;
                {
                    // foreach(var function in funcs)
                    var __enumerator1 = (funcs).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var function = (PdbFunction)__enumerator1.Current;
                        functions.Add(function.token, function);
                    }
                }
            }

            return true;
        }

        public void Read(MethodBody body, InstructionMapper mapper)
        {
            var method_token = body.Method.MetadataToken;

            PdbFunction function;
            if (!functions.TryGetValue(method_token.ToUInt32(), out function))
                return;

            ReadSequencePoints(function, mapper);
            ReadScopeAndLocals(function.scopes, null, body, mapper);
        }

        static void ReadScopeAndLocals(PdbScope[] scopes, Scope parent, MethodBody body, InstructionMapper mapper)
        {
            {
                var __array2 = scopes;
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var scope = (PdbScope)__array2[__i2];
                    ReadScopeAndLocals(scope, parent, body, mapper);
                }
            }
            CreateRootScope(body);
        }

        static void CreateRootScope(MethodBody body)
        {
            if (!body.HasVariables)
                return;

            var instructions = body.Instructions;

            var root = new Scope();
            root.Start = instructions[0];
            root.End = instructions[instructions.Count - 1];

            var variables = body.Variables;
            for (int i = 0; i < variables.Count; i++)
                root.Variables.Add(variables[i]);

            body.Scope = root;
        }

        static void ReadScopeAndLocals(PdbScope scope, Scope parent, MethodBody body, InstructionMapper mapper)
        {
            //Scope s = new Scope ();
            //s.Start = GetInstruction (body, instructions, (int) scope.address);
            //s.End = GetInstruction (body, instructions, (int) scope.length - 1);

            //if (parent != null)
            //	parent.Scopes.Add (s);
            //else
            //	body.Scopes.Add (s);

            if (scope == null)
                return;
            {
                // foreach(var slot in scope.slots)
                var __enumerator3 = (scope.slots).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var slot = (PdbSlot)__enumerator3.Current;
                    {
                        int index = (int)slot.slot;
                        if (index < 0 || index >= body.Variables.Count)
                            continue;

                        VariableDefinition variable = body.Variables[index];
                        variable.Name = slot.name;

                        //s.Variables.Add (variable);
                    }
                }
            }

            ReadScopeAndLocals(scope.scopes, null /* s */, body, mapper);
        }

        void ReadSequencePoints(PdbFunction function, InstructionMapper mapper)
        {
            if (function.lines == null)
                return;
            {
                // foreach(var lines in function.lines)
                var __enumerator4 = (function.lines).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var lines = (PdbLines)__enumerator4.Current;
                    ReadLines(lines, mapper);
                }
            }
        }

        void ReadLines(PdbLines lines, InstructionMapper mapper)
        {
            var document = GetDocument(lines.file);
            {
                var __array = (lines.lines);
                var __arrayLength = __array.Length;
                for (int __i = 0; __i < __arrayLength; ++__i)
                {
                    var line = __array[__i];
                    ReadLine(line, document, mapper);
                }
            }
        }

        static void ReadLine(PdbLine line, Document document, InstructionMapper mapper)
        {
            var instruction = mapper((int)line.offset);
            if (instruction == null)
                return;

            var sequence_point = new SequencePoint(document);
            sequence_point.StartLine = (int)line.lineBegin;
            sequence_point.StartColumn = (int)line.colBegin;
            sequence_point.EndLine = (int)line.lineEnd;
            sequence_point.EndColumn = (int)line.colEnd;

            instruction.SequencePoint = sequence_point;
        }

        Document GetDocument(PdbSource source)
        {
            string name = source.name;
            Document document;
            if (documents.TryGetValue(name, out document))
                return document;

            document = new Document(name)
            {
                Language = GuidMapping.ToLanguage(source.language),
                LanguageVendor = GuidMapping.ToVendor(source.vendor),
                Type = GuidMapping.ToType(source.doctype),
            };
            documents.Add(name, document);
            return document;
        }

        public void Read(MethodSymbols symbols)
        {
            PdbFunction function;
            if (!functions.TryGetValue(symbols.MethodToken.ToUInt32(), out function))
                return;

            ReadSequencePoints(function, symbols);
            ReadLocals(function.scopes, symbols);
        }

        void ReadLocals(PdbScope[] scopes, MethodSymbols symbols)
        {
            {
                var __array6 = scopes;
                var __arrayLength6 = __array6.Length;
                for (int __i6 = 0; __i6 < __arrayLength6; ++__i6)
                {
                    var scope = __array6[__i6];
                    ReadLocals(scope, symbols);
                }
            }
        }

        void ReadLocals(PdbScope scope, MethodSymbols symbols)
        {
            if (scope == null)
                return;
            {

                var __array = (scope.slots);
                var __arrayLength = __array.Length;
                for (int __i = 0; __i < __arrayLength; ++__i)
                {
                    var slot = __array[__i];
                    int index = (int)slot.slot;
                    if (index < 0 || index >= symbols.Variables.Count)
                        continue;

                    var variable = symbols.Variables[index];
                    variable.Name = slot.name;
                }
            }

            ReadLocals(scope.scopes, symbols);
        }

        void ReadSequencePoints(PdbFunction function, MethodSymbols symbols)
        {
            if (function.lines == null)
                return;
            {
                // foreach(var lines in function.lines)
                var __enumerator8 = (function.lines).GetEnumerator();
                while (__enumerator8.MoveNext())
                {
                    var lines = (PdbLines)__enumerator8.Current;
                    ReadLines(lines, symbols);
                }
            }
        }

        void ReadLines(PdbLines lines, MethodSymbols symbols)
        {
            for (int i = 0; i < lines.lines.Length; i++)
            {
                var line = lines.lines[i];

                symbols.Instructions.Add(new InstructionSymbol((int)line.offset, new SequencePoint(GetDocument(lines.file))
                {
                    StartLine = (int)line.lineBegin,
                    StartColumn = (int)line.colBegin,
                    EndLine = (int)line.lineEnd,
                    EndColumn = (int)line.colEnd,
                }));
            }
        }

        public void Dispose()
        {
            pdb_file.Close();
        }
    }
}
