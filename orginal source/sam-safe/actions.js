///////////////////////////////////////////////////////////////////////////////
// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>

'use strict' ;

var actions = {} ;

actions.init = (present) => {
    actions.present = present ;
} ;

actions.present = (data) => {
    return false ;
} ;

actions.edit = (data,next) => {
    data.lastEdited = {title: data.title,  description: data.description, id: data.id } ;
    actions.present(data,next) ;
    return false ;
} ;

actions.save = (data,next) => {
    data.item = {title: data.title, description: data.description, id: data.id || ''} ;
    if (data.item.id !== '') {
        // simulate a slow save after
        // editing an item
        setTimeout(function() { 
            actions.present(data,next) ;
        }, 9000);
    }
    else {
        // proceed as normal when created a new item
        actions.present(data,next) ;
    }
    return false ;
} ;

actions.delete = (data,next) => {
    data.deletedItemId = data.id ;
    actions.present(data,next) ;
    return false ;
} ;

actions.cancel = (data,next) => {
    actions.present(data,next) ;
    return false ;
} ;

actions.intents = { edit: 'edit', save: 'save', delete: 'delete', cancel: 'cancel' } ;

module.exports = actions ;