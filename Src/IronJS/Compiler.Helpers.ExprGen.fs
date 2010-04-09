﻿namespace IronJS.Compiler.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Compiler
open IronJS.Compiler.Types

module ExprGen =

  let newFunction closureType args =
    Dlr.Expr.newGenericArgs Runtime.Function.functionTypeDef [closureType] args

  let callFunction func (args:Et list) =
    let binder = new Runtime.Binders.Invoke(new System.Dynamic.CallInfo(args.Length + 1))
    Dlr.Expr.dynamic binder Constants.clrDynamic (func :: args)

  let convertToObject (expr:Et) =
    if Runtime.Helpers.Core.isObject expr.Type 
      then expr
      else let binder = new Runtime.Binders.Convert(Runtime.Core.objectTypeDef, false) 
           Dlr.Expr.dynamic binder Runtime.Core.objectTypeDef [expr]

  let setProperty (expr:Et) name value = 
    if Runtime.Helpers.Core.isObject expr.Type 
      then Helpers.Object.setProperty expr name value
      else let binder = new Runtime.Binders.SetMember(name, false)
           Dlr.Expr.dynamic binder Constants.clrDynamic (expr :: [value])

  let getProperty (expr:Et) name = 
    if Runtime.Helpers.Core.isObject expr.Type 
      then Helpers.Object.getProperty expr name
      else let binder = new Runtime.Binders.GetMember(name, false)
           Dlr.Expr.dynamic binder Constants.clrDynamic [expr]