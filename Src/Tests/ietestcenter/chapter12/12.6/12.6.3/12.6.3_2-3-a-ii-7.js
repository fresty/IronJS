/// Copyright (c) 2009 Microsoft Corporation 
/// 
/// Redistribution and use in source and binary forms, with or without modification, are permitted provided
/// that the following conditions are met: 
///    * Redistributions of source code must retain the above copyright notice, this list of conditions and
///      the following disclaimer. 
///    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and 
///      the following disclaimer in the documentation and/or other materials provided with the distribution.  
///    * Neither the name of Microsoft nor the names of its contributors may be used to
///      endorse or promote products derived from this software without specific prior written permission.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
/// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
/// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
/// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
/// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
/// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
/// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
/// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

/*
Refer 12.6.3; 
The production 
    IterationStatement : for ( var VariableDeclarationListNoIn ; Expressionopt ; Expressionopt ) Statement
is evaluated as follows:
*/

ES5Harness.registerTest({
    id: "12.6.3_2-3-a-ii-7",

    path: "TestCases/chapter12/12.6/12.6.3/12.6.3_2-3-a-ii-7.js",

    description: "The for Statement - (normal, V, empty) will be returned when first Expression is a String object (value is empty string)",

    test: function testcase() {
        var accessed = false;
        var strObj = new String("");
        for (var i = 0; strObj;) {
            accessed = true;
            break;
        }
        return accessed;
    },

    precondition: function prereq() {
        return true;
    }
});


