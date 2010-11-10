﻿namespace IronJS.Api

open System
open IronJS
open IronJS.Aliases
open IronJS.Utils.Patterns

module Extensions = 

  type Object with 

    //--------------------------------------------------------------------------
    // Properties
    member o.put (name, v:IjsBox) =
      o.Methods.PutBoxProperty.Invoke(o, name, v)

    member o.put (name, v:IjsBool) =
      let v = if v then TaggedBools.True else TaggedBools.False
      o.Methods.PutValProperty.Invoke(o, name, v)

    member o.put (name, v:IjsNum) =
      o.Methods.PutValProperty.Invoke(o, name, v)

    member o.put (name, v:ClrObject) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.Clr)

    member o.put (name, v:IjsStr) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.String)

    member o.put (name, v:Undefined) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.Undefined)

    member o.put (name, v:IjsObj) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.Object)

    member o.put (name, v:IjsFunc) =
      o.Methods.PutRefProperty.Invoke(o, name, v, TypeTags.Function)

    member o.put (name, v:IjsRef, tc:TypeTag) =
      o.Methods.PutRefProperty.Invoke(o, name, v, tc)

    member o.put (name:string, v:IjsBox, attrs:DescriptorAttr) =
      o.put(name, v)
      o.setAttrs(name, attrs)

    member o.put (name:string, v:IjsBool, attrs:DescriptorAttr) =
      o.put(name, v)
      o.setAttrs(name, attrs)

    member o.put (name:string, v:IjsNum, attrs:DescriptorAttr) =
      o.put(name, v)
      o.setAttrs(name, attrs)

    member o.put (name:string, v:ClrObject, attrs:DescriptorAttr) =
      o.put(name, v)
      o.setAttrs(name, attrs)

    member o.put (name:string, v:IjsStr, attrs:DescriptorAttr) =
      o.put(name, v)
      o.setAttrs(name, attrs)

    member o.put (name:string, v:Undefined, attrs:DescriptorAttr) =
      o.put(name, v)
      o.setAttrs(name, attrs)

    member o.put (name:string, v:IjsObj, attrs:DescriptorAttr) =
      o.put(name, v)
      o.setAttrs(name, attrs)

    member o.put (name:string, v:IjsFunc, attrs:DescriptorAttr) =
      o.put(name, v)
      o.setAttrs(name, attrs)

    member o.put (name:string, v:IjsRef, t:TypeTag, attrs:DescriptorAttr) =
      o.put(name, v, t)
      o.setAttrs(name, attrs)

    member o.get (name) =
      o.Methods.GetProperty.Invoke(o, name)

    member o.get<'a> (name) =
      o.Methods.GetProperty.Invoke(o, name) |> Utils.unboxT<'a>

    member o.has (name) =
      o.Methods.HasProperty.Invoke(o, name)

    member o.delete (name) =
      o.Methods.DeleteProperty.Invoke(o, name)
      
    //--------------------------------------------------------------------------
    // Indexes
    member o.put (index, v:IjsBox) =
      o.Methods.PutBoxIndex.Invoke(o, index, v)

    member o.put (index, v:IjsBool) =
      let v = if v then TaggedBools.True else TaggedBools.False
      o.Methods.PutValIndex.Invoke(o, index, v)

    member o.put (index, v:IjsNum) =
      o.Methods.PutValIndex.Invoke(o, index, v)

    member o.put (index, v:ClrObject) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.Clr)

    member o.put (index, v:IjsStr) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.String)

    member o.put (index, v:Undefined) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.Undefined)

    member o.put (index, v:IjsObj) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.Object)

    member o.put (index, v:IjsFunc) =
      o.Methods.PutRefIndex.Invoke(o, index, v, TypeTags.Function)

    member o.put (index, v:IjsRef, tc:TypeTag) =
      o.Methods.PutRefIndex.Invoke(o, index, v, tc)

    member o.get (index) =
      o.Methods.GetIndex.Invoke(o, index)
      
    member o.get<'a> (index) =
      o.Methods.GetIndex.Invoke(o, index) |> Utils.unboxT<'a>

    member o.has (index) =
      o.Methods.HasIndex.Invoke(o, index)

    member o.delete (index) =
      o.Methods.DeleteIndex.Invoke(o, index)

open Extensions

module Environment =

  //----------------------------------------------------------------------------
  let hasCompiler (env:IjsEnv) funcId = env.Compilers.ContainsKey funcId

  //----------------------------------------------------------------------------
  let addCompilerId (env:IjsEnv) funId compiler =
    if hasCompiler env funId |> not then
      env.Compilers.Add(funId, FunctionCompiler compiler)

  //----------------------------------------------------------------------------
  let addCompiler (env:IjsEnv) (f:IjsFunc) compiler =
    if hasCompiler env f.FunctionId |> not then
      f.Compiler <- FunctionCompiler compiler
      env.Compilers.Add(f.FunctionId, f.Compiler)
    
  //----------------------------------------------------------------------------
  let createObject (env:IjsEnv) =
    let map = env.Maps.Base
    let proto = env.Prototypes.Object
    let object' = IjsObj(env, map, proto, Classes.Object)
    object'.Methods <- env.Methods.Object
    object'

  //----------------------------------------------------------------------------
  let createArray (env:IjsEnv) (size:uint32) =
    let array = ArrayObject(env, size)
    array.Methods <- env.Methods.Array
    array.Methods.PutValProperty.Invoke(array, "length", double size)
    array :> IjsObj
    
  //----------------------------------------------------------------------------
  let createString (env:IjsEnv) (s:IjsStr) =
    let map = env.Maps.String
    let proto = env.Prototypes.String
    let string = ValueObject(env, map, proto, Classes.String)
    string.Methods <- env.Methods.Object
    string.Methods.PutValProperty.Invoke(string, "length", double s.Length)
    string.Value.Box.Clr <-s
    string.Value.Box.Tag <- TypeTags.String
    string.Value.HasValue <- true
    string :> IjsObj
    
  //----------------------------------------------------------------------------
  let createNumber (env:IjsEnv) n =
    let map = env.Maps.Number
    let proto = env.Prototypes.Number
    let number = ValueObject(env, map, proto, Classes.Number)
    number.Methods <- env.Methods.Object
    number.Value.Box.Number <- n
    number.Value.HasValue <- true
    number :> IjsObj
    
  //----------------------------------------------------------------------------
  let createBoolean (env:IjsEnv) b =
    let map = env.Maps.Boolean
    let proto = env.Prototypes.Boolean
    let boolean = ValueObject(env, map, proto, Classes.Boolean)
    boolean.Methods <- env.Methods.Object
    boolean.Value.Box.Bool <- b
    boolean.Value.Box.Tag <- TypeTags.Bool
    boolean.Value.HasValue <- true
    boolean :> IjsObj
  
  //----------------------------------------------------------------------------
  let createPrototype (env:IjsEnv) =
    let map = env.Maps.Prototype
    let proto = env.Prototypes.Object
    let prototype = IjsObj(env, map, proto, Classes.Object)
    prototype .Methods <- env.Methods.Object
    prototype
  
  //----------------------------------------------------------------------------
  let createFunction env id (args:int) closureScope dynamicScope =
    let proto = createPrototype env
    let func = IjsFunc(env, id, closureScope, dynamicScope)

    func.Methods <- env.Methods.Object
    func.ConstructorMode <- ConstructorModes.User

    proto.put("constructor", func)

    func.put("prototype", proto, DescriptorAttrs.Immutable)
    func.put("length", double args, DescriptorAttrs.DontDelete)

    func
    
  //----------------------------------------------------------------------------
  let createError (env:IjsEnv) =
    let map = env.Maps.Base
    let proto = env.Prototypes.Error
    let error = IjsObj(env, map, proto, Classes.Error)
    error.Methods <- env.Methods.Object
    error

  let raiseError (env:IjsEnv) prototype (message:IjsStr) =
    let error = createError env
    error.Prototype <- prototype
    error.put("message", message)
    raise (new UserError(Utils.boxObject error))

  let raiseEvalError (env:IjsEnv) = raiseError env env.Prototypes.EvalError
  let raiseRangeError (env:IjsEnv) = raiseError env env.Prototypes.RangeError
  let raiseSyntaxError (env:IjsEnv) = raiseError env env.Prototypes.SyntaxError
  let raiseTypeError (env:IjsEnv) = raiseError env env.Prototypes.TypeError
  let raiseURIError (env:IjsEnv) = raiseError env env.Prototypes.URIError
  let raiseReferenceError (env:IjsEnv) = 
    raiseError env env.Prototypes.ReferenceError
    
  //----------------------------------------------------------------------------
  module Reflected =

    let createObject = 
      Utils.Reflected.methodInfo "Api.Environment" "createObject"

    let createArray = 
      Utils.Reflected.methodInfo "Api.Environment" "createArray"

    let createFunction = 
      Utils.Reflected.methodInfo "Api.Environment" "createFunction"

