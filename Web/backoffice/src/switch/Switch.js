import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { Spinner, Switch as _Switch } from "bumbag"

export const Switch = ({
  name,
  switchRef,
  register,
  registerOptions = {},
  ...props
}) => {
  let formContext = useFormContext()
  register = register ?? formContext?.register

  if (!switchRef && register) {
    switchRef = register({
      required: props.required,
      min: props.min,
      max: props.max,
      minLength: props.minLength,
      maxLength: props.maxLength,
      validate: props.validate,
      ...registerOptions
    })
  }

  let error = formContext?.errors[name]
  let state = error && "danger"

  return <_Switch name={name} switchRef={switchRef} state={state} {...props} />
}
