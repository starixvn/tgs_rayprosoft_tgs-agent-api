import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { Checkbox as _Checkbox } from "bumbag"

export const Checkbox = ({
  name,
  checkboxRef,
  register,
  registerOptions = {},
  ...props
}) => {
  let { errors, ...formContext } = useFormContext()
  register = register ?? formContext.register

  if (!checkboxRef) {
    checkboxRef = register({
      required: props.required,
      min: props.min,
      max: props.max,
      minLength: props.minLength,
      maxLength: props.maxLength,
      validate: props.validate,
      ...registerOptions
    })
  }

  let error = errors[name]
  let state = error && "danger"

  return (
    <_Checkbox name={name} checkboxRef={checkboxRef} state={state} {...props} />
  )
}