//------------------------------------------------------------------------------
// Static class containing all type conversions
type TypeConverter =

  //----------------------------------------------------------------------------
  static member toBox(b:IjsBox) = b
  static member toBox(d:IjsNum) = Utils.boxNumber d
  static member toBox(b:IjsBool) = Utils.boxBool b
  static member toBox(s:IjsStr) = Utils.boxString s
  static member toBox(o:IjsObj) = Utils.boxObject o
  static member toBox(f:IjsFunc) = Utils.boxFunction f
  static member toBox(c:ClrObject) = Utils.boxClr c
  static member toBox(expr:Dlr.Expr) = 
    Dlr.callStaticT<TypeConverter> "toBox" [expr]
    
  //----------------------------------------------------------------------------
  static member toClrObject(d:IjsNum) = box d
  static member toClrObject(b:IjsBool) = box b
  static member toClrObject(s:IjsStr) = box s
  static member toClrObject(o:IjsObj) = box o
  static member toClrObject(f:IjsFunc) = box f
  static member toClrObject(c:ClrObject) = c
  static member toClrObject(b:IjsBox) =
    match b with
    | Number number -> box number
    | Undefined _ -> null
    | String string -> box string
    | Boolean boolean -> box boolean
    | Clr clr -> clr
    | Object obj -> box obj
    | Function func -> box func

  static member toClrObject (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toClrObject" [expr]

  //----------------------------------------------------------------------------
  static member toString (b:IjsBool) = if b then "true" else "false"
  static member toString (s:IjsStr) = s
  static member toString (u:Undefined) = "undefined"
  static member toString (box:IjsBox) =
    match box with
    | Undefined _ -> "undefined"
    | String string -> string
    | Number number -> number |> TypeConverter.toString
    | Boolean boolean -> boolean |> TypeConverter.toString
    | Clr clr -> clr |> TypeConverter.toString
    | Object obj -> obj |> TypeConverter.toString
    | Function func -> (func :> IjsObj) |> TypeConverter.toString

  static member toString (o:IjsObj) = 
    match o.Class with
    | Classes.String -> (o :?> ValueObject).Value.Box.String
    | _ -> 
      o.Methods.Default.Invoke(o, DefaultValue.String)|>TypeConverter.toString

  static member toString (d:IjsNum) = 
    if System.Double.IsInfinity d then "Infinity" else d.ToString()

  static member toString (c:ClrObject) = 
    if FSKit.Utils.isNull c then "null" else c.ToString()

  static member toString (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toString" [expr]
      
  //----------------------------------------------------------------------------
  static member toPrimitive (b:IjsBool, _:byte) = Utils.boxBool b
  static member toPrimitive (d:IjsNum, _:byte) = Utils.boxNumber d
  static member toPrimitive (s:IjsStr, _:byte) = Utils.boxString s
  static member toPrimitive (o:IjsObj, h:byte) = o.Methods.Default.Invoke(o, h)
  static member toPrimitive (o:IjsObj) = o.Methods.Default.Invoke(o, 0uy)
  static member toPrimitive2 (b:IjsBox, h:byte) =
    match b with
    | Number _
    | Boolean _
    | String _
    | Undefined _ -> b
    | Clr clr -> TypeConverter.toPrimitive(clr, h)
    | Object obj -> obj.Methods.Default.Invoke(obj, h)
    | Function func -> func.Methods.Default.Invoke(func, h)
  
  static member toPrimitive (c:ClrObject, _:byte) = 
    Utils.boxClr (if c = null then null else c.ToString())

  static member toPrimitive (u:Undefined, _:byte) = 
    Utils.BoxedConstants.undefined

  static member toPrimitive (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toPrimitive" [expr]
      
  //----------------------------------------------------------------------------
  static member toBoolean (b:IjsBool) = b
  static member toBoolean (d:IjsNum) = d > 0.0 || d < 0.0
  static member toBoolean (c:ClrObject) = if c = null then false else true
  static member toBoolean (s:IjsStr) = s.Length > 0
  static member toBoolean (u:Undefined) = false
  static member toBoolean (o:IjsObj) = true
  static member toBoolean (box:IjsBox) =
    match box with 
    | Number number -> TypeConverter.toBoolean number
    | Boolean boolean -> boolean
    | Undefined _ -> false
    | String string -> string.Length > 0
    | Clr clr -> TypeConverter.toBoolean clr
    | Object _
    | Function _ -> true
    
  static member toBoolean (expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toBoolean" [expr]

  //----------------------------------------------------------------------------
  static member toNumber (b:IjsBool) : double = if b then 1.0 else 0.0
  static member toNumber (c:ClrObject) = if c = null then 0.0 else 1.0
  static member toNumber (u:Undefined) = IjsNum.NaN
  static member toNumber (box:IjsBox) =
    match box with
    | Number number -> number
    | Boolean boolean -> if boolean then 1.0 else 0.0
    | String string -> string |> TypeConverter.toNumber
    | Undefined _ -> NaN
    | Clr clr -> TypeConverter.toNumber clr
    | Object obj -> obj |> TypeConverter.toNumber
    | Function func -> (func :> IjsObj) |> TypeConverter.toNumber

  static member toNumber (o:IjsObj) : IjsNum = 
    match o.Class with
    | Classes.Number -> (Utils.ValueObject.getValue o).Number
    | _ -> 
      let boxedValue = o.Methods.Default.Invoke(o, DefaultValue.Number)
      boxedValue |> TypeConverter.toNumber 

  static member toNumber (d:IjsNum) = 
    if d = TaggedBools.True then 1.0 elif d = TaggedBools.False then 0.0 else d

  static member toNumber (s:IjsStr) = 
    let mutable d = 0.0
    if Double.TryParse(s, anyNumber, invariantCulture, &d) then d else NaN

  static member toNumber (expr:Dlr.Expr) = 
    Dlr.callStaticT<TypeConverter> "toNumber" [expr]
        
  //----------------------------------------------------------------------------
  static member toObject (env:IjsEnv, o:IjsObj) = o
  static member toObject (env:IjsEnv, f:IjsFunc) = f
  static member toObject (env:IjsEnv, u:Undefined) = failwith "[[TypeError]]"
  static member toObject (env:IjsEnv, s:IjsStr) = Environment.createString env s
  static member toObject (env:IjsEnv, n:IjsNum) = Environment.createNumber env n
  static member toObject (env:IjsEnv, b:IjsBool) = 
    Environment.createBoolean env b

  static member toObject (env:IjsEnv, b:Box) =
    match b with
    | Function _ 
    | Object _ -> b.Object

    | Undefined _
    | Clr _ -> Errors.Generic.notImplemented()

    | Number number -> Environment.createNumber env number
    | String string -> Environment.createString env string
    | Boolean boolean -> Environment.createBoolean env boolean

  static member toObject (env:Dlr.Expr, expr:Dlr.Expr) =
    Dlr.callStaticT<TypeConverter> "toObject" [env; expr]
      
  //----------------------------------------------------------------------------
  static member toInt32 (d:IjsNum) = d |> uint32 |> int
  static member toInt32 (b:IjsBox) =
    b |> TypeConverter.toNumber |> TypeConverter.toInt32

  static member toUInt32 (d:IjsNum) = d |> uint32 
  static member toUInt32 (b:IjsBox) =
    b |> TypeConverter.toNumber |> TypeConverter.toUInt32

  static member toUInt16 (d:IjsNum) = d |> uint32 |> uint16
  static member toUInt16 (b:IjsBox) =
    b |> TypeConverter.toNumber |> TypeConverter.toUInt16

  static member toInteger (d:IjsNum) = 
    if d > 2147483647.0 then 2147483647 else d |> uint32 |> int
  static member toInteger (b:IjsBox) =
    b |> TypeConverter.toNumber |> TypeConverter.toInteger

  //-------------------------------------------------------------------------
  static member convertTo (env:Dlr.Expr) (expr:Dlr.Expr) (t:System.Type) =
    if Object.ReferenceEquals(expr.Type, t) then expr
    elif t.IsAssignableFrom(expr.Type) then Dlr.cast t expr
    else 
      if   t = typeof<IjsNum> then TypeConverter.toNumber expr
      elif t = typeof<IjsStr> then TypeConverter.toString expr
      elif t = typeof<IjsBool> then TypeConverter.toBoolean expr
      elif t = typeof<IjsBox> then TypeConverter.toBox expr
      elif t = typeof<IjsObj> then TypeConverter.toObject(env, expr)
      elif t = typeof<ClrObject> then TypeConverter.toClrObject expr
      else Errors.Generic.noConversion expr.Type t

  static member convertToT<'a> env expr = 
    TypeConverter.convertTo env expr typeof<'a>
    
//------------------------------------------------------------------------------
// Operators
type Operators =

  //----------------------------------------------------------------------------
  // Unary
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  // typeof
  static member typeOf (o:IjsBox) = 
    match o with
    | IsNumber _ -> "number" 
    | IsNull -> "object"
    | _ -> TypeTags.Names.[o.Tag]

  static member typeOf expr = Dlr.callStaticT<Operators> "typeOf" [expr]
  
  //----------------------------------------------------------------------------
  // !
  static member not (o) = Dlr.callStaticT<Operators> "not" [o]
  static member not (o:IjsBox) =
    not (TypeConverter.toBoolean o)
    
  //----------------------------------------------------------------------------
  // ~
  static member bitCmpl (o) = Dlr.callStaticT<Operators> "bitCmpl" [o]
  static member bitCmpl (o:IjsBox) =
    let o = TypeConverter.toNumber o
    let o = TypeConverter.toInt32 o
    ~~~ o |> double
      
  //----------------------------------------------------------------------------
  // + (unary)
  static member plus (l, r) = Dlr.callStaticT<Operators> "plus" [l; r]
  static member plus (o:IjsBox) =
    Utils.boxNumber (TypeConverter.toNumber o)
    
  //----------------------------------------------------------------------------
  // - (unary)
  static member minus (l, r) = Dlr.callStaticT<Operators> "minus" [l; r]
  static member minus (o:IjsBox) =
    Utils.boxNumber ((TypeConverter.toNumber o) * -1.0)

  //----------------------------------------------------------------------------
  // Binary
  //----------------------------------------------------------------------------

  // in
  static member in' (env, l,r) = Dlr.callStaticT<Operators> "in'" [env; l; r]
  static member in' (env:IjsEnv, l:IjsBox, r:IjsBox) = 
    if Utils.Box.isObject r.Tag |> not then
      Environment.raiseTypeError env "Right operand is not a object"

    match l with
    | IsIndex i -> r.Object.Methods.HasIndex.Invoke(r.Object, i)
    | _ -> 
      let name = TypeConverter.toString l
      r.Object.Methods.HasProperty.Invoke(r.Object, name)

  // instanceof
  static member instanceOf (env, l,r) = 
    Dlr.callStaticT<Operators> "instanceOf" [env; l; r]

  static member instanceOf(env:IjsEnv, l:IjsBox, r:IjsBox) =
    if Utils.Box.isFunction r.Tag |> not then
      Environment.raiseTypeError env "Right operand is not a function"

    if Utils.Box.isObject l.Tag |> not 
      then false
      else r.Func.Methods.HasInstance.Invoke(r.Func, l.Object)
    
  //----------------------------------------------------------------------------
  // <
  static member lt (l, r) = Dlr.callStaticT<Operators> "lt" [l; r]
  static member lt (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number < r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String < r.String
        else TypeConverter.toNumber l < TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // <=
  static member ltEq (l, r) = Dlr.callStaticT<Operators> "ltEq" [l; r]
  static member ltEq (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number <= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String <= r.String
        else TypeConverter.toNumber l <= TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // >
  static member gt (l, r) = Dlr.callStaticT<Operators> "gt" [l; r]
  static member gt (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number > r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String > r.String
        else TypeConverter.toNumber l > TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // >=
  static member gtEq (l, r) = Dlr.callStaticT<Operators> "gtEq" [l; r]
  static member gtEq (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then l.Number >= r.Number
      elif l.Tag = TypeTags.String && r.Tag = TypeTags.String
        then l.String >= r.String
        else TypeConverter.toNumber l >= TypeConverter.toNumber r
        
  //----------------------------------------------------------------------------
  // ==
  static member eq (l, r) = Dlr.callStaticT<Operators> "eq" [l; r]
  static member eq (l:IjsBox, r:IjsBox) = 
    if Utils.Box.isNumber l.Marker && Utils.Box.isNumber r.Marker then
      l.Number = r.Number

    elif l.Tag = r.Tag then
      match l.Tag with
      | TypeTags.Undefined -> true
      | TypeTags.String -> l.String = r.String
      | TypeTags.Bool -> l.Bool = r.Bool
      | TypeTags.Clr
      | TypeTags.Function
      | TypeTags.Object -> Object.ReferenceEquals(l.Clr, r.Clr)
      | _ -> false

    else
      if l.Tag = TypeTags.Clr 
        && l.Clr = null 
        && r.Tag = TypeTags.Undefined then true
      
      elif r.Tag = TypeTags.Clr 
        && r.Clr = null 
        && l.Tag = TypeTags.Undefined then true

      elif Utils.Box.isNumber l.Marker && r.Tag = TypeTags.String then
        l.Number = TypeConverter.toNumber r.String
        
      elif r.Tag = TypeTags.String && Utils.Box.isNumber r.Marker then
        TypeConverter.toNumber l.String = r.Number

      elif l.Tag = TypeTags.Bool then
        let mutable l = Utils.boxNumber(TypeConverter.toNumber l)
        Operators.eq(l, r)

      elif r.Tag = TypeTags.Bool then
        let mutable r = Utils.boxNumber(TypeConverter.toNumber r)
        Operators.eq(l, r)

      elif r.Tag >= TypeTags.Object then
        match l.Tag with
        | TypeTags.String -> 
          let mutable r = TypeConverter.toPrimitive(r.Object)
          Operators.eq(l, r)

        | _ -> 
          if Utils.Box.isNumber l.Marker then
            let mutable r = TypeConverter.toPrimitive(r.Object)
            Operators.eq(l, r)
          else
            false

      elif l.Tag >= TypeTags.Object then
        match r.Tag with
        | TypeTags.String -> 
          let mutable l = TypeConverter.toPrimitive(l.Object)
          Operators.eq(l, r)

        | _ -> 
          if Utils.Box.isNumber r.Marker then
            let mutable l = TypeConverter.toPrimitive(l.Object)
            Operators.eq(l, r)
          else
            false

      else
        false
        
  //----------------------------------------------------------------------------
  // !=
  static member notEq (l, r) = Dlr.callStaticT<Operators> "notEq" [l; r]
  static member notEq (l:IjsBox, r:IjsBox) = not (Operators.eq(l, r))
  
  //----------------------------------------------------------------------------
  // ===
  static member same (l, r) = Dlr.callStaticT<Operators> "same" [l; r]
  static member same (l:IjsBox, r:IjsBox) = 
    if Utils.Box.isBothNumber l.Marker r.Marker then
      l.Number = r.Number

    elif l.Tag = r.Tag then
      match l.Tag with
      | TypeTags.Undefined -> true
      | TypeTags.String -> l.String = r.String
      | TypeTags.Bool -> l.Bool = r.Bool
      | TypeTags.Clr
      | TypeTags.Function
      | TypeTags.Object -> Object.ReferenceEquals(l.Clr, r.Clr)
      | _ -> false

    else
      false
      
  //----------------------------------------------------------------------------
  // !==
  static member notSame (l, r) = Dlr.callStaticT<Operators> "notSame" [l; r]
  static member notSame (l:IjsBox, r:IjsBox) =
    Operators.same(l, r) |> not
    
  //----------------------------------------------------------------------------
  // +
  static member add (l, r) = Dlr.callStaticT<Operators> "add" [l; r]
  static member add (l:IjsBox, r:IjsBox) = 
    if Utils.Box.isBothNumber l.Marker r.Marker then
      Utils.boxNumber (l.Number + r.Number)

    elif l.Tag = TypeTags.String || r.Tag = TypeTags.String then
      Utils.boxString (TypeConverter.toString(l) + TypeConverter.toString(r))

    else
      Utils.boxNumber (TypeConverter.toNumber(l) + TypeConverter.toNumber(r))
      
  //----------------------------------------------------------------------------
  // -
  static member sub (l, r) = Dlr.callStaticT<Operators> "sub" [l; r]
  static member sub (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker 
      then Utils.boxNumber (l.Number - r.Number)
      else Utils.boxNumber (TypeConverter.toNumber l - TypeConverter.toNumber r)
      
  //----------------------------------------------------------------------------
  // /
  static member div (l, r) = Dlr.callStaticT<Operators> "div" [l; r]
  static member div (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then Utils.boxNumber (l.Number / r.Number)
      else Utils.boxNumber (TypeConverter.toNumber l / TypeConverter.toNumber r)
      
  //----------------------------------------------------------------------------
  // *
  static member mul (l, r) = Dlr.callStaticT<Operators> "mul" [l; r]
  static member mul (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then Utils.boxNumber (l.Number * r.Number)
      else Utils.boxNumber (TypeConverter.toNumber l * TypeConverter.toNumber r)
      
  //----------------------------------------------------------------------------
  // %
  static member mod' (l, r) = Dlr.callStaticT<Operators> "mod'" [l; r]
  static member mod' (l:IjsBox, r:IjsBox) =
    if Utils.Box.isBothNumber l.Marker r.Marker
      then Utils.boxNumber (l.Number % r.Number)
      else Utils.boxNumber (TypeConverter.toNumber l % TypeConverter.toNumber r)
    
  //----------------------------------------------------------------------------
  // &
  static member bitAnd (l, r) = Dlr.callStaticT<Operators> "bitAnd" [l; r]
  static member bitAnd (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    (l &&& r) |> double
    
  //----------------------------------------------------------------------------
  // |
  static member bitOr (l, r) = Dlr.callStaticT<Operators> "bitOr" [l; r]
  static member bitOr (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    (l ||| r) |> double
    
  //----------------------------------------------------------------------------
  // ^
  static member bitXOr (l, r) = Dlr.callStaticT<Operators> "bitXOr" [l; r]
  static member bitXOr (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toInt32 r
    (l ^^^ r) |> double
    
  //----------------------------------------------------------------------------
  // <<
  static member bitLhs (l, r) = Dlr.callStaticT<Operators> "bitLhs" [l; r]
  static member bitLhs (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    (l <<< int r) |> double
    
  //----------------------------------------------------------------------------
  // >>
  static member bitRhs (l, r) = Dlr.callStaticT<Operators> "bitRhs" [l; r]
  static member bitRhs (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    (l >>> int r) |> double
    
  //----------------------------------------------------------------------------
  // >>>
  static member bitURhs (l, r) = Dlr.callStaticT<Operators> "bitURhs" [l; r]
  static member bitURhs (l:IjsBox, r:IjsBox) =
    let l = TypeConverter.toNumber l
    let r = TypeConverter.toNumber r
    let l = TypeConverter.toUInt32 l
    let r = TypeConverter.toUInt32 r &&& 0x1Fu
    (l >>> int r) |> double
    
  //----------------------------------------------------------------------------
  // &&
  static member and' (l, r) = Dlr.callStaticT<Operators> "and'" [l; r]
  static member and' (l:IjsBox, r:IjsBox) =
    if not (TypeConverter.toBoolean l) then l else r
    
  //----------------------------------------------------------------------------
  // ||
  static member or' (l, r) = Dlr.callStaticT<Operators> "or'" [l; r]
  static member or' (l:IjsBox, r:IjsBox) =
    if TypeConverter.toBoolean l then l else r
      
//------------------------------------------------------------------------------
// PropertyMap
//------------------------------------------------------------------------------
module PropertyMap =

  //----------------------------------------------------------------------------
  let getSubMap (map:PropertyMap) name = 
    if map.isDynamic then 
      let index = 
        if map.FreeIndexes.Count > 0 then map.FreeIndexes.Pop()
        else map.NextIndex <- map.NextIndex + 1; map.NextIndex - 1

      map.PropertyMap.Add(name, index)
      map

    else
      let mutable subMap = null
      
      if not(map.SubClasses.TryGetValue(name, &subMap)) then
        let properties = new MutableDict<string, int>(map.PropertyMap)
        properties.Add(name, properties.Count)
        subMap <- IronJS.PropertyMap(map.Env, properties)
        map.SubClasses.Add(name, subMap)

      subMap

  //----------------------------------------------------------------------------
  let buildSubMap (map:PropertyMap) names =
    Seq.fold (fun map name -> getSubMap map name) map names
        
  //----------------------------------------------------------------------------
  let makeDynamic (map:IronJS.PropertyMap) =
    if map.isDynamic then map
    else
      let newMap = PropertyMap(null)
      newMap.Id <- -1L
      newMap.NextIndex <- map.NextIndex
      newMap.FreeIndexes <- new MutableStack<int>()
      newMap.PropertyMap <- new MutableDict<string, int>(map.PropertyMap)
      newMap
        
  //----------------------------------------------------------------------------
  let delete (x:IronJS.PropertyMap, name) =
    let pc = if not x.isDynamic then makeDynamic x else x
    let mutable index = 0

    if pc.PropertyMap.TryGetValue(name, &index) then 
      pc.FreeIndexes.Push index
      pc.PropertyMap.Remove name |> ignore

    pc
      
  //----------------------------------------------------------------------------
  let getIndex (map:PropertyMap) name = map.PropertyMap.[name]
    
//------------------------------------------------------------------------------
// Function API
//------------------------------------------------------------------------------
type Function() =

  //----------------------------------------------------------------------------
  static let getPrototype(f:IjsFunc) =
    let prototype = (f :> IjsObj).Methods.GetProperty.Invoke(f, "prototype")
    match prototype.Tag with
    | TypeTags.Function
    | TypeTags.Object -> prototype.Object
    | _ -> f.Env.Prototypes.Object
    
  //----------------------------------------------------------------------------
  //15.3.5.3
  static member hasInstance (f:IjsFunc, o:IjsObj) =
    let prototype = f.get "prototype"

    if Utils.Box.isObject prototype.Tag |> not then
      Environment.raiseTypeError f.Env "prototype property is not an object"

    if o = null || o.Prototype = null
      then false 
      else Object.ReferenceEquals(prototype.Object, o.Prototype)

  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  // GENERATED FUNCTION METHODS
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,IjsBox>>(f)
    c.Invoke(f,t)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,IjsBox>>(f)
    c.Invoke(f,t,a0)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,IjsBox>>(f)
    c.Invoke(f,t,a0,a1)
  
  #if CLR2
  #else
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3,a4)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3,a4,a5)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3,a4,a5,a6)
  
  //----------------------------------------------------------------------------
  static member call (f:IjsFunc,t,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6,a7:'a7) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,'a7,IjsBox>>(f)
    c.Invoke(f,t,a0,a1,a2,a3,a4,a5,a6,a7)
  #endif

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o) |> ignore
      Utils.boxObject o

    | _ -> 
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0) |> ignore
      Utils.boxObject o

    | _ ->
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1) |> ignore
      Utils.boxObject o

    | _ ->
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"
    
  #if CLR2
  #else
  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2) |> ignore
      Utils.boxObject o

    | _ ->
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3) |> ignore
      Utils.boxObject o

    | _ ->
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4) |> ignore
      Utils.boxObject o

    | _ ->
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5) |> ignore
      Utils.boxObject o

    | _ ->
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5,a6)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5,a6) |> ignore
      Utils.boxObject o

    | _ ->
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"

  //----------------------------------------------------------------------------
  static member construct (f:IjsFunc,t:IjsObj,a0:'a0,a1:'a1,a2:'a2,a3:'a3,a4:'a4,a5:'a5,a6:'a6,a7:'a7) =
    let c = f.Compiler
    let c = c.compileAs<Func<IjsFunc,IjsObj,'a0,'a1,'a2,'a3,'a4,'a5,'a6,'a7,IjsBox>>(f)

    match f.ConstructorMode with
    | ConstructorModes.Host -> c.Invoke(f,null,a0,a1,a2,a3,a4,a5,a6,a7)
    | ConstructorModes.User -> 
      let o = Environment.createObject f.Env
      o.Prototype <- getPrototype f
      c.Invoke(f,o,a0,a1,a2,a3,a4,a5,a6,a7) |> ignore
      Utils.boxObject o

    | _ ->
      Environment.raiseTypeError 
        f.Env "Can't call [[Construct]] on non-constructor"

  #endif

  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  // GENERATED FUNCTION METHODS
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------
  //----------------------------------------------------------------------------

//------------------------------------------------------------------------------
// HostFunction API
module HostFunction =

  [<ReferenceEquality>]
  type DispatchTarget<'a when 'a :> Delegate> = {
    Delegate : ClrType
    Function : IjsHostFunc<'a>
    Invoke: Dlr.Expr -> Dlr.Expr seq -> Dlr.Expr
  }

  //----------------------------------------------------------------------------
  let marshalArgs (args:Dlr.ExprParam array) (env:Dlr.Expr) i t =
    if i < args.Length 
      then TypeConverter.convertTo env args.[i] t
      else
        if FSKit.Utils.isTypeT<IjsBox> t
          then Expr.BoxedConstants.undefined else Dlr.default' t
      
  //----------------------------------------------------------------------------
  let marshalBoxParams (f:IjsHostFunc<_>) args m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map Expr.box
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<IjsBox> x]
    
  //----------------------------------------------------------------------------
  let marshalObjectParams (f:IjsHostFunc<_>) (args:Dlr.ExprParam array) m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map TypeConverter.toClrObject
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<ClrObject> x]
    
  //----------------------------------------------------------------------------
  let private createParam i t = Dlr.param (sprintf "a%i" i) t
  
  //----------------------------------------------------------------------------
  let private addEmptyParamsObject<'a> (args:Dlr.ExprParam array) =
    args |> Array.map (fun x -> x :> Dlr.Expr)
         |> FSKit.Array.appendOne Dlr.newArrayEmptyT<'a> 
         |> Seq.ofArray
  
  //----------------------------------------------------------------------------
  let compileDispatcher (target:DispatchTarget<'a>) = 
    let f = target.Function

    let argTypes = FSKit.Reflection.getDelegateArgTypes target.Delegate
    let args = argTypes |> Array.mapi createParam
    let passedArgs = args |> Seq.skip f.MarshalMode |> Array.ofSeq

    let func = args.[0] :> Dlr.Expr
    let env = Dlr.field func "Env"

    let marshalled = f.ArgTypes |> Seq.mapi (marshalArgs passedArgs env)
    let marshalled = 
      let paramsExist = f.ArgTypes.Length < passedArgs.Length 

      match f.ParamsMode with
      | ParamsModes.BoxParams -> 
        if paramsExist
          then marshalBoxParams f passedArgs marshalled
          else addEmptyParamsObject<IjsBox> passedArgs 

      | ParamsModes.ObjectParams when paramsExist -> 
        if paramsExist
          then marshalObjectParams f passedArgs marshalled
          else addEmptyParamsObject<ClrObject> passedArgs 

      | _ -> marshalled

    let invoke = target.Invoke func marshalled
    let body = 
      if FSKit.Utils.isTypeT<IjsBox> f.ReturnType then invoke
      elif FSKit.Utils.isVoid f.ReturnType then Expr.voidAsUndefined invoke
      else
        Dlr.blockTmpT<Box> (fun tmp ->
          [
            (Expr.setBoxTagOf tmp invoke)
            (Expr.setBoxValue tmp invoke)
            (tmp :> Dlr.Expr)
          ] |> Seq.ofList
        )
            
    let lambda = Dlr.lambda target.Delegate args body
    Debug.printExpr lambda
    lambda.Compile()

  //----------------------------------------------------------------------------
  let generateInvoke<'a when 'a :> Delegate> f args =
    let casted = Dlr.castT<IjsHostFunc<'a>> f
    Dlr.invoke (Dlr.field casted "Delegate") args
  
  //----------------------------------------------------------------------------
  let compile<'a when 'a :> Delegate> (x:IjsFunc) (delegate':System.Type) =
    compileDispatcher {
      Delegate = delegate'
      Function = x :?> IjsHostFunc<'a>
      Invoke = generateInvoke<'a>
    }
    
  //----------------------------------------------------------------------------
  let create (env:IjsEnv) (delegate':'a) =
    let h = IjsHostFunc<'a>(env, delegate')
    let f = h :> IjsFunc
    let o = f :> IjsObj

    o.Methods <- env.Methods.Object
    o.put("length", double h.jsArgsLength, DescriptorAttrs.Immutable)
    Environment.addCompiler env f compile<'a>

    f

//------------------------------------------------------------------------------
module Object =
    
  //----------------------------------------------------------------------------
  module Property = 
  
    //--------------------------------------------------------------------------
    let requiredStorage (o:IjsObj) = o.PropertyMap.PropertyMap.Count
      
    //--------------------------------------------------------------------------
    let isFull (o:IjsObj) = requiredStorage o >= o.PropertyDescriptors.Length
      
    //--------------------------------------------------------------------------
    let setMap (o:IjsObj) pc = o.PropertyMap <- pc
      
    //--------------------------------------------------------------------------
    let getIndex (o:IjsObj) (name:string) =
      o.PropertyMap.PropertyMap.TryGetValue name

    //--------------------------------------------------------------------------
    let makeDynamic (o:IjsObj) =
      if o.PropertyMapId >= 0L then
        o.PropertyMap <- PropertyMap.makeDynamic o.PropertyMap
      
    //--------------------------------------------------------------------------
    let expandStorage (o:IjsObj) =
      let values = o.PropertyDescriptors
      let required = requiredStorage o
      let newValues = Array.zeroCreate (required * 2)

      if values.Length > 0 then 
        Array.Copy(values, newValues, values.Length)
      
      o.PropertyDescriptors <- newValues
      
    //--------------------------------------------------------------------------
    let createIndex (o:IjsObj) (name:string) =
      o.PropertyMap <- PropertyMap.getSubMap o.PropertyMap name
      if isFull o then expandStorage o
      o.PropertyMap.PropertyMap.[name]
        
    //--------------------------------------------------------------------------
    let find (o:IjsObj) name =
      let rec find o name =
        if FSKit.Utils.isNull o then (null, -1)
        else
          match getIndex o name with
          | true, index ->  o, index
          | _ -> find o.Prototype name

      find o name
      
    //--------------------------------------------------------------------------
    let canPut (o:IjsObj) name =

      let rec scanPrototype (o:IjsObj) name =
        if FSKit.Utils.isNull o then true
        else
          match getIndex o name with
          | true, index -> 
            Utils.Descriptor.isWritable 
              o.PropertyDescriptors.[index].Attributes
          | _ -> scanPrototype o.Prototype name

      match getIndex o name with
      | true, index ->
        let canPut =
          Utils.Descriptor.isWritable 
            o.PropertyDescriptors.[index].Attributes
        canPut, index

      | _ ->
        match scanPrototype o.Prototype name with
        | false -> false, -1
        | true -> true, createIndex o name
      
    //--------------------------------------------------------------------------
    let putBox (o:IjsObj) (name:IjsStr) (val':IjsBox) =
      let canPut, index = canPut o name
      if canPut then
        o.PropertyDescriptors.[index].Box <- val'
        o.PropertyDescriptors.[index].HasValue <- true

    //--------------------------------------------------------------------------
    let putRef (o:IjsObj) (name:IjsStr) (val':ClrObject) (tc:TypeTag) =
      let canPut, index = canPut o name
      o.PropertyDescriptors.[index].Box.Clr <- val'
      o.PropertyDescriptors.[index].Box.Tag <- tc
      o.PropertyDescriptors.[index].HasValue <- true
      
    //--------------------------------------------------------------------------
    let putVal (o:IjsObj) (name:IjsStr) (val':IjsNum) =
      let canPut, index = canPut o name
      o.PropertyDescriptors.[index].Box.Number <- val'
      o.PropertyDescriptors.[index].HasValue <- true

    //--------------------------------------------------------------------------
    let get (o:IjsObj) (name:IjsStr) =
      match find o name with
      | _, -1 -> Utils.BoxedConstants.undefined
      | pair -> (fst pair).PropertyDescriptors.[snd pair].Box

    //--------------------------------------------------------------------------
    let has (o:IjsObj) (name:IjsStr) =
      find o name |> snd > -1

    //--------------------------------------------------------------------------
    let delete (o:IjsObj) (name:IjsStr) =
      match getIndex o name with
      | true, index -> 

        let attrs = o.PropertyDescriptors.[index].Attributes
        let canDelete = Utils.Descriptor.isDeletable attrs

        if canDelete then
          setMap o (PropertyMap.delete(o.PropertyMap, name))

          o.PropertyDescriptors.[index].HasValue <- false
          o.PropertyDescriptors.[index].Box.Clr <- null
          o.PropertyDescriptors.[index].Box.Number <- 0.0

        canDelete

      | _ -> true
      
    //--------------------------------------------------------------------------
    module Delegates =
      let putBox = PutBoxProperty putBox
      let putVal = PutValProperty putVal
      let putRef = PutRefProperty putRef
      let get = GetProperty get
      let has = HasProperty has
      let delete = DeleteProperty delete

  //----------------------------------------------------------------------------
  module Index =
  
    //--------------------------------------------------------------------------
    let putBox (o:IjsObj) (index:uint32) (value:IjsBox) = 
      Property.putBox o (string index) value

    //--------------------------------------------------------------------------
    let putVal (o:IjsObj) (index:uint32) (value:IjsNum) = 
      Property.putVal o (string index) value

    //--------------------------------------------------------------------------
    let putRef (o:IjsObj) (index:uint32) (value:ClrObject) (tag:TypeTag) =
      Property.putRef o (string index) value tag

    //--------------------------------------------------------------------------
    let get (o:IjsObj) (index:uint32) = Property.get o (string index)
          
    //--------------------------------------------------------------------------
    let has (o:IjsObj) (index:uint32) = Property.has o (string index)

    //--------------------------------------------------------------------------
    let delete (o:IjsObj) (index:uint32) = Property.delete o (string index)
        
    //--------------------------------------------------------------------------
    module Delegates =
      let putBox = PutBoxIndex putBox
      let putVal = PutValIndex putVal
      let putRef = PutRefIndex putRef
      let get = GetIndex get
      let has = HasIndex has
      let delete = DeleteIndex delete
    
    //--------------------------------------------------------------------------
    type Converters =
    
      //------------------------------------------------------------------------
      static member put (o:IjsObj, index:IjsBox, value:IjsBox) =
        match index with
        | IsIndex i -> o.put(i, value)
        | IsTagged _ -> o.put(TypeConverter.toString index, value)
        | _ -> failwith "Que?"
      
      static member put (o:IjsObj, index:IjsBool, value:IjsBox) =
        o.put(TypeConverter.toString index, value)
      
      static member put (o:IjsObj, index:IjsNum, value:IjsBox) =
        match index with
        | IsNumberIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value)
        
      static member put (o:IjsObj, index:ClrObject, value:IjsBox) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.put(i, value)
        | index -> o.put(index, value)

      static member put (o:IjsObj, index:Undefined, value:IjsBox) =
        o.put("undefined", value)
      
      static member put (o:IjsObj, index:IjsStr, value:IjsBox) =
        match index with
        | IsStringIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value)

      static member put (o:IjsObj, index:IjsObj, value:IjsBox) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.put(i, value)
        | index -> o.put(index, value)
        
      //------------------------------------------------------------------------
      static member put (o:IjsObj, index:IjsBox, value:IjsVal) =
        match index with
        | IsIndex i -> o.put(i, value)
        | IsTagged _ -> o.put(TypeConverter.toString index, value)
        | _ -> failwith "Que?"
      
      static member put (o:IjsObj, index:IjsBool, value:IjsVal) =
        o.put(TypeConverter.toString index, value)
      
      static member put (o:IjsObj, index:IjsNum, value:IjsVal) =
        match index with
        | IsNumberIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value)
        
      static member put (o:IjsObj, index:ClrObject, value:IjsVal) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.put(i, value)
        | index -> o.put(index, value)

      static member put (o:IjsObj, index:Undefined, value:IjsVal) =
        o.put("undefined", value)
      
      static member put (o:IjsObj, index:IjsStr, value:IjsVal) =
        match index with
        | IsStringIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value)

      static member put (o:IjsObj, index:IjsObj, value:IjsVal) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.put(i, value)
        | index -> o.put(index, value)
        
      //------------------------------------------------------------------------
      static member put (o:IjsObj, index:IjsBox, value:IjsRef, tc:TypeTag) =
        match index with
        | IsIndex i -> o.put(i, value, tc)
        | IsTagged tc -> o.put(TypeConverter.toString index, value)
        | _ -> failwith "Que?"
      
      static member put (o:IjsObj, index:IjsBool, value:IjsRef, tc:TypeTag) =
        o.put(TypeConverter.toString index, value, tc)
      
      static member put (o:IjsObj, index:IjsNum, value:IjsRef, tc:TypeTag) =
        match index with
        | IsNumberIndex i -> o.put(i, value)
        | _ -> o.put(TypeConverter.toString index, value, tc)
        
      static member put (o:IjsObj, index:ClrObject, value:IjsRef, tc:TypeTag) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.put(i, value, tc)
        | index -> o.put(index, value, tc)

      static member put (o:IjsObj, index:Undefined, value:IjsRef, tc:TypeTag) =
        o.put("undefined", value, tc)
      
      static member put (o:IjsObj, index:IjsStr, value:IjsRef, tc:TypeTag) =
        match index with
        | IsStringIndex i -> o.put(i, value, tc)
        | _ -> o.put(TypeConverter.toString index, value, tc)

      static member put (o:IjsObj, index:IjsObj, value:IjsRef, tc:TypeTag) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.put(i, value, tc)
        | index -> o.put(index, value, tc)

      //------------------------------------------------------------------------
      static member get (o:IjsObj, index:IjsBox) =
        match index with
        | IsIndex i -> o.get i
        | IsTagged _ -> o.get(TypeConverter.toString index)
        | _ -> failwith "Que?"
      
      static member get (o:IjsObj, index:IjsBool) =
        o.get(TypeConverter.toString index)
      
      static member get (o:IjsObj, index:IjsNum) =
        match index with
        | IsNumberIndex i -> o.get i
        | _ -> o.get(TypeConverter.toString index)
        
      static member get (o:IjsObj, index:ClrObject) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.get i
        | index -> o.get(TypeConverter.toString index)

      static member get (o:IjsObj, index:Undefined) =
        o.get("undefined")
      
      static member get (o:IjsObj, index:IjsStr) =
        match index with
        | IsStringIndex i -> o.get i
        | _ -> o.get index

      static member get (o:IjsObj, index:IjsObj) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.get i
        | index -> o.get index

      //------------------------------------------------------------------------
      static member has (o:IjsObj, index:IjsBox) =
        match index with
        | IsIndex i -> o.has i
        | IsTagged _ -> o.has(TypeConverter.toString index)
        | _ -> failwith "Que?"
      
      static member has (o:IjsObj, index:IjsBool) =
        o.has(TypeConverter.toString index)
      
      static member has (o:IjsObj, index:IjsNum) =
        match index with
        | IsNumberIndex i -> o.has i
        | _ -> o.has(TypeConverter.toString index)
        
      static member has (o:IjsObj, index:ClrObject) =
        match TypeConverter.toString index with
        | IsStringIndex i -> o.has i
        | index -> o.has(TypeConverter.toString index)

      static member has (o:IjsObj, index:Undefined) =
        o.has("undefined")
      
      static member has (o:IjsObj, index:IjsStr) =
        match index with
        | IsStringIndex i -> o.has i
        | _ -> o.has index

      static member has (o:IjsObj, index:IjsObj) =
        Converters.has(o, TypeConverter.toPrimitive index)

  //----------------------------------------------------------------------------
  let defaultvalue (o:IjsObj) (hint:byte) =
    let hint = 
      if hint = DefaultValue.None 
        then DefaultValue.Number 
        else hint

    let valueOf = o.Methods.GetProperty.Invoke(o, "valueOf")
    let toString = o.Methods.GetProperty.Invoke(o, "toString")

    match hint with
    | DefaultValue.Number ->
      match valueOf.Tag with
      | TypeTags.Function ->
        let mutable v = Function.call(valueOf.Func, o)
        if Utils.isPrimitive v then v
        else
          match toString.Tag with
          | TypeTags.Function ->
            let mutable v = Function.call(toString.Func, o)
            if Utils.isPrimitive v then v else Errors.runtime "[[TypeError]]"
          | _ -> Errors.runtime "[[TypeError]]"
      | _ -> Errors.runtime "[[TypeError]]"

    | DefaultValue.String ->
      match toString.Tag with
      | TypeTags.Function ->
        let mutable v = Function.call(toString.Func, o)
        if Utils.isPrimitive v then v
        else 
          match toString.Tag with
          | TypeTags.Function ->
            let mutable v = Function.call(valueOf.Func, o)
            if Utils.isPrimitive v then v else Errors.runtime "[[TypeError]]"
          | _ -> Errors.runtime "[[TypeError]]"
      | _ -> Errors.runtime "[[TypeError]]"

    | _ -> Errors.runtime "Invalid hint"

  let defaultValue' = Default defaultvalue
  
  //----------------------------------------------------------------------------
  let collectProperties (o:IjsObj) =
    let rec collectProperties length (set:MutableSet<IjsStr>) (current:IjsObj) =
      if current <> null then
        let length =
          if current :? IjsArray then
            let array = current :?> IjsArray
            if length < array.Length then array.Length else length

          else 
            length

        let keys = current.PropertyMap.PropertyMap
        for pair in keys do
          let descriptor = current.PropertyDescriptors.[pair.Value]
          let attrs = descriptor.Attributes
          if descriptor.HasValue && Utils.Descriptor.isEnumerable attrs
            then pair.Key |> set.Add |> ignore

        collectProperties length set current.Prototype

      else 
        length, set

    o |> collectProperties 0u (new MutableSet<IjsStr>())

  //----------------------------------------------------------------------------
  let getLength (o:IjsObj) = 
    if o :? IjsArray 
      then (o :?> IjsArray).Length 
      else o.get "length" |> TypeConverter.toUInt32

  module Reflected = 

    let collectProperties = 
      Utils.Reflected.methodInfo "Api.Object" "collectProperties"

module Array =

  module Property =

    let private updateLength (o:IjsObj) (number:IjsNum) =
      let o = o :?> IjsArray
      let length = number |> TypeConverter.toUInt32

      if number < 0.0 then
        failwith "[[RangeError]]"

      if double length <> number then
        failwith "[[RangeError]]"
        
      while length < o.Length do
        if Utils.Array.isDense o then
          let i = int (o.Length-1u)
          o.Dense.[i].Box <- Box()
          o.Dense.[i].Attributes <- 0us
          o.Dense.[i].HasValue <- false

        else
          o.Sparse.Remove (o.Length-1u) |> ignore

        o.Length <- o.Length - 1u

      Object.Property.putVal o "length" number

    let putBox (o:IjsObj) (name:IjsStr) (val':IjsBox) =
      if name = "length" 
        then updateLength o (val' |> TypeConverter.toNumber)
        else Object.Property.putBox o name val'

    let putVal (o:IjsObj) (name:IjsStr) (val':IjsNum) =
      if name = "length" 
        then updateLength o (val' |> TypeConverter.toNumber)
        else Object.Property.putVal o name val'

    let putRef (o:IjsObj) (name:IjsStr) (val':IjsRef) (tag:TypeTag) =
      if name = "length" 
        then updateLength o (val' |> TypeConverter.toNumber)
        else Object.Property.putRef o name val' tag
      
    //--------------------------------------------------------------------------
    module Delegates =
      let putBox = PutBoxProperty putBox
      let putVal = PutValProperty putVal
      let putRef = PutRefProperty putRef

  //----------------------------------------------------------------------------
  module Index =
  
    //--------------------------------------------------------------------------
    let initSparse (o:IjsArray) =
      o.Sparse <- new MutableSorted<uint32, Box>()

      for i = 0 to int (o.Length-1u) do
        if Utils.Descriptor.hasValue o.Dense.[i] then
          o.Sparse.Add(uint32 i, o.Dense.[i].Box)

      o.Dense <- null
      
    //--------------------------------------------------------------------------
    let expandStorage (o:IjsArray) i =
      if o.Sparse = null || o.Dense.Length <= i then
        let size = if i >= 1073741823 then 2147483647 else ((i+1) * 2)
        let values = o.Dense
        let newValues = Array.zeroCreate size

        if values <> null && values.Length > 0 then
          Array.Copy(values, newValues, values.Length)

        o.Dense <- newValues
        
    //--------------------------------------------------------------------------
    let updateLength (o:IjsArray) (i:uint32) =
      if i > o.Length then
        let i = i+1u
        o.Length <- i
        Property.putVal o "length" (double i)

    //--------------------------------------------------------------------------
    let find (o:IjsArray) (i:uint32) =
      let rec find (o:IjsArray) (i:uint32) =
        if FSKit.Utils.isNull o then (null, 0u, false)
        else 
          if Utils.Array.isDense o then
            let ii = int i
            if ii < o.Dense.Length then
              if Utils.Descriptor.hasValue o.Dense.[ii] 
                then (o, i, true)
                else (null, 0u, false)
            else (null, 0u, false)
          else
            if o.Sparse.ContainsKey i 
              then (o, i, false)
              else (null, 0u, false)

      find o i
      
    //--------------------------------------------------------------------------
    let hasIndex (o:IjsArray) (i:uint32) =
      if Utils.Array.isDense o then
        let ii = int i
        if ii < o.Dense.Length 
          then Utils.Descriptor.hasValue o.Dense.[ii]
          else false
      else
        o.Sparse.ContainsKey i

    //--------------------------------------------------------------------------
    let putBox (o:IjsObj) (i:uint32) (v:IjsBox) =
      let o = o :?> IjsArray

      if i > Array.MaxIndex then initSparse o
      if Utils.Array.isDense o then
        if i > 255u && i/2u > o.Length then
          initSparse o
          o.Sparse.[i] <- v

        else
          let i = int i
          if i >= o.Dense.Length then expandStorage o i
          o.Dense.[i].Box <- v
          o.Dense.[i].HasValue <- true

      else
        o.Sparse.[i] <- v

      updateLength o i

    //--------------------------------------------------------------------------
    let putVal (o:IjsObj) (i:uint32) (v:IjsNum) =
      let o = o :?> IjsArray

      if i > Array.MaxIndex then initSparse o
      if Utils.Array.isDense o then
        if i > 255u && i/2u > o.Length then
          initSparse o
          o.Sparse.[i] <- Utils.boxVal v

        else
          let i = int i
          if i >= o.Dense.Length then expandStorage o i
          o.Dense.[i].Box.Number <- v
          o.Dense.[i].HasValue <- true

      else
        o.Sparse.[i] <- Utils.boxVal v

      updateLength o i

    //--------------------------------------------------------------------------
    let putRef (o:IjsObj) (i:uint32) (v:ClrObject) (tc:TypeTag) =
      let o = o :?> IjsArray

      if i > Array.MaxIndex then initSparse o
      if Utils.Array.isDense o then
        if i > 255u && i/2u > o.Length then
          initSparse o
          o.Sparse.[i] <- Utils.boxRef v tc

        else
          let i = int i
          if i >= o.Dense.Length then expandStorage o i
          o.Dense.[i].Box.Clr <- v
          o.Dense.[i].Box.Tag <- tc
          o.Dense.[i].HasValue <- true

      else
        o.Sparse.[i] <- Utils.boxRef v tc

      updateLength o i

    //--------------------------------------------------------------------------
    let get (o:IjsObj) (i:uint32) =
      let o = o :?> IjsArray

      match find o i with
      | null, _, _ -> Utils.BoxedConstants.undefined
      | o, index, isDense ->
        if isDense 
          then o.Dense.[int index].Box
          else o.Sparse.[index]
          
    //--------------------------------------------------------------------------
    let has (o:IjsObj) (i:uint32) =
      let o = o :?> IjsArray

      match find o i with
      | null, _, _ -> false
      | _ -> true

    //--------------------------------------------------------------------------
    let delete (o:IjsObj) (i:uint32) =
      let o = o :?> IjsArray

      match find o i with
      | null, _, _ -> true
      | o2, index, isDense ->
        if FSKit.Utils.refEq o o2 then
          if isDense then 
            o.Dense.[int i].Box <- Box()
            o.Dense.[int i].HasValue <- false
          else
            o.Sparse.Remove i |> ignore

          true
        else false
        
    //--------------------------------------------------------------------------
    module Delegates =
      let putBox = PutBoxIndex putBox
      let putVal = PutValIndex putVal
      let putRef = PutRefIndex putRef
      let get = GetIndex get
      let has = HasIndex has
      let delete = DeleteIndex delete

  //----------------------------------------------------------------------------
  let collectIndexValues (o:IjsObj) =
    //Array
    if o :? IjsArray then 
      let o = o :?> IjsArray

      //Dense array
      if Utils.Array.isDense o then
        seq {
        
          let i = ref 0u
          while !i < o.Length do
            let descr = o.Dense.[int !i]
            if descr.HasValue 
              then yield descr.Box

            elif o.hasPrototype
              then yield o.Prototype.get !i
              else yield Utils.BoxedConstants.undefined

            i := !i + 1u
        }

      //Sparse array
      else 
        seq {
          let i = ref 0u
          while !i < o.Length do
          
            match o.Sparse.TryGetValue !i with
            | true, box -> yield box
            | _ -> 
              if o.hasPrototype 
                then yield o.Prototype.get !i
                else yield Utils.BoxedConstants.undefined

            i := !i + 1u
        }
        
    //Object
    else
      seq { 
        let length = o |> Object.getLength
        let index = ref 0u
        while !index < length do
          yield o.get !index  
          index := !index + 1u
      }


module Arguments =

  module Index =

    //--------------------------------------------------------------------------
    let putBox (o:IjsObj) (i:uint32) (v:IjsBox) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        match a.LinkMap.[ii] with
        | ArgumentsLinkArray.Locals, index -> a.Locals.[index] <- v
        | ArgumentsLinkArray.ClosedOver, index -> a.ClosedOver.[index] <- v
        | _ -> failwith "Que?"

      Array.Index.putBox o i v
  
    //--------------------------------------------------------------------------
    let putVal (o:IjsObj) (i:uint32) (v:IjsNum) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        match a.LinkMap.[ii] with
        | ArgumentsLinkArray.Locals, index -> a.Locals.[index].Number <- v
        | ArgumentsLinkArray.ClosedOver, index -> 
          a.ClosedOver.[index].Number <- v
        | _ -> failwith "Que?"

      Array.Index.putVal o i v

    //--------------------------------------------------------------------------
    let putRef (o:IjsObj) (i:uint32) (v:IjsRef) (tag:TypeTag) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        match a.LinkMap.[ii] with
        | ArgumentsLinkArray.Locals, index -> 
          a.Locals.[index].Clr <- v
          a.Locals.[index].Tag <- tag

        | ArgumentsLinkArray.ClosedOver, index -> 
          a.ClosedOver.[index].Clr <- v
          a.ClosedOver.[index].Tag <- tag

        | _ -> failwith "Que?"

      Array.Index.putRef o i v tag
    
    //--------------------------------------------------------------------------
    let get (o:IjsObj) (i:uint32) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        match a.LinkMap.[ii] with
        | ArgumentsLinkArray.Locals, index -> a.Locals.[index]
        | ArgumentsLinkArray.ClosedOver, index -> a.ClosedOver.[index]
        | _ -> failwith "Que?"

      else
        Array.Index.get o i
        
    //--------------------------------------------------------------------------
    let has (o:IjsObj) (i:uint32) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length 
        then true
        else Array.Index.has o i
        
    //--------------------------------------------------------------------------
    let delete (o:IjsObj) (i:uint32) =
      let a = o :?> Arguments
      let ii = int i

      if a.LinkIntact && ii < a.LinkMap.Length then
        a.copyLinkedValues()
        a.LinkIntact <- false
        a.Locals <- null
        a.ClosedOver <- null

      Array.Index.delete o i
        
    //--------------------------------------------------------------------------
    module Delegates =
      let putBox = PutBoxIndex putBox
      let putVal = PutValIndex putVal
      let putRef = PutRefIndex putRef
      let get = GetIndex get
      let has = HasIndex has
      let delete = DeleteIndex delete
      
//------------------------------------------------------------------------------
module DynamicScope =
  
  //----------------------------------------------------------------------------
  let findObject name (dc:DynamicScope) stop =
    let rec find (dc:DynamicScope) =
      match dc with
      | [] -> None
      | (level, o)::xs ->
        if level >= stop then
          let mutable h = null
          let mutable i = 0
          if o.Methods.HasProperty.Invoke(o, name)
            then Some o
            else find xs
        else
          None

    find dc
      
  //----------------------------------------------------------------------------
  let findVariable name (dc:DynamicScope) stop = 
    match findObject name dc stop with
    | Some o -> Some(o.Methods.GetProperty.Invoke(o, name))
    | _ -> None

  //----------------------------------------------------------------------------
  let get name dc stop (g:IjsObj) (s:Scope) i =
    match findObject name dc stop with
    | Some o -> o.Methods.GetProperty.Invoke(o, name)
    | _ -> if s = null then g.Methods.GetProperty.Invoke(g, name) else s.[i]
      
  //----------------------------------------------------------------------------
  let set name (v:IjsBox) dc stop (g:IjsObj) (s:Scope) i =
    match findObject name dc stop with
    | Some o -> o.Methods.PutBoxProperty.Invoke(o, name, v)
    | _ -> 
      if s = null 
        then g.Methods.PutBoxProperty.Invoke(g, name, v) 
        else s.[i] <- v
          
  //----------------------------------------------------------------------------
  let call<'a when 'a :> Delegate> name args dc stop g (s:Scope) i =

    let this, func = 
      match findObject name dc stop with
      | Some o -> o, (o.Methods.GetProperty.Invoke(o, name))
      | _ -> g, if s=null then g.Methods.GetProperty.Invoke(g, name) else s.[i]

    if func.Tag >= TypeTags.Function then
      let func = func.Func
      let internalArgs = [|func :> obj; this :> obj|]
      let compiled = func.Compiler.compileAs<'a> func
      Utils.box (compiled.DynamicInvoke(Array.append internalArgs args))

    else
      Errors.runtime "Can only call javascript function dynamically"
        
  //----------------------------------------------------------------------------
  let delete (dc:DynamicScope) (g:IjsObj) name =
    match findObject name dc -1 with
    | Some o -> o.Methods.DeleteProperty.Invoke(o, name)
    | _ -> g.Methods.DeleteProperty.Invoke(g, name)

  //----------------------------------------------------------------------------
  let push (dc:DynamicScope byref) new' level =
    dc <- (level, new') :: dc
      
  //----------------------------------------------------------------------------
  let pop (dc:DynamicScope byref) =
    dc <- List.tail dc

  module Reflected =
    let set = Utils.Reflected.methodInfo "Api.DynamicScope" "set"
    let get = Utils.Reflected.methodInfo "Api.DynamicScope" "get"
    let call = Utils.Reflected.methodInfo "Api.DynamicScope" "call"
    let delete = Utils.Reflected.methodInfo "Api.DynamicScope" "delete"
    let push = Utils.Reflected.methodInfo "Api.DynamicScope" "push"
    let pop = Utils.Reflected.methodInfo "Api.DynamicScope" "pop"